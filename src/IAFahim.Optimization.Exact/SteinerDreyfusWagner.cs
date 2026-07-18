namespace IAFahim.Optimization.Exact
{
    using System.Runtime.CompilerServices;

    public static unsafe class SteinerDreyfusWagner
    {
        public static long Run(int n, int m, int* from, int* to, long* w, bool* terminals, long inf, long* dp)
        {
            if (n <= 0 || m <= 0) return 0;
            int full = 1 << m;
            int dpLen = full * n;
            for (int i = 0; i < dpLen; i++) dp[i] = inf;

            int ti = 0;
            for (int v = 0; v < n; v++)
            {
                if (!terminals[v]) continue;
                if (ti >= m) break;
                int mask = 1 << ti;
                dp[mask * n + v] = 0;
                RelaxDistances(n, mask, inf, w, dp);
                ti++;
            }

            for (int mask = 1; mask < full; mask++)
            {
                if ((mask & (mask - 1)) == 0) continue;
                MergeSubsets(n, mask, inf, dp);
                RelaxDistances(n, mask, inf, w, dp);
            }

            int fullMask = full - 1;
            long ans = inf;
            for (int v = 0; v < n; v++)
            {
                long cand = dp[fullMask * n + v];
                if (cand < ans) ans = cand;
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
                        long a = dp[sub * n + v];
                        long b = dp[other * n + v];
                        if (a != inf && b != inf)
                        {
                            long cand = a + b;
                            if (cand < best) best = cand;
                        }
                    }
                    sub = (sub - 1) & mask;
                }
                if (best < dp[mask * n + v]) dp[mask * n + v] = best;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RelaxDistances(int n, int mask, long inf, long* w, long* dp)
        {
            for (int iter = 0; iter < n; iter++)
            {
                bool changed = false;
                for (int i = 0; i < n; i++)
                {
                    long di = dp[mask * n + i];
                    if (di == inf) continue;
                    for (int j = 0; j < n; j++)
                    {
                        long wij = w[i * n + j];
                        if (wij == inf) continue;
                        long cand = di + wij;
                        if (cand < dp[mask * n + j])
                        {
                            dp[mask * n + j] = cand;
                            changed = true;
                        }
                    }
                }
                if (!changed) break;
            }
        }
    }
}
