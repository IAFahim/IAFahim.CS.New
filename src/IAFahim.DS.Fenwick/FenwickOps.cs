namespace IAFahim.DS.Fenwick
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FenwickAdd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* bit, int bitLen, int idx, int val)
        {
            for (idx += 1; idx <= bitLen; idx += idx & -idx)
                bit[idx] += val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunLong(long* bit, int bitLen, int idx, long val)
        {
            for (idx += 1; idx <= bitLen; idx += idx & -idx)
                bit[idx] += val;
        }
    }

    public static unsafe class FenwickSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* bit, int idx)
        {
            int res = 0;
            for (idx += 1; idx > 0; idx -= idx & -idx)
                res += bit[idx];
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RunLong(long* bit, int idx)
        {
            long res = 0;
            for (idx += 1; idx > 0; idx -= idx & -idx)
                res += bit[idx];
            return res;
        }
    }

    public static unsafe class FenwickRangeSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* bit, int l, int r)
        {
            if (l > r) return 0;
            int res = FenwickSum.Run(bit, r);
            if (l == 0) return res;
            return res - FenwickSum.Run(bit, l - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RunLong(long* bit, int l, int r)
        {
            if (l > r) return 0;
            long res = FenwickSum.RunLong(bit, r);
            if (l == 0) return res;
            return res - FenwickSum.RunLong(bit, l - 1);
        }
    }

    public static unsafe class FenwickLowerBound
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long* bit, int n, long target)
        {
            int idx = 0;
            int bitMask = 1;
            while ((bitMask << 1) <= n)
            {
                bitMask <<= 1;
            }
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
    }

    public static unsafe class Fenwick2DAdd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* bit, int n, int m, int x, int y, long val)
        {
            for (int i = x + 1; i <= n; i += i & -i)
            {
                for (int j = y + 1; j <= m; j += j & -j)
                {
                    bit[i * (m + 1) + j] += val;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunLinear(long* bit, int n, int x, int y, long val)
        {
            for (int i = x + 1; i <= n; i += i & -i)
            {
                for (int j = y + 1; j <= n; j += j & -j)
                {
                    bit[i * (n + 1) + j] += val;
                }
            }
        }
    }

    public static unsafe class Fenwick2DSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* bit, int n, int m, int x, int y)
        {
            long res = 0;
            for (int i = x + 1; i > 0; i -= i & -i)
            {
                for (int j = y + 1; j > 0; j -= j & -j)
                {
                    res += bit[i * (m + 1) + j];
                }
            }
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RunLinear(long* bit, int n, int x, int y)
        {
            long res = 0;
            for (int i = x + 1; i > 0; i -= i & -i)
            {
                for (int j = y + 1; j > 0; j -= j & -j)
                {
                    res += bit[i * (n + 1) + j];
                }
            }
            return res;
        }
    }

    public static unsafe class FenwickRangeAdd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* bit1, long* bit2, int bitLen, int idx, long val)
        {
            long valStart = val * idx;
            for (idx += 1; idx <= bitLen; idx += idx & -idx)
            {
                bit1[idx] += val;
                bit2[idx] += valStart;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RangeAdd(long* bit1, long* bit2, int bitLen, int l, int r, long val)
        {
            Run(bit1, bit2, bitLen, l, val);
            Run(bit1, bit2, bitLen, r + 1, -val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PrefixQuery(long* bit1, long* bit2, int idx)
        {
            long res1 = 0, res2 = 0;
            int q = idx + 1;
            for (idx += 1; idx > 0; idx -= idx & -idx)
            {
                res1 += bit1[idx];
                res2 += bit2[idx];
            }
            return res1 * q - res2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RangeQuery(long* bit1, long* bit2, int l, int r)
        {
            if (l > r) return 0;
            long res = PrefixQuery(bit1, bit2, r);
            if (l == 0) return res;
            return res - PrefixQuery(bit1, bit2, l - 1);
        }
    }

    public static unsafe class FenwickPointQuery
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* bit, int idx)
        {
            return FenwickSum.RunLong(bit, idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PrefixSum(long* bit, int idx)
        {
            return FenwickSum.RunLong(bit, idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RangeQuery(long* bit, int l, int r)
        {
            return FenwickRangeSum.RunLong(bit, l, r);
        }
    }
}