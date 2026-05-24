namespace IAFahim.DS.Heap.Tests
{
    using IAFahim.DS.Heap;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class HeapTests
    {
        [Test]
        public void PushPop_SingleElement()
        {
            int* heap = stackalloc int[10];
            int len = 0;
            HeapPush.Run(heap, len++, 5);
            Assert.AreEqual(1, len);
            Assert.AreEqual(5, heap[0]);
            int val = HeapPop.Run(heap, len--);
            Assert.AreEqual(5, val);
            Assert.AreEqual(0, len);
        }

        [Test]
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

            Assert.AreEqual(7, len);
            Assert.AreEqual(1, HeapPop.Run(heap, len--));
            Assert.AreEqual(2, HeapPop.Run(heap, len--));
            Assert.AreEqual(3, HeapPop.Run(heap, len--));
            Assert.AreEqual(5, HeapPop.Run(heap, len--));
            Assert.AreEqual(7, HeapPop.Run(heap, len--));
            Assert.AreEqual(8, HeapPop.Run(heap, len--));
            Assert.AreEqual(9, HeapPop.Run(heap, len--));
            Assert.AreEqual(0, len);
        }

        [Test]
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
            Assert.AreEqual(3, len);
            Assert.AreEqual(1, HeapPop.Run(heap, len--));
            Assert.AreEqual(3, HeapPop.Run(heap, len--));
            Assert.AreEqual(4, HeapPop.Run(heap, len--));
        }

        [Test]
        public void Deque_BasicOperations()
        {
            const int cap = 5;
            int* deque = stackalloc int[cap];
            int front = 0;
            int back = 0;

            DequePush.PushBackInt32(deque, &front, &back, cap, 10);
            DequePush.PushBackInt32(deque, &front, &back, cap, 20);
            DequePush.PushFrontInt32(deque, &front, &back, cap, 5);

            Assert.AreEqual(5, DequePop.PopFrontInt32(deque, &front, &back, cap));
            Assert.AreEqual(20, DequePop.PopBackInt32(deque, &front, &back, cap));
            Assert.AreEqual(10, DequePop.PopFrontInt32(deque, &front, &back, cap));
        }

        [Test]
        public void MonotonicQueueMin_Basic()
        {
            int* src = stackalloc int[] { 2, 1, 4, 3, 6, 5 };
            int len = 6;
            int* dst = stackalloc int[4];
            MonotonicQueueMin.Run(src, dst, len, 3);
            Assert.AreEqual(1, dst[0]);
            Assert.AreEqual(1, dst[1]);
            Assert.AreEqual(3, dst[2]);
            Assert.AreEqual(3, dst[3]);
        }

        [Test]
        public void MonotonicQueuePush_MinInt32()
        {
            int* mono = stackalloc int[10];
            int size = 0;
            MonotonicQueuePush.MinInt32(mono, &size, 3);
            MonotonicQueuePush.MinInt32(mono, &size, 5);
            MonotonicQueuePush.MinInt32(mono, &size, 2);
            Assert.AreEqual(1, size);
            Assert.AreEqual(2, mono[0]);
        }

        [Test]
        public void MonotonicStackProcess_NextGreaterInt32()
        {
            int* src = stackalloc int[] { 2, 1, 4, 3, 6, 5 };
            int len = 6;
            int* dst = stackalloc int[len];
            for (int i = 0; i < len; i++) dst[i] = -1;
            MonotonicStackProcess.NextGreaterInt32(src, dst, len);
            Assert.AreEqual(2, dst[0]);
            Assert.AreEqual(2, dst[1]);
            Assert.AreEqual(4, dst[2]);
            Assert.AreEqual(4, dst[3]);
            Assert.AreEqual(-1, dst[4]);
            Assert.AreEqual(-1, dst[5]);
        }
    }
}