namespace IAFahim.Optimization.Exact.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class SteinerTests
    {
        [Test]
        public void Empty_ReturnsZero()
        {
            long ans = SteinerDreyfusWagner.Run(0, 0, null, null, null, null, long.MaxValue / 4, null);
            Assert.AreEqual(0, ans);
        }

        [Test]
        public void TwoTerminals_PathEqualsEdgeWeight()
        {
            const int N = 2;
            const int M = 2;
            const long Inf = 1_000_000_000L;
            long* w = (long*)Marshal.AllocHGlobal(N * N * sizeof(long));
            bool* terminals = (bool*)Marshal.AllocHGlobal(N * sizeof(bool));
            long* dp = (long*)Marshal.AllocHGlobal((1 << M) * N * sizeof(long));
            try
            {
                for (int i = 0; i < N * N; i++) w[i] = Inf;
                w[0 * N + 1] = 7;
                w[1 * N + 0] = 7;
                w[0 * N + 0] = 0;
                w[1 * N + 1] = 0;
                terminals[0] = true;
                terminals[1] = true;
                long ans = SteinerDreyfusWagner.Run(N, M, null, null, w, terminals, Inf, dp);
                Assert.AreEqual(7, ans);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)w);
                Marshal.FreeHGlobal((nint)terminals);
                Marshal.FreeHGlobal((nint)dp);
            }
        }

        [Test]
        public void ThreeTerminals_StarUsesSteinerCenter()
        {
            const int N = 4;
            const int M = 3;
            const long Inf = 1_000_000_000L;
            long* w = (long*)Marshal.AllocHGlobal(N * N * sizeof(long));
            bool* terminals = (bool*)Marshal.AllocHGlobal(N * sizeof(bool));
            long* dp = (long*)Marshal.AllocHGlobal((1 << M) * N * sizeof(long));
            try
            {
                for (int i = 0; i < N * N; i++) w[i] = Inf;
                for (int i = 0; i < N; i++) w[i * N + i] = 0;
                w[0 * N + 3] = 1; w[3 * N + 0] = 1;
                w[1 * N + 3] = 1; w[3 * N + 1] = 1;
                w[2 * N + 3] = 1; w[3 * N + 2] = 1;
                w[0 * N + 1] = 10; w[1 * N + 0] = 10;
                w[0 * N + 2] = 10; w[2 * N + 0] = 10;
                w[1 * N + 2] = 10; w[2 * N + 1] = 10;
                terminals[0] = true;
                terminals[1] = true;
                terminals[2] = true;
                terminals[3] = false;
                long ans = SteinerDreyfusWagner.Run(N, M, null, null, w, terminals, Inf, dp);
                Assert.AreEqual(3, ans);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)w);
                Marshal.FreeHGlobal((nint)terminals);
                Marshal.FreeHGlobal((nint)dp);
            }
        }

        [Test]
        public void DistinctTerminalMasks_ThreeLeavesOnPath()
        {
            const int N = 3;
            const int M = 3;
            const long Inf = 1_000_000_000L;
            long* w = (long*)Marshal.AllocHGlobal(N * N * sizeof(long));
            bool* terminals = (bool*)Marshal.AllocHGlobal(N * sizeof(bool));
            long* dp = (long*)Marshal.AllocHGlobal((1 << M) * N * sizeof(long));
            try
            {
                for (int i = 0; i < N * N; i++) w[i] = Inf;
                for (int i = 0; i < N; i++) w[i * N + i] = 0;
                w[0 * N + 1] = 2; w[1 * N + 0] = 2;
                w[1 * N + 2] = 3; w[2 * N + 1] = 3;
                w[0 * N + 2] = 100; w[2 * N + 0] = 100;
                terminals[0] = true;
                terminals[1] = true;
                terminals[2] = true;
                long ans = SteinerDreyfusWagner.Run(N, M, null, null, w, terminals, Inf, dp);
                Assert.AreEqual(5, ans);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)w);
                Marshal.FreeHGlobal((nint)terminals);
                Marshal.FreeHGlobal((nint)dp);
            }
        }
    }
}
