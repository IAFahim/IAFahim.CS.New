using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Sort.Specialized
{
    public static unsafe class SortInt64s
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* ptr, int len)
        {
            for (int i = 1; i < len; i++)
            {
                long key = ptr[i];
                int j = i - 1;
                while (j >= 0 && ptr[j] > key)
                {
                    ptr[j + 1] = ptr[j];
                    j--;
                }
                ptr[j + 1] = key;
            }
        }
    }
}