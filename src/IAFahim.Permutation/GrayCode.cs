using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Permutation
{
    public static unsafe class GrayCode
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToGray(int n)
        {
            uint u = (uint)n;
            return (int)(u ^ (u >> 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FromGray(int g)
        {
            uint n = (uint)g;
            n ^= n >> 1;
            n ^= n >> 2;
            n ^= n >> 4;
            n ^= n >> 8;
            n ^= n >> 16;
            return (int)n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Generate(int* dst, int bits)
        {
            int count = 1 << bits;
            for (int i = 0; i < count; i++)
                dst[i] = ToGray(i);
        }
    }
}