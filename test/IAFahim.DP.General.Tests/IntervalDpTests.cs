namespace IAFahim.DP.General.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class IntervalDpTests
    {
        [Test]
        public void SingleElement_Zero()
        {
            long* dp = stackalloc long[1];
            int* a = stackalloc int[1];
            a[0] = 1;
            long ans = IntervalDp.Run(1, a, dp, null);
            Assert.AreEqual(0, ans);
            Assert.AreEqual(0, dp[0]);
        }

        [Test]
        public void TwoElements_OneMerge()
        {
            const int N = 2;
            long* dp = (long*)Marshal.AllocHGlobal(N * N * sizeof(long));
            int* a = stackalloc int[2];
            try
            {
                for (int i = 0; i < N * N; i++) dp[i] = long.MaxValue;
                long ans = IntervalDp.Run(N, a, dp, null);
                Assert.AreEqual(1, ans);
                Assert.AreEqual(0, dp[0 * N + 0]);
                Assert.AreEqual(0, dp[1 * N + 1]);
                Assert.AreEqual(1, dp[0 * N + 1]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)dp);
            }
        }

        [Test]
        public void ThreeElements_TwoMerges()
        {
            const int N = 3;
            long* dp = (long*)Marshal.AllocHGlobal(N * N * sizeof(long));
            int* a = stackalloc int[3];
            try
            {
                for (int i = 0; i < N * N; i++) dp[i] = long.MaxValue;
                long ans = IntervalDp.Run(N, a, dp, null);
                Assert.AreEqual(2, ans);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)dp);
            }
        }

        [Test]
        public void DiagonalBase_NotRowZero()
        {
            const int N = 4;
            long* dp = (long*)Marshal.AllocHGlobal(N * N * sizeof(long));
            int* a = stackalloc int[4];
            try
            {
                for (int i = 0; i < N * N; i++) dp[i] = 999;
                IntervalDp.Run(N, a, dp, null);
                for (int i = 0; i < N; i++)
                    Assert.AreEqual(0, dp[i * N + i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)dp);
            }
        }
    }

    public sealed unsafe class MinPlusConvolutionTests
    {
        [Test]
        public void Simple_Convolution()
        {
            const long INF = long.MaxValue / 4;
            long* a = stackalloc long[2];
            long* b = stackalloc long[2];
            long* c = stackalloc long[4];
            a[0] = 1; a[1] = 3;
            b[0] = 2; b[1] = 4;
            MinPlusConvolution.Run(2, 2, a, b, c, INF);
            Assert.AreEqual(3, c[0]);
            Assert.AreEqual(5, c[1]);
            Assert.AreEqual(7, c[2]);
        }
    }
}
