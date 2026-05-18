namespace IAFahim.DS.SegmentTree
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DualSegmentApply
    {
        public static void RangeAddInt64(long* bit, int idx, long val)
        {
            bit[idx] += val;
        }

        public static void RangeAddInt32(int* bit, int idx, int val)
        {
            bit[idx] += val;
        }
    }

    public static unsafe class DualSegmentGet
    {
        public static long RangeSumInt64(long* bit, int l, int r)
        {
            long res = 0;
            for (int i = r; i >= l; i--)
            {
                res += bit[i];
            }
            return res;
        }

        public static int RangeSumInt32(int* bit, int l, int r)
        {
            int res = 0;
            for (int i = r; i >= l; i--)
            {
                res += bit[i];
            }
            return res;
        }
    }
}