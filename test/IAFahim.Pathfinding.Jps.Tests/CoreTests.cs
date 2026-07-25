namespace IAFahim.Pathfinding.Jps.Tests
{
    using NUnit.Framework;

    public sealed unsafe class JumpPointSearchTests
    {
        [Test]
        public void OpenGrid_FindsPath()
        {
            const int W = 5, H = 5;
            byte* g = stackalloc byte[W * H];
            for (int i = 0; i < W * H; i++) g[i] = 0;
            int* path = stackalloc int[64];
            int len = JumpPointSearch.FindPath(g, W, H, 0, 0, 4, 4, path, 32);
            Assert.IsTrue(len > 0);
            Assert.AreEqual(0, path[0]);
            Assert.AreEqual(0, path[1]);
            Assert.AreEqual(4, path[(len-1)*2]);
            Assert.AreEqual(4, path[(len-1)*2+1]);
        }

        [Test]
        public void BlockedStart_ReturnsNeg1()
        {
            byte* g = stackalloc byte[4] { 1, 0, 0, 0 };
            int* path = stackalloc int[8];
            Assert.AreEqual(-1, JumpPointSearch.FindPath(g, 2, 2, 0, 0, 1, 1, path, 4));
        }
    }
}
