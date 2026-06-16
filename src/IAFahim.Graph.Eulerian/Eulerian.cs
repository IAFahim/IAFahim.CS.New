namespace IAFahim.Graph.Eulerian
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class EulerianPathUndirected
    {
        public static int Run(int n, int* head, int* to, int* next, int start, int* path)
        {
            int* deg = stackalloc int[n];
            for (int i = 0; i < n; i++) deg[i] = 0;
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e])
                    deg[u]++;
            int oddCount = 0;
            for (int i = 0; i < n; i++) if ((deg[i] & 1) != 0) oddCount++;
            if (oddCount != 0 && oddCount != 2) return 0;

            int* stack = stackalloc int[n];
            int* curHead = stackalloc int[n];
            for (int i = 0; i < n; i++) curHead[i] = head[i];
            int top = 0;
            stack[top++] = start;
            int edgeCap = n * 4;
            int* edgeUsed = stackalloc int[edgeCap];
            for (int i = 0; i < edgeCap; i++) edgeUsed[i] = 0;
            int pathLen = 0;
            while (top > 0)
            {
                int u = stack[top - 1];
                bool pushed = false;
                for (int e = curHead[u]; e != 0; e = next[e])
                {
                    if (edgeUsed[e] != 0) { curHead[u] = next[e]; continue; }
                    int v = to[e];
                    edgeUsed[e] = 1; edgeUsed[e ^ 1] = 1;
                    curHead[u] = next[e];
                    stack[top++] = v;
                    pushed = true;
                    break;
                }
                if (!pushed) { top--; path[pathLen++] = u; }
            }
            for (int i = 0, j = pathLen - 1; i < j; i++, j--)
            {
                int tmp = path[i];
                path[i] = path[j];
                path[j] = tmp;
            }
            return pathLen;
        }
    }

    public static unsafe class EulerianPathDirected
    {
        public static int Run(int n, int* head, int* to, int* next, int start, int* path)
        {
            int* indeg = stackalloc int[n];
            int* outdeg = stackalloc int[n];
            for (int i = 0; i < n; i++) { indeg[i] = 0; outdeg[i] = 0; }
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e]) { indeg[to[e]]++; outdeg[u]++; }

            int startNode = start, endNode = -1;
            for (int i = 0; i < n; i++)
            {
                if (outdeg[i] - indeg[i] == 1) startNode = i;
                if (outdeg[i] - indeg[i] == -1) endNode = i;
            }

            int* stack = stackalloc int[n];
            int* curHead = stackalloc int[n];
            for (int i = 0; i < n; i++) curHead[i] = head[i];
            int top = 0;
            stack[top++] = startNode;
            int edgeCap = n * 4;
            int* edgeUsed = stackalloc int[edgeCap];
            for (int i = 0; i < edgeCap; i++) edgeUsed[i] = 0;
            int pathLen = 0;

            while (top > 0)
            {
                int u = stack[top - 1];
                bool pushed = false;
                for (int e = curHead[u]; e != 0; e = next[e])
                {
                    if (edgeUsed[e] != 0) { curHead[u] = next[e]; continue; }
                    edgeUsed[e] = 1;
                    curHead[u] = next[e];
                    stack[top++] = to[e];
                    pushed = true;
                    break;
                }
                if (!pushed) { top--; path[pathLen++] = u; }
            }
            for (int i = 0, j = pathLen - 1; i < j; i++, j--)
            {
                int tmp = path[i];
                path[i] = path[j];
                path[j] = tmp;
            }
            return pathLen;
        }
    }
}
