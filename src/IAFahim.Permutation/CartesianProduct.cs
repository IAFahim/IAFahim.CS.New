using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Permutation
{
    public static unsafe class CartesianProduct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ComputeCount(int* setSizes, int setCount)
        {
            int total = 1;
            for (int i = 0; i < setCount; i++)
                total *= setSizes[i];
            return total;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetAt(int* setSizes, int setCount, int index, int* dst)
        {
            for (int i = setCount - 1; i >= 0; i--)
            {
                dst[i] = index % setSizes[i];
                index /= setSizes[i];
            }
        }
    }
}