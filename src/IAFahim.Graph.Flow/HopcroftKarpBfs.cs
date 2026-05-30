namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HopcroftKarpBfs
    {
        public static int Run(int nLeft, int nRight, int* head, int* to, int* next, int* pairU, int* pairV, int* dist, int* q)
        {
            int qh = 0, qt = 0;
            for (int u = 0; u < nLeft; u++)
            {
                if (pairU[u] == -1) { dist[u] = 0; q[qt++] = u; }
                else dist[u] = -1;
            }
            int found = -1;
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    int pu = pairV[v];
                    if (pu != -1 && dist[pu] == -1) { dist[pu] = dist[u] + 1; q[qt++] = pu; }
                    else if (pu == -1) found = dist[u] + 1;
                }
            }
            return found;
        }
    }
}
