using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Interval
{
    public static unsafe class IntervalSearch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountOverlapping(int* starts, int* ends, int len, int targetStart, int targetEnd)
        {
            int count = 0;
            for (int i = 0; i < len; i++)
            {
                if (starts[i] < targetEnd && targetStart < ends[i])
                {
                    count++;
                }
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindFirstOverlapping(int* starts, int* ends, int len, int targetStart, int targetEnd)
        {
            for (int i = 0; i < len; i++)
            {
                if (starts[i] < targetEnd && targetStart < ends[i])
                {
                    return i;
                }
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountContained(int* starts, int* ends, int len, int point)
        {
            int count = 0;
            for (int i = 0; i < len; i++)
            {
                if (starts[i] <= point && point < ends[i])
                {
                    count++;
                }
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortByStart(int* starts, int* ends, int len)
        {
            for (int i = (len >> 1) - 1; i >= 0; i--) SiftDownInterval(starts, ends, i, len);
            for (int end = len - 1; end > 0; end--)
            {
                SwapInterval(starts, ends, 0, end);
                SiftDownInterval(starts, ends, 0, end);
            }
        }

        private static void SiftDownInterval(int* starts, int* ends, int i, int n)
        {
            while (true)
            {
                int l = 2 * i + 1, r = 2 * i + 2, m = i;
                if (l < n && starts[l] > starts[m]) m = l;
                if (r < n && starts[r] > starts[m]) m = r;
                if (m == i) break;
                SwapInterval(starts, ends, i, m);
                i = m;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SwapInterval(int* starts, int* ends, int a, int b)
        {
            int ts = starts[a]; starts[a] = starts[b]; starts[b] = ts;
            int te = ends[a]; ends[a] = ends[b]; ends[b] = te;
        }
    }
}