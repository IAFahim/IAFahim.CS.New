namespace IAFahim.Sort.QuickSort
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class QuickSort
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            Run(ptr, 0, len - 1);
        }

        public static void Run<T>(T* ptr, int lo, int hi) where T : unmanaged, IComparable<T>
        {
            while (lo < hi)
            {
                int p = Partition(ptr, lo, hi);
                if (p - lo < hi - p)
                { Run(ptr, lo, p - 1); lo = p + 1; }
                else
                { Run(ptr, p + 1, hi); hi = p - 1; }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Partition<T>(T* ptr, int lo, int hi) where T : unmanaged, IComparable<T>
        {
            T pivot = ptr[lo];
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
                int p = PartitionInt32(ptr, lo, hi);
                if (p - lo < hi - p)
                { RunInt32(ptr, lo, p - 1); lo = p + 1; }
                else
                { RunInt32(ptr, p + 1, hi); hi = p - 1; }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PartitionInt32(int* ptr, int lo, int hi)
        {
            int pivot = ptr[lo];
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
