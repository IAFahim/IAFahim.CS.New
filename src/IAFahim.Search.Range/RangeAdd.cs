using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Range
{
    public static unsafe class RangeAdd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* diff, int len, int start, int end, int val)
        {
            if ((uint)start >= (uint)len) return;
            if (end >= len) end = len - 1;
            if (start > end) return;
            diff[start] += val;
            if (end + 1 < len)
                diff[end + 1] -= val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Materialize(int* dst, int* diff, int len)
        {
            int cur = 0;
            for (int i = 0; i < len; i++)
            {
                cur += diff[i];
                dst[i] = cur;
            }
        }
    }
}