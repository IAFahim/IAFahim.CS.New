namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DinicWithLinkCut
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
        {
            long result = 0;
            int* level = stackalloc int[n];
            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            int* excess = stackalloc int[n];
            while (BfsLayer(n, s, t, head, to, next, cap, flow, level))
            {
                for (int i = 0; i < n; i++) { parent[i] = -1; parentEdge[i] = 0; excess[i] = 0; }
                excess[s] = int.MaxValue;
                int* q = stackalloc int[n];
                int qh = 0, qt = 0;
                q[qt++] = s;
                while (qh < qt)
                {
                    int u = q[qh++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        if (cap[e] - flow[e] > 0 && parent[v] == -1 && level[v] == level[u] + 1)
                        {
                            parent[v] = u; parentEdge[v] = e;
                            int add = Math.Min(excess[u], cap[e] - flow[e]);
                            excess[v] += add; excess[u] -= add;
                            flow[e] += add; flow[e ^ 1] -= add;
                            if (excess[v] > 0) q[qt++] = v;
                        }
                    }
                }
                result += Augment(n, s, t, parent, parentEdge, cap, flow);
            }
            return result;
        }

        private static bool BfsLayer(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, int* level)
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
                    if (cap[e] - flow[e] > 0 && level[to[e]] == -1)
                    {
                        level[to[e]] = level[u] + 1; q[qt++] = to[e];
                    }
                }
            }
            return level[t] != -1;
        }

        private static long Augment(int n, int s, int t, int* parent, int* parentEdge, int* cap, int* flow)
        {
            int v = t;
            int add = int.MaxValue;
            while (v != s) { add = Math.Min(add, cap[parentEdge[v]] - flow[parentEdge[v]]); v = parent[v]; }
            if (add <= 0) return 0;
            v = t;
            long cost = 0;
            while (v != s) { int e = parentEdge[v]; flow[e] += add; flow[e ^ 1] -= add; cost += add; v = parent[v]; }
            return cost;
        }
    }
}