namespace IAFahim.Optimization.Knapsack
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DivideConquerKnapsack
    {
        public static long Run(long* w, long* v, int* cnt, int n, int cap)
        {
            long* dp = stackalloc long[cap + 1];
            for (int i = 0; i <= cap; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
            {
                if ((long)cnt[i] * w[i] >= cap)
                {
                    for (int j = (int)w[i]; j <= cap; j++)
                    {
                        long cand = dp[j - (int)w[i]] + v[i];
                        if (cand > dp[j]) dp[j] = cand;
                    }
                }
                else
                {
                    int k = 1, c = cnt[i];
                    while (c > 0)
                    {
                        int take = Math.Min(k, c);
                        long tw = take * w[i], tv = take * v[i];
                        for (int j = cap; j >= tw; j--)
                        {
                            long cand = dp[j - (int)tw] + tv;
                            if (cand > dp[j]) dp[j] = cand;
                        }
                        c -= take;
                        k <<= 1;
                    }
                }
            }
            return dp[cap];
        }
    }
}
