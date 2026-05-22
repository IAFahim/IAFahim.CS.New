namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HopcroftKarpBfs
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int n, int* pairU, int* pairV, int* dist, int* head, int* to, int* next)
        {
            int* q = stackalloc int[n];
            int qh = 0, qt = 0;
            for (int u = 1; u <= n; u++)
            {
                if (pairU[u] == 0)
                {
                    dist[u] = 0;
                    q[qt++] = u;
                }
                else
                {
                    dist[u] = int.MaxValue;
                }
            }
            dist[0] = int.MaxValue;
            while (qh < qt)
            {
                int u = q[qh++];
                if (dist[u] < dist[0])
                {
                    for (int e = head[u]; e != -1; e = next[e])
                    {
                        int v = to[e];
                        if (dist[pairV[v]] == int.MaxValue)
                        {
                            dist[pairV[v]] = dist[u] + 1;
                            q[qt++] = pairV[v];
                        }
                    }
                }
            }
            return dist[0] != int.MaxValue;
        }
    }
}