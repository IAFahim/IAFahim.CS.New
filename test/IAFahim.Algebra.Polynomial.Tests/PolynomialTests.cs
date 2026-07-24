namespace IAFahim.Algebra.Polynomial.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class BerlekampMasseyTests
    {
        [Test]
        public void Empty_ReturnsConstantOne()
        {
            long* c = stackalloc long[2];
            int len = BerlekampMassey.Run(null, 0, 998244353, c);
            Assert.AreEqual(1, len);
            Assert.AreEqual(1L, c[0]);
        }

        [Test]
        public void FibonacciSequence_RecoversDegreeTwo()
        {
            const int MOD = 998244353;
            const int N = 16;
            long* s = stackalloc long[N];
            long* c = stackalloc long[N + 1];
            s[0] = 0;
            s[1] = 1;
            for (int i = 2; i < N; i++)
                s[i] = (s[i - 1] + s[i - 2]) % MOD;

            int len = BerlekampMassey.Run(s, N, MOD, c);
            Assert.AreEqual(3, len);
            Assert.AreEqual(1L, c[0]);
            long c1 = c[1] % MOD;
            if (c1 < 0) c1 += MOD;
            long c2 = c[2] % MOD;
            if (c2 < 0) c2 += MOD;
            Assert.AreEqual((MOD - 1L) % MOD, c1);
            Assert.AreEqual((MOD - 1L) % MOD, c2);
        }
    }

    public sealed unsafe class BostanMoriTests
    {
        [Test]
        public void GeometricSeries_OneOverOneMinusX()
        {
            const int MOD = 998244353;
            long* p = stackalloc long[1];
            long* q = stackalloc long[2];
            p[0] = 1;
            q[0] = 1;
            q[1] = MOD - 1;
            for (long k = 0; k < 20; k++)
            {
                q[0] = 1;
                q[1] = MOD - 1;
                p[0] = 1;
                long got = BostanMori.Run(p, 1, q, 2, k, MOD);
                Assert.AreEqual(1L, got);
            }
        }

        [Test]
        public void Linear_ClosedForm()
        {
            const int MOD = 998244353;
            long* p = stackalloc long[1];
            long* q = stackalloc long[2];
            p[0] = 1;
            q[0] = 1;
            q[1] = MOD - 2;
            q[0] = 1;
            q[1] = MOD - 2;
            p[0] = 1;
            long got = BostanMori.Run(p, 1, q, 2, 5, MOD);
            long expect = 1;
            for (int i = 0; i < 5; i++) expect = expect * 2 % MOD;
            Assert.AreEqual(expect, got);
        }
    }

    public sealed unsafe class GcdTests
    {
        [Test]
        public void Gcd_X2MinusOne_And_XMinusOne()
        {
            const int MOD = 998244353;
            long* a = stackalloc long[3];
            long* b = stackalloc long[2];
            long* g = stackalloc long[3];
            a[0] = MOD - 1;
            a[1] = 0;
            a[2] = 1;
            b[0] = MOD - 1;
            b[1] = 1;
            int lenG;
            Gcd.Run(a, 3, b, 2, g, out lenG, MOD);
            Assert.AreEqual(2, lenG);
            Assert.AreEqual(MOD - 1L, g[0] % MOD == 0 ? 0 : (g[0] % MOD + MOD) % MOD);
            long g0 = ((g[0] % MOD) + MOD) % MOD;
            long g1 = ((g[1] % MOD) + MOD) % MOD;
            Assert.AreEqual(MOD - 1L, g0);
            Assert.AreEqual(1L, g1);
        }

        [Test]
        public void Gcd_SwappedOrder_Same()
        {
            const int MOD = 998244353;
            long* a = stackalloc long[2];
            long* b = stackalloc long[3];
            long* g = stackalloc long[3];
            a[0] = MOD - 1;
            a[1] = 1;
            b[0] = MOD - 1;
            b[1] = 0;
            b[2] = 1;
            int lenG;
            Gcd.Run(a, 2, b, 3, g, out lenG, MOD);
            Assert.AreEqual(2, lenG);
            long g0 = ((g[0] % MOD) + MOD) % MOD;
            long g1 = ((g[1] % MOD) + MOD) % MOD;
            Assert.AreEqual(MOD - 1L, g0);
            Assert.AreEqual(1L, g1);
        }
    }

    public sealed unsafe class ToomCookTests
    {
        [Test]
        public void Multiply_OnePlusX_Squared()
        {
            const int MOD = 998244353;
            long* a = stackalloc long[2];
            long* b = stackalloc long[2];
            long* r = stackalloc long[4];
            a[0] = 1; a[1] = 1;
            b[0] = 1; b[1] = 1;
            ToomCook.Multiply(a, 2, b, 2, r, MOD);
            Assert.AreEqual(1L, r[0]);
            Assert.AreEqual(2L, r[1]);
            Assert.AreEqual(1L, r[2]);
        }

        [Test]
        public void Multiply_NegativeCoeff_Normalized()
        {
            const int MOD = 17;
            long* a = stackalloc long[1];
            long* b = stackalloc long[1];
            long* r = stackalloc long[1];
            a[0] = -3;
            b[0] = 5;
            ToomCook.Multiply(a, 1, b, 1, r, MOD);
            Assert.IsTrue(r[0] >= 0 && r[0] < MOD);
            Assert.AreEqual(((-3L * 5) % MOD + MOD) % MOD, r[0]);
        }
    }
}
