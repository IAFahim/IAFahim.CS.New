namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HopcroftKarpDfs
    {
        public static bool Run(int u, int nLeft, int nRight, int* head, int* to, int* next, int* pairU, int* pairV, int* dist, int* it)
        {
            if (u == -1) return true;
            for (int e = it[u]; e != 0; e = next[e])
            {
                it[u] = e;
                int v = to[e];
                int pu = pairV[v];
                if (pu != -1 && (dist[pu] != dist[u] + 1 || !Run(pu, nLeft, nRight, head, to, next, pairU, pairV, dist, it)))
                    continue;
                pairU[u] = v; pairV[v] = u;
                return true;
            }
            dist[u] = int.MaxValue;
            return false;
        }
    }
}
