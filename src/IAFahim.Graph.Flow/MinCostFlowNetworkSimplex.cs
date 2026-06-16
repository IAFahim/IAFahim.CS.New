namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowNetworkSimplex
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow)
        {
            long totalCost = 0;
            for (int i = 0; i < n * 2; i++) flow[i] = 0;
            long* pi = stackalloc long[n];
            for (int i = 0; i < n; i++) pi[i] = 0;
            while (true)
            {
                bool updated = false;
                for (int u = 0; u < n; u++)
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        if (cap[e] - flow[e] <= 0) continue;
                        int vv = to[e];
                        long rcost = cost[e] + pi[u] - pi[vv];
                        if (rcost < 0) { pi[vv] -= rcost; updated = true; }
                    }
                if (!updated) break;
            }

            long* dist = stackalloc long[n];
            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            int qcap = n + 1;
            int* q = stackalloc int[qcap];
            byte* inQueue = stackalloc byte[n];

            while (true)
            {
                for (int i = 0; i < n; i++) { dist[i] = long.MaxValue; parent[i] = -1; inQueue[i] = 0; }
                int qh = 0, qt = 0;
                dist[s] = 0;
                q[qt++] = s; inQueue[s] = 1;
                while (qh != qt)
                {
                    int uu = q[qh++];
                    if (qh == qcap) qh = 0;
                    inQueue[uu] = 0;
                    long du = dist[uu];
                    for (int e = head[uu]; e != 0; e = next[e])
                    {
                        if (cap[e] - flow[e] <= 0) continue;
                        int vv = to[e];
                        long nd = du + cost[e] + pi[uu] - pi[vv];
                        if (nd < dist[vv])
                        {
                            dist[vv] = nd;
                            parent[vv] = uu;
                            parentEdge[vv] = e;
                            if (inQueue[vv] == 0)
                            {
                                inQueue[vv] = 1;
                                q[qt++] = vv;
                                if (qt == qcap) qt = 0;
                            }
                        }
                    }
                }
                if (dist[t] == long.MaxValue) break;
                for (int i = 0; i < n; i++) if (dist[i] < long.MaxValue) pi[i] += dist[i];

                int add = int.MaxValue;
                for (int vt = t; vt != s; vt = parent[vt]) { int e = parentEdge[vt]; if (cap[e] - flow[e] < add) add = cap[e] - flow[e]; }
                for (int vt = t; vt != s; vt = parent[vt]) { int e = parentEdge[vt]; flow[e] += add; flow[e ^ 1] -= add; totalCost += (long)cost[e] * add; }
            }
            return totalCost;
        }
    }
}
