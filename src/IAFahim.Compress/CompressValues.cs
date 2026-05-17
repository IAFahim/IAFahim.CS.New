using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Compress
{
    public static unsafe class CompressValues
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* src, long* dst, int len)
        {
            for (int i = 0; i < len; i++)
            {
                dst[i] = src[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RunUnique(int* src, long* dst, int len)
        {
            if (len <= 1)
            {
                if (len == 1)
                    dst[0] = src[0];
                return len;
            }
            int count = 1;
            dst[0] = src[0];
            for (int i = 1; i < len; i++)
            {
                if (src[i] != src[i - 1])
                {
                    dst[count++] = src[i];
                }
            }
            return count;
        }
    }
}