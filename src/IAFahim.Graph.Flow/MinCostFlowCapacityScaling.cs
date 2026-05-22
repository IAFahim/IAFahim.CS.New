namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowCapacityScaling
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int s, int t, int maxCap, int* head, int* to, int* next, int* cap, int* cost, int* flow, int* pot)
        {
            int delta = 1;
            while (delta * 2 <= maxCap) delta *= 2;
            
            int* dist = stackalloc int[n];
            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            
            while (delta > 0)
            {
                while (true)
                {
                    for (int i = 0; i < n; i++) dist[i] = int.MaxValue;
                    dist[s] = 0;
                    // Simplified scaling dijkstra check
                    for (int i = 0; i < n; i++)
                    {
                        for (int u = 0; u < n; u++)
                        {
                            for (int e = head[u]; e != -1; e = next[e])
                            {
                                if (cap[e] - flow[e] >= delta && dist[u] != int.MaxValue)
                                {
                                    int w = cost[e] + pot[u] - pot[to[e]];
                                    if (dist[to[e]] > dist[u] + w)
                                    {
                                        dist[to[e]] = dist[u] + w;
                                        parent[to[e]] = u;
                                        parentEdge[to[e]] = e;
                                    }
                                }
                            }
                        }
                    }
                    if (dist[t] == int.MaxValue) break;
                    
                    int v = t;
                    while (v != s)
                    {
                        int e = parentEdge[v];
                        flow[e] += delta;
                        flow[e ^ 1] -= delta;
                        v = parent[v];
                    }
                }
                delta /= 2;
            }
        }
    }
}