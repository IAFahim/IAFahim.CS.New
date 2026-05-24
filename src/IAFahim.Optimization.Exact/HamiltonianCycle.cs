namespace IAFahim.Optimization.Exact
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HamiltonianCycle
    {
        public static long Run(int n, long* w, long inf, long* dp)
        {
            if (n < 3) return inf;
            int size = 1 << n;
            for (int i = 0; i < size * n; i++) dp[i] = inf;
            dp[(1 << 0) * n + 0] = 0;
            for (int mask = 1; mask < size; mask++)
            {
                ProcessCycleMask(n, w, inf, dp, mask);
            }
            return FindMinBestCycle(n, w, inf, dp, size - 1);
        }

        private static void ProcessCycleMask(int n, long* w, long inf, long* dp, int mask)
        {
            for (int last = 0; last < n; last++)
            {
                if ((mask & (1 << last)) == 0 || dp[mask * n + last] >= inf) continue;
                TryExtendPath(n, w, inf, dp, mask, last);
            }
        }

        private static void TryExtendPath(int n, long* w, long inf, long* dp, int mask, int last)
        {
            for (int u = 0; u < n; u++)
            {
                if ((mask & (1 << u)) != 0 || w[last * n + u] >= inf) continue;
                long cand = dp[mask * n + last] + w[last * n + u];
                if (cand < dp[(mask | (1 << u)) * n + u]) dp[(mask | (1 << u)) * n + u] = cand;
            }
        }

        private static long FindMinBestCycle(int n, long* w, long inf, long* dp, int fullMask)
        {
            long best = inf;
            for (int last = 1; last < n; last++)
            {
                if (dp[fullMask * n + last] >= inf || w[last * n + 0] >= inf) continue;
                long cand = dp[fullMask * n + last] + w[last * n + 0];
                if (cand < best) best = cand;
            }
            return best;
        }
    }
}
