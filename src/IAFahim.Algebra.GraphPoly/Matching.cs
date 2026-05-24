namespace IAFahim.Algebra.GraphPoly
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Matching
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsMatching(int n, bool* adj, int mask)
        {
            for (int i = 0; i < n; i++)
            {
                if ((mask & (1 << i)) == 0) continue;
                for (int j = i + 1; j < n; j++)
                {
                    if ((mask & (1 << j)) == 0) continue;
                    if (adj[i * n + j]) return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CountEdges(int n, int mask)
        {
            int edgeCount = 0;
            for (int i = 0; i < n; i++)
            {
                if ((mask & (1 << i)) == 0) continue;
                for (int j = i + 1; j < n; j++)
                {
                    if ((mask & (1 << j)) != 0) edgeCount++;
                }
            }
            return edgeCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Polynomial(int n, bool* adj, long x, int MOD)
        {
            long result = 0;
            int size = 1 << n;
            for (int mask = 0; mask < size; mask++)
            {
                if (IsMatching(n, adj, mask))
                {
                    int edgeCount = CountEdges(n, mask);
                    long sign = (edgeCount % 2 == 0) ? 1 : -1;
                    long xPow = 1;
                    int m = mask;
                    while (m > 0) { if ((m & 1) != 0) xPow = xPow * x % MOD; m >>= 1; }
                    result = (result + sign * xPow % MOD + MOD) % MOD;
                }
            }
            return result;
        }
    }
}
