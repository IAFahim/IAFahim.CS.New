namespace IAFahim.Algebra.GraphPoly
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Reliability
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Find(int* parent, int i)
        {
            while (parent[i] != i)
            {
                parent[i] = parent[parent[i]];
                i = parent[i];
            }
            return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsConnected(int n, int edges, int* from, int* to, int mask, int* parent)
        {
            for (int i = 0; i < n; i++) parent[i] = i;
            int components = n;
            for (int e = 0; e < edges; e++)
            {
                if ((mask & (1 << e)) == 0) continue;
                int u = Find(parent, from[e]), v = Find(parent, to[e]);
                if (u != v)
                {
                    parent[u] = v;
                    components--;
                }
            }
            return components == 1;
        }

        public static long Run(int n, int edges, int* from, int* to, long p, int MOD)
        {
            long mod = (long)MOD;
            long q = (1L - p + mod) % mod;

            // Probability of a subgraph depends only on its present-edge count:
            // present edges contribute p, absent edges contribute q=(1-p).
            // Precompute pPow[i]=p^i and qPow[i]=q^i for i in [0,edges].
            long* pPow = stackalloc long[edges + 1];
            long* qPow = stackalloc long[edges + 1];
            pPow[0] = 1L;
            qPow[0] = 1L;
            for (int i = 1; i <= edges; i++)
            {
                pPow[i] = (pPow[i - 1] * p) % mod;
                qPow[i] = (qPow[i - 1] * q) % mod;
            }

            long result = 0L;
            int size = 1 << edges;
            int* parent = stackalloc int[n];
            for (int mask = 0; mask < size; mask++)
            {
                if (IsConnected(n, edges, from, to, mask, parent))
                {
                    int edgeCount = 0;
                    int m = mask;
                    while (m > 0) { m &= m - 1; edgeCount++; }
                    long prob = (pPow[edgeCount] * qPow[edges - edgeCount]) % mod;
                    result = (result + prob) % mod;
                }
            }
            return result;
        }
    }
}