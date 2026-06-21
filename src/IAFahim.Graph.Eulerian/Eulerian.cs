namespace IAFahim.Graph.Eulerian
{
    using System;
    using System.Runtime.CompilerServices;

    internal static unsafe class EulerShared
    {
        public const int EdgeFree = 0;

        public const int EdgeUsed = 1;

        public const int ReverseEdge = 1;

        public const int EdgeCapacityFactor = 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReversePath(int* path, int len)
        {
            int i = 0, j = len - 1;
            while (i < j)
            {
                int tmp = path[i];
                path[i] = path[j];
                path[j] = tmp;
                i++;
                j--;
            }
        }

        public static int BuildTrail(int n, int* head, int* to, int* next, int startNode, int* path, bool markReverse)
        {
            int* curHead = stackalloc int[n];
            for (int i = 0; i < n; i++) curHead[i] = head[i];
            int* stack = stackalloc int[n];
            int top = 0;
            stack[top++] = startNode;
            int edgeCap = n * EdgeCapacityFactor;
            int* edgeUsed = stackalloc int[edgeCap];
            for (int i = 0; i < edgeCap; i++) edgeUsed[i] = EdgeFree;
            int pathLen = 0;
            while (top > 0)
            {
                int u = stack[top - 1];
                bool pushed = false;
                for (int e = curHead[u]; e != 0; e = next[e])
                {
                    if (edgeUsed[e] != EdgeFree) { curHead[u] = next[e]; continue; }
                    int v = to[e];
                    edgeUsed[e] = EdgeUsed;
                    if (markReverse) edgeUsed[e ^ ReverseEdge] = EdgeUsed;
                    curHead[u] = next[e];
                    stack[top++] = v;
                    pushed = true;
                    break;
                }
                if (!pushed) { top--; path[pathLen++] = u; }
            }
            ReversePath(path, pathLen);
            return pathLen;
        }
    }

    public static unsafe class EulerianPathUndirected
    {
        private const int EulerCircuitOddDegrees = 0;

        private const int EulerPathOddDegrees = 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CountOddDegrees(int* deg, int n)
        {
            int odd = 0;
            for (int i = 0; i < n; i++) if ((deg[i] & 1) != 0) odd++;
            return odd;
        }

        public static int Run(int n, int* head, int* to, int* next, int start, int* path)
        {
            int* deg = stackalloc int[n];
            for (int i = 0; i < n; i++) deg[i] = 0;
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e])
                    deg[u]++;
            int oddCount = CountOddDegrees(deg, n);
            if (oddCount != EulerCircuitOddDegrees && oddCount != EulerPathOddDegrees) return 0;
            return EulerShared.BuildTrail(n, head, to, next, start, path, true);
        }
    }

    public static unsafe class EulerianPathDirected
    {
        private const int ExcessOutDegree = 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int DetectTrailStart(int* indeg, int* outdeg, int n, int fallbackStart)
        {
            int startNode = fallbackStart;
            for (int i = 0; i < n; i++)
                if (outdeg[i] - indeg[i] == ExcessOutDegree) startNode = i;
            return startNode;
        }

        public static int Run(int n, int* head, int* to, int* next, int start, int* path)
        {
            int* indeg = stackalloc int[n];
            int* outdeg = stackalloc int[n];
            for (int i = 0; i < n; i++) { indeg[i] = 0; outdeg[i] = 0; }
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e]) { indeg[to[e]]++; outdeg[u]++; }
            int startNode = DetectTrailStart(indeg, outdeg, n, start);
            return EulerShared.BuildTrail(n, head, to, next, startNode, path, false);
        }
    }
}
