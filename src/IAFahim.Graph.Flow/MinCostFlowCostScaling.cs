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
            int* excess = stackalloc int[n];
            long* potential = stackalloc long[n];
            for (int i = 0; i < n; i++) { excess[i] = 0; potential[i] = 0; }
            excess[s] = int.MaxValue;
            int* q = stackalloc int[n];
            while (true)
            {
                int qh = 0, qt = 0;
                for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
                dist[s] = 0;
                q[qt++] = s;
                while (qh < qt)
                {
                    int u = q[qh++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        if (cap[e] - flow[e] <= 0) continue;
                        int v = to[e];
                        long rcost = cost[e] + potential[u] - potential[v];
                        if (rcost < 0 && dist[v] > dist[u] + rcost)
                        {
                            dist[v] = dist[u] + rcost;
                            q[qt++] = v;
                        }
                    }
                }
                for (int i = 0; i < n; i++) if (dist[i] < long.MaxValue) potential[i] += dist[i];
                int* bucket = stackalloc int[n];
                int bhead = 0, btail = 0;
                for (int i = 0; i < n; i++) bucket[i] = -1;
                bucket[s] = 0;
                int* inBucket = stackalloc int[n];
                for (int i = 0; i < n; i++) inBucket[i] = 0;
                inBucket[s] = 1; btail++;
                while (bhead < btail)
                {
                    int u = bucket[bhead++];
                    if (bhead >= n) bhead = 0;
                    inBucket[u] = 0;
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        if (cap[e] - flow[e] <= 0) continue;
                        int v = to[e];
                        long rcost = cost[e] + potential[u] - potential[v];
                        if (rcost < 0)
                        {
                            int push = Math.Min(excess[u], cap[e] - flow[e]);
                            flow[e] += push; flow[e ^ 1] -= push;
                            totalCost += (long)cost[e] * push;
                            excess[v] += push; excess[u] -= push;
                            if (excess[v] > 0 && inBucket[v] == 0) { bucket[btail++] = v; if (btail >= n) btail = 0; inBucket[v] = 1; }
                        }
                    }
                }
                if (excess[t] > 0) continue;
                bool any = false;
                for (int u = 0; u < n; u++)
                    if (excess[u] > 0)
                    {
                        any = true;
                        break;
                    }
                if (!any) break;
            }
            return totalCost;
        }
    }
}
