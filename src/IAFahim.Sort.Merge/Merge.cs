using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
            if (len <= 1) return;
            T* tmp = (T*)Marshal.AllocHGlobal((nint)((long)len * sizeof(T)));
            RunInPlaceCore(ptr, len, tmp);
            Marshal.FreeHGlobal((nint)tmp);
        }

        private static void RunInPlaceCore<T>(T* ptr, int len, T* tmp) where T : unmanaged, IComparable<T>
        {
            int width = 1;
            while (width < len)
            {
                int l = 0;
                while (l < len)
                {
                    int m = (int)Math.Min((long)l + width, len);
                    int r = (int)Math.Min((long)l + 2 * width, len);
                    int segLen = r - l;
                    for (int i = 0; i < segLen; i++) tmp[i] = ptr[l + i];
                    int ia = 0, ib = m - l, ic = l;
                    int aEnd = m - l, bEnd = r - l;
                    while (ia < aEnd && ib < bEnd)
                        ptr[ic++] = tmp[ia].CompareTo(tmp[ib]) <= 0 ? tmp[ia++] : tmp[ib++];
                    while (ia < aEnd)
                        ptr[ic++] = tmp[ia++];
                    while (ib < bEnd)
                        ptr[ic++] = tmp[ib++];
                    l = (int)((long)l + 2 * width);
                }
                width <<= 1;
            }
        }
    }
}