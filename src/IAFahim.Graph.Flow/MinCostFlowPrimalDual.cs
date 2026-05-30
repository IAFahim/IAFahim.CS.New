namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowPrimalDual
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow)
        {
            long totalCost = 0;
            for (int i = 0; i < n * 2; i++) flow[i] = 0;
            long* dist = stackalloc long[n];
            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            long* pot = stackalloc long[n];
            for (int i = 0; i < n; i++) pot[i] = 0;
            int* level = stackalloc int[n];
            for (int i = 0; i < n; i++) level[i] = 0;

            while (true)
            {
                for (int i = 0; i < n; i++) { dist[i] = long.MaxValue; parent[i] = -1; }
                dist[s] = 0;
                int* q = stackalloc int[n];
                int qh = 0, qt = 0;
                q[qt++] = s;

                while (qh < qt)
                {
                    int u = q[qh++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        if (cap[e] - flow[e] <= 0) continue;
                        int v = to[e];
                        long nd = dist[u] + cost[e] + pot[u] - pot[v];
                        if (nd < dist[v]) { dist[v] = nd; parent[v] = u; parentEdge[v] = e; q[qt++] = v; }
                    }
                }
                if (dist[t] == long.MaxValue) break;
                for (int i = 0; i < n; i++) if (dist[i] < long.MaxValue) pot[i] += dist[i];

                int add = int.MaxValue;
                for (int v = t; v != s; v = parent[v]) add = Math.Min(add, cap[parentEdge[v]] - flow[parentEdge[v]]);
                for (int v = t; v != s; v = parent[v]) { int e = parentEdge[v]; flow[e] += add; flow[e ^ 1] -= add; totalCost += (long)cost[e] * add; }
            }
            return totalCost;
        }
    }
}
