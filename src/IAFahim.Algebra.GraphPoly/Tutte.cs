namespace IAFahim.Algebra.GraphPoly
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Tutte
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Subset(int n, int edges, int* from, int* to, long x, long y, int MOD)
        {
            long result = 0;
            int size = 1 << edges;
            for (int mask = 0; mask < size; mask++)
            {
                int* parent = stackalloc int[n];
                for (int i = 0; i < n; i++) parent[i] = i;
                int edgeCount = 0;
                for (int e = 0; e < edges; e++)
                {
                    if ((mask & (1 << e)) == 0) continue;
                    edgeCount++;
                    int u = from[e], v = to[e];
                    while (parent[u] != u) u = parent[u];
                    while (parent[v] != v) v = parent[v];
                    if (u != v) parent[u] = v;
                }
                for (int i = 0; i < n; i++)
                {
                    int r = i;
                    while (parent[r] != r) r = parent[r];
                    parent[i] = r;
                }
                int components = 0;
                bool* counted = stackalloc bool[n];
                for (int i = 0; i < n; i++) counted[i] = false;
                for (int i = 0; i < n; i++)
                {
                    int r = i;
                    while (parent[r] != r) r = parent[r];
                    if (!counted[r]) { counted[r] = true; components++; }
                }
                int internalEdges = edgeCount - (n - components);
                long xPow = ModPow(x, internalEdges, MOD);
                long yPow = ModPow(y, edges - edgeCount - (n - components) + internalEdges, MOD);
                if (edges - edgeCount - (n - components) + internalEdges < 0) yPow = 1;
                result = (result + xPow * yPow) % MOD;
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModPow(long b, long e, long mod)
        {
            long r = 1; b %= mod; if (b < 0) b += mod;
            while (e > 0) { if ((e & 1) != 0) r = r * b % mod; b = b * b % mod; e >>= 1; }
            return r;
        }
    }
}
