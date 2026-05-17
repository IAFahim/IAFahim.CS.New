using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Sort.Partition
{
    public static unsafe class Partition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run<T>(T* ptr, int len, int pivotIdx) where T : unmanaged, IComparable<T>
        {
            if ((uint)pivotIdx >= (uint)len)
                return -1;
            T pivot = ptr[pivotIdx];
            ptr[pivotIdx] = ptr[len - 1];
            ptr[len - 1] = pivot;
            int storeIdx = 0;
            for (int i = 0; i < len - 1; i++)
            {
                if (ptr[i].CompareTo(pivot) < 0)
                {
                    T tmp = ptr[i];
                    ptr[i] = ptr[storeIdx];
                    ptr[storeIdx] = tmp;
                    storeIdx++;
                }
            }
            T tmp2 = ptr[len - 1];
            ptr[len - 1] = ptr[storeIdx];
            ptr[storeIdx] = tmp2;
            return storeIdx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNthElement<T>(T* ptr, int len, int n, out T result) where T : unmanaged, IComparable<T>
        {
            if ((uint)n >= (uint)len)
            {
                result = default;
                return false;
            }
            int lo = 0;
            int hi = len;
            while (hi > lo + 1)
            {
                int mid = lo + ((hi - lo) >> 1);
                T pivot = ptr[mid];
                int i = lo;
                int j = hi;
                ptr[mid] = ptr[lo];
                ptr[lo] = pivot;
                while (true)
                {
                    do { j--; } while (ptr[j].CompareTo(pivot) > 0);
                    do { i++; } while (i < hi && ptr[i].CompareTo(pivot) < 0);
                    if (i >= j) break;
                    T tmp = ptr[i];
                    ptr[i] = ptr[j];
                    ptr[j] = tmp;
                }
                ptr[lo] = ptr[j];
                ptr[j] = pivot;
                if (j == n)
                {
                    result = ptr[j];
                    return true;
                }
                if (j < n)
                    lo = j + 1;
                else
                    hi = j;
            }
            result = ptr[n];
            return true;
        }
    }
}