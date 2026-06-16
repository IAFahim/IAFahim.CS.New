namespace IAFahim.Optimization.Exact
{
    using System;

    public static unsafe class TspMeetInMiddle
    {
        // Largest instance solved on the stack. The dynamic-programming table holds
        // (1 << n) * n longs, so this bound keeps the stackalloc at 2^12 * 12 * 8 bytes
        // (~384 KB) to avoid stack overflow on constrained (mobile) threads. Larger n
        // returns inf (not solvable within the stack budget).
        private const int MaxStackSolvableN = 12;

        public static long Run(int n, long* w, long inf)
        {
            if (n < 3) return 0;
            if (n > MaxStackSolvableN) return inf;

            int size = 1 << n;
            int total = size * n;
            long* dp = stackalloc long[total];
            for (int i = 0; i < total; i++) dp[i] = inf;

            // Path cost starting at city 0, visiting exactly the cities in mask, ending at city 0.
            dp[(1 << 0) * n + 0] = 0;

            for (int mask = 1; mask < size; mask++)
            {
                long* maskRow = dp + mask * n;
                for (int last = 0; last < n; last++)
                {
                    if ((mask & (1 << last)) == 0) continue;
                    long cur = maskRow[last];
                    if (cur >= inf) continue;

                    long* wLastRow = w + last * n;
                    for (int u = 0; u < n; u++)
                    {
                        if ((mask & (1 << u)) != 0) continue;
                        long edge = wLastRow[u];
                        if (edge >= inf) continue;
                        long cand = cur + edge;
                        long* slot = dp + (mask | (1 << u)) * n + u;
                        if (cand < *slot) *slot = cand;
                    }
                }
            }

            int fullMask = size - 1;
            long* fullRow = dp + fullMask * n;
            long best = inf;
            for (int last = 1; last < n; last++)
            {
                long pathCost = fullRow[last];
                if (pathCost >= inf) continue;
                long back = w[last * n + 0];
                if (back >= inf) continue;
                long cand = pathCost + back;
                if (cand < best) best = cand;
            }
            return best;
        }
    }
}
