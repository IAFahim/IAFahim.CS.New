namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowPrimalDual
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow, int* pot, ref int totalFlow, ref int minCost)
        {
            int* dist = stackalloc int[n];
            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            var pq = new MinHeap(n);
            try
            {
                for (int i = 0; i < n; i++) pot[i] = 0;
                
                while (true)
                {
                    MinCostFlowDijkstra.Run(n, s, t, head, to, next, cap, cost, flow, dist, parent, parentEdge, pot, &pq);
                    if (dist[t] == int.MaxValue) break;
                    
                    for (int i = 0; i < n; i++)
                    {
                        if (dist[i] != int.MaxValue) pot[i] += dist[i];
                    }
                    
                    int push = int.MaxValue;
                    for (int v = t; v != s; v = parent[v])
                    {
                        int e = parentEdge[v];
                        push = Math.Min(push, cap[e] - flow[e]);
                    }
                    
                    for (int v = t; v != s; v = parent[v])
                    {
                        int e = parentEdge[v];
                        flow[e] += push;
                        flow[e ^ 1] -= push;
                        minCost += push * cost[e];
                    }
                    totalFlow += push;
                }
            }
            finally { pq.Dispose(); }
        }
    }
}