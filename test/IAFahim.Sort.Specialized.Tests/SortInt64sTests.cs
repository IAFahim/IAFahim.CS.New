namespace IAFahim.Sort.Specialized.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class SortInt64sTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            SortInt64s.Run(null, 0);
        }

        [Fact]
        public void SingleElement_Unchanged()
        {
            long val = 42L;
            SortInt64s.Run(&val, 1);
            Assert.Equal(42L, val);
        }

        [Fact]
        public void AlreadySorted_Unchanged()
        {
            long* ptr = stackalloc long[] { 1, 2, 3, 4 };
            SortInt64s.Run(ptr, 4);
            Assert.Equal(1, ptr[0]);
            Assert.Equal(2, ptr[1]);
            Assert.Equal(3, ptr[2]);
            Assert.Equal(4, ptr[3]);
        }

        [Fact]
        public void Reversed_Sorts()
        {
            long* ptr = stackalloc long[] { 4, 3, 2, 1 };
            SortInt64s.Run(ptr, 4);
            Assert.Equal(1, ptr[0]);
            Assert.Equal(2, ptr[1]);
            Assert.Equal(3, ptr[2]);
            Assert.Equal(4, ptr[3]);
        }

        [Fact]
        public void LargeN_CorrectOrder()
        {
            const int N = 512;
            long* ptr = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            for (int i = 0; i < N; i++)
                ptr[i] = N - i;
            SortInt64s.Run(ptr, N);
            for (int i = 0; i < N; i++)
                Assert.Equal(i + 1, ptr[i]);
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}