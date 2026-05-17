using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Sort.Merge
{
    public static unsafe class MergeSorted
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(T* a, int aLen, T* b, int bLen, T* dst) where T : unmanaged, IComparable<T>
        {
            int ia = 0, ib = 0, id = 0;
            while (ia < aLen && ib < bLen)
            {
                dst[id++] = a[ia].CompareTo(b[ib]) <= 0 ? a[ia++] : b[ib++];
            }
            while (ia < aLen)
                dst[id++] = a[ia++];
            while (ib < bLen)
                dst[id++] = b[ib++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunInPlace<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            int width = 1;
            while (width < len)
            {
                int l = 0;
                while (l < len)
                {
                    int m = Math.Min(l + width, len);
                    int r = Math.Min(l + 2 * width, len);
                    int i = l, j = m, k = l;
                    while (i < m && j < r)
                        ptr[k++] = ptr[i].CompareTo(ptr[j]) <= 0 ? ptr[i++] : ptr[j++];
                    while (i < m)
                        ptr[k++] = ptr[i++];
                    while (j < r)
                        ptr[k++] = ptr[j++];
                    l += 2 * width;
                }
                width <<= 1;
            }
        }
    }
}