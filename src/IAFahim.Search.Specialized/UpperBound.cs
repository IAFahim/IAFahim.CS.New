using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Specialized
{
    public static unsafe class UpperBound
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* ptr, int len, int key)
        {
            int lo = 0;
            int hi = len;
            while (lo < hi)
            {
                int mid = lo + ((hi - lo) >> 1);
                if (ptr[mid] > key)
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
    }
}