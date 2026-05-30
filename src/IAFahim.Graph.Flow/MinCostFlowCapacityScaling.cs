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
            int maxDelta = 1;
            for (int i = 0; i < n; i++)
                for (int e = head[i]; e != 0; e = next[e])
                    if (cap[e] > maxDelta) maxDelta = cap[e];
            for (int delta = maxDelta; delta > 0; delta >>= 1)
            {
                for (int iter = 0; iter < n; iter++)
                {
                    int* parent = stackalloc int[n];
                    long* dist = stackalloc long[n];
                    int* parentEdge = stackalloc int[n];
                    for (int i = 0; i < n; i++) { dist[i] = long.MaxValue; parent[i] = -1; }
                    dist[s] = 0;
                    int* q = stackalloc int[n];
                    int qh = 0, qt = 0;
                    q[qt++] = s;
                    while (qh < qt)
                    {
                        int uu = q[qh++];
                        for (int e2 = head[uu]; e2 != 0; e2 = next[e2])
                        {
                            if (cap[e2] - flow[e2] < delta) continue;
                            int vv = to[e2];
                            long nd = dist[uu] + cost[e2];
                            if (nd < dist[vv]) { dist[vv] = nd; parent[vv] = uu; parentEdge[vv] = e2; q[qt++] = vv; }
                        }
                    }
                    if (parent[t] == -1) continue;
                    int add = delta;
                    int vt = t;
                    while (vt != s) { int e = parentEdge[vt]; add = Math.Min(add, cap[e] - flow[e]); vt = parent[vt]; }
                    vt = t;
                    while (vt != s) { int e = parentEdge[vt]; flow[e] += add; flow[e ^ 1] -= add; totalCost += (long)cost[e] * add; vt = parent[vt]; }
                }
            }
            return totalCost;
        }
    }
}