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
            HeapSort(tmp, len);
            dstMap[0] = tmp[0];
            int unique = 1;
            for (int i = 1; i < len; i++)
            {
                if (tmp[i] != tmp[i - 1])
                    dstMap[unique++] = tmp[i];
            }
            return unique;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HeapSort(int* a, int len)
        {
            for (int i = (len >> 1) - 1; i >= 0; i--) SiftDown(a, i, len);
            for (int end = len - 1; end > 0; end--)
            {
                int t = a[0]; a[0] = a[end]; a[end] = t;
                SiftDown(a, 0, end);
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
    }
}