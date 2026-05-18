namespace IAFahim.Search.Window.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class SlidingWindowMinTests
    {
        [Fact]
        public void Normal()
        {
            int* src = stackalloc int[] { 3, 1, 4, 1, 5, 9, 2, 6 };
            int* dst = (int*)Marshal.AllocHGlobal(6 * sizeof(int));
            try
            {
                SlidingWindowMin.Run(src, dst, 8, 3);
                Assert.Equal(1, dst[0]);
                Assert.Equal(1, dst[1]);
                Assert.Equal(1, dst[2]);
                Assert.Equal(1, dst[3]);
                Assert.Equal(2, dst[4]);
                Assert.Equal(2, dst[5]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)dst);
            }
        }

        [Fact]
        public void WindowSizeOne()
        {
            int* src = stackalloc int[] { 3, 1, 4 };
            int* dst = stackalloc int[3];
            SlidingWindowMin.Run(src, dst, 3, 1);
            Assert.Equal(3, dst[0]);
            Assert.Equal(1, dst[1]);
            Assert.Equal(4, dst[2]);
        }
    }

    public sealed unsafe class SlidingWindowMaxTests
    {
        [Fact]
        public void Normal()
        {
            int* src = stackalloc int[] { 3, 1, 4, 1, 5, 9, 2, 6 };
            int* dst = (int*)Marshal.AllocHGlobal(6 * sizeof(int));
            try
            {
                SlidingWindowMax.Run(src, dst, 8, 3);
                Assert.Equal(4, dst[0]);
                Assert.Equal(4, dst[1]);
                Assert.Equal(5, dst[2]);
                Assert.Equal(9, dst[3]);
                Assert.Equal(9, dst[4]);
                Assert.Equal(9, dst[5]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)dst);
            }
        }

        [Fact]
        public void WindowSizeOne()
        {
            int* src = stackalloc int[] { 3, 1, 4 };
            int* dst = stackalloc int[3];
            SlidingWindowMax.Run(src, dst, 3, 1);
            Assert.Equal(3, dst[0]);
            Assert.Equal(1, dst[1]);
            Assert.Equal(4, dst[2]);
        }
    }
}