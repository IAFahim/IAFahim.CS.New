using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Unique
{
    public static unsafe class UniqueInts
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* ptr, int len)
        {
            if (len <= 1)
                return len;
            int dst = 1;
            for (int i = 1; i < len; i++)
            {
                if (ptr[i] != ptr[i - 1])
                {
                    ptr[dst++] = ptr[i];
                }
            }
            return dst;
        }
    }
}