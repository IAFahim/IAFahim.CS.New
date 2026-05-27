namespace IAFahim.DS.SegmentTree
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DualSegmentApply
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RangeAddInt64(long* bit, int n, int l, int r, long val)
        {
            AddInt64(bit, n, l, val);
            AddInt64(bit, n, r + 1, -val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddInt64(long* bit, int n, int idx, long val)
        {
            if (idx < 0 || idx > n) return;
            idx++;
            while (idx <= n)
            {
                bit[idx] += val;
                idx += idx & -idx;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RangeAddInt32(int* bit, int n, int l, int r, int val)
        {
            AddInt32(bit, n, l, val);
            AddInt32(bit, n, r + 1, -val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddInt32(int* bit, int n, int idx, int val)
        {
            if (idx < 0 || idx > n) return;
            idx++;
            while (idx <= n)
            {
                bit[idx] += val;
                idx += idx & -idx;
            }
        }
    }

    public static unsafe class DualSegmentGet
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RangeSumInt64(long* bit, int n, int l, int r)
        {
            if (l > r) return 0;
            long sumR = PrefixSumInt64(bit, n, r);
            long sumL = (l > 0) ? PrefixSumInt64(bit, n, l - 1) : 0;
            return sumR - sumL;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PrefixSumInt64(long* bit, int n, int idx)
        {
            long sum = 0;
            idx++;
            while (idx > 0)
            {
                sum += bit[idx];
                idx -= idx & -idx;
            }
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RangeSumInt32(int* bit, int n, int l, int r)
        {
            if (l > r) return 0;
            int sumR = PrefixSumInt32(bit, n, r);
            int sumL = (l > 0) ? PrefixSumInt32(bit, n, l - 1) : 0;
            return sumR - sumL;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PrefixSumInt32(int* bit, int n, int idx)
        {
            int sum = 0;
            idx++;
            while (idx > 0)
            {
                sum += bit[idx];
                idx -= idx & -idx;
            }
            return sum;
        }
    }
}
