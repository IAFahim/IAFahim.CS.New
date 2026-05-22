namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HopcroftKarpDfs
    {
        public static bool Run(int u, int* pairU, int* pairV, int* dist, int* head, int* to, int* next)
        {
            if (u != 0)
            {
                for (int e = head[u]; e != -1; e = next[e])
                {
                    int v = to[e];
                    if (dist[pairV[v]] == dist[u] + 1)
                    {
                        if (Run(pairV[v], pairU, pairV, dist, head, to, next))
                        {
                            pairV[v] = u;
                            pairU[u] = v;
                            return true;
                        }
                    }
                }
                dist[u] = int.MaxValue;
                return false;
            }
            return true;
        }
    }
}