namespace IAFahim.Compress.Coordinate.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class CoordinateCompressTests
    {
        [Test]
        public void Normal()
        {
            int* src = stackalloc int[] { 5, 2, 8, 2, 5 };
            int* tmp = stackalloc int[5];
            int* dst = stackalloc int[5];
            int unique = CoordinateCompress.Run(src, tmp, dst, 5);
            Assert.AreEqual(3, unique);
        }

        [Test]
        public void AllSame()
        {
            int* src = stackalloc int[] { 7, 7, 7 };
            int* tmp = stackalloc int[3];
            int* dst = stackalloc int[3];
            int unique = CoordinateCompress.Run(src, tmp, dst, 3);
            Assert.AreEqual(1, unique);
        }

        [Test]
        public void Empty()
        {
            int* tmp = stackalloc int[0];
            int* dst = stackalloc int[0];
            int unique = CoordinateCompress.Run(null, tmp, dst, 0);
            Assert.AreEqual(0, unique);
        }
    }

    public sealed unsafe class RankCompressTests
    {
        [Test]
        public void Normal()
        {
            int* src = stackalloc int[] { 5, 2, 8, 2, 5 };
            int* dst = stackalloc int[5];
            int* tmp = (int*)Marshal.AllocHGlobal(5 * sizeof(int));
            try
            {
                int unique = RankCompress.Run(src, dst, tmp, 5);
                Assert.AreEqual(3, unique);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)tmp);
            }
        }

        [Test]
        public void Empty()
        {
            int* tmp = stackalloc int[0];
            int* dst = stackalloc int[0];
            int unique = RankCompress.Run(null, dst, tmp, 0);
            Assert.AreEqual(0, unique);
        }
    }

    public sealed unsafe class DiscretizeTests
    {
        [Test]
        public void Normal()
        {
            int* src = stackalloc int[] { 5, 2, 8, 2, 5 };
            int unique = Discretize.Run(src, 5);
            Assert.AreEqual(3, unique);
        }

        [Test]
        public void AllSame()
        {
            int* src = stackalloc int[] { 7, 7, 7 };
            int unique = Discretize.Run(src, 3);
            Assert.AreEqual(1, unique);
        }
    }
}