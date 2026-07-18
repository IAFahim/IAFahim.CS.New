namespace IAFahim.Optimization.Treewidth.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class CutAndCountTests
    {
        [Test]
        public void EmptyBag_ReturnsZero()
        {
            long* dp = (long*)Marshal.AllocHGlobal(sizeof(long));
            try
            {
                int c = CutAndCount.Run(0, null, null, 0, dp);
                Assert.AreEqual(0, c);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)dp);
            }
        }

        [Test]
        public void PathOfThree_ConnectedSubsetCount()
        {
            const int N = 3;
            bool* adj = (bool*)Marshal.AllocHGlobal(N * N * sizeof(bool));
            int* bag = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            long* dp = (long*)Marshal.AllocHGlobal((1 << N) * sizeof(long));
            try
            {
                for (int i = 0; i < N * N; i++) adj[i] = false;
                adj[0 * N + 1] = true; adj[1 * N + 0] = true;
                adj[1 * N + 2] = true; adj[2 * N + 1] = true;
                bag[0] = 0; bag[1] = 1; bag[2] = 2;
                int count = CutAndCount.Run(N, adj, bag, N, dp);
                Assert.AreEqual(6, count);
                Assert.AreEqual(1, dp[0b001]);
                Assert.AreEqual(1, dp[0b010]);
                Assert.AreEqual(1, dp[0b100]);
                Assert.AreEqual(1, dp[0b011]);
                Assert.AreEqual(1, dp[0b110]);
                Assert.AreEqual(0, dp[0b101]);
                Assert.AreEqual(1, dp[0b111]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)adj);
                Marshal.FreeHGlobal((nint)bag);
                Marshal.FreeHGlobal((nint)dp);
            }
        }

        [Test]
        public void DisjointPair_FullMaskDisconnected()
        {
            const int N = 2;
            bool* adj = (bool*)Marshal.AllocHGlobal(N * N * sizeof(bool));
            int* bag = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            long* dp = (long*)Marshal.AllocHGlobal((1 << N) * sizeof(long));
            try
            {
                for (int i = 0; i < N * N; i++) adj[i] = false;
                bag[0] = 0; bag[1] = 1;
                int count = CutAndCount.Run(N, adj, bag, N, dp);
                Assert.AreEqual(2, count);
                Assert.AreEqual(1, dp[0b01]);
                Assert.AreEqual(1, dp[0b10]);
                Assert.AreEqual(0, dp[0b11]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)adj);
                Marshal.FreeHGlobal((nint)bag);
                Marshal.FreeHGlobal((nint)dp);
            }
        }
    }
}
