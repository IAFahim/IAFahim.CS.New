namespace IAFahim.Optimization.Exact
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SteinerDreyfusWagner
    {
        private const int SingletonMaskBase = 1;

        public static long Run(int n, int m, int* from, int* to, long* w, bool* terminals, long inf, long* dp)
        {
            int full = 1 << m;
            int dpLen = full * n;
            for (int i = 0; i < dpLen; i++) dp[i] = inf;
            for (int v = 0; v < n; v++)
            {
                if (terminals[v]) dp[SingletonMaskBase * n + v] = 0;
            }
            for (int mask = 1; mask < full; mask++)
            {
                if ((mask & (mask - 1)) == 0) continue;
                MergeSubsets(n, mask, inf, dp);
                RelaxThroughGraph(n, mask, inf, w, dp);
            }
            int fullMask = full - 1;
            long ans = inf;
            for (int v = 0; v < n; v++)
            {
                long candAns = dp[fullMask * n + v];
                if (candAns < ans) ans = candAns;
            }
            return ans;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MergeSubsets(int n, int mask, long inf, long* dp)
        {
            for (int v = 0; v < n; v++)
            {
                long best = inf;
                int sub = (mask - 1) & mask;
                while (sub > 0)
                {
                    int other = mask ^ sub;
                    if (sub < other)
                    {
                        long cand = dp[sub * n + v] + dp[other * n + v];
                        if (cand < best) best = cand;
                    }
                    sub = (sub - 1) & mask;
                }
                dp[mask * n + v] = best;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RelaxThroughGraph(int n, int mask, long inf, long* w, long* dp)
        {
            for (int i = 0; i < n; i++)
            {
                long d1 = dp[mask * n + i];
                if (d1 == inf) continue;
                RelaxFromSource(n, mask, inf, d1, i, w, dp);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RelaxFromSource(int n, int mask, long inf, long d1, int i, long* w, long* dp)
        {
            for (int j = 0; j < n; j++)
            {
                long w1 = w[i * n + j];
                if (w1 == inf) continue;
                for (int k = 0; k < n; k++)
                {
                    long w2 = w[j * n + k];
                    if (w2 != inf)
                    {
                        long cand = d1 + w1 + w2;
                        if (cand < dp[mask * n + k]) dp[mask * n + k] = cand;
                    }
                }
            }
        }
    }
}
