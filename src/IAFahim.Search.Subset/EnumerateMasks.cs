using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Subset
{
    public static unsafe class EnumerateMasks
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(int bits)
        {
            return 1 << bits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* dst, int bits)
        {
            int count = 1 << bits;
            for (int i = 0; i < count; i++)
                dst[i] = i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountPopBits(int mask)
        {
            int count = 0;
            int m = mask;
            while (m != 0)
            {
                count++;
                m &= m - 1;
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextWithSamePopCount(int mask)
        {
            int c = (mask & -mask);
            int r = mask + c;
            return (((r ^ mask) >> 2) / c) | r;
        }
    }
}