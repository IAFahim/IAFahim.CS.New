namespace IAFahim.Sort.Specialized.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class SortIntsTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            SortInts.Run(null, 0);
        }

        [Fact]
        public void SingleElement_Unchanged()
        {
            int val = 42;
            SortInts.Run(&val, 1);
            Assert.Equal(42, val);
        }

        [Fact]
        public void AlreadySorted_Unchanged()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4 };
            SortInts.Run(ptr, 4);
            Assert.Equal(1, ptr[0]);
            Assert.Equal(2, ptr[1]);
            Assert.Equal(3, ptr[2]);
            Assert.Equal(4, ptr[3]);
        }

        [Fact]
        public void Reversed_Sorts()
        {
            int* ptr = stackalloc int[] { 4, 3, 2, 1 };
            SortInts.Run(ptr, 4);
            Assert.Equal(1, ptr[0]);
            Assert.Equal(2, ptr[1]);
            Assert.Equal(3, ptr[2]);
            Assert.Equal(4, ptr[3]);
        }

        [Fact]
        public void AllDuplicates_Unchanged()
        {
            int* ptr = stackalloc int[] { 7, 7, 7, 7 };
            SortInts.Run(ptr, 4);
            for (int i = 0; i < 4; i++)
                Assert.Equal(7, ptr[i]);
        }

        [Fact]
        public void LargeN_CorrectOrder()
        {
            const int N = 1024;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++)
                    ptr[i] = N - i;
                SortInts.Run(ptr, N);
                for (int i = 0; i < N; i++)
                    Assert.Equal(i + 1, ptr[i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }

        [Fact]
        public void NegativeNumbers_Sorts()
        {
            int* ptr = stackalloc int[] { -5, 3, 0, -1, 2 };
            SortInts.Run(ptr, 5);
            Assert.Equal(-5, ptr[0]);
            Assert.Equal(-1, ptr[1]);
            Assert.Equal(0, ptr[2]);
            Assert.Equal(2, ptr[3]);
            Assert.Equal(3, ptr[4]);
        }
    }
}