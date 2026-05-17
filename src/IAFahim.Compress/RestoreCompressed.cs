using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Compress
{
    public static unsafe class RestoreCompressed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* src, int* dst, int len)
        {
            for (int i = 0; i < len; i++)
            {
                dst[i] = (int)src[i];
            }
        }
    }
}