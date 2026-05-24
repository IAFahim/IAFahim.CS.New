namespace IAFahim.Search.Range.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class RangeAddTests
    {
        [Test]
        public void Normal()
        {
            int* diff = stackalloc int[10];
            for (int i = 0; i < 10; i++) diff[i] = 0;
            RangeAdd.Run(diff, 10, 2, 5, 3);
            Assert.AreEqual(3, diff[2]);
            Assert.AreEqual(-3, diff[6]);
        }

        [Test]
        public void Materialize()
        {
            int* diff = stackalloc int[10];
            int* dst = stackalloc int[10];
            for (int i = 0; i < 10; i++) diff[i] = 0;
            RangeAdd.Run(diff, 10, 1, 3, 2);
            RangeAdd.Materialize(dst, diff, 10);
            for (int i = 0; i < 10; i++)
            {
                if (i >= 1 && i <= 3)
                    Assert.AreEqual(2, dst[i]);
                else
                    Assert.AreEqual(0, dst[i]);
            }
        }
    }

    public sealed unsafe class RangeSumTests
    {
        [Test]
        public void BuildPrefix()
        {
            int* src = stackalloc int[] { 1, 2, 3, 4 };
            int* dst = stackalloc int[4];
            RangeSum.BuildPrefix(dst, src, 4);
            Assert.AreEqual(1, dst[0]);
            Assert.AreEqual(3, dst[1]);
            Assert.AreEqual(6, dst[2]);
            Assert.AreEqual(10, dst[3]);
        }

        [Test]
        public void Run()
        {
            int* src = stackalloc int[] { 1, 2, 3, 4 };
            int* prefix = stackalloc int[5];
            RangeSum.BuildPrefix(prefix, src, 4);
            prefix[4] = 10;
            Assert.AreEqual(9, RangeSum.Run(prefix, 1, 3));
            Assert.AreEqual(10, RangeSum.Run(prefix, 0, 3));
            Assert.AreEqual(1, RangeSum.Run(prefix, 0, 0));
        }
    }

    public sealed unsafe class RangeMinTests
    {
        [Test]
        public void BuildAndQuery()
        {
            int* src = stackalloc int[] { 3, 1, 4, 1, 5, 9, 2, 6 };
            int* sparse = (int*)Marshal.AllocHGlobal(8 * 4 * 3);
            try
            {
                RangeMin.BuildSparse(sparse, src, 8);
                Assert.AreEqual(1, RangeMin.Query(sparse, src, 8, 0, 3));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)sparse);
            }
        }
    }

    public sealed unsafe class RangeMaxTests
    {
        [Test]
        public void BuildAndQuery()
        {
            int* src = stackalloc int[] { 3, 1, 4, 1, 5, 9, 2, 6 };
            int* sparse = (int*)Marshal.AllocHGlobal(8 * 4 * 3);
            try
            {
                RangeMax.BuildSparse(sparse, src, 8);
                Assert.AreEqual(4, RangeMax.Query(sparse, 8, 0, 3));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)sparse);
            }
        }
    }
}