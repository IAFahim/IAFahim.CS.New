using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Specialized
{
    public static unsafe class TernarySearch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* ptr, int len, int key)
        {
            int lo = 0;
            int hi = len - 1;
            while (lo <= hi)
            {
                int mid1 = lo + (hi - lo) / 3;
                int mid2 = hi - (hi - lo) / 3;
                if (ptr[mid1] == key)
                    return mid1;
                if (ptr[mid2] == key)
                    return mid2;
                if (ptr[mid1] > key)
                    hi = mid1 - 1;
                else if (ptr[mid2] < key)
                    lo = mid2 + 1;
                else
                {
                    lo = mid1 + 1;
                    hi = mid2 - 1;
                }
            }
            return ~lo;
        }
    }
}