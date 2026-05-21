namespace IAFahim.Math.NT.Tests
{
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class LinearSieveTests
    {
        [Fact]
        public void LinearSieveMinPrime_EmptyRange_NoOp()
        {
            int* mp = stackalloc int[2];
            int* pr = stackalloc int[1];
            LinearSieveMinPrime.Run(mp, pr, 1, out int cnt);
            Assert.Equal(0, cnt);
        }

        [Fact]
        public void LinearSieveMinPrime_Basic()
        {
            const int N = 30;
            int* mp = stackalloc int[N + 1];
            int* pr = stackalloc int[N];
            LinearSieveMinPrime.Run(mp, pr, N, out int cnt);
            Assert.Equal(2, mp[4]);
            Assert.Equal(3, mp[9]);
            Assert.Equal(5, mp[25]);
            Assert.Equal(7, mp[7]);
            Assert.True(cnt > 0);
        }

        [Fact]
        public void LinearSieveMaxPrime_Basic()
        {
            const int N = 30;
            int* mp = stackalloc int[N + 1];
            int* pr = stackalloc int[N];
            LinearSieveMaxPrime.Run(mp, pr, N, out int cnt);
            Assert.Equal(2, mp[4]);
            Assert.Equal(5, mp[10]);
            Assert.Equal(7, mp[21]);
            Assert.Equal(7, mp[7]);
        }

        [Fact]
        public void LinearSievePhi_Basic()
        {
            const int N = 20;
            int* phi = stackalloc int[N + 1];
            int* pr = stackalloc int[N];
            LinearSievePhi.Run(phi, pr, N, out int cnt);
            Assert.Equal(1, phi[1]);
            Assert.Equal(1, phi[2]);
            Assert.Equal(2, phi[3]);
            Assert.Equal(2, phi[4]);
            Assert.Equal(4, phi[5]);
            Assert.Equal(4, phi[8]);
            Assert.Equal(6, phi[7]);
        }

        [Fact]
        public void LinearSieveDivisorCount_Basic()
        {
            const int N = 20;
            int* d = stackalloc int[N + 1];
            int* e = stackalloc int[N + 1];
            int* pr = stackalloc int[N];
            LinearSieveDivisorCount.Run(d, e, pr, N, out int cnt);
            Assert.Equal(1, d[1]);
            Assert.Equal(2, d[2]);
            Assert.Equal(2, d[3]);
            Assert.Equal(3, d[4]);
            Assert.Equal(4, d[6]);
            Assert.Equal(6, d[12]);
        }

        [Fact]
        public void LinearSieveDivisorSum_Basic()
        {
            const int N = 20;
            long* sigma = stackalloc long[N + 1];
            long* sp = stackalloc long[N + 1];
            int* pr = stackalloc int[N];
            LinearSieveDivisorSum.Run(sigma, sp, pr, N, out int cnt);
            Assert.Equal(1L, sigma[1]);
            Assert.Equal(3L, sigma[2]);
            Assert.Equal(4L, sigma[3]);
            Assert.Equal(7L, sigma[4]);
            Assert.Equal(12L, sigma[6]);
        }

        [Fact]
        public void MinPrimeFactor_Basic()
        {
            Assert.Equal(0L, MinPrimeFactor.Run(1));
            Assert.Equal(2L, MinPrimeFactor.Run(2));
            Assert.Equal(3L, MinPrimeFactor.Run(9));
            Assert.Equal(5L, MinPrimeFactor.Run(25));
            Assert.Equal(7L, MinPrimeFactor.Run(49));
            Assert.Equal(97L, MinPrimeFactor.Run(97));
        }

        [Fact]
        public void PrimeFactorPower_WithTable()
        {
            const int N = 100;
            int* mp = stackalloc int[N + 1];
            int* pr = stackalloc int[N];
            LinearSieveMinPrime.Run(mp, pr, N, out int _);

            int* outP = stackalloc int[10];
            int* outE = stackalloc int[10];
            int cnt = PrimeFactorPower.Run(12, mp, outP, outE);
            Assert.Equal(2, cnt);
        }

        [Fact]
        public void PrimeFactorPower_TrialDivision()
        {
            int* outP = stackalloc int[10];
            int* outE = stackalloc int[10];
            int cnt = PrimeFactorPower.Run(60L, outP, outE);
            Assert.Equal(3, cnt);
        }

        [Fact]
        public void PrimePi_Small()
        {
            long* small = stackalloc long[1002];
            long* large = stackalloc long[1002];
            Assert.Equal(4L, PrimePi.Run(10, small, large));
            Assert.Equal(25L, PrimePi.Run(100, small, large));
            Assert.Equal(168L, PrimePi.Run(1000, small, large));
        }

        [Fact]
        public void MinMaxDivisorTransform_Max()
        {
            const int N = 10;
            long* f = stackalloc long[N + 1];
            for (int i = 1; i <= N; i++) f[i] = i;
            MinMaxDivisorTransform.RunMax(f, N);
            Assert.True(f[6] >= 3);
            Assert.True(f[6] >= f[2]);
        }

        [Fact]
        public void GcdConvolution_Basic()
        {
            const int N = 10;
            long* a = stackalloc long[N + 1];
            long* b = stackalloc long[N + 1];
            long* res = stackalloc long[N + 1];
            int* mu = stackalloc int[N + 1];
            int* pr = stackalloc int[N];
            MobiusSieve.Run(mu, pr, N, out int _);
            for (int i = 1; i <= N; i++) { a[i] = 1; b[i] = 1; }
            GcdConvolution.Run(a, b, res, N, mu);
        }

        [Fact]
        public void LcmConvolution_Basic()
        {
            const int N = 10;
            long* a = stackalloc long[N + 1];
            long* b = stackalloc long[N + 1];
            long* res = stackalloc long[N + 1];
            int* mu = stackalloc int[N + 1];
            int* pr = stackalloc int[N];
            MobiusSieve.Run(mu, pr, N, out int _);
            for (int i = 1; i <= N; i++) { a[i] = 1; b[i] = 1; }
            LcmConvolution.Run(a, b, res, N, mu);
        }
    }
}
