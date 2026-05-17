using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Compress.Coordinate
{
    public static unsafe class CoordinateCompress
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* src, int* tmp, int* dstMap, int len)
        {
            if (len == 0) return 0;
            for (int i = 0; i < len; i++)
                tmp[i] = src[i];
            for (int i = 1; i < len; i++)
            {
                int key = tmp[i];
                int j = i - 1;
                while (j >= 0 && tmp[j] > key)
                {
                    tmp[j + 1] = tmp[j];
                    j--;
                }
                tmp[j + 1] = key;
            }
            dstMap[0] = tmp[0];
            int unique = 1;
            for (int i = 1; i < len; i++)
            {
                if (tmp[i] != tmp[i - 1])
                    dstMap[unique++] = tmp[i];
            }
            return unique;
        }
    }
}