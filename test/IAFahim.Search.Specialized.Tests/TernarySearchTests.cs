namespace IAFahim.Search.Specialized.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class TernarySearchTests
    {
        [Test]
        public void EmptyInput_ReturnsNegatedZero()
        {
            int pos = IAFahim.Search.Specialized.TernarySearch.Run(null, 0, 5);
            Assert.AreEqual(~0, pos);
        }

        [Test]
        public void SingleElement_Found()
        {
            int* ptr = stackalloc int[] { 5 };
            int pos = IAFahim.Search.Specialized.TernarySearch.Run(ptr, 1, 5);
            Assert.AreEqual(0, pos);
        }

        [Test]
        public void SingleElement_NotFound()
        {
            int* ptr = stackalloc int[] { 3 };
            int pos = IAFahim.Search.Specialized.TernarySearch.Run(ptr, 1, 5);
            Assert.AreEqual(~1, pos);
        }

        [Test]
        public void MultipleElements_FoundFirstMid()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4, 5 };
            int pos = IAFahim.Search.Specialized.TernarySearch.Run(ptr, 5, 3);
            Assert.AreEqual(2, pos);
        }

        [Test]
        public void MultipleElements_FoundSecondMid()
        {
            int* ptr = stackalloc int[] { 1, 2, 4, 5, 6 };
            int pos = IAFahim.Search.Specialized.TernarySearch.Run(ptr, 5, 4);
            Assert.AreEqual(2, pos);
        }

        [Test]
        public void MultipleElements_NotFound()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7 };
            int pos = IAFahim.Search.Specialized.TernarySearch.Run(ptr, 4, 4);
            Assert.IsTrue(pos < 0);
        }

        [Test]
        public void LargeN_FoundCorrectly()
        {
            const int N = 1024;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++)
                    ptr[i] = i * 2;
                int pos = IAFahim.Search.Specialized.TernarySearch.Run(ptr, N, 500);
                Assert.AreEqual(250, pos);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }

        [Test]
        public void LargeN_NotFound()
        {
            const int N = 1024;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++)
                    ptr[i] = i * 2;
                int pos = IAFahim.Search.Specialized.TernarySearch.Run(ptr, N, 501);
                Assert.IsTrue(pos < 0);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }
    }
}