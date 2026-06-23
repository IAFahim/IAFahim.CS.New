namespace IAFahim.Sort.QuickSort
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class QuickSort
    {
        private const int InsertionThreshold = 16;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            Run(ptr, 0, len - 1);
        }

        public static void Run<T>(T* ptr, int lo, int hi) where T : unmanaged, IComparable<T>
        {
            while (lo < hi)
            {
                if (hi - lo < InsertionThreshold)
                {
                    InsertionSort(ptr, lo, hi);
                    return;
                }

                int p = Partition(ptr, lo, hi);
                // Hoare partition returns the boundary index p with ptr[lo..p] <= ptr[p+1..hi];
                // the pivot is NOT fixed at p, so the left half must include p.
                if (p - lo < hi - p)
                { Run(ptr, lo, p); lo = p + 1; }
                else
                { Run(ptr, p + 1, hi); hi = p; }
            }
        }

        private static void InsertionSort<T>(T* ptr, int lo, int hi) where T : unmanaged, IComparable<T>
        {
            for (int i = lo + 1; i <= hi; i++)
            {
                T key = ptr[i];
                int j = i - 1;
                while (j >= lo && ptr[j].CompareTo(key) > 0)
                {
                    ptr[j + 1] = ptr[j];
                    j--;
                }
                ptr[j + 1] = key;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T MedianOfThree<T>(T a, T b, T c) where T : unmanaged, IComparable<T>
        {
            if (a.CompareTo(b) < 0)
            {
                if (b.CompareTo(c) < 0) return b;
                else if (a.CompareTo(c) < 0) return c;
                else return a;
            }
            if (a.CompareTo(c) < 0) return a;
            else if (b.CompareTo(c) < 0) return c;
            else return b;
        }

        private static int Partition<T>(T* ptr, int lo, int hi) where T : unmanaged, IComparable<T>
        {
            // Median-of-three pivot selection: avoids O(n^2) on sorted/reverse-sorted input.
            int mid = lo + ((hi - lo) >> 1);
            T pivot = MedianOfThree(ptr[lo], ptr[mid], ptr[hi]);

            int i = lo - 1;
            int j = hi + 1;
            while (true)
            {
                do { i++; } while (ptr[i].CompareTo(pivot) < 0);
                do { j--; } while (ptr[j].CompareTo(pivot) > 0);
                if (i >= j) return j;
                T tmp = ptr[i]; ptr[i] = ptr[j]; ptr[j] = tmp;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunInt32(int* ptr, int len)
        {
            RunInt32(ptr, 0, len - 1);
        }

        public static void RunInt32(int* ptr, int lo, int hi)
        {
            while (lo < hi)
            {
                if (hi - lo < InsertionThreshold)
                {
                    InsertionSortInt32(ptr, lo, hi);
                    return;
                }

                int p = PartitionInt32(ptr, lo, hi);
                // Hoare partition returns the boundary index p with ptr[lo..p] <= ptr[p+1..hi];
                // the pivot is NOT fixed at p, so the left half must include p.
                if (p - lo < hi - p)
                { RunInt32(ptr, lo, p); lo = p + 1; }
                else
                { RunInt32(ptr, p + 1, hi); hi = p; }
            }
        }

        private static void InsertionSortInt32(int* ptr, int lo, int hi)
        {
            for (int i = lo + 1; i <= hi; i++)
            {
                int key = ptr[i];
                int j = i - 1;
                while (j >= lo && ptr[j] > key)
                {
                    ptr[j + 1] = ptr[j];
                    j--;
                }
                ptr[j + 1] = key;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int MedianOfThreeInt32(int a, int b, int c)
        {
            if (a < b)
            {
                if (b < c) return b;
                else if (a < c) return c;
                else return a;
            }
            if (a < c) return a;
            else if (b < c) return c;
            else return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PartitionInt32(int* ptr, int lo, int hi)
        {
            // Median-of-three pivot selection: avoids O(n^2) on sorted/reverse-sorted input.
            int mid = lo + ((hi - lo) >> 1);
            int pivot = MedianOfThreeInt32(ptr[lo], ptr[mid], ptr[hi]);

            int i = lo - 1;
            int j = hi + 1;
            while (true)
            {
                do { i++; } while (ptr[i] < pivot);
                do { j--; } while (ptr[j] > pivot);
                if (i >= j) return j;
                int tmp = ptr[i]; ptr[i] = ptr[j]; ptr[j] = tmp;
            }
        }
    }
}
