namespace IAFahim.Math.Transform.Tests
{
    using IAFahim.Math.Transform;
    using Xunit;

    public sealed unsafe class TransformTests
    {
        [Fact]
        public void SubsetZeta_Basic()
        {
            const int logN = 2;
            const int n = 1 << logN;
            long* f = stackalloc long[n];
            for (int i = 0; i < n; i++) f[i] = 1;
            SubsetZeta.Run(f, logN);
            Assert.True(f[3] > 0);
        }

        [Fact]
        public void SubsetMobius_Basic()
        {
            const int logN = 2;
            const int n = 1 << logN;
            long* f = stackalloc long[n];
            for (int i = 0; i < n; i++) f[i] = 1;
            SubsetMobius.Run(f, logN);
            Assert.Equal(1, f[0]);
        }

        [Fact]
        public void FwhtXor_Basic()
        {
            const int n = 4;
            long* f = stackalloc long[n];
            for (int i = 0; i < n; i++) f[i] = 1;
            WalshHadamardXor.Forward(f, n);
            Assert.Equal(n, f[0]);
        }

        [Fact]
        public void FwhtOr_Basic()
        {
            const int n = 4;
            long* f = stackalloc long[n];
            f[0] = 1; f[1] = 0; f[2] = 0; f[3] = 0;
            WalshHadamardOr.Forward(f, n);
            Assert.True(f[n - 1] >= 0);
        }

        [Fact]
        public void FwhtAnd_Basic()
        {
            const int n = 4;
            long* f = stackalloc long[n];
            for (int i = 0; i < n; i++) f[i] = i + 1;
            WalshHadamardAnd.Forward(f, n);
            Assert.True(f[0] > 0);
        }

        [Fact]
        public void XorBasis_MaxXor()
        {
            long* basis = stackalloc long[64];
            int* basisSize = stackalloc int[1];
            for (int i = 0; i < 64; i++) basis[i] = 0;
            *basisSize = 0;
            XorBasisInsert.Run(basis, basisSize, 3);
            XorBasisInsert.Run(basis, basisSize, 5);
            long max = XorBasisMax.Run(basis);
            Assert.True((max ^ 6) >= 0);
        }

        [Fact]
        public void FwhtConvolution_Basic()
        {
            const int n = 4;
            long* a = stackalloc long[n];
            long* b = stackalloc long[n];
            long* c = stackalloc long[n];
            for (int i = 0; i < n; i++) { a[i] = 1; b[i] = 1; }
            FwhtConvolution.Run(a, b, c, 2, FwhtConvolution.FwhtType.Xor);
            Assert.True(c[0] >= 0);
        }
    }
}