namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowCapacityScaling
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow)
        {
            long totalCost = 0;
            for (int i = 0; i < n * 2; i++) flow[i] = 0;

            long* dist = stackalloc long[n];
            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            long* pot = stackalloc long[n];
            int* q = stackalloc int[n];
            byte* inqueue = stackalloc byte[n];
            int* cnt = stackalloc int[n];
            for (int i = 0; i < n; i++) pot[i] = 0;

            while (true)
            {
                for (int i = 0; i < n; i++) { dist[i] = long.MaxValue; parent[i] = -1; inqueue[i] = 0; cnt[i] = 0; }
                dist[s] = 0;
                int qh = 0, qt = 0;
                q[qt++] = s; inqueue[s] = 1; cnt[s] = 1;

                while (qh != qt)
                {
                    int uu = q[qh++]; if (qh >= n) qh = 0; inqueue[uu] = 0;
                    long du = dist[uu];
                    for (int e = head[uu]; e != 0; e = next[e])
                    {
                        if (cap[e] - flow[e] <= 0) continue;
                        int vv = to[e];
                        long nd = du + cost[e] + pot[uu] - pot[vv];
                        if (nd < dist[vv])
                        {
                            dist[vv] = nd; parent[vv] = uu; parentEdge[vv] = e;
                            if (inqueue[vv] == 0)
                            {
                                q[qt++] = vv; if (qt >= n) qt = 0; inqueue[vv] = 1;
                                if (++cnt[vv] > n) { if (parent[t] != -1) break; return totalCost; }
                            }
                        }
                    }
                }

                if (dist[t] == long.MaxValue) break;

                for (int i = 0; i < n; i++) if (dist[i] < long.MaxValue) pot[i] += dist[i];

                int add = int.MaxValue;
                for (int v = t; v != s; v = parent[v]) { int e = parentEdge[v]; if (cap[e] - flow[e] < add) add = cap[e] - flow[e]; }
                for (int v = t; v != s; v = parent[v]) { int e = parentEdge[v]; flow[e] += add; flow[e ^ 1] -= add; totalCost += (long)cost[e] * add; }
            }

            return totalCost;
        }
    }
}
