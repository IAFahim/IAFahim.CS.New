namespace IAFahim.DP.Knapsack
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Knapsack01
    {
        public static long Run(int n, long cap, long* w, long* v, long* dp)
        {
            for (int i = 0; i <= cap; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
            {
                for (long c = cap; c >= w[i]; c--)
                {
                    long val = dp[c - (int)w[i]] + v[i];
                    if (val > dp[c]) dp[c] = val;
                }
            }
            return dp[cap];
        }

        public static long RunSpaceEfficient(int n, long cap, long* w, long* v)
        {
            int icap = (int)cap;
            long* dp = stackalloc long[icap + 1];
            for (int i = 0; i <= icap; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
            {
                for (long c = cap; c >= w[i]; c--)
                {
                    long val = dp[(int)(c - w[i])] + v[i];
                    if (val > dp[(int)c]) dp[(int)c] = val;
                }
            }
            long result = dp[icap];
            return result;
        }
    }

    public static unsafe class KnapsackUnbounded
    {
        public static long Run(int n, long cap, long* w, long* v, long* dp)
        {
            for (int i = 0; i <= cap; i++) dp[i] = 0;
            for (long c = 1; c <= cap; c++)
            {
                for (int i = 0; i < n; i++)
                {
                    if (w[i] <= c)
                    {
                        long val = dp[c - (int)w[i]] + v[i];
                        if (val > dp[c]) dp[c] = val;
                    }
                }
            }
            return dp[cap];
        }

        public static long RunSpaceEfficient(int n, long cap, long* w, long* v)
        {
            int icap = (int)cap;
            long* dp = stackalloc long[icap + 1];
            for (int i = 0; i <= icap; i++) dp[i] = 0;
            for (long c = 1; c <= cap; c++)
            {
                for (int i = 0; i < n; i++)
                {
                    if (w[i] <= c)
                    {
                        long val = dp[(int)(c - w[i])] + v[i];
                        if (val > dp[(int)c]) dp[(int)c] = val;
                    }
                }
            }
            return dp[icap];
        }
    }

    public static unsafe class KnapsackBounded
    {
        public static long Run(int n, long cap, long* w, long* v, long* cnt, long* dp)
        {
            for (int i = 0; i <= cap; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
            {
                if (cnt[i] == 1)
                {
                    for (long c = cap; c >= w[i]; c--)
                    {
                        long val = dp[c - (int)w[i]] + v[i];
                        if (val > dp[c]) dp[c] = val;
                    }
                }
                else if (cnt[i] > 1)
                {
                    long maxUse = Math.Min(cnt[i], cap / w[i]);
                    for (long c = cap; c >= 0; c--)
                    {
                        for (long k = 1; k <= maxUse && c >= k * w[i]; k++)
                        {
                            long val = dp[c - (int)(k * w[i])] + k * v[i];
                            if (val > dp[c]) dp[c] = val;
                        }
                    }
                }
            }
            return dp[cap];
        }
    }

    public static unsafe class SubsetSum
    {
        public static bool Run(int n, long target, long* a, bool* dp)
        {
            for (int i = 0; i <= target; i++) dp[i] = false;
            dp[0] = true;
            for (int i = 0; i < n; i++)
            {
                for (long s = target; s >= a[i]; s--)
                {
                    if (dp[s - (int)a[i]]) dp[s] = true;
                }
            }
            return dp[target];
        }
    }

    public static unsafe class BitsetSubsetSum
    {
        public static long Run(int n, long target, long* a)
        {
            int size = (int)((target + 63) >> 6);
            long* bits = stackalloc long[size];
            for (int i = 0; i < size; i++) bits[i] = 0;
            bits[0] = 1L;
            for (int i = 0; i < n; i++)
            {
                long val = a[i];
                int shift = (int)(val & 63);
                int offset = (int)(val >> 6);
                if (shift == 0)
                {
                    for (int j = size - 1; j >= offset; j--)
                        bits[j] |= bits[j - offset];
                }
                else
                {
                    for (int j = size - 1; j >= offset; j--)
                    {
                        bits[j] |= bits[j - offset] << shift;
                        if (j - offset - 1 >= 0)
                            bits[j] |= (long)((ulong)bits[j - offset - 1] >> (64 - shift));
                    }
                }
            }
            int targetIdx = (int)(target >> 6);
            int targetBit = (int)(target & 63);
            return (targetIdx < size && (bits[targetIdx] & (1L << targetBit)) != 0) ? 1 : 0;
        }
    }
}
