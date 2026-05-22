namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowCancelCycle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int* head, int* to, int* next, int* cap, int* cost, int* flow, int* dist, int* parent, int* parentEdge)
        {
            // Outline of negative cycle cancelation
            for (int i = 0; i < n; i++) dist[i] = 0;
            
            int x = -1;
            for (int i = 0; i < n; i++)
            {
                x = -1;
                for (int u = 0; u < n; u++)
                {
                    for (int e = head[u]; e != -1; e = next[e])
                    {
                        if (cap[e] - flow[e] > 0 && dist[u] != int.MaxValue && dist[to[e]] > dist[u] + cost[e])
                        {
                            dist[to[e]] = dist[u] + cost[e];
                            parent[to[e]] = u;
                            parentEdge[to[e]] = e;
                            x = to[e];
                        }
                    }
                }
            }
            if (x != -1)
            {
                for (int i = 0; i < n; i++) x = parent[x];
                int v = x;
                int minCap = int.MaxValue;
                do
                {
                    int e = parentEdge[v];
                    minCap = Math.Min(minCap, cap[e] - flow[e]);
                    v = parent[v];
                } while (v != x);
                
                v = x;
                do
                {
                    int e = parentEdge[v];
                    flow[e] += minCap;
                    flow[e ^ 1] -= minCap;
                    v = parent[v];
                } while (v != x);
            }
        }
    }
}