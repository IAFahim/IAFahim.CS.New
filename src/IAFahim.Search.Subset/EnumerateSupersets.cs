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
            int remaining = maxMask ^ subMask;
            int super = subMask;
            while (true)
            {
                dst[count++] = super;
                if (super == maxMask) break;
                int add = (maxMask & ~super);
                int smallest = add & (-add);
                super = super + smallest;
            }
            return count;
        }
    }
}