namespace IAFahim.DS.Mo
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MoAdd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* freq, int val) => freq[val]++;
    }

    public static unsafe class MoRemove
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* freq, int val) => freq[val]--;
    }

    public static unsafe class MoAnswer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* freq, int n)
        {
            for (int i = 0; i < n; i++) if (freq[i] > 0) return i;
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DistinctCount(int* freq, int n)
        {
            int count = 0;
            for (int i = 0; i < n; i++) if (freq[i] > 0) count++;
            return count;
        }
    }

    public static unsafe class MoSort
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Less(int l1, int r1, int b1, int l2, int r2, int b2)
        {
            if (b1 != b2) return b1 < b2;
            return (b1 & 1) != 0 ? r1 < r2 : r1 > r2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap(int* q, int* l, int* r, int* b, int i, int j)
        {
            int t;
            t = q[i]; q[i] = q[j]; q[j] = t;
            t = l[i]; l[i] = l[j]; l[j] = t;
            t = r[i]; r[i] = r[j]; r[j] = t;
            t = b[i]; b[i] = b[j]; b[j] = t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void QuickSort(int* q, int* l, int* r, int* b, int left, int right)
        {
            if (left >= right) return;
            int pivotIdx = left + ((right - left) >> 1);
            int pl = l[pivotIdx], pr = r[pivotIdx], pb = b[pivotIdx];
            int i = left, j = right;
            while (i <= j)
            {
                while (Less(l[i], r[i], b[i], pl, pr, pb)) i++;
                while (Less(pl, pr, pb, l[j], r[j], b[j])) j--;
                if (i <= j) { Swap(q, l, r, b, i, j); i++; j--; }
            }
            QuickSort(q, l, r, b, left, j);
            QuickSort(q, l, r, b, i, right);
        }

        public static void Run(int* queries, int* l, int* r, int* block, int q, int blockSize)
        {
            if (q <= 1) return;
            QuickSort(queries, l, r, block, 0, q - 1);
        }
    }

    public static unsafe class MoRollback
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* freq, int n) { for (int i = 0; i < n; i++) freq[i] = 0; }
    }

    public static unsafe class MoDistinctCounter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddInt(int* freq, int* curDistinct, int val) { if (freq[val] == 0) (*curDistinct)++; freq[val]++; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveInt(int* freq, int* curDistinct, int val) { freq[val]--; if (freq[val] == 0) (*curDistinct)--; }
    }
}
