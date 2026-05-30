namespace IAFahim.Algebra.GraphPoly
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Matching
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Polynomial(int n, bool* adj, long x, int MOD)
        {
            int size = 1 << n;
            long* dp = stackalloc long[size];
            for (int i = 0; i < size; i++) dp[i] = 0L;
            dp[0] = 1L;

            for (int mask = 1; mask < size; mask++)
            {
                int v = -1;
                for (int i = 0; i < n; i++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        v = i;
                        break;
                    }
                }

                int maskWithoutV = mask ^ (1 << v);
                long ways = dp[maskWithoutV];

                for (int u = v + 1; u < n; u++)
                {
                    if ((mask & (1 << u)) != 0 && adj[(long)v * n + u])
                    {
                        int maskWithoutUAndV = maskWithoutV ^ (1 << u);
                        long edgeWays = (dp[maskWithoutUAndV] * x) % MOD;
                        ways = (ways + edgeWays) % MOD;
                    }
                }

                dp[mask] = ways;
            }

            return dp[size - 1];
        }
    }
}
