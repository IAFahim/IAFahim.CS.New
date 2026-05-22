namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowDijkstra
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow, int* dist, int* parent, int* parentEdge, int* pot, MinHeap* pq)
        {
            for (int i = 0; i < n; i++) dist[i] = int.MaxValue;
            dist[s] = 0;
            pq->Size = 0;
            pq->PushOrUpdate(s, 0);
            while (pq->Size > 0)
            {
                int u = pq->Pop(out long d);
                if (d != dist[u]) continue;
                if (u == t) break;
                for (int e = head[u]; e != -1; e = next[e])
                {
                    if (cap[e] - flow[e] > 0)
                    {
                        int v = to[e];
                        int w = cost[e] + pot[u] - pot[v];
                        if ((long)dist[u] + w < dist[v])
                        {
                            dist[v] = dist[u] + w;
                            parent[v] = u;
                            parentEdge[v] = e;
                            pq->PushOrUpdate(v, dist[v]);
                        }
                    }
                }
            }
        }
    }
}