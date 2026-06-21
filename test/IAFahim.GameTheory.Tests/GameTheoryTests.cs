namespace IAFahim.GameTheory.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class GameTheoryTests
    {
        [Test]
        public void NimSum_Empty_Zero()
        {
            long* piles = stackalloc long[0];
            Assert.AreEqual(0, NimSum.Run(0, piles));
        }

        [Test]
        public void NimSum_Single_ReturnsIt()
        {
            long* piles = stackalloc long[1] { 42 };
            Assert.AreEqual(42, NimSum.Run(1, piles));
        }

        [Test]
        public void NimSum_BalancedPosition_Zero()
        {
            long* piles = stackalloc long[3] { 3, 4, 7 };
            Assert.AreEqual(0, NimSum.Run(3, piles));
            long* piles2 = stackalloc long[3] { 1, 2, 3 };
            Assert.AreEqual(0, NimSum.Run(3, piles2));
        }

        [Test]
        public void NimSum_Unbalanced_Nonzero()
        {
            long* piles = stackalloc long[3] { 1, 2, 4 };
            Assert.AreEqual(1 ^ 2 ^ 4, NimSum.Run(3, piles));
            Assert.AreNotEqual(0, NimSum.Run(3, piles));
        }

        [Test]
        public void GrundyDAG_Chain_AlternatesZeroOne()
        {
            // Functional graph chain: 0->1->2->(-1). Each node has exactly one successor.
            // grundy[2]=0 (no successor), grundy[1]=mex(grundy[2])=1, grundy[0]=mex(grundy[1])=0.
            const int N = 3;
            int* to = stackalloc int[N] { 1, 2, -1 };
            int* grundy = stackalloc int[N];
            int* indeg = stackalloc int[N] { 0, 1, 1 };
            int* queue = stackalloc int[N];
            int processed = GrundyDAG.Run(N, to, grundy, indeg, queue);
            Assert.AreEqual(3, processed);
            Assert.AreEqual(0, grundy[0]);
            Assert.AreEqual(1, grundy[1]);
            Assert.AreEqual(0, grundy[2]);
        }

        [Test]
        public void GrundyDAG_SingleSink_GrundyZero()
        {
            // Single node, no successor (to=-1). Grundy = mex(empty) = 0.
            const int N = 1;
            int* to = stackalloc int[N] { -1 };
            int* grundy = stackalloc int[N];
            int* indeg = stackalloc int[N] { 0 };
            int* queue = stackalloc int[N];
            GrundyDAG.Run(N, to, grundy, indeg, queue);
            Assert.AreEqual(0, grundy[0]);
        }

        [Test]
        public void GrundyDAG_TwoSourcesSameSink_MergedCorrectly()
        {
            // 0->2, 1->2. Both point to sink 2. indeg: {0,0,2}.
            // grundy[2]=0, grundy[0]=mex(0)=1, grundy[1]=mex(0)=1.
            const int N = 3;
            int* to = stackalloc int[N] { 2, 2, -1 };
            int* grundy = stackalloc int[N];
            int* indeg = stackalloc int[N] { 0, 0, 2 };
            int* queue = stackalloc int[N];
            GrundyDAG.Run(N, to, grundy, indeg, queue);
            Assert.AreEqual(1, grundy[2]);
            Assert.AreEqual(0, grundy[0]);
            Assert.AreEqual(0, grundy[1]);
        }

        [Test]
        public void GameDp_SubtractionGame_HalvingPattern()
        {
            // Moves {1,3,4}: dp[i] = mex(dp[i-1], dp[i-3], dp[i-4]).
            // Classic subtraction game Grundy: 0,1,0,1,2,3,2,0,1,0,...
            const int N = 10;
            long* dp = stackalloc long[N];
            long* a = stackalloc long[N];
            int* moves = stackalloc int[3] { 1, 3, 4 };
            GameDp.Run(N, dp, a, moves, 3);
            long[] expected = { 0, 1, 0, 1, 2, 3, 2, 0, 1, 0 };
            for (int i = 0; i < N; i++)
                Assert.AreEqual(expected[i], dp[i], $"dp[{i}]");
        }

        [Test]
        public void GameDp_SingleMoveOne_Alternates()
        {
            // Move {1} only: dp[0]=0, dp[1]=mex(0)=1, dp[2]=mex(1)=0, ...
            const int N = 8;
            long* dp = stackalloc long[N];
            long* a = stackalloc long[N];
            int* moves = stackalloc int[1] { 1 };
            GameDp.Run(N, dp, a, moves, 1);
            for (int i = 0; i < N; i++)
                Assert.AreEqual(i % 2, dp[i], $"dp[{i}]");
        }
    }
}
