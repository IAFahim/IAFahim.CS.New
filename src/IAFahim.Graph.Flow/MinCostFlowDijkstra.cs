namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowDijkstra
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow, int* dist, int* parent, int* parentEdge, int* pot)
        {
            for (int i = 0; i < n; i++) dist[i] = int.MaxValue;
            dist[s] = 0;
            bool* vis = stackalloc bool[n];
            for (int i = 0; i < n; i++) vis[i] = false;
            
            // O(N^2) Dijkstra for simplicity
            for (int i = 0; i < n; i++)
            {
                int u = -1;
                for (int j = 0; j < n; j++)
                {
                    if (!vis[j] && dist[j] != int.MaxValue && (u == -1 || dist[j] < dist[u]))
                    {
                        u = j;
                    }
                }
                if (u == -1) break;
                vis[u] = true;
                
                for (int e = head[u]; e != -1; e = next[e])
                {
                    int v = to[e];
                    if (cap[e] - flow[e] > 0)
                    {
                        int w = cost[e] + pot[u] - pot[v];
                        if (dist[u] + w < dist[v])
                        {
                            dist[v] = dist[u] + w;
                            parent[v] = u;
                            parentEdge[v] = e;
                        }
                    }
                }
            }
        }
    }
}