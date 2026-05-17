using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Sort.Specialized
{
    public static unsafe class SortInts
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* ptr, int len)
        {
            for (int i = 1; i < len; i++)
            {
                int key = ptr[i];
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