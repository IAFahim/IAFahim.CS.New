namespace IAFahim.DS.Fenwick.Tests
{
    using IAFahim.DS.Fenwick;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class FenwickTests
    {
        [Test]
        public void EmptyInput_NoOp()
        {
            int* bit = stackalloc int[2];
            FenwickAdd.Run(bit, 1, 0, 5);
            Assert.AreEqual(5, FenwickSum.Run(bit, 0));
        }

        [Test]
        public void SingleElement_AddAndSum()
        {
            int* bit = stackalloc int[2];
            FenwickAdd.Run(bit, 1, 0, 5);
            Assert.AreEqual(5, FenwickSum.Run(bit, 0));
        }

        [Test]
        public void RangeSum_MultipleAdds()
        {
            const int n = 8;
            int* bit = stackalloc int[n + 1];
            FenwickAdd.Run(bit, n, 0, 1);
            FenwickAdd.Run(bit, n, 1, 2);
            FenwickAdd.Run(bit, n, 2, 3);
            FenwickAdd.Run(bit, n, 3, 4);
            Assert.AreEqual(1, FenwickSum.Run(bit, 0));
            Assert.AreEqual(3, FenwickSum.Run(bit, 1));
            Assert.AreEqual(6, FenwickSum.Run(bit, 2));
            Assert.AreEqual(10, FenwickSum.Run(bit, 3));
        }

        [Test]
        public void LowerBound_FindPrefixSum()
        {
            const int n = 10;
            long* bit = stackalloc long[n + 1];
            for (int i = 0; i < n; i++)
                FenwickAdd.RunLong(bit, n, i, (long)(i + 1));

            Assert.AreEqual(0, FenwickLowerBound.Run(bit, n, 1));
            Assert.AreEqual(1, FenwickLowerBound.Run(bit, n, 2));
            Assert.AreEqual(3, FenwickLowerBound.Run(bit, n, 10));
        }

        [Test]
        public void Fenwick2D_Basic()
        {
            const int n = 4, m = 4;
            long* bit = stackalloc long[(n + 1) * (m + 1)];
            for (int i = 0; i < (n + 1) * (m + 1); i++)
                bit[i] = 0;
            Fenwick2DAdd.Run(bit, n, m, 1, 1, 5);
            Fenwick2DAdd.Run(bit, n, m, 2, 2, 3);
            Assert.AreEqual(5, Fenwick2DSum.Run(bit, n, m, 1, 1));
            Assert.AreEqual(8, Fenwick2DSum.Run(bit, n, m, 2, 2));
        }

        [Test]
        public void LargeN_CorrectPrefixSums()
        {
            const int n = 1024;
            int* bit = (int*)Marshal.AllocHGlobal((n + 1) * sizeof(int));
            try
            {
                for (int i = 0; i <= n; i++)
                    bit[i] = 0;
                for (int i = 0; i < n; i++)
                    FenwickAdd.Run(bit, n, i, i);
                for (int i = 1; i < n; i++)
                {
                    long expected = (long)(i - 1) * i / 2;
                    Assert.AreEqual(expected, FenwickSum.Run(bit, i - 1));
                }
            }
            finally { Marshal.FreeHGlobal((nint)bit); }
        }
    }
}
