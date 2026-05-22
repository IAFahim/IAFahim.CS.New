namespace IAFahim.Math.Transform.Fft.Tests
{
    using System;
    using Xunit;

    public sealed unsafe class FftTransformTests
    {
        private const double Epsilon = 1e-6;

        [Fact]
        public void RoundTrip_Length8()
        {
            const int N = 8;
            double* re = stackalloc double[N] { 0.5, -1.25, 3.75, 2.0, -0.5, 1.5, 0.0, -2.25 };
            double* im = stackalloc double[N] { 0, 0, 0, 0, 0, 0, 0, 0 };
            double* origRe = stackalloc double[N];
            for (int i = 0; i < N; i++) origRe[i] = re[i];
            FftTransform.Forward(re, im, N);
            FftTransform.Inverse(re, im, N);
            for (int i = 0; i < N; i++)
                Assert.True(Math.Abs(re[i] - origRe[i]) < Epsilon, $"Index {i}: {re[i]} vs {origRe[i]}");
        }
    }

    public sealed unsafe class FftConvolutionTests
    {
        private const double Epsilon = 1e-6;

        [Fact]
        public void ConvolutionMatchesNaive()
        {
            const int N = 4;
            const int M = 3;
            double* a = stackalloc double[N] { 1.0, -2.0, 3.0, -4.0 };
            double* b = stackalloc double[M] { 2.0, 0.5, -1.5 };
            double* actual = stackalloc double[N + M - 1];
            double* expected = stackalloc double[N + M - 1];
            for (int i = 0; i < N + M - 1; i++) expected[i] = 0;
            FftTestHelper.NaiveConvolution(a, N, b, M, expected);
            int len = FftConvolution.Run(a, N, b, M, actual);
            Assert.Equal(N + M - 1, len);
            for (int i = 0; i < len; i++)
                Assert.True(Math.Abs(actual[i] - expected[i]) < Epsilon, $"Index {i}: {actual[i]} vs {expected[i]}");
        }
    }

    internal static unsafe class FftTestHelper
    {
        internal static void NaiveConvolution(double* a, int n, double* b, int m, double* res)
        {
            int len = n + m - 1;
            for (int i = 0; i < len; i++) res[i] = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    res[i + j] += a[i] * b[j];
        }
    }
}
