namespace IAFahim.Search
{
    using System.Runtime.CompilerServices;

    public static unsafe class BinarySearch
    {
        // Lower bound on sorted ascending array: first index i with ptr[i] >= key, or len.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LowerBound(int* ptr, int len, int key)
        {
            int lo = 0, hi = len;
            while (lo < hi)
            {
                int mid = lo + ((hi - lo) >> 1);
                if (ptr[mid] < key) lo = mid + 1;
                else hi = mid;
            }
            return lo;
        }

        // Upper bound: first index i with ptr[i] > key, or len.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UpperBound(int* ptr, int len, int key)
        {
            int lo = 0, hi = len;
            while (lo < hi)
            {
                int mid = lo + ((hi - lo) >> 1);
                if (ptr[mid] <= key) lo = mid + 1;
                else hi = mid;
            }
            return lo;
        }

        // Exact find: index of key or -1.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Find(int* ptr, int len, int key)
        {
            int i = LowerBound(ptr, len, key);
            if (i < len && ptr[i] == key) return i;
            return -1;
        }

        public static bool TryFind(int* ptr, int len, int key, out int index)
        {
            index = Find(ptr, len, key);
            return index >= 0;
        }
    }
}
