using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Range
{
    public static unsafe class RangeSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int* prefix, int start, int end)
        {
            if (start > end) return 0;
            if (start == 0) return prefix[end];
            return prefix[end] - prefix[start - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BuildPrefix(int* dst, int* src, int len)
        {
            if (len == 0) return;
            dst[0] = src[0];
            for (int i = 1; i < len; i++)
                dst[i] = dst[i - 1] + src[i];
        }
    }
}