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
            for (int i = 0; i < n; i++) if (deg[i] % 2 == 1) oddCount++;
            if (oddCount != 0 && oddCount != 2) return 0;

            int* stack = stackalloc int[n];
            int* curHead = stackalloc int[n];
            for (int i = 0; i < n; i++) curHead[i] = head[i];
            int top = 0;
            stack[top++] = start;
            int* edgeUsed = stackalloc int[n * 4];
            int ec = 0;
            while (top > 0)
            {
                int u = stack[top - 1];
                bool pushed = false;
                for (int e = curHead[u]; e != 0; e = next[e])
                {
                    if (ec > 0 && edgeUsed[e] != 0) continue;
                    int v = to[e];
                    edgeUsed[e] = 1; edgeUsed[e ^ 1] = 1;
                    curHead[u] = next[e];
                    stack[top++] = v;
                    pushed = true;
                    ec++;
                    break;
                }
                if (!pushed) { top--; path[n - 1 - top] = u; }
            }
            return n;
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
            int* edgeUsed = stackalloc int[n * 4];
            for (int i = 0; i < n * 4; i++) edgeUsed[i] = 0;
            int pathLen = 0;
            int m = 0;
            for (int u = 0; u < n; u++)
                for (int e = head[u]; e != 0; e = next[e]) m++;

            while (top > 0)
            {
                int u = stack[top - 1];
                bool pushed = false;
                for (int e = curHead[u]; e != 0; e = next[e])
                {
                    if (edgeUsed[e] != 0) continue;
                    edgeUsed[e] = 1;
                    curHead[u] = next[e];
                    stack[top++] = to[e];
                    pushed = true;
                    break;
                }
                if (!pushed) { top--; path[pathLen++] = u; }
            }
            return pathLen;
        }
    }
}
