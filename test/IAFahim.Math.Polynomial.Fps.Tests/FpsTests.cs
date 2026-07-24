namespace IAFahim.Math.Polynomial.Fps.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class FormalPowerSeriesInverseTests
    {
        private const long Mod = 1000000007;

        [Test]
        public void Inverse_MultipliesToOne()
        {
            const int N = 4;
            long* a = stackalloc long[N] { 2, 3, 1, 0 };
            long* inv = stackalloc long[N];
            int len = FormalPowerSeriesInverse.Run(N, a, inv, Mod);
            long* prod = stackalloc long[2 * N];
            FpsTestHelper.Mul(N, a, N, inv, prod, Mod);
            Assert.AreEqual(N, len);
            Assert.AreEqual(1, FpsTestHelper.ModNorm(prod[0], Mod));
            for (int i = 1; i < N; i++)
                Assert.AreEqual(0, FpsTestHelper.ModNorm(prod[i], Mod));
        }
    }

    public sealed unsafe class FormalPowerSeriesLogExpTests
    {
        private const long Mod = 1000000007;

        [Test]
        public void ExpThenLog_RoundTrips()
        {
            const int N = 4;
            long* a = stackalloc long[N] { 0, 1, 2, 3 };
            long* exp = stackalloc long[N];
            FormalPowerSeriesExp.Run(N, a, exp, Mod);
            long* log = stackalloc long[N];
            FormalPowerSeriesLog.Run(N, exp, log, Mod);
            for (int i = 0; i < N; i++)
                Assert.AreEqual(FpsTestHelper.ModNorm(a[i], Mod), FpsTestHelper.ModNorm(log[i], Mod));
        }
    }

    public sealed unsafe class FormalPowerSeriesPowTests
    {
        private const long Mod = 1000000007;

        [Test]
        public void PowerMatchesNaive()
        {
            const int N = 5;
            long* a = stackalloc long[N] { 1, 1, 1, 0, 0 };
            long* actual = stackalloc long[N];
            FormalPowerSeriesPow.Run(N, a, 3, actual, Mod);
            long* expected = stackalloc long[N];
            expected[0] = 1;
            for (int i = 1; i < N; i++) expected[i] = 0;
            long* tmp = stackalloc long[2 * N];
            for (int step = 0; step < 3; step++)
            {
                FpsTestHelper.Mul(N, expected, N, a, tmp, Mod);
                for (int i = 0; i < N; i++) expected[i] = FpsTestHelper.ModNorm(tmp[i], Mod);
            }
            for (int i = 0; i < N; i++)
                Assert.AreEqual(FpsTestHelper.ModNorm(expected[i], Mod), FpsTestHelper.ModNorm(actual[i], Mod));
        }
    }

    public sealed unsafe class FormalPowerSeriesSqrtTests
    {
        private const long Mod = 1000000007;

        [Test]
        public void SqrtSquaredMatchesOriginal()
        {
            const int N = 4;
            long* basePoly = stackalloc long[N] { 1, 2, 3, 4 };
            long* square = stackalloc long[2 * N];
            FpsTestHelper.Mul(N, basePoly, N, basePoly, square, Mod);
            long* a = stackalloc long[N];
            for (int i = 0; i < N; i++) a[i] = square[i];
            long* res = stackalloc long[N];
            int len = FormalPowerSeriesSqrt.Run(N, a, res, Mod);
            Assert.AreEqual(N, len);
            for (int i = 0; i < N; i++)
                Assert.AreEqual(FpsTestHelper.ModNorm(basePoly[i], Mod), FpsTestHelper.ModNorm(res[i], Mod));
        }

        [Test]
        public void Sqrt_ZeroSeries_ReturnsZero()
        {
            const int N = 4;
            long* a = stackalloc long[N];
            long* res = stackalloc long[N];
            for (int i = 0; i < N; i++) a[i] = 0;
            int len = FormalPowerSeriesSqrt.Run(N, a, res, Mod);
            Assert.AreEqual(N, len);
            for (int i = 0; i < N; i++) Assert.AreEqual(0, res[i]);
        }

        [Test]
        public void Sqrt_LeadingZeroNonzeroTail_Fails()
        {
            long* a = stackalloc long[] { 0, 1, 0, 0 };
            long* res = stackalloc long[4];
            Assert.AreEqual(-1, FormalPowerSeriesSqrt.Run(4, a, res, Mod));
        }
    }

    internal static unsafe class FpsTestHelper
    {
        internal static void Mul(int n, long* a, int m, long* b, long* res, long mod)
        {
            int len = n + m - 1;
            for (int i = 0; i < len; i++) res[i] = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    res[i + j] = (res[i + j] + a[i] * b[j]) % mod;
        }

        internal static long ModNorm(long value, long mod)
        {
            long r = value % mod;
            if (r < 0) r += mod;
            return r;
        }
    }
}
