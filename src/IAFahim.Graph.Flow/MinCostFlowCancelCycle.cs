namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowCancelCycle
    {
        public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow)
        {
            long totalCost = 0;
            for (int i = 0; i < n * 2; i++) flow[i] = 0;
            while (true)
            {
                int* parent = stackalloc int[n];
                int* parentEdge = stackalloc int[n];
                long* dist = stackalloc long[n];
                byte* inqueue = stackalloc byte[n];
                for (int i = 0; i < n; i++) { dist[i] = long.MaxValue; parent[i] = -1; inqueue[i] = 0; }
                if (!Spfa(n, s, head, to, next, cap, cost, flow, dist, parent, parentEdge, inqueue))
                    break;
                int v = t, minCap = int.MaxValue;
                while (v != s) { int e = parentEdge[v]; minCap = Math.Min(minCap, cap[e] - flow[e]); v = parent[v]; }
                v = t;
                while (v != s) { int e = parentEdge[v]; flow[e] += minCap; flow[e ^ 1] -= minCap; totalCost += (long)cost[e] * minCap; v = parent[v]; }
            }
            return totalCost;
        }

        private static bool Spfa(int n, int s, int* head, int* to, int* next, int* cap, int* cost, int* flow, long* dist, int* parent, int* parentEdge, byte* inqueue)
        {
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            dist[s] = 0; q[qt++] = s; inqueue[s] = 1;
            int* cnt = stackalloc int[n];
            for (int i = 0; i < n; i++) cnt[i] = 0;
            cnt[s] = 1;
            while (qt > qh)
            {
                int u = q[qh++]; if (qh >= n) qh = 0; inqueue[u] = 0;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    if (cap[e] - flow[e] <= 0) continue;
                    int v = to[e];
                    long nd = dist[u] + cost[e];
                    if (nd < dist[v])
                    {
                        dist[v] = nd; parent[v] = u; parentEdge[v] = e;
                        if (inqueue[v] == 0)
                        {
                            q[qt++] = v; if (qt >= n) qt = 0; inqueue[v] = 1;
                            if (++cnt[v] > n) return false;
                        }
                    }
                }
            }
            return dist[s] < long.MaxValue;
        }
    }
}
