namespace IAFahim.Optimization.Exact
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HamiltonianPath
    {
        public static long Run(int n, long* w, long inf, long* dp, int* perm)
        {
            int size = 1 << n;
            for (int i = 0; i < size * n; i++) dp[i] = inf;
            for (int v = 0; v < n; v++)
                dp[(1 << v) * n + v] = 0;
            for (int mask = 1; mask < size; mask++)
            {
                for (int v = 0; v < n; v++)
                {
                    int bit = 1 << v;
                    if ((mask & bit) == 0) continue;
                    if (dp[mask * n + v] >= inf) continue;
                    for (int u = 0; u < n; u++)
                    {
                        if ((mask & (1 << u)) != 0) continue;
                        long wvu = w[v * n + u];
                        if (wvu >= inf) continue;
                        long cand = dp[mask * n + v] + wvu;
                        int newMask = mask | (1 << u);
                        if (cand < dp[newMask * n + u]) dp[newMask * n + u] = cand;
                    }
                }
            }
            long bestFinal = inf;
            for (int v = 0; v < n; v++)
            {
                long cand = dp[(size - 1) * n + v];
                if (cand < bestFinal) bestFinal = cand;
            }
            return bestFinal;
        }
    }
}
