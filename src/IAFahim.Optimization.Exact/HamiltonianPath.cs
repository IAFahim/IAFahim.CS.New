namespace IAFahim.Optimization.Exact
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HamiltonianPath
    {
        public static long Run(int n, long* w, long inf, long* dp, int* perm)
        {
            InitializePathDp(n, inf, dp);
            int size = 1 << n;
            for (int mask = 1; mask < size; mask++)
            {
                ProcessPathMask(n, w, inf, dp, mask);
            }
            return FindMinBestPath(n, inf, dp, size - 1);
        }

        private static void InitializePathDp(int n, long inf, long* dp)
        {
            for (int i = 0; i < (1 << n) * n; i++) dp[i] = inf;
            for (int v = 0; v < n; v++) dp[(1 << v) * n + v] = 0;
        }

        private static void ProcessPathMask(int n, long* w, long inf, long* dp, int mask)
        {
            for (int v = 0; v < n; v++)
            {
                if ((mask & (1 << v)) == 0 || dp[mask * n + v] >= inf) continue;
                TryExtendPath(n, w, inf, dp, mask, v);
            }
        }

        private static void TryExtendPath(int n, long* w, long inf, long* dp, int mask, int v)
        {
            for (int u = 0; u < n; u++)
            {
                if ((mask & (1 << u)) != 0 || w[v * n + u] >= inf) continue;
                long cand = dp[mask * n + v] + w[v * n + u];
                if (cand < dp[(mask | (1 << u)) * n + u]) dp[(mask | (1 << u)) * n + u] = cand;
            }
        }

        private static long FindMinBestPath(int n, long inf, long* dp, int fullMask)
        {
            long best = inf;
            for (int v = 0; v < n; v++) if (dp[fullMask * n + v] < best) best = dp[fullMask * n + v];
            return best;
        }
    }
}
