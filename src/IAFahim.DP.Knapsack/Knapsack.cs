namespace IAFahim.DP.Knapsack
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Knapsack01
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long cap, long* w, long* v, long* dp)
        {
            for (long i = 0; i <= cap; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
            {
                for (long c = cap; c >= w[i]; c--)
                {
                    long val = dp[(int)(c - w[i])] + v[i];
                    if (val > dp[(int)c]) dp[(int)c] = val;
                }
            }
            return dp[(int)cap];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RunSpaceEfficient(int n, long cap, long* w, long* v, long* dp)
        {
            for (long i = 0; i <= cap; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
            {
                for (long c = cap; c >= w[i]; c--)
                {
                    long val = dp[(int)(c - w[i])] + v[i];
                    if (val > dp[(int)c]) dp[(int)c] = val;
                }
            }
            return dp[(int)cap];
        }
    }

    public static unsafe class KnapsackUnbounded
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long cap, long* w, long* v, long* dp)
        {
            for (long i = 0; i <= cap; i++) dp[i] = 0;
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
            return dp[(int)cap];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RunSpaceEfficient(int n, long cap, long* w, long* v, long* dp)
        {
            for (long i = 0; i <= cap; i++) dp[i] = 0;
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
            return dp[(int)cap];
        }
    }

    public static unsafe class KnapsackBounded
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long cap, long* w, long* v, long* cnt, long* dp)
        {
            for (long i = 0; i <= cap; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
            {
                if (cnt[i] == 1)
                {
                    for (long c = cap; c >= w[i]; c--)
                    {
                        long val = dp[(int)(c - w[i])] + v[i];
                        if (val > dp[(int)c]) dp[(int)c] = val;
                    }
                }
                else if (cnt[i] > 1)
                {
                    long maxUse = Math.Min(cnt[i], cap / w[i]);
                    for (long c = cap; c >= 0; c--)
                    {
                        for (long k = 1; k <= maxUse && c >= k * w[i]; k++)
                        {
                            long val = dp[(int)(c - k * w[i])] + k * v[i];
                            if (val > dp[(int)c]) dp[(int)c] = val;
                        }
                    }
                }
            }
            return dp[(int)cap];
        }
    }

    public static unsafe class SubsetSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int n, long target, long* a, bool* dp)
        {
            for (long i = 0; i <= target; i++) dp[i] = false;
            dp[0] = true;
            for (int i = 0; i < n; i++)
            {
                for (long s = target; s >= a[i]; s--)
                {
                    if (dp[(int)(s - a[i])]) dp[(int)s] = true;
                }
            }
            return dp[(int)target];
        }
    }

    public static unsafe class BitsetSubsetSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long target, long* a, long* bits)
        {
            int size = (int)((target + 63) >> 6);
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