using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Compress.Coordinate
{
    public static unsafe class Discretize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* src, int len)
        {
            if (len == 0) return 0;
            int unique = 1;
            for (int i = 1; i < len; i++)
            {
                int key = src[i];
                int j = i - 1;
                while (j >= 0 && src[j] > key)
                {
                    src[j + 1] = src[j];
                    j--;
                }
                src[j + 1] = key;
            }
            for (int i = 1; i < len; i++)
            {
                if (src[i] != src[unique - 1])
                    src[unique++] = src[i];
            }
            return unique;
        }
    }
}