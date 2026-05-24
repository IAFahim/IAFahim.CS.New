namespace IAFahim.Search.Specialized.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class LowerBoundTests
    {
        [Test]
        public void EmptyInput_ReturnsZero()
        {
            int pos = IAFahim.Search.Specialized.LowerBound.Run(null, 0, 5);
            Assert.AreEqual(0, pos);
        }

        [Test]
        public void SingleElement_Found()
        {
            int* ptr = stackalloc int[] { 5 };
            int pos = IAFahim.Search.Specialized.LowerBound.Run(ptr, 1, 5);
            Assert.AreEqual(0, pos);
        }

        [Test]
        public void SingleElement_NotFound()
        {
            int* ptr = stackalloc int[] { 3 };
            int pos = IAFahim.Search.Specialized.LowerBound.Run(ptr, 1, 5);
            Assert.AreEqual(1, pos);
        }

        [Test]
        public void MultipleElements_FoundFirst()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7, 9 };
            int pos = IAFahim.Search.Specialized.LowerBound.Run(ptr, 5, 3);
            Assert.AreEqual(1, pos);
        }

        [Test]
        public void MultipleElements_NotFoundInsertsAtEnd()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7 };
            int pos = IAFahim.Search.Specialized.LowerBound.Run(ptr, 4, 10);
            Assert.AreEqual(4, pos);
        }

        [Test]
        public void MultipleElements_InsertsAtBegin()
        {
            int* ptr = stackalloc int[] { 3, 5, 7, 9 };
            int pos = IAFahim.Search.Specialized.LowerBound.Run(ptr, 4, 1);
            Assert.AreEqual(0, pos);
        }

        [Test]
        public void AllSameElements_ReturnsFirst()
        {
            int* ptr = stackalloc int[] { 5, 5, 5, 5 };
            int pos = IAFahim.Search.Specialized.LowerBound.Run(ptr, 4, 5);
            Assert.AreEqual(0, pos);
        }

        [Test]
        public void LargeN_CorrectPosition()
        {
            const int N = 1024;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++)
                    ptr[i] = i * 2;
                int pos = IAFahim.Search.Specialized.LowerBound.Run(ptr, N, 1000);
                Assert.AreEqual(500, pos);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }
    }
}