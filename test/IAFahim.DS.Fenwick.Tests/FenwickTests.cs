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
            int* bit = stackalloc int[1];
            FenwickAdd.Run(bit, 1, 0, 5);
            Assert.Equal(5, FenwickSum.Run(bit, 0));
        }

        [Fact]
        public void SingleElement_AddAndSum()
        {
            int* bit = stackalloc int[1];
            FenwickAdd.Run(bit, 1, 0, 5);
            Assert.Equal(5, FenwickSum.Run(bit, 0));
        }

        [Fact]
        public void RangeSum_MultipleAdds()
        {
            const int n = 8;
            int* bit = stackalloc int[n];
            FenwickAdd.Run(bit, n, 0, 1);
            FenwickAdd.Run(bit, n, 1, 2);
            FenwickAdd.Run(bit, n, 2, 3);
            FenwickAdd.Run(bit, n, 3, 4);
            Assert.Equal(1, FenwickSum.Run(bit, 0));
            Assert.Equal(3, FenwickSum.Run(bit, 1));
            Assert.Equal(6, FenwickSum.Run(bit, 2));
            Assert.Equal(10, FenwickSum.Run(bit, 3));
        }

        [Fact]
        public void LowerBound_FindPrefixSum()
        {
            const int n = 10;
            long* bit = stackalloc long[n];
            for (int i = 0; i < n; i++)
                FenwickAdd.RunLong(bit, n, i, (long)(i + 1));

            Assert.Equal(0, FenwickLowerBound.Run(bit, n, 1));
            Assert.Equal(1, FenwickLowerBound.Run(bit, n, 2));
            Assert.Equal(8, FenwickLowerBound.Run(bit, n, 10));
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
        public void LargeN_CorrectPrefixSums()
        {
            const int n = 1024;
            int* bit = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++)
                    FenwickAdd.Run(bit, n, i, i);
                for (int i = 1; i < n; i++)
                {
                    long expected = (long)(i - 1) * i / 2;
                    Assert.Equal(expected, FenwickSum.Run(bit, i - 1));
                }
            }
            finally { Marshal.FreeHGlobal((nint)bit); }
        }
    }
}
