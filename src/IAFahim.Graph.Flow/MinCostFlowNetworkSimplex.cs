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
            while (true)
            {
                int* d = stackalloc int[n];
                int* p = stackalloc int[n];
                int* eid = stackalloc int[n];
                for (int i = 0; i < n; i++) { d[i] = 0; p[i] = -1; eid[i] = 0; }
                int qh = 0, qt = 0;
                int* q = stackalloc int[n];
                d[s] = 1; q[qt++] = s;
                while (qh < qt)
                {
                    int uu = q[qh++];
                    for (int e = head[uu]; e != 0; e = next[e])
                    {
                        if (cap[e] - flow[e] <= 0) continue;
                        int vv = to[e];
                        if (d[vv] == 0) { d[vv] = 1; p[vv] = uu; eid[vv] = e; q[qt++] = vv; }
                    }
                    for (int e = head[uu]; e != 0; e = next[e])
                    {
                        if (flow[e] <= 0) continue;
                        int vv = to[e];
                        if (d[vv] == 0) { d[vv] = 1; p[vv] = uu; eid[vv] = e; q[qt++] = vv; }
                    }
                }
                if (d[t] == 0) break;
                int add = int.MaxValue;
                int vt = t;
                while (vt != s) { int e = eid[vt]; add = Math.Min(add, cap[e] - flow[e]); vt = p[vt]; }
                vt = t;
                while (vt != s) { int e = eid[vt]; flow[e] += add; flow[e ^ 1] -= add; totalCost += (long)cost[e] * add; vt = p[vt]; }
            }
            return totalCost;
        }
    }
}