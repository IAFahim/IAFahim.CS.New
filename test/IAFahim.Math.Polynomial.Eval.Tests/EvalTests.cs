namespace IAFahim.Math.Polynomial.Eval.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class MultiPointEvalTests
    {
        private const long Mod = 1000000007;
        private const long ModAlt = 998244353;

        [Test]
        public void SinglePoint_EvaluatesCorrectly()
        {
            long* poly = stackalloc long[2] { 1, 2 };
            long* x = stackalloc long[1] { 3 };
            long* res = stackalloc long[1];
            MultiPointEval.Run(2, poly, 1, x, res, Mod);
            Assert.AreEqual((1 + 2 * 3) % Mod, res[0]);
        }

        [Test]
        public void MultiplePoints_EvaluatesAll()
        {
            long* poly = stackalloc long[3] { 1, 2, 3 };
            long* x = stackalloc long[3] { 1, 2, 3 };
            long* res = stackalloc long[3];
            MultiPointEval.Run(3, poly, 3, x, res, Mod);
            Assert.AreEqual((1 + 2 * 1 + 3 * 1 * 1) % Mod, res[0]);
            Assert.AreEqual((1 + 2 * 2 + 3 * 2 * 2) % Mod, res[1]);
            Assert.AreEqual((1 + 2 * 3 + 3 * 3 * 3) % Mod, res[2]);
        }

        [Test]
        public void ZeroEvaluation_ReturnsConstant()
        {
            long* poly = stackalloc long[3] { 5, 2, 3 };
            long* x = stackalloc long[1] { 0 };
            long* res = stackalloc long[1];
            MultiPointEval.Run(3, poly, 1, x, res, Mod);
            Assert.AreEqual(5, res[0]);
        }

        [Test]
        public void AlternateMod_EvaluatesCorrectly()
        {
            const int N = 4;
            long* poly = stackalloc long[N] { 5, 7, 11, 13 };
            long* x = stackalloc long[N] { 0, 1, 2, 3 };
            long* res = stackalloc long[N];
            MultiPointEval.Run(N, poly, N, x, res, ModAlt);
            for (int i = 0; i < N; i++)
                Assert.AreEqual(Evaluate(poly, N, x[i], ModAlt), res[i]);
        }

        private static long Evaluate(long* poly, int n, long x, long mod)
        {
            long res = 0;
            long cur = 1;
            for (int i = 0; i < n; i++)
            {
                res = (res + poly[i] * cur) % mod;
                cur = (cur * x) % mod;
            }
            return res;
        }
    }

    public sealed unsafe class ChirpZTransformTests
    {
        private const long Mod = 1000000007;

        [Test]
        public void UnityParameters_AllEvalsEqualSum()
        {
            const int N = 3;
            long* a = stackalloc long[N] { 1, 2, 3 };
            long* res = stackalloc long[N];
            for (int i = 0; i < N; i++) res[i] = 0;
            int len = ChirpZTransform.Run(N, a, 1, 1, res, Mod);
            Assert.AreEqual(N, len);
            for (int i = 0; i < N; i++)
                Assert.AreEqual(6, res[i]);
        }

        [Test]
        public void LinearChirp_ComputesCorrectly()
        {
            const int N = 4;
            long c = 2;
            long d = 1;
            long* a = stackalloc long[N];
            for (int i = 0; i < N; i++) a[i] = 1;
            long* res = stackalloc long[N];
            for (int i = 0; i < N; i++) res[i] = 0;
            int len = ChirpZTransform.Run(N, a, c, d, res, Mod);
            Assert.AreEqual(N, len);
            for (int i = 0; i < N; i++)
                Assert.IsTrue(res[i] >= 0);
        }
    }
}
