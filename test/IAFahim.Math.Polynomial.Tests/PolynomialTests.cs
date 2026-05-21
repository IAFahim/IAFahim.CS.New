namespace IAFahim.Math.Polynomial.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class PolynomialAddTests
    {
        private const long MOD = 1000000007;

        [Fact]
        public void Empty_NoOp()
        {
            PolynomialAdd.Run(0, null, 0, null, null);
        }

        [Fact]
        public void SingleElement_Adds()
        {
            long a = 5;
            long b = 3;
            long res = 0;
            PolynomialAdd.Run(1, &a, 1, &b, &res);
            Assert.Equal(8, res);
        }

        [Fact]
        public void DifferentLengths_CombinesCorrectly()
        {
            long* a = stackalloc long[3] { 1, 2, 3 };
            long* b = stackalloc long[2] { 4, 5 };
            long* res = stackalloc long[3];
            int len = PolynomialAdd.Run(3, a, 2, b, res);
            Assert.Equal(3, len);
            Assert.Equal(5, res[0]);
            Assert.Equal(7, res[1]);
            Assert.Equal(3, res[2]);
        }

        [Fact]
        public void TwoPolynomials_AddsAllTerms()
        {
            long* a = stackalloc long[4] { 1, 2, 3, 4 };
            long* b = stackalloc long[4] { 5, 6, 7, 8 };
            long* res = stackalloc long[4];
            int len = PolynomialAdd.Run(4, a, 4, b, res);
            Assert.Equal(4, len);
            for (int i = 0; i < 4; i++)
                Assert.Equal(a[i] + b[i], res[i]);
        }
    }

    public sealed unsafe class PolynomialSubTests
    {
        [Fact]
        public void Empty_NoOp()
        {
            PolynomialSub.Run(0, null, 0, null, null);
        }

        [Fact]
        public void SingleElement_Subtracts()
        {
            long a = 10;
            long b = 3;
            long res = 0;
            PolynomialSub.Run(1, &a, 1, &b, &res);
            Assert.Equal(7, res);
        }

        [Fact]
        public void DifferentLengths_SubtractsCorrectly()
        {
            long* a = stackalloc long[3] { 10, 20, 30 };
            long* b = stackalloc long[2] { 4, 5 };
            long* res = stackalloc long[3];
            int len = PolynomialSub.Run(3, a, 2, b, res);
            Assert.Equal(3, len);
            Assert.Equal(6, res[0]);
            Assert.Equal(15, res[1]);
            Assert.Equal(30, res[2]);
        }
    }

    public sealed unsafe class PolynomialMulTests
    {
        [Fact]
        public void SingleElement_Multiplies()
        {
            long a = 3;
            long b = 4;
            long res = 0;
            int len = PolynomialMul.Run(1, &a, 1, &b, &res);
            Assert.Equal(1, len);
            Assert.Equal(12, res);
        }

        [Fact]
        public void TwoLinear_ProducesQuadratic()
        {
            long* a = stackalloc long[2] { 1, 1 };
            long* b = stackalloc long[2] { 1, 1 };
            long* res = stackalloc long[3];
            int len = PolynomialMul.Run(2, a, 2, b, res);
            Assert.Equal(3, len);
            Assert.Equal(1, res[0]);
            Assert.Equal(2, res[1]);
            Assert.Equal(1, res[2]);
        }

        [Fact]
        public void Polynomial_MultiplyCorrectly()
        {
            long* a = stackalloc long[2] { 2, 3 };
            long* b = stackalloc long[2] { 1, 2 };
            long* res = stackalloc long[3];
            int len = PolynomialMul.Run(2, a, 2, b, res);
            Assert.Equal(3, len);
            Assert.Equal(2, res[0]);
            Assert.Equal(7, res[1]);
            Assert.Equal(6, res[2]);
        }
    }

    public sealed unsafe class PolynomialDivTests
    {
        [Fact]
        public void SmallDividendByLarger_ReturnsQuotientZero()
        {
            long* a = stackalloc long[2] { 1, 2 };
            long* b = stackalloc long[3] { 1, 0, 1 };
            long* q = stackalloc long[1];
            long* r = stackalloc long[2];
            int remLen = PolynomialDiv.Run(2, a, 3, b, q, r);
            Assert.Equal(2, remLen);
            Assert.Equal(1, r[0]);
            Assert.Equal(2, r[1]);
        }

        [Fact]
        public void EqualSize_DividesCorrectly()
        {
            long* a = stackalloc long[2] { 6, 5 };
            long* b = stackalloc long[2] { 2, 1 };
            long* q = stackalloc long[1];
            long* r = stackalloc long[2];
            PolynomialDiv.Run(2, a, 2, b, q, r);
            Assert.Equal(5, q[0]);
        }
    }

    public sealed unsafe class PolynomialDerivativeTests
    {
        [Fact]
        public void Constant_DerivativeZero()
        {
            long a = 5;
            long res = 0;
            int len = PolynomialDerivative.Run(1, &a, &res);
            Assert.Equal(1, len);
            Assert.Equal(0, res);
        }

        [Fact]
        public void Linear_DerivativeConstant()
        {
            long* a = stackalloc long[2] { 3, 2 };
            long* res = stackalloc long[2];
            int len = PolynomialDerivative.Run(2, a, res);
            Assert.Equal(1, len);
            Assert.Equal(2, res[0]);
        }

        [Fact]
        public void Quadratic_DerivativeLinear()
        {
            long* a = stackalloc long[3] { 1, 2, 3 };
            long* res = stackalloc long[3];
            int len = PolynomialDerivative.Run(3, a, res);
            Assert.Equal(2, len);
            Assert.Equal(2, res[0]);
            Assert.Equal(6, res[1]);
        }
    }

    public sealed unsafe class PolynomialModTests
    {
        [Fact]
        public void RemainderWithLinearDivisor()
        {
            long* a = stackalloc long[3] { 3, 5, 2 };
            long* b = stackalloc long[2] { 1, 1 };
            long* r = stackalloc long[1];
            int len = PolynomialMod.Run(3, a, 2, b, r);
            Assert.Equal(1, len);
            Assert.Equal(0, r[0]);
        }
    }

    public sealed unsafe class PolynomialIntegralTests
    {
        private const long Mod = 1000000007;

        [Fact]
        public void IntegralThenDerivative_RoundTrips()
        {
            const int N = 4;
            long* a = stackalloc long[N] { 3, 5, 7, 11 };
            long* integral = stackalloc long[N + 1];
            int len = PolynomialIntegral.Run(N, a, integral, Mod);
            long* derivative = stackalloc long[N + 1];
            int dLen = PolynomialDerivative.Run(len, integral, derivative);
            Assert.Equal(N, dLen);
            for (int i = 0; i < N; i++)
                Assert.Equal(a[i], derivative[i] % Mod);
        }
    }

    public sealed unsafe class PolynomialInverseTests
    {
        private const long Mod = 1000000007;

        [Fact]
        public void Inverse_MultipliesToOne()
        {
            const int N = 4;
            long* a = stackalloc long[N] { 2, 3, 1, 0 };
            long* inv = stackalloc long[N];
            int len = PolynomialInverse.Run(N, a, inv, Mod);
            long* prod = stackalloc long[2 * N];
            int prodLen = PolynomialMul.Run(N, a, N, inv, prod);
            Assert.Equal(N, len);
            Assert.Equal(1, ModNorm(prod[0]));
            for (int i = 1; i < N; i++)
                Assert.Equal(0, ModNorm(prod[i]));
        }

        private static long ModNorm(long value)
        {
            long r = value % Mod;
            if (r < 0) r += Mod;
            return r;
        }
    }

    public sealed unsafe class PolynomialLogExpTests
    {
        private const long Mod = 1000000007;

        [Fact]
        public void ExpThenLog_RoundTrips()
        {
            const int N = 4;
            long* a = stackalloc long[N] { 0, 1, 2, 3 };
            long* exp = stackalloc long[N];
            int expLen = PolynomialExp.Run(N, a, exp, Mod);
            long* log = stackalloc long[N];
            int logLen = PolynomialLog.Run(N, exp, log, Mod);
            Assert.Equal(N, expLen);
            Assert.Equal(N, logLen);
            for (int i = 0; i < N; i++)
                Assert.Equal(ModNorm(a[i]), ModNorm(log[i]));
        }

        private static long ModNorm(long value)
        {
            long r = value % Mod;
            if (r < 0) r += Mod;
            return r;
        }
    }

    public sealed unsafe class PolynomialPowTests
    {
        private const long Mod = 1000000007;

        [Fact]
        public void SquareMatchesExpected()
        {
            const int N = 5;
            long* a = stackalloc long[N] { 1, 2, 1, 0, 0 };
            long* res = stackalloc long[N];
            int len = PolynomialPow.Run(N, a, 2, res, Mod);
            Assert.Equal(5, len);
            Assert.Equal(1, ModNorm(res[0]));
            Assert.Equal(4, ModNorm(res[1]));
            Assert.Equal(6, ModNorm(res[2]));
            Assert.Equal(4, ModNorm(res[3]));
            Assert.Equal(1, ModNorm(res[4]));
        }

        private static long ModNorm(long value)
        {
            long r = value % Mod;
            if (r < 0) r += Mod;
            return r;
        }
    }

    public sealed unsafe class PolynomialSqrtTests
    {
        private const long Mod = 1000000007;

        [Fact]
        public void SqrtSquaredMatchesOriginal()
        {
            const int N = 4;
            long* basePoly = stackalloc long[N] { 1, 2, 0, 0 };
            long* square = stackalloc long[2 * N];
            int squareLen = PolynomialMul.Run(N, basePoly, N, basePoly, square);
            long* a = stackalloc long[N];
            for (int i = 0; i < N; i++) a[i] = square[i];
            long* res = stackalloc long[N];
            int len = PolynomialSqrt.Run(N, a, res, Mod);
            Assert.Equal(N, len);
            for (int i = 0; i < N; i++)
                Assert.Equal(ModNorm(basePoly[i]), ModNorm(res[i]));
        }

        private static long ModNorm(long value)
        {
            long r = value % Mod;
            if (r < 0) r += Mod;
            return r;
        }
    }

    public sealed unsafe class PolynomialEvalTests
    {
        private const long Mod = 1000000007;

        [Fact]
        public void EvaluatesCorrectly()
        {
            const int N = 4;
            long* a = stackalloc long[N] { 1, 2, 3, 4 };
            long x = 3;
            long expected = 0;
            long cur = 1;
            for (int i = 0; i < N; i++)
            {
                expected = (expected + a[i] * cur) % Mod;
                cur = (cur * x) % Mod;
            }
            long actual = PolynomialEval.Run(N, a, x, Mod);
            Assert.Equal(expected, actual);
        }
    }

    public sealed unsafe class PolynomialInterpolateTests
    {
        private const long Mod = 1000000007;

        [Fact]
        public void InterpolatesLinearPolynomial()
        {
            const int N = 2;
            long* x = stackalloc long[N] { 0, 1 };
            long* y = stackalloc long[N] { 1, 3 };
            long* res = stackalloc long[N];
            for (int i = 0; i < N; i++) res[i] = 0;
            int len = PolynomialInterpolate.Run(N, x, y, res, Mod);
            Assert.Equal(N, len);
            Assert.Equal(1, ModNorm(res[0]));
            Assert.Equal(2, ModNorm(res[1]));
        }

        private static long ModNorm(long value)
        {
            long r = value % Mod;
            if (r < 0) r += Mod;
            return r;
        }
    }

    public sealed unsafe class LagrangeInterpolateTests
    {
        private const long Mod = 1000000007;

        [Fact]
        public void EvaluatesAtPoint()
        {
            const int N = 2;
            long* x = stackalloc long[N] { 0, 1 };
            long* y = stackalloc long[N] { 1, 3 };
            long value = LagrangeInterpolate.Run(x, y, N, 2, Mod);
            Assert.Equal(5, value);
        }
    }

    public sealed unsafe class ConvolutionPrimitiveTests
    {
        [Fact]
        public void KaratsubaMultiply_Basic()
        {
            long* a = stackalloc long[3] { 1, 2, 3 };
            long* b = stackalloc long[3] { 4, 5, 6 };
            long* res = stackalloc long[5];
            long* scratch = stackalloc long[60];
            int len = KaratsubaMultiply.Run(3, a, 3, b, res, scratch);
            Assert.Equal(5, len);
            Assert.Equal(4, res[0]);
            Assert.Equal(13, res[1]);
            Assert.Equal(28, res[2]);
            Assert.Equal(27, res[3]);
            Assert.Equal(18, res[4]);
        }

        [Fact]
        public void PolynomialShift_Basic()
        {
            long* a = stackalloc long[5] { 1, 2, 3, 4, 5 };
            PolynomialShift.RunLeft(a, 5, 2);
            Assert.Equal(3, a[0]);
            Assert.Equal(5, a[2]);
            Assert.Equal(0, a[3]);

            long* b = stackalloc long[5] { 1, 2, 3, 0, 0 };
            PolynomialShift.RunRight(b, 5, 2);
            Assert.Equal(0, b[0]);
            Assert.Equal(0, b[1]);
            Assert.Equal(1, b[2]);
            Assert.Equal(3, b[4]);
        }

        [Fact]
        public void PolynomialComposition_Basic()
        {
            long* f = stackalloc long[3] { 1, 2, 1 }; // 1 + 2x + x^2
            long* g = stackalloc long[2] { 0, 1 };    // x
            long* res = stackalloc long[3];
            long* tmp = stackalloc long[20];
            PolynomialComposition.Run(3, f, 2, g, res, 1000000007, tmp);
            Assert.Equal(1, res[0]);
            Assert.Equal(2, res[1]);
            Assert.Equal(1, res[2]);
        }

        [Fact]
        public void PolynomialComposition_Naive_Basic()
        {
            long* f = stackalloc long[3] { 1, 2, 1 };
            long* g = stackalloc long[2] { 0, 1 };
            long* res = stackalloc long[3];
            PolynomialComposition.RunNaive(3, f, 2, g, res, 1000000007);
            Assert.Equal(1, res[0]);
            Assert.Equal(2, res[1]);
            Assert.Equal(1, res[2]);
        }
    }
}
