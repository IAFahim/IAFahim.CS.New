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
        private static int CountComponents(int n, int edges, int* from, int* to, int mask, int* parent)
        {
            for (int i = 0; i < n; i++) parent[i] = i;
            for (int e = 0; e < edges; e++)
                if ((mask & (1 << e)) != 0) { int u = Find(parent, from[e]), v = Find(parent, to[e]); if (u != v) parent[u] = v; }
            int comps = 0; for (int i = 0; i < n; i++) if (parent[i] == i) comps++;
            return comps;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Subset(int n, int edges, int* from, int* to, long x, long y, int MOD)
        {
            long result = 0L; int size = 1 << edges;
            int* parent = stackalloc int[n];
            int compsE = CountComponents(n, edges, from, to, size - 1, parent);
            int rkE = n - compsE;
            for (int mask = 0; mask < size; mask++)
            {
                int comps = CountComponents(n, edges, from, to, mask, parent);
                int edgeCount = PopCount(mask);
                result = (result + CalculateTerm(n, rkE, comps, edgeCount, x, y, MOD)) % (long)MOD;
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PopCount(int m) { int c = 0; while (m > 0) { m &= (m - 1); c++; } return c; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long CalculateTerm(int n, int rkE, int comps, int eCount, long x, long y, int MOD)
        {
            int rkA = n - comps;
            int xExp = rkE - rkA;
            int yExp = eCount - rkA;
            long xPow = ModPow(x, (long)xExp, (long)MOD);
            long yPow = ModPow(y, (long)yExp, (long)MOD);
            return (xPow * yPow) % (long)MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModPow(long b, long e, long mod)
        {
            long r = 1L; b %= mod; if (b < 0L) b += mod;
            while (e > 0L) { if ((e & 1L) != 0L) r = (r * b) % mod; b = (b * b) % mod; e >>= 1; }
            return r;
        }
    }
}