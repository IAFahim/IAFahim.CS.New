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

            HeapSort(tmpSorted, len);
            int unique = Unique(tmpSorted, len);
            
            for (int i = 0; i < len; i++)
                dst[i] = BinarySearch(tmpSorted, unique, src[i]);
            
            return unique;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HeapSort(int* arr, int len)
        {
            for (int i = (len >> 1) - 1; i >= 0; i--) SiftDown(arr, i, len);
            for (int end = len - 1; end > 0; end--)
            {
                int t = arr[0]; arr[0] = arr[end]; arr[end] = t;
                SiftDown(arr, 0, end);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SiftDown(int* a, int i, int n)
        {
            while (true)
            {
                int l = 2 * i + 1, r = 2 * i + 2, m = i;
                if (l < n && a[l] > a[m]) m = l;
                if (r < n && a[r] > a[m]) m = r;
                if (m == i) break;
                int t = a[i]; a[i] = a[m]; a[m] = t;
                i = m;
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