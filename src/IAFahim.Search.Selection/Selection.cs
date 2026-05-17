using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Selection
{
    public static unsafe class Selection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SelectTopK<intT>(intT* ptr, int len, int k) where intT : unmanaged, IComparable<intT>
        {
            if ((uint)k >= (uint)len)
            {
                return;
            }
            int lo = 0;
            int hi = len;
            while (hi > lo + 1)
            {
                int mid = lo + ((hi - lo) >> 1);
                intT pivot = ptr[mid];
                int i = lo;
                int j = hi;
                ptr[mid] = ptr[lo];
                ptr[lo] = pivot;
                while (true)
                {
                    do { j--; } while (ptr[j].CompareTo(pivot) > 0);
                    do { i++; } while (i < hi && ptr[i].CompareTo(pivot) < 0);
                    if (i >= j)
                    {
                        break;
                    }
                    intT tmp = ptr[i];
                    ptr[i] = ptr[j];
                    ptr[j] = tmp;
                }
                ptr[lo] = ptr[j];
                ptr[j] = pivot;
                if (j == k)
                {
                    return;
                }
                if (j < k)
                {
                    lo = j + 1;
                }
                else
                {
                    hi = j;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetKth<intT>(intT* ptr, int len, int k, out intT result) where intT : unmanaged, IComparable<intT>
        {
            if ((uint)k >= (uint)len)
            {
                result = default;
                return false;
            }
            SelectTopK(ptr, len, k);
            result = ptr[k];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MedianIndex(int len)
        {
            return len >> 1;
        }
    }
}