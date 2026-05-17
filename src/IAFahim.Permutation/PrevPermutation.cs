using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Permutation
{
    public static unsafe class PrevPermutation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            if (len <= 1) return false;
            int i = len - 2;
            while (i >= 0 && ptr[i].CompareTo(ptr[i + 1]) <= 0) i--;
            if (i < 0) return false;
            int j = len - 1;
            while (ptr[j].CompareTo(ptr[i]) >= 0) j--;
            T tmp = ptr[i];
            ptr[i] = ptr[j];
            ptr[j] = tmp;
            int lo = i + 1, hi = len - 1;
            while (lo < hi)
            {
                tmp = ptr[lo];
                ptr[lo] = ptr[hi];
                ptr[hi] = tmp;
                lo++;
                hi--;
            }
            return true;
        }
    }
}