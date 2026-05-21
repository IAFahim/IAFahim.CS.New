namespace IAFahim.Optimization.Knapsack
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BoundedKnapsack
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long BinarySplit(long* w, long* v, int* cnt, int n, int cap)
        {
            long* dp = stackalloc long[cap + 1];
            for (int i = 0; i <= cap; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
            {
                int k = 1;
                int c = cnt[i];
                while (c > 0)
                {
                    int take = Math.Min(k, c);
                    long tw = take * w[i];
                    long tv = take * v[i];
                    for (int j = cap; j >= tw; j--)
                    {
                        long cand = dp[j - (int)tw] + tv;
                        if (cand > dp[j]) dp[j] = cand;
                    }
                    c -= take;
                    k <<= 1;
                }
            }
            return dp[cap];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MonotoneQueue(long* w, long* v, int* cnt, int n, int cap)
        {
            long* dp = stackalloc long[cap + 1];
            for (int i = 0; i <= cap; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
            {
                for (int r = 0; r < w[i] && r <= cap; r++)
                {
                    int* q = stackalloc int[cap + 2];
                    int head = 0, tail = 0;
                    int maxJ = (int)((cap - r) / w[i]);
                    for (int j = 0; j <= maxJ; j++)
                    {
                        long val = dp[r + j * (int)w[i]] - j * v[i];
                        while (tail > head && val >= dp[r + q[tail - 1] * (int)w[i]] - q[tail - 1] * v[i])
                            tail--;
                        q[tail++] = j;
                        while (q[head] < j - cnt[i]) head++;
                        long best = dp[r + q[head] * (int)w[i]] + (j - q[head]) * v[i];
                        if (best > dp[r + j * (int)w[i]]) dp[r + j * (int)w[i]] = best;
                    }
                }
            }
            return dp[cap];
        }
    }
}
