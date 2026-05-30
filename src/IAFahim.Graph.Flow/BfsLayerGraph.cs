namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BfsLayerGraph
    {
        public static bool Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* level)
        {
            for (int i = 0; i < n; i++) level[i] = -1;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            level[s] = 0; q[qt++] = s;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (cap[e] - flow[e] > 0 && level[v] == -1)
                    {
                        level[v] = level[u] + 1;
                        if (v == t) return true;
                        q[qt++] = v;
                    }
                }
            }
            return level[t] != -1;
        }
    }
}
