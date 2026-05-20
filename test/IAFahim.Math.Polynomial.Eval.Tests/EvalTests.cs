namespace IAFahim.Math.Polynomial.Eval.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class MultiPointEvalTests
    {
        private const long Mod = 1000000007;
        private const long ModAlt = 998244353;

        [Fact]
        public void SinglePoint_EvaluatesCorrectly()
        {
            long* poly = stackalloc long[2] { 1, 2 };
            long* x = stackalloc long[1] { 3 };
            long* res = stackalloc long[1];
            MultiPointEval.Run(2, poly, 1, x, res, Mod);
            Assert.Equal((1 + 2 * 3) % Mod, res[0]);
        }

        [Fact]
        public void MultiplePoints_EvaluatesAll()
        {
            long* poly = stackalloc long[3] { 1, 2, 3 };
            long* x = stackalloc long[3] { 1, 2, 3 };
            long* res = stackalloc long[3];
            MultiPointEval.Run(3, poly, 3, x, res, Mod);
            Assert.Equal((1 + 2 * 1 + 3 * 1 * 1) % Mod, res[0]);
            Assert.Equal((1 + 2 * 2 + 3 * 2 * 2) % Mod, res[1]);
            Assert.Equal((1 + 2 * 3 + 3 * 3 * 3) % Mod, res[2]);
        }

        [Fact]
        public void ZeroEvaluation_ReturnsConstant()
        {
            long* poly = stackalloc long[3] { 5, 2, 3 };
            long* x = stackalloc long[1] { 0 };
            long* res = stackalloc long[1];
            MultiPointEval.Run(3, poly, 1, x, res, Mod);
            Assert.Equal(5, res[0]);
        }

        [Fact]
        public void AlternateMod_EvaluatesCorrectly()
        {
            const int N = 4;
            long* poly = stackalloc long[N] { 5, 7, 11, 13 };
            long* x = stackalloc long[N] { 0, 1, 2, 3 };
            long* res = stackalloc long[N];
            MultiPointEval.Run(N, poly, N, x, res, ModAlt);
            for (int i = 0; i < N; i++)
                Assert.Equal(Evaluate(poly, N, x[i], ModAlt), res[i]);
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

        [Fact]
        public void UnityParameters_MatchesConvolutionWithOnes()
        {
            const int N = 3;
            long* a = stackalloc long[N] { 1, 2, 3 };
            long* res = stackalloc long[2 * N];
            for (int i = 0; i < 2 * N; i++) res[i] = 0;
            int len = ChirpZTransform.Run(N, a, 1, 1, res, Mod);
            Assert.Equal(2 * N - 1, len);
            Assert.Equal(1, res[0]);
            Assert.Equal(3, res[1]);
            Assert.Equal(6, res[2]);
            Assert.Equal(5, res[3]);
            Assert.Equal(3, res[4]);
        }
    }
}
