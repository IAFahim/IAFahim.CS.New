namespace IAFahim.Math.NT.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.Math.NT;
    using Xunit;

    public sealed unsafe class Phase10NTTests
    {
        [Fact]
        public void AllFactorizations_ValidInput_GeneratesCorrectly()
        {
            long* outBuffer = (long*)Marshal.AllocHGlobal(1000 * sizeof(long));
            try
            {
                int count = AllFactorizations.Run(12, outBuffer, out int outOffset);
                Assert.Equal(4, count);
                // 12 can be factored as:
                // 1. [12]
                // 2. [2, 6]
                // 3. [2, 2, 3]
                // 4. [3, 4]
            }
            finally
            {
                Marshal.FreeHGlobal((nint)outBuffer);
            }
        }

        [Fact]
        public void HighlyCompositeNumbers_UnderLimit_CorrectSequence()
        {
            long* res = (long*)Marshal.AllocHGlobal(100 * sizeof(long));
            try
            {
                HighlyCompositeCandidate* scratch = stackalloc HighlyCompositeCandidate[20000];
                int count = HighlyCompositeNumbers.Run(100, res, scratch);
                Assert.True(count >= 5);
                Assert.Equal(1, res[0]);
                Assert.Equal(2, res[1]);
                Assert.Equal(4, res[2]);
                Assert.Equal(6, res[3]);
                Assert.Equal(12, res[4]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)res);
            }
        }

        [Fact]
        public void AntiprimeGenerate_ValidK_MatchesHCN()
        {
            long* res = (long*)Marshal.AllocHGlobal(100 * sizeof(long));
            try
            {
                HighlyCompositeCandidate* scratch = stackalloc HighlyCompositeCandidate[20000];
                AntiprimeGenerate.Run(5, res, scratch);
                Assert.Equal(1, res[0]);
                Assert.Equal(2, res[1]);
                Assert.Equal(4, res[2]);
                Assert.Equal(6, res[3]);
                Assert.Equal(12, res[4]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)res);
            }
        }

        [Fact]
        public void SmoothNumbers_UnderLimit_GeneratesCorrectly()
        {
            long* res = (long*)Marshal.AllocHGlobal(100 * sizeof(long));
            try
            {
                int* primes = stackalloc int[6];
                int count = SmoothNumbers.Generate(5, 10, res, primes);
                Assert.Equal(9, count);
                Assert.Equal(1, res[0]);
                Assert.Equal(2, res[1]);
                Assert.Equal(3, res[2]);
                Assert.Equal(4, res[3]);
                Assert.Equal(5, res[4]);
                Assert.Equal(6, res[5]);
                Assert.Equal(8, res[6]);
                Assert.Equal(9, res[7]);
                Assert.Equal(10, res[8]);

                int* primesDirect = stackalloc int[6];
                long countDirect = SmoothNumbers.Count(5, 10, primesDirect);
                Assert.Equal(9, countDirect);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)res);
            }
        }

        [Fact]
        public void PowerfulNumbers_UnderLimit_GeneratesCorrectly()
        {
            long* res = (long*)Marshal.AllocHGlobal(100 * sizeof(long));
            try
            {
                int count = PowerfulNumbers.Generate(50, res);
                Assert.Equal(10, count);
                Assert.Equal(1, res[0]);
                Assert.Equal(4, res[1]);
                Assert.Equal(8, res[2]);
                Assert.Equal(9, res[3]);
                Assert.Equal(16, res[4]);
                Assert.Equal(25, res[5]);
                Assert.Equal(27, res[6]);
                Assert.Equal(32, res[7]);
                Assert.Equal(36, res[8]);
                Assert.Equal(49, res[9]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)res);
            }
        }

        [Fact]
        public void SquareFree_Functions_CorrectResults()
        {
            Assert.Equal(6, SquareFree.Kernel(12));
            Assert.Equal(6, SquareFree.Kernel(18));
            int* mu = stackalloc int[4];
            Assert.Equal(7, SquareFree.Count(10, mu)); // 1, 2, 3, 5, 6, 7, 10

            int* prefix = (int*)Marshal.AllocHGlobal(11 * sizeof(int));
            try
            {
                SquareFree.Prefix(10, prefix);
                Assert.Equal(7, prefix[10]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)prefix);
            }
        }

        [Fact]
        public void MoebiusPrefix_ValidInput_CorrectSums()
        {
            int* prefix = (int*)Marshal.AllocHGlobal(7 * sizeof(int));
            int* mu = (int*)Marshal.AllocHGlobal(7 * sizeof(int));
            int* primes = (int*)Marshal.AllocHGlobal(7 * sizeof(int));
            bool* isPrime = (bool*)Marshal.AllocHGlobal(7 * sizeof(bool));
            try
            {
                MoebiusPrefix.Run(6, prefix, mu, primes, isPrime);
                Assert.Equal(0, prefix[0]);
                Assert.Equal(1, prefix[1]);
                Assert.Equal(0, prefix[2]);
                Assert.Equal(-1, prefix[3]);
                Assert.Equal(-1, prefix[4]);
                Assert.Equal(-2, prefix[5]);
                Assert.Equal(-1, prefix[6]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)prefix);
                Marshal.FreeHGlobal((nint)mu);
                Marshal.FreeHGlobal((nint)primes);
                Marshal.FreeHGlobal((nint)isPrime);
            }
        }

        [Fact]
        public void TotientPrefix_ValidInput_CorrectSums()
        {
            long* prefix = (long*)Marshal.AllocHGlobal(7 * sizeof(long));
            int* phi = (int*)Marshal.AllocHGlobal(7 * sizeof(int));
            int* primes = (int*)Marshal.AllocHGlobal(7 * sizeof(int));
            bool* isPrime = (bool*)Marshal.AllocHGlobal(7 * sizeof(bool));
            try
            {
                TotientPrefix.Run(6, prefix, phi, primes, isPrime);
                Assert.Equal(0, prefix[0]);
                Assert.Equal(1, prefix[1]);
                Assert.Equal(2, prefix[2]);
                Assert.Equal(4, prefix[3]);
                Assert.Equal(6, prefix[4]);
                Assert.Equal(10, prefix[5]);
                Assert.Equal(12, prefix[6]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)prefix);
                Marshal.FreeHGlobal((nint)phi);
                Marshal.FreeHGlobal((nint)primes);
                Marshal.FreeHGlobal((nint)isPrime);
            }
        }

        [Fact]
        public void DirichletPrefixSum_ConvolutionAndHyperbola_CorrectSums()
        {
            int n = 10;
            long* f = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            long* g = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            long* result = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            long* prefixF = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            long* prefixG = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));

            try
            {
                f[0] = 0; g[0] = 0;
                prefixF[0] = 0; prefixG[0] = 0;
                for (int i = 1; i <= n; i++)
                {
                    f[i] = 1;
                    g[i] = 1;
                    prefixF[i] = prefixF[i - 1] + f[i];
                    prefixG[i] = prefixG[i - 1] + g[i];
                }

                DirichletPrefixSum.ConvolutionPrefixSum(n, f, g, result);
                long hyp = DirichletPrefixSum.Hyperbola(n, prefixF, prefixG);

                // f*g = d(n) (divisor count). Sum of d(i) for 1..10:
                // 1:1, 2:2, 3:2, 4:3, 5:2, 6:4, 7:2, 8:4, 9:3, 10:4 -> Total = 27
                Assert.Equal(27, result[n]);
                Assert.Equal(27, hyp);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)f);
                Marshal.FreeHGlobal((nint)g);
                Marshal.FreeHGlobal((nint)result);
                Marshal.FreeHGlobal((nint)prefixF);
                Marshal.FreeHGlobal((nint)prefixG);
            }
        }

        private static long TestPower(int p, int k)
        {
            long ans = 1;
            for (int i = 0; i < k; i++)
            {
                ans *= p;
            }
            return ans;
        }

        [Fact]
        public void LinearSieveMultiplicative_IdentityFunction_CorrectValues()
        {
            int n = 20;
            long* f = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            int* primes = (int*)Marshal.AllocHGlobal((n + 1) * sizeof(int));
            int* e = (int*)Marshal.AllocHGlobal((n + 1) * sizeof(int));
            long* pk = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            bool* isPrime = (bool*)Marshal.AllocHGlobal((n + 1) * sizeof(bool));
            try
            {
                delegate* managed<int, int, long> fPower = &TestPower;
                LinearSieveMultiplicative.Run(f, primes, n, out int primeCount, fPower, e, pk, isPrime);
                for (int i = 1; i <= n; i++)
                {
                    Assert.Equal(i, f[i]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)f);
                Marshal.FreeHGlobal((nint)primes);
                Marshal.FreeHGlobal((nint)e);
                Marshal.FreeHGlobal((nint)pk);
                Marshal.FreeHGlobal((nint)isPrime);
            }
        }

        [Fact]
        public void Min25Sieve_BasicAlgorithms_CorrectResults()
        {
            const int N = 100;
            int v = 10;
            int* primes = stackalloc int[v + 1];
            bool* isPrime = stackalloc bool[v + 1];
            long* w = stackalloc long[2 * v + 2];
            long* g = stackalloc long[2 * v + 2];
            int* map1 = stackalloc int[v + 1];
            int* map2 = stackalloc int[v + 1];
            Assert.Equal(25, Min25Sieve.PrimePi(N, primes, isPrime, w, g, map1, map2));

            int vSum = 3;
            int* primesSum = stackalloc int[vSum + 1];
            bool* isPrimeSum = stackalloc bool[vSum + 1];
            long* wSum = stackalloc long[2 * vSum + 2];
            long* gSum = stackalloc long[2 * vSum + 2];
            int* map1Sum = stackalloc int[vSum + 1];
            int* map2Sum = stackalloc int[vSum + 1];
            Assert.Equal(17, Min25Sieve.PrimeSum(10, 1000000007, primesSum, isPrimeSum, wSum, gSum, map1Sum, map2Sum));

            delegate* managed<long, int, long> fPower = &TestPowerMin25;
            long mod = 1000000007;

            int vMult = 3;
            int* primesMult = stackalloc int[vMult + 1];
            bool* isPrimeMult = stackalloc bool[vMult + 1];
            long* wMult = stackalloc long[2 * vMult + 2];
            long* gMult = stackalloc long[2 * vMult + 2];
            long* g0Mult = stackalloc long[2 * vMult + 2];
            long* g1Mult = stackalloc long[2 * vMult + 2];
            int* map1Mult = stackalloc int[vMult + 1];
            int* map2Mult = stackalloc int[vMult + 1];
            long* gPrimeSumMult = stackalloc long[vMult + 2];
            long sum = Min25Sieve.MultiplicativeSum(10, 0, 1, fPower, mod, primesMult, isPrimeMult, wMult, gMult, g0Mult, g1Mult, map1Mult, map2Mult, gPrimeSumMult); // Sum of identity function up to 10
            Assert.Equal(55, sum);
        }

        private static long TestPowerMin25(long p, int k)
        {
            long ans = 1;
            for (int i = 0; i < k; i++)
            {
                ans *= p;
            }
            return ans;
        }

        [Fact]
        public void DuJiao_SublinearPrefixSums_CorrectResults()
        {
            // Compare with linear sieve values
            long n = 1000;
            long phiSum = DuJiao.Phi(n);
            long mobiusSum = DuJiao.Mobius(n);

            long* prefixPhi = (long*)Marshal.AllocHGlobal((int)(n + 1) * sizeof(long));
            int* prefixMu = (int*)Marshal.AllocHGlobal((int)(n + 1) * sizeof(int));
            int* phi = (int*)Marshal.AllocHGlobal((int)(n + 1) * sizeof(int));
            int* mu = (int*)Marshal.AllocHGlobal((int)(n + 1) * sizeof(int));
            int* primes = (int*)Marshal.AllocHGlobal((int)(n + 1) * sizeof(int));
            bool* isPrime = (bool*)Marshal.AllocHGlobal((int)(n + 1) * sizeof(bool));

            try
            {
                TotientPrefix.Run((int)n, prefixPhi, phi, primes, isPrime);
                MoebiusPrefix.Run((int)n, prefixMu, mu, primes, isPrime);

                Assert.Equal(prefixPhi[n], phiSum);
                Assert.Equal(prefixMu[n], mobiusSum);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)prefixPhi);
                Marshal.FreeHGlobal((nint)prefixMu);
                Marshal.FreeHGlobal((nint)phi);
                Marshal.FreeHGlobal((nint)mu);
                Marshal.FreeHGlobal((nint)primes);
                Marshal.FreeHGlobal((nint)isPrime);
            }
        }
    }
}
