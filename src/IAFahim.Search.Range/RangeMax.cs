using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Range
{
    public static unsafe class RangeMax
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BuildSparse(int* dst, int* src, int len)
        {
            int log = 0;
            while ((1 << (log + 1)) <= len) log++;
            for (int i = 0; i < len; i++)
                dst[i] = src[i];
            for (int k = 1; k <= log; k++)
            {
                int half = 1 << (k - 1);
                for (int i = 0; i + (1 << k) <= len; i++)
                {
                    int a = dst[i + (k - 1) * len];
                    int b = dst[i + half + (k - 1) * len];
                    dst[i + k * len] = a > b ? a : b;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Query(int* sparse, int len, int start, int end)
        {
            int range = end - start + 1;
            int k = 0;
            while ((1 << (k + 1)) <= range) k++;
            int a = sparse[start + k * len];
            int b = sparse[end - (1 << k) + 1 + k * len];
            return a > b ? a : b;
        }
    }
}