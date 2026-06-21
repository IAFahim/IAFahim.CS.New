using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Sort.Specialized
{
    public static unsafe class SortInts
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* ptr, int len)
        {
            for (int i = (len >> 1) - 1; i >= 0; i--) SiftDown(ptr, i, len);
            for (int end = len - 1; end > 0; end--)
            {
                int t = ptr[0]; ptr[0] = ptr[end]; ptr[end] = t;
                SiftDown(ptr, 0, end);
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