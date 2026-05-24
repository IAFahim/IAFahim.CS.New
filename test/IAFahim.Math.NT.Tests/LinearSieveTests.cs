namespace IAFahim.Math.NT.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class LinearSieveTests
    {
        [Test]
        public void LinearSieveMinPrime_EmptyRange_NoOp()
        {
            int* mp = stackalloc int[2];
            int* pr = stackalloc int[1];
            LinearSieveMinPrime.Run(mp, pr, 1, out int cnt);
            Assert.AreEqual(0, cnt);
        }

        [Test]
        public void LinearSieveMinPrime_Basic()
        {
            const int N = 30;
            int* mp = stackalloc int[N + 1];
            int* pr = stackalloc int[N];
            LinearSieveMinPrime.Run(mp, pr, N, out int cnt);
            Assert.AreEqual(2, mp[4]);
            Assert.AreEqual(3, mp[9]);
            Assert.AreEqual(5, mp[25]);
            Assert.AreEqual(7, mp[7]);
            Assert.IsTrue(cnt > 0);
        }

        [Test]
        public void LinearSieveMaxPrime_Basic()
        {
            const int N = 30;
            int* mp = stackalloc int[N + 1];
            int* pr = stackalloc int[N];
            LinearSieveMaxPrime.Run(mp, pr, N, out int cnt);
            Assert.AreEqual(2, mp[4]);
            Assert.AreEqual(5, mp[10]);
            Assert.AreEqual(7, mp[21]);
            Assert.AreEqual(7, mp[7]);
        }

        [Test]
        public void LinearSievePhi_Basic()
        {
            const int N = 20;
            int* phi = stackalloc int[N + 1];
            int* pr = stackalloc int[N];
            LinearSievePhi.Run(phi, pr, N, out int cnt);
            Assert.AreEqual(1, phi[1]);
            Assert.AreEqual(1, phi[2]);
            Assert.AreEqual(2, phi[3]);
            Assert.AreEqual(2, phi[4]);
            Assert.AreEqual(4, phi[5]);
            Assert.AreEqual(4, phi[8]);
            Assert.AreEqual(6, phi[7]);
        }

        [Test]
        public void LinearSieveDivisorCount_Basic()
        {
            const int N = 20;
            int* d = stackalloc int[N + 1];
            int* e = stackalloc int[N + 1];
            int* pr = stackalloc int[N];
            LinearSieveDivisorCount.Run(d, e, pr, N, out int cnt);
            Assert.AreEqual(1, d[1]);
            Assert.AreEqual(2, d[2]);
            Assert.AreEqual(2, d[3]);
            Assert.AreEqual(3, d[4]);
            Assert.AreEqual(4, d[6]);
            Assert.AreEqual(6, d[12]);
        }

        [Test]
        public void LinearSieveDivisorSum_Basic()
        {
            const int N = 20;
            long* sigma = stackalloc long[N + 1];
            long* sp = stackalloc long[N + 1];
            int* pr = stackalloc int[N];
            LinearSieveDivisorSum.Run(sigma, sp, pr, N, out int cnt);
            Assert.AreEqual(1L, sigma[1]);
            Assert.AreEqual(3L, sigma[2]);
            Assert.AreEqual(4L, sigma[3]);
            Assert.AreEqual(7L, sigma[4]);
            Assert.AreEqual(12L, sigma[6]);
        }

        [Test]
        public void MinPrimeFactor_Basic()
        {
            Assert.AreEqual(0L, MinPrimeFactor.Run(1));
            Assert.AreEqual(2L, MinPrimeFactor.Run(2));
            Assert.AreEqual(3L, MinPrimeFactor.Run(9));
            Assert.AreEqual(5L, MinPrimeFactor.Run(25));
            Assert.AreEqual(7L, MinPrimeFactor.Run(49));
            Assert.AreEqual(97L, MinPrimeFactor.Run(97));
        }

        [Test]
        public void PrimeFactorPower_WithTable()
        {
            const int N = 100;
            int* mp = stackalloc int[N + 1];
            int* pr = stackalloc int[N];
            LinearSieveMinPrime.Run(mp, pr, N, out int _);

            int* outP = stackalloc int[10];
            int* outE = stackalloc int[10];
            int cnt = PrimeFactorPower.Run(12, mp, outP, outE);
            Assert.AreEqual(2, cnt);
        }

        [Test]
        public void PrimeFactorPower_TrialDivision()
        {
            int* outP = stackalloc int[10];
            int* outE = stackalloc int[10];
            int cnt = PrimeFactorPower.Run(60L, outP, outE);
            Assert.AreEqual(3, cnt);
        }

        [Test]
        public void PrimePi_Small()
        {
            long* small = stackalloc long[1002];
            long* large = stackalloc long[1002];
            Assert.AreEqual(4L, PrimePi.Run(10, small, large));
            Assert.AreEqual(25L, PrimePi.Run(100, small, large));
            Assert.AreEqual(168L, PrimePi.Run(1000, small, large));
        }

        [Test]
        public void PrimePiLehmer_Basic()
        {
            int N = 1000000;
            int* mp = (int*)Marshal.AllocHGlobal((N + 1) * sizeof(int));
            int* pr = (int*)Marshal.AllocHGlobal((N + 1) * sizeof(int));
            int* cache = (int*)Marshal.AllocHGlobal(20000 * 101 * sizeof(int));
            try
            {
                LinearSieveMinPrime.Run(mp, pr, N, out int pc);
                PrimePiLehmer.InitPhiCache(cache, pr, pc);
                
                Assert.AreEqual(4L, PrimePiLehmer.Run(10, pr, pc, cache));
                Assert.AreEqual(25L, PrimePiLehmer.Run(100, pr, pc, cache));
                Assert.AreEqual(168L, PrimePiLehmer.Run(1000, pr, pc, cache));
                Assert.AreEqual(78498L, PrimePiLehmer.Run(1000000, pr, pc, cache));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)mp);
                Marshal.FreeHGlobal((nint)pr);
                Marshal.FreeHGlobal((nint)cache);
            }
        }

        [Test]
        public void PrimePiMeissel_Basic()
        {
            int N = 1000000;
            int* mp = (int*)Marshal.AllocHGlobal((N + 1) * sizeof(int));
            int* pr = (int*)Marshal.AllocHGlobal((N + 1) * sizeof(int));
            int* cache = (int*)Marshal.AllocHGlobal(20000 * 101 * sizeof(int));
            try
            {
                LinearSieveMinPrime.Run(mp, pr, N, out int pc);
                PrimePiLehmer.InitPhiCache(cache, pr, pc);
                
                Assert.AreEqual(4L, PrimePiMeissel.Run(10, pr, pc, cache));
                Assert.AreEqual(25L, PrimePiMeissel.Run(100, pr, pc, cache));
                Assert.AreEqual(168L, PrimePiMeissel.Run(1000, pr, pc, cache));
                Assert.AreEqual(78498L, PrimePiMeissel.Run(1000000, pr, pc, cache));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)mp);
                Marshal.FreeHGlobal((nint)pr);
                Marshal.FreeHGlobal((nint)cache);
            }
        }

        [Test]
        public void MinMaxDivisorTransform_Max()
        {
            const int N = 10;
            long* f = stackalloc long[N + 1];
            for (int i = 1; i <= N; i++) f[i] = i;
            MinMaxDivisorTransform.RunMax(f, N);
            Assert.IsTrue(f[6] >= 3);
            Assert.IsTrue(f[6] >= f[2]);
        }

        [Test]
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

        [Test]
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
