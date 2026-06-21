namespace IAFahim.DP.Knapsack
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Knapsack01
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long cap, long* w, long* v, long* dp)
        {
            InitializeDp(cap, dp);
            for (int i = 0; i < n; i++)
                UpdateKnapsack01(cap, w[i], v[i], dp);
            return dp[(int)cap];
        }

        private static void InitializeDp(long cap, long* dp)
        {
            for (long i = 0; i <= cap; i++) dp[i] = 0;
        }

        private static void UpdateKnapsack01(long cap, long weight, long value, long* dp)
        {
            for (long c = cap; c >= weight; c--)
            {
                long val = dp[(int)(c - weight)] + value;
                if (val > dp[(int)c]) dp[(int)c] = val;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RunSpaceEfficient(int n, long cap, long* w, long* v, long* dp) => Run(n, cap, w, v, dp);
    }

    public static unsafe class KnapsackUnbounded
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long cap, long* w, long* v, long* dp)
        {
            for (long i = 0; i <= cap; i++) dp[i] = 0;
            for (long c = 1; c <= cap; c++)
                UpdateKnapsackUnbounded(c, n, w, v, dp);
            return dp[(int)cap];
        }

        private static void UpdateKnapsackUnbounded(long c, int n, long* w, long* v, long* dp)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RunSpaceEfficient(int n, long cap, long* w, long* v, long* dp) => Run(n, cap, w, v, dp);
    }

    public static unsafe class KnapsackBounded
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long cap, long* w, long* v, long* cnt, long* dp)
        {
            for (long i = 0; i <= cap; i++) dp[i] = 0;
            for (int i = 0; i < n; i++)
                ProcessBoundedItem(cap, w[i], v[i], cnt[i], dp);
            return dp[(int)cap];
        }

        private static void ProcessBoundedItem(long cap, long weight, long value, long count, long* dp)
        {
            if (count == 1)
            {
                UpdateKnapsack01(cap, weight, value, dp);
            }
            else if (count > 1)
            {
                long maxUse = Math.Min(count, cap / weight);
                for (long c = cap; c >= 0; c--)
                    UpdateBoundedCap(c, maxUse, weight, value, dp);
            }
        }

        private static void UpdateKnapsack01(long cap, long weight, long value, long* dp)
        {
            for (long c = cap; c >= weight; c--)
            {
                long val = dp[(int)(c - weight)] + value;
                if (val > dp[(int)c]) dp[(int)c] = val;
            }
        }

        private static void UpdateBoundedCap(long c, long maxUse, long weight, long value, long* dp)
        {
            for (long k = 1; k <= maxUse && c >= k * weight; k++)
            {
                long val = dp[(int)(c - k * weight)] + k * value;
                if (val > dp[(int)c]) dp[(int)c] = val;
            }
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
                UpdateSubsetSum(target, a[i], dp);
            return dp[(int)target];
        }

        private static void UpdateSubsetSum(long target, long val, bool* dp)
        {
            for (long s = target; s >= val; s--)
                if (dp[(int)(s - val)]) dp[(int)s] = true;
        }
    }

    public static unsafe class BitsetSubsetSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long target, long* a, long* bits)
        {
            int size = (int)((target >> 6) + 1);
            for (int i = 0; i < size; i++) bits[i] = 0;
            bits[0] = 1L;
            for (int i = 0; i < n; i++)
                BitsetShiftOr(size, a[i], bits);
            
            int targetIdx = (int)(target >> 6);
            int targetBit = (int)(target & 63);
            return (targetIdx < size && (bits[targetIdx] & (1L << targetBit)) != 0) ? 1 : 0;
        }

        private static void BitsetShiftOr(int size, long val, long* bits)
        {
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
    }
}