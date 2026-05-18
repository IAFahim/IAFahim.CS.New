using System;
using System.Runtime.CompilerServices;

namespace IAFahim.DS.OrderedSet
{
    public static unsafe class OrderedSet
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Insert<T>(T* ptr, int len, T key) where T : unmanaged, IComparable<T>
        {
            int idx = LowerBound(ptr, len, key);
            if (idx < len && ptr[idx].CompareTo(key) == 0) return len;
            for (int i = len; i > idx; i--)
                ptr[i] = ptr[i - 1];
            ptr[idx] = key;
            return len + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Erase<T>(T* ptr, int len, T key) where T : unmanaged, IComparable<T>
        {
            int idx = LowerBound(ptr, len, key);
            if (idx >= len || ptr[idx].CompareTo(key) != 0) return len;
            for (int i = idx; i < len - 1; i++)
                ptr[i] = ptr[i + 1];
            return len - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Rank<T>(T* ptr, int len, T key) where T : unmanaged, IComparable<T>
        {
            return LowerBound(ptr, len, key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Kth<T>(T* ptr, int len, int k) where T : unmanaged, IComparable<T>
        {
            return ptr[k];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LowerBound<T>(T* ptr, int len, T key) where T : unmanaged, IComparable<T>
        {
            int lo = 0, hi = len;
            while (lo < hi)
            {
                int mid = lo + (hi - lo) / 2;
                if (ptr[mid].CompareTo(key) < 0) lo = mid + 1;
                else hi = mid;
            }
            return lo;
        }
    }
}
