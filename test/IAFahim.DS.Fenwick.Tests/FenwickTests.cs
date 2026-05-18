namespace IAFahim.DS.Fenwick.Tests
{
    using IAFahim.DS.Fenwick;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class FenwickTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            int* bit = stackalloc int[0];
            FenwickAdd.Run(bit, 0, 5);
            Assert.Equal(0, FenwickSum.Run(bit, 0));
        }

        [Fact]
        public void SingleElement_AddAndSum()
        {
            int* bit = stackalloc int[1];
            FenwickAdd.Run(bit, 0, 5);
            Assert.Equal(5, FenwickSum.Run(bit, 1));
        }

        [Fact]
        public void RangeSum_MultipleAdds()
        {
            const int n = 8;
            int* bit = stackalloc int[n];
            FenwickAdd.Run(bit, 0, 1);
            FenwickAdd.Run(bit, 1, 2);
            FenwickAdd.Run(bit, 2, 3);
            FenwickAdd.Run(bit, 3, 4);
            Assert.Equal(1, FenwickSum.Run(bit, 1));
            Assert.Equal(3, FenwickSum.Run(bit, 2));
            Assert.Equal(6, FenwickSum.Run(bit, 3));
            Assert.Equal(10, FenwickSum.Run(bit, 4));
        }

        [Fact]
        public void LowerBound_FindPrefixSum()
        {
            const int n = 10;
            int* bit = stackalloc int[n];
            for (int i = 0; i < n; i++)
                FenwickAdd.Run(bit, i, i + 1);

            Assert.Equal(0, FenwickLowerBound.Run(bit, n, 1));
            Assert.Equal(1, FenwickLowerBound.Run(bit, n, 2));
            Assert.Equal(9, FenwickLowerBound.Run(bit, n, 10));
            Assert.Equal(9, FenwickLowerBound.Run(bit, n, 55));
        }

        [Fact]
        public void Fenwick2D_Basic()
        {
            const int n = 4, m = 4;
            long* bit = stackalloc long[n * m];
            Fenwick2DAdd.Run(bit, n, m, 1, 1, 5);
            Fenwick2DAdd.Run(bit, n, m, 2, 2, 3);
            Assert.Equal(5, Fenwick2DSum.Run(bit, n, m, 2, 2));
        }

        [Fact]
        public void RangeAdd_PointQuery()
        {
            const int n = 5;
            int* bit = stackalloc int[n];
            FenwickRangeAdd.Run(bit, 0, 4, 10);
            FenwickRangeAdd.Run(bit, 1, 3, 5);
            Assert.Equal(10, FenwickPointQuery.Run(bit, 0));
            Assert.Equal(15, FenwickPointQuery.Run(bit, 1));
            Assert.Equal(15, FenwickPointQuery.Run(bit, 3));
            Assert.Equal(10, FenwickPointQuery.Run(bit, 4));
        }

        [Fact]
        public void LargeN_CorrectPrefixSums()
        {
            const int n = 1024;
            int* bit = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++)
                    FenwickAdd.Run(bit, i, i);
                for (int i = 0; i < n; i++)
                {
                    long expected = (long)i * (i + 1) / 2;
                    Assert.Equal(expected, FenwickSum.Run(bit, i + 1));
                }
            }
            finally { Marshal.FreeHGlobal((nint)bit); }
        }
    }
}