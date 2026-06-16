namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowCostScaling
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow)
        {
            long totalCost = 0;
            for (int i = 0; i < n * 2; i++) flow[i] = 0;
            long* dist = stackalloc long[n];
            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            long* potential = stackalloc long[n];
            int* q = stackalloc int[n + 1];
            byte* inq = stackalloc byte[n];
            int* cnt = stackalloc int[n];
            for (int i = 0; i < n; i++) potential[i] = 0;

            while (true)
            {
                for (int i = 0; i < n; i++) { dist[i] = long.MaxValue; parent[i] = -1; inq[i] = 0; cnt[i] = 0; }
                dist[s] = 0;
                int qh = 0, qt = 0;
                q[qt++] = s; inq[s] = 1; cnt[s] = 1;
                while (qh != qt)
                {
                    int u = q[qh++]; if (qh > n) qh = 0; inq[u] = 0;
                    long pu = potential[u];
                    long du = dist[u];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        if (cap[e] - flow[e] <= 0) continue;
                        int v = to[e];
                        long nd = du + cost[e] + pu - potential[v];
                        if (nd < dist[v])
                        {
                            dist[v] = nd; parent[v] = u; parentEdge[v] = e;
                            if (inq[v] == 0)
                            {
                                q[qt++] = v; if (qt > n) qt = 0; inq[v] = 1;
                                if (++cnt[v] > n) { qh = qt; break; }
                            }
                        }
                    }
                }
                if (dist[t] == long.MaxValue) break;
                for (int i = 0; i < n; i++) if (dist[i] < long.MaxValue) potential[i] += dist[i];
                int add = int.MaxValue;
                for (int v = t; v != s; v = parent[v])
                {
                    int e = parentEdge[v];
                    int res = cap[e] - flow[e];
                    if (res < add) add = res;
                }
                for (int v = t; v != s; v = parent[v])
                {
                    int e = parentEdge[v];
                    flow[e] += add; flow[e ^ 1] -= add;
                    totalCost += (long)cost[e] * add;
                }
            }
            return totalCost;
        }
    }
}
