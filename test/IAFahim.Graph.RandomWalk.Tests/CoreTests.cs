namespace IAFahim.Graph.RandomWalk.Tests
{
    using NUnit.Framework;

    public sealed unsafe class RandomWalkTests
    {
        [Test]
        public void Walk_StaysOnGraph()
        {
            // 0-1-2 path
            int* head = stackalloc int[3];
            int* to = stackalloc int[8];
            int* next = stackalloc int[8];
            for (int i = 0; i < 3; i++) head[i] = 0;
            int e = 1;
            void Add(int u, int v) { to[e]=v; next[e]=head[u]; head[u]=e++; }
            Add(0,1); Add(1,0); Add(1,2); Add(2,1);
            int* path = stackalloc int[16];
            uint rng = 42;
            Assert.IsTrue(SimpleRandomWalk.Run(3, 0, 5, head, to, next, &rng, path));
            Assert.AreEqual(0, path[0]);
            for (int i = 0; i <= 5; i++) Assert.IsTrue(path[i] >= 0 && path[i] < 3);
        }
    }
}
