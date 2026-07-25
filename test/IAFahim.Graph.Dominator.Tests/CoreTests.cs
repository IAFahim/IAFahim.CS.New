namespace IAFahim.Graph.Dominator.Tests
{
    using NUnit.Framework;

    public sealed unsafe class DominatorTests
    {
        [Test]
        public void Diamond_RootDominatesAll()
        {
            // 0->1, 0->2, 1->3, 2->3
            int* head = stackalloc int[4];
            int* to = stackalloc int[16];
            int* next = stackalloc int[16];
            for (int i = 0; i < 4; i++) head[i] = 0;
            int e = 1;
            void Add(int u, int v) { to[e]=v; next[e]=head[u]; head[u]=e++; }
            Add(0,1); Add(0,2); Add(1,3); Add(2,3);
            int* idom = stackalloc int[4];
            Assert.IsTrue(SimpleDominators.Run(4, 0, head, to, next, idom));
            Assert.AreEqual(0, idom[0]);
            Assert.AreEqual(0, idom[1]);
            Assert.AreEqual(0, idom[2]);
            Assert.AreEqual(0, idom[3]);
            Assert.IsTrue(SimpleDominators.Dominates(idom, 0, 3));
        }
    }
}
