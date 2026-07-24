namespace IAFahim.Graph.ShortestPath.Tests
{
    using NUnit.Framework;

    public sealed unsafe class MinPlusTests
    {
        [Test]
        public void AllPairsMinPlus_PathComposition()
        {
            const int N = 2;
            long Inf = long.MaxValue;
            long* a = stackalloc long[N * N];
            long* b = stackalloc long[N * N];
            long* c = stackalloc long[N * N];
            a[0] = 0; a[1] = 3; a[2] = Inf; a[3] = 0;
            b[0] = 0; b[1] = 4; b[2] = Inf; b[3] = 0;
            AllPairsMinPlus.Run(N, a, b, c);
            Assert.AreEqual(0, c[0]);
            Assert.AreEqual(3, c[1]);
            Assert.AreEqual(Inf, c[2]);
            Assert.AreEqual(0, c[3]);
        }
    }

    public sealed unsafe class DynamicShortestPathTests
    {
        [Test]
        public void EdgeDecreased_Tightens()
        {
            const int N = 2;
            long Inf = long.MaxValue;
            long* dist = stackalloc long[N * N];
            dist[0] = 0; dist[1] = 10;
            dist[2] = Inf; dist[3] = 0;
            DynamicShortestPathUpdate.EdgeDecreased(N, dist, 0, 1, 5);
            Assert.AreEqual(5, dist[1]);
        }
    }
}
