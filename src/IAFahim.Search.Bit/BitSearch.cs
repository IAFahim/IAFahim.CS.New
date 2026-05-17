using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Bit
{
    public static unsafe class BitSearch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LowerBound<T>(T* ptr, int len, T key) where T : unmanaged, IComparable<T>
        {
            int lo = 0;
            int hi = len;
            while (lo < hi)
            {
                int mid = lo + ((hi - lo) >> 1);
                if (ptr[mid].CompareTo(key) < 0)
                {
                    lo = mid + 1;
                }
                else
                {
                    hi = mid;
                }
            }
            return lo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UpperBound<T>(T* ptr, int len, T key) where T : unmanaged, IComparable<T>
        {
            int lo = 0;
            int hi = len;
            while (lo < hi)
            {
                int mid = lo + ((hi - lo) >> 1);
                if (ptr[mid].CompareTo(key) > 0)
                {
                    hi = mid;
                }
                else
                {
                    lo = mid + 1;
                }
            }
            return lo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryFind<T>(T* ptr, int len, T key, out int index) where T : unmanaged, IComparable<T>
        {
            int pos = LowerBound(ptr, len, key);
            if ((uint)pos < (uint)len && ptr[pos].CompareTo(key) == 0)
            {
                index = pos;
                return true;
            }
            index = default;
            return false;
        }
    }
}