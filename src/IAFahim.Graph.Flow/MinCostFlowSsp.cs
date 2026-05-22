namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlowSsp
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* cost, int* flow, int* dist, int* parent, int* parentEdge, bool* inq)
        {
            for (int i = 0; i < n; i++) { dist[i] = int.MaxValue; inq[i] = false; }
            dist[s] = 0;
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            q[qt++] = s;
            inq[s] = true;
            
            while (qh != qt)
            {
                int u = q[qh++];
                if (qh == n) qh = 0;
                inq[u] = false;
                
                for (int e = head[u]; e != -1; e = next[e])
                {
                    int v = to[e];
                    if (cap[e] - flow[e] > 0 && dist[v] > dist[u] + cost[e])
                    {
                        dist[v] = dist[u] + cost[e];
                        parent[v] = u;
                        parentEdge[v] = e;
                        if (!inq[v])
                        {
                            q[qt++] = v;
                            if (qt == n) qt = 0;
                            inq[v] = true;
                        }
                    }
                }
            }
        }
    }
}