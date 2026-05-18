namespace IAFahim.DS.Heap.Tests
{
    using IAFahim.DS.Heap;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class HeapTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            int* heap = stackalloc int[1];
            int len = 0;
            HeapPush.Run(heap, &len, 5);
            Assert.Equal(0, len);
        }

        [Fact]
        public void PushPop_SingleElement()
        {
            int* heap = stackalloc int[10];
            int len = 0;
            HeapPush.Run(heap, &len, 5);
            Assert.Equal(1, len);
            Assert.Equal(5, heap[0]);
            int val = HeapPop.Run(heap, &len);
            Assert.Equal(5, val);
            Assert.Equal(0, len);
        }

        [Fact]
        public void PushPop_MaintainsMinHeap()
        {
            int* heap = stackalloc int[100];
            int len = 0;
            int[] vals = { 5, 3, 7, 1, 9, 2, 8 };
            foreach (int v in vals) HeapPush.Run(heap, &len, v);

            Assert.Equal(1, HeapPop.Run(heap, &len));
            Assert.Equal(2, HeapPop.Run(heap, &len));
            Assert.Equal(3, HeapPop.Run(heap, &len));
            Assert.Equal(5, HeapPop.Run(heap, &len));
            Assert.Equal(7, HeapPop.Run(heap, &len));
            Assert.Equal(8, HeapPop.Run(heap, &len));
            Assert.Equal(9, HeapPop.Run(heap, &len));
            Assert.Equal(0, len);
        }

        [Fact]
        public void HeapRemove_Middle()
        {
            int* heap = stackalloc int[100];
            int len = 0;
            HeapPush.Run(heap, &len, 1);
            HeapPush.Run(heap, &len, 2);
            HeapPush.Run(heap, &len, 3);
            HeapPush.Run(heap, &len, 4);
            HeapRemove.Run(heap, &len, 1);
            int first = HeapPop.Run(heap, &len);
            Assert.True(first == 1 || first == 2 || first == 4);
        }

        [Fact]
        public void MonotonicQueueMin_Basic()
        {
            int* dq = stackalloc int[100];
            int* mq = stackalloc int[100];
            int dlen = 0, mlen = 0;
            MonotonicQueuePush.Run(dq, &dlen, mq, &mlen, 3);
            MonotonicQueuePush.Run(dq, &dlen, mq, &mlen, 5);
            MonotonicQueuePush.Run(dq, &dlen, mq, &mlen, 2);
            Assert.Equal(2, MonotonicQueueMin.Run(dq, dlen, mq, mlen));
            MonotonicQueuePop.Run(dq, &dlen, mq, &mlen, 3);
            Assert.Equal(2, MonotonicQueueMin.Run(dq, dlen, mq, mlen));
        }

        [Fact]
        public void MonotonicStackProcess_Basic()
        {
            int* arr = stackalloc int[] { 2, 1, 4, 3, 6, 5 };
            int n = 6;
            int* left = stackalloc int[n];
            int* right = stackalloc int[n];
            for (int i = 0; i < n; i++) { left[i] = -1; right[i] = -1; }
            MonotonicStackProcess.Run(arr, n, left, right);
            Assert.True(left[0] == -1);
            Assert.Equal(0, left[1]);
        }
    }
}