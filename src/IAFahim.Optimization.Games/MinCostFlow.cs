namespace IAFahim.Optimization.Games
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlow
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MinCostCirculation(int n, int m, int* from, int* to, long* cap, long* cost)
        {
            long totalCost = 0;
            for (int iter = 0; iter < m; iter++)
            {
                if (cap[iter] > 0)
                    totalCost += cost[iter] * cap[iter];
            }
            return totalCost;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MinCostArborescence(int n, int* from, int* to, long* w, int m, int root)
        {
            long total = 0;
            int* inEdge = stackalloc int[n];
            for (int i = 0; i < n; i++) inEdge[i] = -1;
            for (int e = 0; e < m; e++)
            {
                int v = to[e];
                if (v == root) continue;
                if (inEdge[v] < 0 || w[e] < w[inEdge[v]])
                    inEdge[v] = e;
            }
            for (int v = 0; v < n; v++)
            {
                if (v == root) continue;
                if (inEdge[v] < 0) return long.MaxValue;
                total += w[inEdge[v]];
            }
            return total;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MinMeanCycle(int n, int* from, int* to, long* w, int m)
        {
            long* dist = stackalloc long[n];
            int* cnt = stackalloc int[n];
            for (int i = 0; i < n; i++) { dist[i] = 0; cnt[i] = 0; }
            for (int i = 0; i < n; i++)
            {
                for (int e = 0; e < m; e++)
                {
                    int u = from[e], v = to[e];
                    if (dist[v] > dist[u] + w[e])
                    {
                        dist[v] = dist[u] + w[e];
                        cnt[v] = cnt[u] + 1;
                    }
                }
            }
            long minCycle = long.MaxValue;
            for (int e = 0; e < m; e++)
            {
                int u = from[e], v = to[e];
                if (cnt[u] >= n && dist[v] > dist[u] + w[e])
                {
                    long cycle = dist[v] - dist[u] - w[e];
                    int len = cnt[v] - cnt[u] + 1;
                    if (cycle < minCycle * len)
                        minCycle = cycle / len;
                }
            }
            return minCycle;
        }
    }
}
