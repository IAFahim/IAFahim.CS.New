namespace IAFahim.DS.Heap
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HeapPush
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Parent(int i) => (i - 1) >> 1;

        public static void Run<T>(T* ptr, int len, T val) where T : unmanaged, IComparable<T>
        {
            int i = len;
            ptr[i] = val;
            while (i > 0)
            {
                int p = Parent(i);
                if (ptr[p].CompareTo(ptr[i]) <= 0) break;
                T tmp = ptr[p];
                ptr[p] = ptr[i];
                ptr[i] = tmp;
                i = p;
            }
        }
    }

    public static unsafe class HeapPop
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Left(int i) => (i << 1) + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Right(int i) => (i << 1) + 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap<T>(T* a, T* b) where T : unmanaged { T t = *a; *a = *b; *b = t; }

        public static T Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>
        {
            T result = ptr[0];
            ptr[0] = ptr[len - 1];
            HeapFix.Run(ptr, 0, len - 1);
            return result;
        }
    }

    public static unsafe class HeapFix
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Left(int i) => (i << 1) + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Right(int i) => (i << 1) + 2;

        public static void Run<T>(T* ptr, int i, int len) where T : unmanaged, IComparable<T>
        {
            while (true)
            {
                int smallest = i;
                int l = Left(i);
                int r = Right(i);
                if (l < len && ptr[l].CompareTo(ptr[smallest]) < 0) smallest = l;
                if (r < len && ptr[r].CompareTo(ptr[smallest]) < 0) smallest = r;
                if (smallest == i) break;
                T tmp = ptr[i];
                ptr[i] = ptr[smallest];
                ptr[smallest] = tmp;
                i = smallest;
            }
        }
    }

    public static unsafe class HeapRemove
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Parent(int i) => (i - 1) >> 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Left(int i) => (i << 1) + 1;

        public static void Run<T>(T* ptr, int i, int len) where T : unmanaged, IComparable<T>
        {
            ptr[i] = ptr[len - 1];
            int cur = i;
            while (cur > 0)
            {
                int p = Parent(cur);
                if (ptr[p].CompareTo(ptr[cur]) <= 0) break;
                T tmp = ptr[p];
                ptr[p] = ptr[cur];
                ptr[cur] = tmp;
                cur = p;
            }
            int n = len - 1;
            if (cur != i) return;
            int l = Left(cur);
            while (l < n)
            {
                int smallest = cur;
                if (ptr[l].CompareTo(ptr[smallest]) < 0) smallest = l;
                int r = l + 1;
                if (r < n && ptr[r].CompareTo(ptr[smallest]) < 0) smallest = r;
                if (smallest == cur) break;
                T tmp = ptr[cur];
                ptr[cur] = ptr[smallest];
                ptr[smallest] = tmp;
                cur = smallest;
                l = Left(cur);
            }
        }
    }

    public static unsafe class DequePush
    {
        public static void PushFrontInt32(int* deque, int* front, int* back, int cap, int val)
        {
            *front = (*front - 1 + cap) % cap;
            deque[*front] = val;
        }

        public static void PushBackInt32(int* deque, int* front, int* back, int cap, int val)
        {
            deque[*back] = val;
            *back = (*back + 1) % cap;
        }
    }

    public static unsafe class DequePop
    {
        public static int PopFrontInt32(int* deque, int* front, int* back, int cap)
        {
            int val = deque[*front];
            *front = (*front + 1) % cap;
            return val;
        }

        public static int PopBackInt32(int* deque, int* front, int* back, int cap)
        {
            *back = (*back - 1 + cap) % cap;
            return deque[*back];
        }
    }

    public static unsafe class MonotonicQueueMin
    {
        public static void Run(int* src, int* dst, int len, int windowSize)
        {
            if (len == 0 || windowSize == 0) return;
            int* deque = stackalloc int[len];
            int front = 0, back = 0;
            for (int i = 0; i < len; i++)
            {
                while (front < back && src[deque[back - 1]] >= src[i]) back--;
                deque[back++] = i;
                if (deque[front] <= i - windowSize) front++;
                if (i >= windowSize - 1)
                    dst[i - windowSize + 1] = src[deque[front]];
            }
        }
    }

    public static unsafe class MonotonicQueuePush
    {
        public static void MinInt32(int* mono, int* size, int val)
        {
            while (*size > 0 && mono[*size - 1] >= val) (*size)--;
            mono[*size] = val;
            (*size)++;
        }
    }

    public static unsafe class MonotonicStackProcess
    {
        public static int NextGreaterInt32(int* src, int* dst, int len)
        {
            if (len == 0) return 0;
            int* stack = stackalloc int[len];
            int top = 0;
            for (int i = 0; i < len; i++)
            {
                while (top > 0 && src[stack[top - 1]] < src[i])
                {
                    dst[stack[--top]] = i;
                }
                stack[top++] = i;
            }
            int count = top;
            return count;
        }
    }
}