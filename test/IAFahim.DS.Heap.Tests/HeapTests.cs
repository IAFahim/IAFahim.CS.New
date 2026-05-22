namespace IAFahim.DS.Heap.Tests
{
    using IAFahim.DS.Heap;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class HeapTests
    {
        [Fact]
        public void PushPop_SingleElement()
        {
            int* heap = stackalloc int[10];
            int len = 0;
            HeapPush.Run(heap, len++, 5);
            Assert.Equal(1, len);
            Assert.Equal(5, heap[0]);
            int val = HeapPop.Run(heap, len--);
            Assert.Equal(5, val);
            Assert.Equal(0, len);
        }

        [Fact]
        public void PushPop_MaintainsMinHeap()
        {
            int* heap = stackalloc int[100];
            int len = 0;
            int[] vals = { 5, 3, 7, 1, 9, 2, 8 };
            foreach (int v in vals)
            {
                HeapPush.Run(heap, len, v);
                len++;
            }

            Assert.Equal(7, len);
            Assert.Equal(1, HeapPop.Run(heap, len--));
            Assert.Equal(2, HeapPop.Run(heap, len--));
            Assert.Equal(3, HeapPop.Run(heap, len--));
            Assert.Equal(5, HeapPop.Run(heap, len--));
            Assert.Equal(7, HeapPop.Run(heap, len--));
            Assert.Equal(8, HeapPop.Run(heap, len--));
            Assert.Equal(9, HeapPop.Run(heap, len--));
            Assert.Equal(0, len);
        }

        [Fact]
        public void HeapRemove_Middle()
        {
            int* heap = stackalloc int[100];
            int len = 0;
            HeapPush.Run(heap, len++, 1);
            HeapPush.Run(heap, len++, 2);
            HeapPush.Run(heap, len++, 3);
            HeapPush.Run(heap, len++, 4);
            HeapRemove.Run(heap, 1, len);
            len--;
            Assert.Equal(3, len);
            Assert.Equal(1, HeapPop.Run(heap, len--));
            Assert.Equal(3, HeapPop.Run(heap, len--));
            Assert.Equal(4, HeapPop.Run(heap, len--));
        }

        [Fact]
        public void Deque_BasicOperations()
        {
            const int cap = 5;
            int* deque = stackalloc int[cap];
            int front = 0;
            int back = 0;

            DequePush.PushBackInt32(deque, &front, &back, cap, 10);
            DequePush.PushBackInt32(deque, &front, &back, cap, 20);
            DequePush.PushFrontInt32(deque, &front, &back, cap, 5);

            Assert.Equal(5, DequePop.PopFrontInt32(deque, &front, &back, cap));
            Assert.Equal(20, DequePop.PopBackInt32(deque, &front, &back, cap));
            Assert.Equal(10, DequePop.PopFrontInt32(deque, &front, &back, cap));
        }

        [Fact]
        public void MonotonicQueueMin_Basic()
        {
            int* src = stackalloc int[] { 2, 1, 4, 3, 6, 5 };
            int len = 6;
            int* dst = stackalloc int[4];
            MonotonicQueueMin.Run(src, dst, len, 3);
            Assert.Equal(1, dst[0]);
            Assert.Equal(1, dst[1]);
            Assert.Equal(3, dst[2]);
            Assert.Equal(3, dst[3]);
        }

        [Fact]
        public void MonotonicQueuePush_MinInt32()
        {
            int* mono = stackalloc int[10];
            int size = 0;
            MonotonicQueuePush.MinInt32(mono, &size, 3);
            MonotonicQueuePush.MinInt32(mono, &size, 5);
            MonotonicQueuePush.MinInt32(mono, &size, 2);
            Assert.Equal(1, size);
            Assert.Equal(2, mono[0]);
        }

        [Fact]
        public void MonotonicStackProcess_NextGreaterInt32()
        {
            int* src = stackalloc int[] { 2, 1, 4, 3, 6, 5 };
            int len = 6;
            int* dst = stackalloc int[len];
            for (int i = 0; i < len; i++) dst[i] = -1;
            MonotonicStackProcess.NextGreaterInt32(src, dst, len);
            Assert.Equal(2, dst[0]);
            Assert.Equal(2, dst[1]);
            Assert.Equal(4, dst[2]);
            Assert.Equal(4, dst[3]);
            Assert.Equal(-1, dst[4]);
            Assert.Equal(-1, dst[5]);
        }
    }
}