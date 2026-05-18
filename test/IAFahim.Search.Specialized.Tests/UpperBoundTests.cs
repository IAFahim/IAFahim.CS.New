namespace IAFahim.Search.Specialized.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class UpperBoundTests
    {
        [Fact]
        public void EmptyInput_ReturnsZero()
        {
            int pos = IAFahim.Search.Specialized.UpperBound.Run(null, 0, 5);
            Assert.Equal(0, pos);
        }

        [Fact]
        public void SingleElement_Found()
        {
            int* ptr = stackalloc int[] { 5 };
            int pos = IAFahim.Search.Specialized.UpperBound.Run(ptr, 1, 5);
            Assert.Equal(1, pos);
        }

        [Fact]
        public void SingleElement_NotFound()
        {
            int* ptr = stackalloc int[] { 3 };
            int pos = IAFahim.Search.Specialized.UpperBound.Run(ptr, 1, 5);
            Assert.Equal(1, pos);
        }

        [Fact]
        public void MultipleElements_ReturnsPastLast()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7, 9 };
            int pos = IAFahim.Search.Specialized.UpperBound.Run(ptr, 5, 5);
            Assert.Equal(3, pos);
        }

        [Fact]
        public void MultipleElements_NotFoundAtEnd()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7 };
            int pos = IAFahim.Search.Specialized.UpperBound.Run(ptr, 4, 10);
            Assert.Equal(4, pos);
        }

        [Fact]
        public void MultipleElements_ReturnsZero()
        {
            int* ptr = stackalloc int[] { 3, 5, 7, 9 };
            int pos = IAFahim.Search.Specialized.UpperBound.Run(ptr, 4, 1);
            Assert.Equal(0, pos);
        }

        [Fact]
        public void AllSameElements_ReturnsEnd()
        {
            int* ptr = stackalloc int[] { 5, 5, 5, 5 };
            int pos = IAFahim.Search.Specialized.UpperBound.Run(ptr, 4, 5);
            Assert.Equal(4, pos);
        }

        [Fact]
        public void LargeN_CorrectPosition()
        {
            const int N = 1024;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++)
                    ptr[i] = i * 2;
                int pos = IAFahim.Search.Specialized.UpperBound.Run(ptr, N, 1000);
                Assert.Equal(501, pos);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }
    }
}