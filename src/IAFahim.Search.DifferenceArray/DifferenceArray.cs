using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.DifferenceArray
{
    public static unsafe class Diff
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Apply(int* diff, int len, int start, int end, int val)
        {
            if ((uint)start >= (uint)len || (uint)end >= (uint)len)
            {
                return;
            }
            diff[start] += val;
            if (end + 1 < len)
            {
                diff[end + 1] -= val;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Build(int* output, int* diff, int len)
        {
            int cur = 0;
            for (int i = 0; i < len; i++)
            {
                cur += diff[i];
                output[i] = cur;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RangeSum(int* prefix, int idx)
        {
            if ((uint)idx >= (uint)prefix[0])
            {
                return 0;
            }
            return prefix[idx + 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PrefixFromDiff(int* prefix, int* diff, int len)
        {
            prefix[0] = len;
            int cur = 0;
            for (int i = 0; i < len; i++)
            {
                cur += diff[i];
                prefix[i + 1] = cur;
            }
        }
    }
}