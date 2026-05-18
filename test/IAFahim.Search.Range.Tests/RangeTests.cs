namespace IAFahim.Search.Range.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class RangeAddTests
    {
        [Fact]
        public void Normal()
        {
            int* diff = stackalloc int[10];
            for (int i = 0; i < 10; i++) diff[i] = 0;
            RangeAdd.Run(diff, 10, 2, 5, 3);
            Assert.Equal(3, diff[2]);
            Assert.Equal(-3, diff[6]);
        }

        [Fact]
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
                    Assert.Equal(2, dst[i]);
                else
                    Assert.Equal(0, dst[i]);
            }
        }
    }

    public sealed unsafe class RangeSumTests
    {
        [Fact]
        public void BuildPrefix()
        {
            int* src = stackalloc int[] { 1, 2, 3, 4 };
            int* dst = stackalloc int[4];
            RangeSum.BuildPrefix(dst, src, 4);
            Assert.Equal(1, dst[0]);
            Assert.Equal(3, dst[1]);
            Assert.Equal(6, dst[2]);
            Assert.Equal(10, dst[3]);
        }

        [Fact]
        public void Run()
        {
            int* src = stackalloc int[] { 1, 2, 3, 4 };
            int* prefix = stackalloc int[5];
            RangeSum.BuildPrefix(prefix, src, 4);
            prefix[4] = 10;
            Assert.Equal(9, RangeSum.Run(prefix, 1, 3));
            Assert.Equal(10, RangeSum.Run(prefix, 0, 3));
            Assert.Equal(1, RangeSum.Run(prefix, 0, 0));
        }
    }

    public sealed unsafe class RangeMinTests
    {
        [Fact]
        public void BuildAndQuery()
        {
            int* src = stackalloc int[] { 3, 1, 4, 1, 5, 9, 2, 6 };
            int* sparse = (int*)Marshal.AllocHGlobal(8 * 4 * 3);
            try
            {
                RangeMin.BuildSparse(sparse, src, 8);
                Assert.Equal(1, RangeMin.Query(sparse, src, 8, 0, 3));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)sparse);
            }
        }
    }

    public sealed unsafe class RangeMaxTests
    {
        [Fact]
        public void BuildAndQuery()
        {
            int* src = stackalloc int[] { 3, 1, 4, 1, 5, 9, 2, 6 };
            int* sparse = (int*)Marshal.AllocHGlobal(8 * 4 * 3);
            try
            {
                RangeMax.BuildSparse(sparse, src, 8);
                Assert.Equal(4, RangeMax.Query(sparse, 8, 0, 3));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)sparse);
            }
        }
    }
}