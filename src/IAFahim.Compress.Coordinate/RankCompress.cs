using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Compress.Coordinate
{
    public static unsafe class RankCompress
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* src, int* dst, int* tmpSorted, int len)
        {
            if (len == 0) return 0;
            for (int i = 0; i < len; i++)
                tmpSorted[i] = src[i];
            for (int i = 1; i < len; i++)
            {
                int key = tmpSorted[i];
                int j = i - 1;
                while (j >= 0 && tmpSorted[j] > key)
                {
                    tmpSorted[j + 1] = tmpSorted[j];
                    j--;
                }
                tmpSorted[j + 1] = key;
            }
            int unique = 1;
            for (int i = 1; i < len; i++)
            {
                if (tmpSorted[i] != tmpSorted[i - 1])
                    tmpSorted[unique++] = tmpSorted[i];
            }
            for (int i = 0; i < len; i++)
            {
                int lo = 0, hi = unique;
                while (lo < hi)
                {
                    int mid = lo + ((hi - lo) >> 1);
                    if (tmpSorted[mid] < src[i])
                        lo = mid + 1;
                    else
                        hi = mid;
                }
                dst[i] = lo;
            }
            return unique;
        }
    }
}