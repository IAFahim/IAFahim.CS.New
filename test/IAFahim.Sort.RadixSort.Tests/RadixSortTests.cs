namespace IAFahim.Sort.RadixSort.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class RadixSortTests
    {
        [Test]
        public void Run_SortsSignedIntEdges()
        {
            const int N = 5;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                ptr[0] = int.MaxValue; ptr[1] = -1; ptr[2] = 0; ptr[3] = int.MinValue; ptr[4] = 1;
                RadixSortLsd.Run(ptr, N);
                Assert.AreEqual(int.MinValue, ptr[0]);
                Assert.AreEqual(int.MaxValue, ptr[N - 1]);
                for (int i = 1; i < N; i++) Assert.LessOrEqual(ptr[i - 1], ptr[i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }

        [Test]
        public void Run_RandomizedMatchesArraySort()
        {
            const int N = 128;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            int[] expected = new int[N];
            try
            {
                Random rng = new Random(42);
                for (int i = 0; i < N; i++)
                {
                    int v = rng.Next();
                    ptr[i] = v;
                    expected[i] = v;
                }
                Array.Sort(expected);
                RadixSortLsd.Run(ptr, N);
                for (int i = 0; i < N; i++) Assert.AreEqual(expected[i], ptr[i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }
    }
}
