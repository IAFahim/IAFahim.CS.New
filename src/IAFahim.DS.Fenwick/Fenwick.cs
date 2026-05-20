namespace IAFahim.DS.Fenwick
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Fenwick
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddInt64(long* bit, int n, int idx, long val)
        {
            while (idx < n)
            {
                bit[idx] += val;
                idx = (idx + 1) & -idx;
                if (idx == 0) break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SumInt64(long* bit, int idx)
        {
            long res = 0;
            while (idx >= 0)
            {
                res += bit[idx];
                idx = (idx + 1) & -idx;
                if (idx == 0) break;
            }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LowerBoundInt64(long* bit, int n, long target)
        {
            int idx = 0;
            for (int bitMask = 1 << 20; bitMask != 0; bitMask >>= 1)
            {
                int next = idx + bitMask;
                if (next < n && bit[next] < target)
                {
                    idx = next;
                    target -= bit[next];
                }
            }
            return Math.Min(idx, n - 1);
        }
    }
}