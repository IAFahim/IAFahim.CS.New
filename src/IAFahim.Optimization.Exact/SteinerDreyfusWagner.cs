namespace IAFahim.Optimization.Exact
{
    using System;

    public static unsafe class SteinerDreyfusWagner
    {
        public static long Run(int n, int m, int* from, int* to, long* w, bool* terminals, long inf, long* dp)
        {
            for (int i = 0; i < (1 << m) * n; i++) dp[i] = inf;
            for (int v = 0; v < n; v++)
            {
                if (terminals[v]) dp[1 * n + v] = 0;
            }
            for (int mask = 1; mask < (1 << m); mask++)
            {
                if ((mask & (mask - 1)) == 0) continue;
                for (int v = 0; v < n; v++)
                {
                    long best = inf;
                    int sub = mask;
                    while ((sub = (sub - 1) & mask) > 0)
                    {
                        if (sub == mask) { sub = (sub - 1) & mask; continue; }
                        int other = mask ^ sub;
                        long cand = dp[sub * n + v] + dp[other * n + v];
                        if (cand < best) best = cand;
                        if (sub == 0) break;
                    }
                    dp[mask * n + v] = best;
                }
                for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                for (int k = 0; k < n; k++)
                {
                    long cand = dp[mask * n + i] + w[i * n + j] + w[j * n + k];
                    if (cand < dp[mask * n + k]) dp[mask * n + k] = cand;
                }
            }
            return dp[((1 << m) - 1) * n];
        }
    }
}
