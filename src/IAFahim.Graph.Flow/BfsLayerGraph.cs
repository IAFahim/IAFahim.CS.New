namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BfsLayerGraph
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int s, int* head, int* to, int* next, int* cap, int* flow, int* level)
        {
            for (int i = 0; i < n; i++) level[i] = -1;
            level[s] = 0;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != -1; e = next[e])
                {
                    int v = to[e];
                    if (level[v] == -1 && cap[e] - flow[e] > 0)
                    {
                        level[v] = level[u] + 1;
                        q[qt++] = v;
                    }
                }
            }
        }
    }
}