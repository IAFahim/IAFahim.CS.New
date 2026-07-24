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

    public sealed unsafe class RankDpTests
    {
        [Test]
        public void ComputeOrder_ParentKeyedByVertex()
        {
            const int N = 3;
            int* adj = stackalloc int[N * N];
            for (int i = 0; i < N * N; i++) adj[i] = 0;
            adj[0 * N + 1] = 1; adj[1 * N + 0] = 1;
            adj[1 * N + 2] = 1; adj[2 * N + 1] = 1;
            int* order = stackalloc int[N];
            int* parent = stackalloc int[N];
            int* rank = stackalloc int[N];
            RankDp.ComputeOrder(N, adj, order, parent, rank);
            Assert.AreEqual(0, order[0]);
            Assert.AreEqual(-1, parent[order[0]]);
            Assert.AreEqual(order[0], parent[order[1]]);
        }

        [Test]
        public void Run_AccumulatesOnParentVertex()
        {
            const int N = 3;
            long* edgeW = stackalloc long[N * N];
            for (int i = 0; i < N * N; i++) edgeW[i] = 0;
            edgeW[0 * N + 1] = 2; edgeW[1 * N + 0] = 2;
            edgeW[1 * N + 2] = 3; edgeW[2 * N + 1] = 3;
            int* order = stackalloc int[] { 0, 1, 2 };
            int* parent = stackalloc int[] { -1, 0, 1 };
            long* dp = stackalloc long[N];
            long root = RankDp.Run(N, edgeW, order, parent, dp);
            Assert.IsTrue(root >= 0);
            Assert.AreEqual(0, dp[2]);
        }

        [Test]
        public void FillBag_IncludesNeighbors()
        {
            const int N = 3;
            int* adj = stackalloc int[N * N];
            for (int i = 0; i < N * N; i++) adj[i] = 0;
            adj[0 * N + 1] = 1; adj[1 * N + 0] = 1;
            int* bag = stackalloc int[N];
            int bagSize = 0;
            RankDp.FillBag(N, 0, adj, bag, &bagSize);
            Assert.IsTrue(bagSize >= 2);
            Assert.AreEqual(0, bag[0]);
        }
    }

    public sealed unsafe class ConvexHullPropertyTests
    {
        [Test]
        public void CheckMonge_AndQuadrangle_Identity()
        {
            long* b = stackalloc long[] { 0, 0, 0, 0 };
            Assert.IsTrue(ConvexHull.CheckMonge(b, 2, 2));
            Assert.IsTrue(ConvexHull.CheckQuadrangle(b, 2, 2));
        }
    }
}
