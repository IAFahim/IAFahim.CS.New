namespace IAFahim.Search.Selection
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SelectionShared
    {
        public static int Partition(int* ptr, int left, int right)
        {
            int pivot = ptr[right], i = left - 1;
            for (int j = left; j < right; j++)
                if (ptr[j] <= pivot) { i++; int t = ptr[i]; ptr[i] = ptr[j]; ptr[j] = t; }
            int t2 = ptr[i + 1]; ptr[i + 1] = ptr[right]; ptr[right] = t2;
            return i + 1;
        }

        public static int PartitionLong(long* ptr, int left, int right)
        {
            long pivot = ptr[right]; int i = left - 1;
            for (int j = left; j < right; j++)
                if (ptr[j] >= pivot) { i++; long t = ptr[i]; ptr[i] = ptr[j]; ptr[j] = t; }
            long t2 = ptr[i + 1]; ptr[i + 1] = ptr[right]; ptr[right] = t2;
            return i + 1;
        }

        public static void InsertionSort(int* ptr, int k)
        {
            for (int i = 1; i < k; i++)
            {
                int val = ptr[i], j = i - 1;
                while (j >= 0 && ptr[j] > val) { ptr[j + 1] = ptr[j]; j--; }
                ptr[j + 1] = val;
            }
        }
    }

    public static unsafe class Selection
    {
        public static void SelectTopK(int* ptr, int len, int k)
        {
            if (k <= 0 || len == 0 || k >= len) return;
            int left = 0, right = len - 1;
            while (left < right)
            {
                int idx = SelectionShared.Partition(ptr, left, right);
                if (idx == k) break; else if (idx < k) left = idx + 1; else right = idx - 1;
            }
            SelectionShared.InsertionSort(ptr, k);
        }

        public static bool TryGetKth(int* ptr, int len, int k, out int result)
        {
            result = 0; if ((uint)k >= (uint)len) return false;
            int left = 0, right = len - 1;
            while (left < right)
            {
                int idx = SelectionShared.Partition(ptr, left, right);
                if (idx == k) { result = ptr[idx]; return true; }
                else if (idx < k) left = idx + 1; else right = idx - 1;
            }
            result = ptr[left]; return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MedianIndex(int len) => len == 0 ? 0 : (len - 1) >> 1;
    }

    public static unsafe class TopK
    {
        public static int Run(int n, long* a, int k, long* res)
        {
            if (k <= 0) return 0;
            long* temp = stackalloc long[n]; for (int i = 0; i < n; i++) temp[i] = a[i];
            int left = 0, right = n - 1;
            while (left < right)
            {
                int idx = SelectionShared.PartitionLong(temp, left, right);
                if (idx == k) break; else if (idx < k) left = idx; else right = idx - 1;
            }
            int count = Math.Min(k, n);
            for (int i = 0; i < count; i++) res[i] = temp[i];
            return count;
        }
    }

    public static unsafe class MedianMaintain
    {
        public static long Run(int n, long* a, long* res)
        {
            long* maxHeap = stackalloc long[n], minHeap = stackalloc long[n];
            int maxSize = 0, minSize = 0; maxHeap[0] = a[0]; maxSize = 1; res[0] = a[0];
            for (int i = 1; i < n; i++)
            {
                if (a[i] <= maxHeap[0]) { maxHeap[maxSize++] = a[i]; SiftUpMax(maxHeap, maxSize - 1); }
                else { minHeap[minSize++] = a[i]; SiftUpMin(minHeap, minSize - 1); }
                RebalanceHeaps(maxHeap, ref maxSize, minHeap, ref minSize);
                res[i] = (maxSize + minSize) % 2 == 0 ? (maxHeap[0] + minHeap[0]) >> 1 : maxHeap[0];
            }
            return res[(n - 1) / 2];
        }

        private static void RebalanceHeaps(long* maxHeap, ref int maxSize, long* minHeap, ref int minSize)
        {
            if (maxSize > minSize + 1)
            {
                minHeap[minSize++] = maxHeap[0]; SiftUpMin(minHeap, minSize - 1);
                maxHeap[0] = maxHeap[--maxSize]; SiftDownMax(maxHeap, 0, maxSize);
            }
            else if (minSize > maxSize)
            {
                maxHeap[maxSize++] = minHeap[0]; SiftUpMax(maxHeap, maxSize - 1);
                minHeap[0] = minHeap[--minSize]; SiftDownMin(minHeap, 0, minSize);
            }
        }

        private static void SiftUpMax(long* h, int i) { while (i > 0) { int p = (i - 1) >> 1; if (h[p] < h[i]) { long t = h[p]; h[p] = h[i]; h[i] = t; i = p; } else break; } }
        private static void SiftDownMax(long* h, int i, int s) { while (true) { int l = 2 * i + 1, r = 2 * i + 2, j = i; if (l < s && h[l] > h[j]) j = l; if (r < s && h[r] > h[j]) j = r; if (j != i) { long t = h[i]; h[i] = h[j]; h[j] = t; i = j; } else break; } }
        private static void SiftUpMin(long* h, int i) { while (i > 0) { int p = (i - 1) >> 1; if (h[p] > h[i]) { long t = h[p]; h[p] = h[i]; h[i] = t; i = p; } else break; } }
        private static void SiftDownMin(long* h, int i, int s) { while (true) { int l = 2 * i + 1, r = 2 * i + 2, j = i; if (l < s && h[l] < h[j]) j = l; if (r < s && h[r] < h[j]) j = r; if (j != i) { long t = h[i]; h[i] = h[j]; h[j] = t; i = j; } else break; } }
    }
}
