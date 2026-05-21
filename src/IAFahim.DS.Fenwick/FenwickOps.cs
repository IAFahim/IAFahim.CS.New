namespace IAFahim.DS.Fenwick
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FenwickAdd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* bit, int bitLen, int idx, int val)
        {
            while (idx < bitLen)
            {
                bit[idx] += val;
                idx = (idx + 1) & -idx;
                if (idx == 0) break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunLong(long* bit, int bitLen, int idx, long val)
        {
            while (idx < bitLen)
            {
                bit[idx] += val;
                idx = (idx + 1) & -idx;
                if (idx == 0) break;
            }
        }
    }

    public static unsafe class FenwickSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* bit, int idx)
        {
            int res = 0;
            while (idx >= 0)
            {
                res += bit[idx];
                idx = (idx + 1) & -idx;
                if (idx == 0) break;
            }
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RunLong(long* bit, int idx)
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

    public static unsafe class Fenwick2DAdd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* bit, int n, int m, int x, int y, long val)
        {
            for (int i = x; i < n; i += i & -i)
            {
                for (int j = y; j < m; j += j & -j)
                {
                    bit[i * m + j] += val;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunLinear(long* bit, int n, int x, int y, long val)
        {
            for (int i = x; i < n; i += i & -i)
            {
                for (int j = y; j < n; j += j & -j)
                {
                    bit[i * n + j] += val;
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
            for (int i = x; i >= 0; i -= i & -i)
            {
                for (int j = y; j >= 0; j -= j & -j)
                {
                    res += bit[i * m + j];
                }
            }
            return res;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RunLinear(long* bit, int n, int x, int y)
        {
            long res = 0;
            for (int i = x; i >= 0; i -= i & -i)
            {
                for (int j = y; j >= 0; j -= j & -j)
                {
                    res += bit[i * n + j];
                }
            }
            return res;
        }
    }

    public static unsafe class FenwickRangeAdd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* bit, int bitLen, int idx, long val)
        {
            FenwickAdd.RunLong(bit, bitLen, idx, val);
            FenwickAdd.RunLong(bit, bitLen, idx, val * idx);
        }

        public static void RangeAdd(long* bit, int bitLen, int l, int r, long val)
        {
            Run(bit, bitLen, l, val);
            Run(bit, bitLen, r + 1, -val);
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