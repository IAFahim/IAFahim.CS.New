namespace IAFahim.Search.Selection
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Selection
    {
        public static void SelectTopK(int* ptr, int len, int k)
        {
            if (k <= 0 || len == 0) return;
            if (k >= len) return;
            int left = 0, right = len - 1;
            while (left < right)
            {
                int pivot = ptr[right];
                int i = left - 1;
                for (int j = left; j < right; j++)
                {
                    if (ptr[j] <= pivot)
                    {
                        i++;
                        int t = ptr[i]; ptr[i] = ptr[j]; ptr[j] = t;
                    }
                }
                int t2 = ptr[i + 1]; ptr[i + 1] = ptr[right]; ptr[right] = t2;
                int idx = i + 1;
                if (idx == k) break;
                else if (idx < k) left = idx + 1;
                else right = idx - 1;
            }
            for (int i = 1; i < k; i++)
            {
                int val = ptr[i];
                int j = i - 1;
                while (j >= 0 && ptr[j] > val)
                {
                    ptr[j + 1] = ptr[j];
                    j--;
                }
                ptr[j + 1] = val;
            }
        }

        public static bool TryGetKth(int* ptr, int len, int k, out int result)
        {
            result = 0;
            if ((uint)k >= (uint)len) return false;
            int left = 0, right = len - 1;
            while (left < right)
            {
                int pivot = ptr[right];
                int i = left - 1;
                for (int j = left; j < right; j++)
                {
                    if (ptr[j] <= pivot)
                    {
                        i++;
                        int t = ptr[i]; ptr[i] = ptr[j]; ptr[j] = t;
                    }
                }
                int t2 = ptr[i + 1]; ptr[i + 1] = ptr[right]; ptr[right] = t2;
                int idx = i + 1;
                if (idx == k) { result = ptr[idx]; return true; }
                else if (idx < k) left = idx + 1;
                else right = idx - 1;
            }
            result = ptr[left];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MedianIndex(int len)
        {
            return len == 0 ? 0 : (len - 1) >> 1;
        }
    }

    public static unsafe class TopK
    {
        public static int Run(int n, long* a, int k, long* res)
        {
            if (k <= 0) return 0;
            long* temp = stackalloc long[n];
            for (int i = 0; i < n; i++) temp[i] = a[i];
            int left = 0, right = n - 1;
            while (left < right)
            {
                long pivot = temp[right];
                int i = left - 1;
                for (int j = left; j < right; j++)
                {
                    if (temp[j] >= pivot)
                    {
                        i++;
                        long t = temp[i]; temp[i] = temp[j]; temp[j] = t;
                    }
                }
                long t2 = temp[i + 1]; temp[i + 1] = temp[right]; temp[right] = t2;
                int idx = i + 1;
                if (idx == k) break;
                else if (idx < k) left = idx;
                else right = idx - 1;
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
            long* maxHeap = stackalloc long[n];
            long* minHeap = stackalloc long[n];
            int maxSize = 0, minSize = 0;
            maxHeap[0] = a[0];
            maxSize = 1;
            res[0] = a[0];
            for (int i = 1; i < n; i++)
            {
                if (a[i] <= maxHeap[0])
                {
                    maxHeap[maxSize++] = a[i];
                    SiftUpMax(maxHeap, maxSize - 1);
                }
                else
                {
                    minHeap[minSize++] = a[i];
                    SiftUpMin(minHeap, minSize - 1);
                }
                if (maxSize > minSize + 1)
                {
                    minHeap[minSize++] = maxHeap[0];
                    SiftUpMin(minHeap, minSize - 1);
                    maxHeap[0] = maxHeap[--maxSize];
                    SiftDownMax(maxHeap, 0, maxSize);
                }
                else if (minSize > maxSize)
                {
                    maxHeap[maxSize++] = minHeap[0];
                    SiftUpMax(maxHeap, maxSize - 1);
                    minHeap[0] = minHeap[--minSize];
                    SiftDownMin(minHeap, 0, minSize);
                }
                if ((maxSize + minSize) % 2 == 0)
                    res[i] = (maxHeap[0] + minHeap[0]) >> 1;
                else
                    res[i] = maxHeap[0];
            }
            return res[(n - 1) / 2];
        }

        private static void SiftUpMax(long* heap, int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) >> 1;
                if (heap[parent] < heap[i])
                {
                    long t = heap[parent]; heap[parent] = heap[i]; heap[i] = t;
                    i = parent;
                }
                else break;
            }
        }

        private static void SiftDownMax(long* heap, int i, int size)
        {
            while (true)
            {
                int left = 2 * i + 1, right = 2 * i + 2, largest = i;
                if (left < size && heap[left] > heap[largest]) largest = left;
                if (right < size && heap[right] > heap[largest]) largest = right;
                if (largest != i)
                {
                    long t = heap[i]; heap[i] = heap[largest]; heap[largest] = t;
                    i = largest;
                }
                else break;
            }
        }

        private static void SiftUpMin(long* heap, int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) >> 1;
                if (heap[parent] > heap[i])
                {
                    long t = heap[parent]; heap[parent] = heap[i]; heap[i] = t;
                    i = parent;
                }
                else break;
            }
        }

        private static void SiftDownMin(long* heap, int i, int size)
        {
            while (true)
            {
                int left = 2 * i + 1, right = 2 * i + 2, smallest = i;
                if (left < size && heap[left] < heap[smallest]) smallest = left;
                if (right < size && heap[right] < heap[smallest]) smallest = right;
                if (smallest != i)
                {
                    long t = heap[i]; heap[i] = heap[smallest]; heap[smallest] = t;
                    i = smallest;
                }
                else break;
            }
        }
    }

    public static unsafe class OrderStatistic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Select(int n, long* a, int k)
        {
            if ((uint)k >= (uint)n) return long.MinValue;
            long* temp = stackalloc long[n];
            for (int i = 0; i < n; i++) temp[i] = a[i];
            int left = 0, right = n - 1;
            while (left < right)
            {
                long pivot = temp[right];
                int i = left - 1;
                for (int j = left; j < right; j++)
                {
                    if (temp[j] >= pivot)
                    {
                        i++;
                        long t = temp[i]; temp[i] = temp[j]; temp[j] = t;
                    }
                }
                long t2 = temp[i + 1]; temp[i + 1] = temp[right]; temp[right] = t2;
                int idx = i + 1;
                if (idx == k) return temp[idx];
                else if (idx < k) left = idx;
                else right = idx - 1;
            }
            return temp[left];
        }
    }
}
