namespace IAFahim.Unique.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class UniqueIntsTests
    {
        [Fact]
        public void EmptyInput_ReturnsZero()
        {
            int count = IAFahim.Unique.UniqueInts.Run(null, 0);
            Assert.Equal(0, count);
        }

        [Fact]
        public void SingleElement_ReturnsOne()
        {
            int* ptr = stackalloc int[] { 42 };
            int count = IAFahim.Unique.UniqueInts.Run(ptr, 1);
            Assert.Equal(1, count);
            Assert.Equal(42, ptr[0]);
        }

        [Fact]
        public void AllUnique_ReturnsLen()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4, 5 };
            int count = IAFahim.Unique.UniqueInts.Run(ptr, 5);
            Assert.Equal(5, count);
        }

        [Fact]
        public void AllDuplicates_ReturnsOne()
        {
            int* ptr = stackalloc int[] { 7, 7, 7, 7 };
            int count = IAFahim.Unique.UniqueInts.Run(ptr, 4);
            Assert.Equal(1, count);
            Assert.Equal(7, ptr[0]);
        }

        [Fact]
        public void SomeDuplicates_RemovesDuplicates()
        {
            int* ptr = stackalloc int[] { 1, 1, 2, 2, 3, 3 };
            int count = IAFahim.Unique.UniqueInts.Run(ptr, 6);
            Assert.Equal(3, count);
            Assert.Equal(1, ptr[0]);
            Assert.Equal(2, ptr[1]);
            Assert.Equal(3, ptr[2]);
        }

        [Fact]
        public void LargeN_CorrectCount()
        {
            const int N = 1024;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++)
                    ptr[i] = i / 2;
                int count = IAFahim.Unique.UniqueInts.Run(ptr, N);
                Assert.Equal(N / 2, count);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }
    }
}