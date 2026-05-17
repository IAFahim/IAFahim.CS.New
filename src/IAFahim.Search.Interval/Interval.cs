using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Interval
{
    public struct Interval
    {
        public int Start;
        public int End;
    }

    public static unsafe class MergeIntervals
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(Interval* ptr, int len)
        {
            if (len <= 1)
                return len;
            int outIdx = 0;
            for (int i = 1; i < len; i++)
            {
                if (ptr[outIdx].End >= ptr[i].Start)
                {
                    if (ptr[i].End > ptr[outIdx].End)
                        ptr[outIdx].End = ptr[i].End;
                }
                else
                {
                    outIdx++;
                    ptr[outIdx] = ptr[i];
                }
            }
            return outIdx + 1;
        }
    }

    public static unsafe class IntersectIntervals
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(Interval* a, int aLen, Interval* b, int bLen, Interval* dst)
        {
            int i = 0, j = 0, k = 0;
            while (i < aLen && j < bLen)
            {
                int lo = Math.Max(a[i].Start, b[j].Start);
                int hi = Math.Min(a[i].End, b[j].End);
                if (lo <= hi)
                {
                    dst[k].Start = lo;
                    dst[k].End = hi;
                    k++;
                }
                if (a[i].End < b[j].End)
                    i++;
                else
                    j++;
            }
            return k;
        }
    }

    public static unsafe class NormalizeIntervals
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(Interval* ptr, int len)
        {
            return MergeIntervals.Run(ptr, len);
        }
    }
}