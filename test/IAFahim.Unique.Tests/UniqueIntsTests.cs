namespace IAFahim.Unique.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class UniqueIntsTests
    {
        [Test]
        public void EmptyInput_ReturnsZero()
        {
            int count = IAFahim.Unique.UniqueInts.Run(null, 0);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void SingleElement_ReturnsOne()
        {
            int* ptr = stackalloc int[] { 42 };
            int count = IAFahim.Unique.UniqueInts.Run(ptr, 1);
            Assert.AreEqual(1, count);
            Assert.AreEqual(42, ptr[0]);
        }

        [Test]
        public void AllUnique_ReturnsLen()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4, 5 };
            int count = IAFahim.Unique.UniqueInts.Run(ptr, 5);
            Assert.AreEqual(5, count);
        }

        [Test]
        public void AllDuplicates_ReturnsOne()
        {
            int* ptr = stackalloc int[] { 7, 7, 7, 7 };
            int count = IAFahim.Unique.UniqueInts.Run(ptr, 4);
            Assert.AreEqual(1, count);
            Assert.AreEqual(7, ptr[0]);
        }

        [Test]
        public void SomeDuplicates_RemovesDuplicates()
        {
            int* ptr = stackalloc int[] { 1, 1, 2, 2, 3, 3 };
            int count = IAFahim.Unique.UniqueInts.Run(ptr, 6);
            Assert.AreEqual(3, count);
            Assert.AreEqual(1, ptr[0]);
            Assert.AreEqual(2, ptr[1]);
            Assert.AreEqual(3, ptr[2]);
        }

        [Test]
        public void LargeN_CorrectCount()
        {
            const int N = 1024;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++)
                    ptr[i] = i / 2;
                int count = IAFahim.Unique.UniqueInts.Run(ptr, N);
                Assert.AreEqual(N / 2, count);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }
    }
}