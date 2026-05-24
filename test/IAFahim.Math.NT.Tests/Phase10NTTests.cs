namespace IAFahim.Math.NT.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using IAFahim.Math.NT;
    using IAFahim.Math.Modular;
    using IAFahim.Math.Combinatorics;

    public sealed unsafe class Phase10NTTests
    {
        [Test]
        public void MillerRabin_LargePrimes_Correct()
        {
            Assert.IsTrue(MillerRabin.Run(999999937L));
            Assert.IsTrue(MillerRabin.Run(1000000007L));
            Assert.IsFalse(MillerRabin.Run(1000000007L * 999999937L));
        }

        [Test]
        public void PollardRho_FactorizeLarge_Correct()
        {
            long n = 1000000007L * 999999937L;
            long* factors = stackalloc long[64];
            int count = Factorize.Run(n, factors);
            Assert.AreEqual(2, count);
            long p1 = factors[0], p2 = factors[1];
            if (p1 > p2) { long t = p1; p1 = p2; p2 = t; }
            Assert.AreEqual(999999937L, p1);
            Assert.AreEqual(1000000007L, p2);
        }

        [Test]
        public void PrimePi_LargeN_Correct()
        {
            long n = 1000000;
            int* primes = (int*)Marshal.AllocHGlobal(100000 * sizeof(int));
            bool* isPrime = (bool*)Marshal.AllocHGlobal(1000001 * sizeof(bool));
            long* w = (long*)Marshal.AllocHGlobal(2005 * sizeof(long));
            long* g = (long*)Marshal.AllocHGlobal(2005 * sizeof(long));
            int* map1 = (int*)Marshal.AllocHGlobal(1000001 * sizeof(int));
            int* map2 = (int*)Marshal.AllocHGlobal(1000001 * sizeof(int));
            try
            {
                long pi = Min25Sieve.PrimePi(n, primes, isPrime, w, g, map1, map2);
                Assert.AreEqual(78498, pi);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)primes);
                Marshal.FreeHGlobal((nint)isPrime);
                Marshal.FreeHGlobal((nint)w);
                Marshal.FreeHGlobal((nint)g);
                Marshal.FreeHGlobal((nint)map1);
                Marshal.FreeHGlobal((nint)map2);
            }
        }

        [Test]
        public void SmoothNumbers_Generate_Correct()
        {
            long limit = 100;
            int* primes = stackalloc int[] { 2, 3, 5 };
            long* res = stackalloc long[100];
            int count = SmoothNumbers.Generate(3, limit, res, primes);
            Assert.IsTrue(count > 0);
            for (int i = 0; i < count; i++)
            {
                long x = res[i];
                while (x % 2 == 0) x /= 2;
                while (x % 3 == 0) x /= 3;
                while (x % 5 == 0) x /= 5;
                Assert.AreEqual(1, x);
            }
        }

        [Test]
        public void HighlyComposite_Generate_Correct()
        {
            long* res = stackalloc long[100];
            HighlyCompositeCandidate* scratch = stackalloc HighlyCompositeCandidate[20000];
            int count = HighlyCompositeNumbers.Run(1000, res, scratch);
            Assert.IsTrue(count >= 3);
            Assert.AreEqual(1, res[0]);
            Assert.AreEqual(2, res[1]);
            Assert.AreEqual(4, res[2]);
            Assert.AreEqual(6, res[3]);
        }

        [Test]
        public void Bsgs_DiscreteLog_Correct()
        {
            long a = 2, b = 8, mod = 11;
            long* sk = stackalloc long[1000], sv = stackalloc long[1000];
            long x = Bsgs.Run(a, b, mod, sk, sv);
            Assert.AreEqual(3, x);
        }

        [Test]
        public void TonelliShanks_Sqrt_Correct()
        {
            long n = 5, p = 11;
            long r = TonelliShanks.Run(n, p);
            Assert.IsTrue(r == 4 || r == 7);
        }

        [Test]
        public void AllFactorizations_Generate_Correct()
        {
            long n = 12;
            long* res = stackalloc long[100];
            int offset;
            int count = AllFactorizations.Run(n, res, out offset);
            Assert.IsTrue(count > 0);
        }

        [Test]
        public void Divisors_SublinearCount_Correct()
        {
            long n = 12;
            long* divs = stackalloc long[100];
            int count = Divisors.Run(n, divs);
            Assert.AreEqual(6, count);
        }

        [Test]
        public void LinearSieve_AllFunctions_Correct()
        {
            int n = 100;
            long* f = stackalloc long[n + 1];
            int* primes = stackalloc int[n + 1];
            int* e = stackalloc int[n + 1];
            long* pk = stackalloc long[n + 1];
            bool* isPrime = stackalloc bool[n + 1];
            
            int pCount = LinearSieveMultiplicative.Run(f, primes, n, &FPower, e, pk, isPrime);
            Assert.AreEqual(25, pCount);
            Assert.AreEqual(4, f[10]);
        }

        private static long FPower(int p, int k) => p - 1;

        [Test]
        public void Lucas_LargenCrModP_Correct()
        {
            long n = 10, k = 3, p = 7;
            long res = BinomLucas.Run(n, k, p);
            Assert.AreEqual(1, res);
        }

        [Test]
        public void ChineseRemainder_General_Correct()
        {
            long* a = stackalloc long[] { 2, 3, 2 };
            long* m = stackalloc long[] { 3, 5, 7 };
            long res = Excrt.Run(a, m, 3);
            Assert.AreEqual(23, res);
        }

        [Test]
        public void PrimeFactorPowerSum_CorrectResults()
        {
            long n = 12; // 2^2 * 3^1
            long sum = PrimeFactorPowerSum.Run(n, &FPowerSum);
            Assert.AreEqual(7, sum);
        }

        private static long FPowerSum(long p, int e) => (long)Math.Pow(p, e);

        [Test]
        public void Dirichlet_Convolution_Correct()
        {
            int n = 10;
            long* f = stackalloc long[] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            long* g = stackalloc long[] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            long* h = stackalloc long[n + 1];
            DirichletConvolution.Run(n, f, g, h);
            Assert.AreEqual(4, h[6]);
        }

        [Test]
        public void DuJiao_SublinearPrefixSums_CorrectResults()
        {
            long n = 1000;
            long b = (long)Math.Pow(n, 2.0 / 3.0) + 10000;
            long* preSum = (long*)Marshal.AllocHGlobal((int)b * sizeof(long));
            long* memo = (long*)Marshal.AllocHGlobal((int)(n / b + 10) * sizeof(long));
            bool* memoized = (bool*)Marshal.AllocHGlobal((int)(n / b + 10) * sizeof(bool));

            try
            {
                long phiSum = DuJiao.Phi(n, preSum, memo, memoized);
                long mobiusSum = DuJiao.Mobius(n, preSum, memo, memoized);
                Assert.IsTrue(phiSum > 0);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)preSum);
                Marshal.FreeHGlobal((nint)memo);
                Marshal.FreeHGlobal((nint)memoized);
            }
        }
    }
}
