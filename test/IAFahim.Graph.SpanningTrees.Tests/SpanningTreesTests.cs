namespace IAFahim.Graph.SpanningTrees.Tests
{
    using NUnit.Framework;

    // MinimumPathCoverDag: min path cover in DAG = n - max_matching (Hopcroft-Karp).
    public sealed unsafe class SpanningTreesTests
    {
        [Test]
        public void PathCover_NoEdges_EveryNodeItsOwnPath()
        {
            // 3 isolated nodes, no edges => 3 paths.
            const int N = 3, M = 0;
            int* eu = stackalloc int[0];
            int* ev = stackalloc int[0];
            int* mL = stackalloc int[N + 1];
            int* mR = stackalloc int[N + 1];
            int cover = MinimumPathCoverDag.Run(N, M, eu, ev, mL, mR);
            Assert.AreEqual(3, cover);
        }

        [Test]
        public void PathCover_Chain_OnePath()
        {
            // 0->1->2. Min path cover = 1 (single path covers all 3).
            const int N = 3, M = 2;
            int* eu = stackalloc int[2] { 0, 1 };
            int* ev = stackalloc int[2] { 1, 2 };
            int* mL = stackalloc int[N + 1];
            int* mR = stackalloc int[N + 1];
            int cover = MinimumPathCoverDag.Run(N, M, eu, ev, mL, mR);
            Assert.AreEqual(1, cover);
        }

        [Test]
        public void PathCover_Fork_TwoPaths()
        {
            // 0->1, 0->2. Min path cover = 2 (one path 0->1, one for 2; or 0->2, one for 1).
            const int N = 3, M = 2;
            int* eu = stackalloc int[2] { 0, 0 };
            int* ev = stackalloc int[2] { 1, 2 };
            int* mL = stackalloc int[N + 1];
            int* mR = stackalloc int[N + 1];
            int cover = MinimumPathCoverDag.Run(N, M, eu, ev, mL, mR);
            Assert.AreEqual(2, cover);
        }

        [Test]
        public void PathCover_Diamond_TwoPaths()
        {
            // 0->1, 0->2, 1->3, 2->3 (diamond). Min path cover = 2.
            const int N = 4, M = 4;
            int* eu = stackalloc int[4] { 0, 0, 1, 2 };
            int* ev = stackalloc int[4] { 1, 2, 3, 3 };
            int* mL = stackalloc int[N + 1];
            int* mR = stackalloc int[N + 1];
            int cover = MinimumPathCoverDag.Run(N, M, eu, ev, mL, mR);
            Assert.AreEqual(2, cover);
        }

        [Test]
        public void PathCover_SingleNode_OnePath()
        {
            const int N = 1, M = 0;
            int* eu = stackalloc int[0];
            int* ev = stackalloc int[0];
            int* mL = stackalloc int[N + 1];
            int* mR = stackalloc int[N + 1];
            int cover = MinimumPathCoverDag.Run(N, M, eu, ev, mL, mR);
            Assert.AreEqual(1, cover);
        }

        [Test]
        public void PathCover_LongChain_OnePath()
        {
            // 0->1->2->3->4->5->6->7. Min path cover = 1.
            const int N = 8, M = 7;
            int* eu = stackalloc int[7] { 0, 1, 2, 3, 4, 5, 6 };
            int* ev = stackalloc int[7] { 1, 2, 3, 4, 5, 6, 7 };
            int* mL = stackalloc int[N + 1];
            int* mR = stackalloc int[N + 1];
            int cover = MinimumPathCoverDag.Run(N, M, eu, ev, mL, mR);
            Assert.AreEqual(1, cover);
        }
    }
}
