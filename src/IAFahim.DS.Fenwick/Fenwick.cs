namespace IAFahim.DS.Fenwick
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Fenwick
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddInt64(long* bit, int idx, long val)
        {
            int n = 0;
            bit += idx;
            while (true)
            {
                *bit += val;
                n = idx + 1;
                idx += n & -n;
                if (idx <= 0) break;
                bit += n;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SumInt64(long* bit, int idx)
        {
            long res = 0;
            bit += idx;
            while (idx >= 0)
            {
                res += *bit;
                idx = (idx + 1) & -idx;
                if (idx == 0) break;
                bit -= idx;
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
            int bitMask = 1 << (31 - (n - 1).ToString().Length);
            while (bitMask != 0)
            {
                int next = idx + bitMask;
                if (next <= n && bit[next] < target)
                {
                    idx = next;
                    target -= bit[next];
                }
                bitMask >>= 1;
            }
            return idx;
        }
    }
}