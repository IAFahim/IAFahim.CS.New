namespace IAFahim.Sort.QuickSort.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class QuickSortTests
    {
        [Test]
        public void EmptyInput_NoOp()
        {
            QuickSort.Run<int>(null, 0);
            QuickSort.RunInt32(null, 0);
        }

        [Test]
        public void SingleElement_NoOp()
        {
            int v = 42;
            QuickSort.Run(&v, 1);
            Assert.AreEqual(42, v);
            QuickSort.RunInt32(&v, 1);
            Assert.AreEqual(42, v);
        }

        [Test]
        public void AlreadySorted_StableOrder()
        {
            const int N = 32;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = i;
                QuickSort.Run(ptr, N);
                for (int i = 0; i < N; i++) Assert.AreEqual(i, ptr[i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }

        [Test]
        public void Reversed_SortsAscending()
        {
            const int N = 64;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = N - i;
                QuickSort.Run(ptr, N);
                for (int i = 0; i < N; i++) Assert.AreEqual(i + 1, ptr[i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }

        [Test]
        public void Duplicates_Sorts()
        {
            const int N = 48;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = i % 5;
                QuickSort.RunInt32(ptr, N);
                for (int i = 1; i < N; i++) Assert.LessOrEqual(ptr[i - 1], ptr[i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }

        [Test]
        public void Randomized_MatchesArraySort()
        {
            const int Trials = 20;
            const int N = 256;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            int[] expected = new int[N];
            try
            {
                Random rng = new Random(42);
                for (int t = 0; t < Trials; t++)
                {
                    for (int i = 0; i < N; i++)
                    {
                        int v = rng.Next(int.MinValue, int.MaxValue);
                        ptr[i] = v;
                        expected[i] = v;
                    }
                    Array.Sort(expected);
                    QuickSort.RunInt32(ptr, N);
                    for (int i = 0; i < N; i++) Assert.AreEqual(expected[i], ptr[i]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }

        [Test]
        public void SignedEdges_Sorts()
        {
            int* ptr = (int*)Marshal.AllocHGlobal(5 * sizeof(int));
            try
            {
                ptr[0] = int.MaxValue;
                ptr[1] = -1;
                ptr[2] = 0;
                ptr[3] = int.MinValue;
                ptr[4] = 1;
                QuickSort.Run(ptr, 5);
                Assert.AreEqual(int.MinValue, ptr[0]);
                Assert.AreEqual(-1, ptr[1]);
                Assert.AreEqual(0, ptr[2]);
                Assert.AreEqual(1, ptr[3]);
                Assert.AreEqual(int.MaxValue, ptr[4]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }
    }
}
