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
            
            InsertionSort(tmpSorted, len);
            int unique = Unique(tmpSorted, len);
            
            for (int i = 0; i < len; i++)
                dst[i] = BinarySearch(tmpSorted, unique, src[i]);
            
            return unique;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InsertionSort(int* arr, int len)
        {
            for (int i = 1; i < len; i++)
            {
                int key = arr[i];
                int j = i - 1;
                while (j >= 0 && arr[j] > key)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }
                arr[j + 1] = key;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Unique(int* arr, int len)
        {
            int unique = 1;
            for (int i = 1; i < len; i++)
            {
                if (arr[i] != arr[i - 1])
                    arr[unique++] = arr[i];
            }
            return unique;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BinarySearch(int* arr, int len, int val)
        {
            int lo = 0, hi = len;
            while (lo < hi)
            {
                int mid = lo + ((hi - lo) >> 1);
                if (arr[mid] < val)
                    lo = mid + 1;
                else
                    hi = mid;
            }
            return lo;
        }
    }
}