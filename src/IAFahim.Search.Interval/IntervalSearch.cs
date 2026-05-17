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
                if ((uint)starts[i] <= (uint)point && (uint)point < (uint)ends[i])
                {
                    count++;
                }
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortByStart(int* starts, int* ends, int len)
        {
            for (int i = 1; i < len; i++)
            {
                int ks = starts[i];
                int ke = ends[i];
                int j = i - 1;
                while (j >= 0 && starts[j] > ks)
                {
                    starts[j + 1] = starts[j];
                    ends[j + 1] = ends[j];
                    j--;
                }
                starts[j + 1] = ks;
                ends[j + 1] = ke;
            }
        }
    }
}