namespace IAFahim.Algebra.GraphPoly
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Tutte
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
        private static int CountComponents(int n, int edges, int* from, int* to, long mask, int* parent)
        {
            for (int i = 0; i < n; i++) parent[i] = i;
            int comps = n;
            for (int e = 0; e < edges; e++)
                if ((mask & (1L << e)) != 0L)
                {
                    int u = Find(parent, from[e]), v = Find(parent, to[e]);
                    if (u != v) { parent[u] = v; comps--; }
                }
            return comps;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Subset(int n, int edges, int* from, int* to, long x, long y, int MOD)
        {
            long modL = MOD;
            long result = 0L;
            long size = 1L << edges;
            int* parent = stackalloc int[n];

            // Precompute power tables. xExp = rkE - rkA in [0, n-1]; yExp = eCount - rkA in [0, edges].
            long* xPow = stackalloc long[n];
            long* yPow = stackalloc long[edges + 1];
            long xb = x % modL; if (xb < 0L) xb += modL;
            long yb = y % modL; if (yb < 0L) yb += modL;
            xPow[0] = 1L % modL;
            for (int i = 1; i < n; i++) xPow[i] = (xPow[i - 1] * xb) % modL;
            yPow[0] = 1L % modL;
            for (int i = 1; i <= edges; i++) yPow[i] = (yPow[i - 1] * yb) % modL;

            int compsE = CountComponents(n, edges, from, to, size - 1L, parent);
            int rkE = n - compsE;
            for (long mask = 0L; mask < size; mask++)
            {
                int comps = CountComponents(n, edges, from, to, mask, parent);
                int eCount = PopCount(mask);
                int rkA = n - comps;
                int xExp = rkE - rkA;
                int yExp = eCount - rkA;
                long term = (xPow[xExp] * yPow[yExp]) % modL;
                result = (result + term) % modL;
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PopCount(long m)
        {
            int c = 0;
            ulong u = (ulong)m;
            while (u != 0UL) { u &= (u - 1UL); c++; }
            return c;
        }
    }
}
