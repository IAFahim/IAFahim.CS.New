using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Specialized
{
    public static unsafe class BinarySearch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryFind(int* ptr, int len, int key, out int index)
        {
            int lo = 0;
            int hi = len;
            while (lo < hi)
            {
                int mid = lo + ((hi - lo) >> 1);
                if (ptr[mid] < key)
                {
                    lo = mid + 1;
                }
                else if (ptr[mid] > key)
                {
                    hi = mid;
                }
                else
                {
                    index = mid;
                    return true;
                }
            }
            index = default;
            return false;
        }
    }
}