namespace IAFahim.Optimization.DivideConquer
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SlopeTrick
    {
        public struct State
        {
            public long L, R;
            public long Lc, Rc;
            public long Offset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Init(State* s)
        {
            s->L = s->R = 0;
            s->Lc = s->Rc = long.MaxValue;
            s->Offset = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddAbs(State* s, long a)
        {
            long l = a - s->L, r = s->R - a;
            if (l > r) UpdateLeftSlope(s, a, l);
            else UpdateRightSlope(s, a, r);
        }

        private static void UpdateLeftSlope(State* s, long a, long l)
        {
            s->Offset += l;
            if (a < s->Lc) s->Lc = a;
            s->R = s->Lc;
        }

        private static void UpdateRightSlope(State* s, long a, long r)
        {
            s->Offset += r;
            if (a > s->Rc) s->Rc = a;
            s->L = s->Rc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddAbsWithHeaps(State* s, long a, long* leftHeap, ref int leftSize, long* rightHeap, ref int rightSize)
        {
            MaxHeapPush(leftHeap, ref leftSize, a);
            MinHeapPush(rightHeap, ref rightSize, a);

            if (leftSize > 0 && rightSize > 0)
            {
                AdjustHeaps(s, leftHeap, ref leftSize, rightHeap, ref rightSize);
            }
        }

        private static void AdjustHeaps(State* s, long* leftHeap, ref int leftSize, long* rightHeap, ref int rightSize)
        {
            long topL = MaxHeapPeek(leftHeap, leftSize);
            long topR = MinHeapPeek(rightHeap, rightSize);
            if (topL > topR)
            {
                MaxHeapPop(leftHeap, ref leftSize);
                MinHeapPop(rightHeap, ref rightSize);
                MaxHeapPush(leftHeap, ref leftSize, topR);
                MinHeapPush(rightHeap, ref rightSize, topL);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddMaxZero(State* s)
        {
            s->L--; s->R++;
            if (s->L > 0) { s->Offset += s->L; s->L = 0; }
            if (s->R < 0) { s->Offset -= s->R; s->R = 0; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shift(State* s, long add) { s->L += add; s->R += add; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Query(State* s)
        {
            if (s->Lc <= s->Rc) return s->Offset + s->L * s->L;
            return s->Offset + s->R * s->R;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MaxHeapPush(long* heap, ref int size, long val)
        {
            int idx = size++;
            heap[idx] = val;
            while (idx > 0)
            {
                int parent = (idx - 1) >> 1;
                if (heap[parent] >= heap[idx]) break;
                long tmp = heap[parent]; heap[parent] = heap[idx]; heap[idx] = tmp;
                idx = parent;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long MaxHeapPeek(long* heap, int size)
        {
            return heap[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MaxHeapPop(long* heap, ref int size)
        {
            size--;
            heap[0] = heap[size];
            int idx = 0;
            while (true)
            {
                int left = idx * 2 + 1;
                int right = idx * 2 + 2;
                int largest = idx;
                if (left < size && heap[left] > heap[largest]) largest = left;
                if (right < size && heap[right] > heap[largest]) largest = right;
                if (largest == idx) break;
                long tmp = heap[idx]; heap[idx] = heap[largest]; heap[largest] = tmp;
                idx = largest;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MinHeapPush(long* heap, ref int size, long val)
        {
            int idx = size++;
            heap[idx] = val;
            while (idx > 0)
            {
                int parent = (idx - 1) >> 1;
                if (heap[parent] <= heap[idx]) break;
                long tmp = heap[parent]; heap[parent] = heap[idx]; heap[idx] = tmp;
                idx = parent;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long MinHeapPeek(long* heap, int size)
        {
            return heap[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MinHeapPop(long* heap, ref int size)
        {
            size--;
            heap[0] = heap[size];
            int idx = 0;
            while (true)
            {
                int left = idx * 2 + 1;
                int right = idx * 2 + 2;
                int smallest = idx;
                if (left < size && heap[left] < heap[smallest]) smallest = left;
                if (right < size && heap[right] < heap[smallest]) smallest = right;
                if (smallest == idx) break;
                long tmp = heap[idx]; heap[idx] = heap[smallest]; heap[smallest] = tmp;
                idx = smallest;
            }
        }
    }
}
