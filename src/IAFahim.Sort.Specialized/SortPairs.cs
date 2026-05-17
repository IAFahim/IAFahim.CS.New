using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Sort.Specialized
{
    public static unsafe class SortPairs
    {
        public struct Pair
        {
            public int Key;
            public int Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(Pair* ptr, int len)
        {
            for (int i = 1; i < len; i++)
            {
                Pair key = ptr[i];
                int j = i - 1;
                while (j >= 0 && ptr[j].Key > key.Key)
                {
                    ptr[j + 1] = ptr[j];
                    j--;
                }
                ptr[j + 1] = key;
            }
        }
    }
}