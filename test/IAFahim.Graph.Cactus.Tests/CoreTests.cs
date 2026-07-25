namespace IAFahim.Graph.Cactus.Tests
{
    using NUnit.Framework;

    public sealed unsafe class CactusLcaTests
    {
        [Test]
        public void BinaryLifting_PathTree()
        {
            // tree: 0-1-2
            int n = 3;
            int* parent = stackalloc int[3] { -1, 0, 1 };
            int* depth = stackalloc int[3];
            int* queue = stackalloc int[3];
            CactusLca.BuildDepth(n, parent, depth, queue);
            int maxLog = CactusLca.MaxLog(n);
            int* up = stackalloc int[n * maxLog];
            CactusLca.BuildJump(n, parent, depth, up, maxLog);
            Assert.AreEqual(0, CactusLca.Query(0, 2, depth, up, maxLog));
            Assert.AreEqual(1, CactusLca.Query(1, 2, depth, up, maxLog));
            Assert.AreEqual(1, BlockCutTreeLca.Query(1, 2, depth, up, maxLog));
        }
    }
}
