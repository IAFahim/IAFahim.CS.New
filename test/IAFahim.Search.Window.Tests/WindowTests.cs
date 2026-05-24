namespace IAFahim.Search.Window.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class SlidingWindowMinTests
    {
        [Test]
        public void Normal()
        {
            int* src = stackalloc int[] { 3, 1, 4, 1, 5, 9, 2, 6 };
            int* dst = (int*)Marshal.AllocHGlobal(6 * sizeof(int));
            try
            {
                SlidingWindowMin.Run(src, dst, 8, 3);
                Assert.AreEqual(1, dst[0]);
                Assert.AreEqual(1, dst[1]);
                Assert.AreEqual(1, dst[2]);
                Assert.AreEqual(1, dst[3]);
                Assert.AreEqual(2, dst[4]);
                Assert.AreEqual(2, dst[5]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)dst);
            }
        }

        [Test]
        public void WindowSizeOne()
        {
            int* src = stackalloc int[] { 3, 1, 4 };
            int* dst = stackalloc int[3];
            SlidingWindowMin.Run(src, dst, 3, 1);
            Assert.AreEqual(3, dst[0]);
            Assert.AreEqual(1, dst[1]);
            Assert.AreEqual(4, dst[2]);
        }
    }

    public sealed unsafe class SlidingWindowMaxTests
    {
        [Test]
        public void Normal()
        {
            int* src = stackalloc int[] { 3, 1, 4, 1, 5, 9, 2, 6 };
            int* dst = (int*)Marshal.AllocHGlobal(6 * sizeof(int));
            try
            {
                SlidingWindowMax.Run(src, dst, 8, 3);
                Assert.AreEqual(4, dst[0]);
                Assert.AreEqual(4, dst[1]);
                Assert.AreEqual(5, dst[2]);
                Assert.AreEqual(9, dst[3]);
                Assert.AreEqual(9, dst[4]);
                Assert.AreEqual(9, dst[5]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)dst);
            }
        }

        [Test]
        public void WindowSizeOne()
        {
            int* src = stackalloc int[] { 3, 1, 4 };
            int* dst = stackalloc int[3];
            SlidingWindowMax.Run(src, dst, 3, 1);
            Assert.AreEqual(3, dst[0]);
            Assert.AreEqual(1, dst[1]);
            Assert.AreEqual(4, dst[2]);
        }
    }
}