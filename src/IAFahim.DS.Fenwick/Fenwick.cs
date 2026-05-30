namespace IAFahim.DS.Fenwick
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Fenwick
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddInt64(long* bit, int n, int idx, long val)
        {
            for (idx += 1; idx <= n; idx += idx & -idx)
                bit[idx] += val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SumInt64(long* bit, int idx)
        {
            long res = 0;
            for (idx += 1; idx > 0; idx -= idx & -idx)
                res += bit[idx];
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RangeSumInt64(long* bit, int l, int r)
        {
            if (l > r) return 0;
            long res = SumInt64(bit, r);
            if (l == 0) return res;
            return res - SumInt64(bit, l - 1);
        }

        /// <summary>
        /// Lower bound: finds smallest index i where prefix sum [0, i] >= target.
        /// Returns n if target exceeds total sum.
        /// Assumes all values are non-negative.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LowerBoundInt64(long* bit, int n, long target)
        {
            if (target <= 0) return 0;
            int idx = 0;
            int bitMask = 1;
            while ((bitMask << 1) <= n) bitMask <<= 1;
            for (; bitMask != 0; bitMask >>= 1)
            {
                int next = idx + bitMask;
                if (next <= n && bit[next] < target)
                {
                    idx = next;
                    target -= bit[next];
                }
            }
            return idx;
        }

        /// <summary>
        /// Upper bound: finds smallest index i where prefix sum [0, i] > target.
        /// Returns n if target >= total sum.
        /// Assumes all values are non-negative.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UpperBoundInt64(long* bit, int n, long target)
        {
            if (target < 0) return 0;
            long total = SumInt64(bit, n - 1); // total sum of array [0..n-1]
            if (target >= total) return n;
            int idx = 0;
            int bitMask = 1;
            while ((bitMask << 1) <= n) bitMask <<= 1;
            for (; bitMask != 0; bitMask >>= 1)
            {
                int next = idx + bitMask;
                if (next <= n && bit[next] <= target)
                {
                    idx = next;
                    target -= bit[next];
                }
            }
            return idx;
        }
    }
}
