using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Bit
{
    public static unsafe class FirstTrue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            int lo = 0, hi = len;
            while (lo < hi)
            {
                int mid = lo + (hi - lo >> 1);
                if (ptr[mid].CompareTo(default(T)) != 0)
                    hi = mid;
                else
                    lo = mid + 1;
            }
            return lo < len && ptr[lo].CompareTo(default(T)) != 0 ? lo : -1;
        }
    }

    public static unsafe class LastTrue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            int lo = 0, hi = len;
            while (lo < hi)
            {
                int mid = lo + (hi - lo >> 1);
                if (ptr[mid].CompareTo(default(T)) == 0)
                    lo = mid + 1;
                else
                    hi = mid;
            }
            return lo > 0 && ptr[lo - 1].CompareTo(default(T)) != 0 ? lo - 1 : -1;
        }
    }
}