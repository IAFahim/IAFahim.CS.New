using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Subset
{
    public static unsafe class EnumerateSupersets
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int subMask, int maxMask, int* dst)
        {
            int count = 0;
            int free = maxMask & ~subMask;
            int s = free;
            while (true)
            {
                dst[count++] = subMask | s;
                if (s == 0) break;
                s = (s - 1) & free;
            }
            return count;
        }
    }
}