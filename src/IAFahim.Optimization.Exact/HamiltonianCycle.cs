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
                for (int last = 0; last < n; last++)
                {
                    int bit = 1 << last;
                    if ((mask & bit) == 0) continue;
                    if (dp[mask * n + last] >= inf) continue;
                    for (int u = 0; u < n; u++)
                    {
                        if ((mask & (1 << u)) != 0) continue;
                        long wlu = w[last * n + u];
                        if (wlu >= inf) continue;
                        int newMask = mask | (1 << u);
                        long current = dp[mask * n + last];
                        if (current != inf && wlu != inf)
                        {
                            long cand = current + wlu;
                            if (cand < dp[newMask * n + u]) dp[newMask * n + u] = cand;
                        }
                    }
                }
            }
            long best = inf;
            int full = size - 1;
            for (int last = 1; last < n; last++)
            {
                if (dp[full * n + last] >= inf) continue;
                long wln = w[last * n + 0];
                if (wln >= inf) continue;
                long current = dp[full * n + last];
                if (current != inf && wln != inf)
                {
                    long cand = current + wln;
                    if (cand < best) best = cand;
                }
            }
            return best;
        }
    }
}
