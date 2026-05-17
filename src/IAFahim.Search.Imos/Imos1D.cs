using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Imos
{
    public static unsafe class Imos1D
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(int* diff, int len, int start, int end, int val)
        {
            if (start < 0) start = 0;
            if (end >= len) end = len - 1;
            if (start > end) return;
            diff[start] += val;
            if (end + 1 < len)
                diff[end + 1] -= val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Build(int* dst, int* diff, int len)
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