namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowSpfa
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int s, int* head, int* to, int* next, int* cap, int* cost, int* flow, int* dist, bool* inq)
        {
            for (int i = 0; i < n; i++) { dist[i] = int.MaxValue; inq[i] = false; }
            dist[s] = 0;
            int* q = stackalloc int[n * 2]; // arbitrary
            int qh = 0, qt = 0;
            q[qt++] = s;
            inq[s] = true;
            
            while (qh < qt)
            {
                int u = q[qh++];
                inq[u] = false;
                
                for (int e = head[u]; e != -1; e = next[e])
                {
                    int v = to[e];
                    if (cap[e] - flow[e] > 0 && dist[v] > dist[u] + cost[e])
                    {
                        dist[v] = dist[u] + cost[e];
                        if (!inq[v])
                        {
                            q[qt++] = v;
                            inq[v] = true;
                        }
                    }
                }
            }
        }
    }
}