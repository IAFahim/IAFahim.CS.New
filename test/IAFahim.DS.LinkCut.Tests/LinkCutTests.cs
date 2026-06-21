namespace IAFahim.DS.LinkCut.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class LinkCutTests
    {
        private static LctNode* MakeNodes(int n, long value)
        {
            LctNode* nodes = (LctNode*)Marshal.AllocHGlobal(n * sizeof(LctNode));
            for (int i = 0; i < n; i++)
            {
                nodes[i] = new LctNode { Index = i, Value = value, PathSum = value, Rev = false, Left = null, Right = null, Parent = null };
            }
            return nodes;
        }

        private static bool Connected(LctNode* x, LctNode* y) => LinkCut.FindRoot(x) == LinkCut.FindRoot(y);

        [Test]
        public void SingleNode_FindRoot_Self()
        {
            LctNode* n = MakeNodes(1, 5);
            try { Assert.IsTrue(n == LinkCut.FindRoot(n)); }
            finally { Marshal.FreeHGlobal((nint)n); }
        }

        [Test]
        public void Link_ThenConnected_Cut_ThenDisconnected()
        {
            LctNode* n = MakeNodes(4, 1);
            try
            {
                Assert.IsFalse(Connected(n + 0, n + 1));
                LinkCut.Link(n + 0, n + 1);
                Assert.IsTrue(Connected(n + 0, n + 1));
                LinkCut.Cut(n + 0, n + 1);
                Assert.IsFalse(Connected(n + 0, n + 1));
            }
            finally { Marshal.FreeHGlobal((nint)n); }
        }

        [Test]
        public void PathChain_LinkCutQuery_SumsPath()
        {
            // 0 - 1 - 2 - 3, values 1,2,3,4. Query(0,3) = 1+2+3+4 = 10.
            LctNode* n = MakeNodes(4, 0);
            try
            {
                n[0].Value = 1; n[1].Value = 2; n[2].Value = 3; n[3].Value = 4;
                LinkCut.Link(n + 0, n + 1);
                LinkCut.Link(n + 1, n + 2);
                LinkCut.Link(n + 2, n + 3);
                Assert.AreEqual(10, LinkCut.Query(n + 0, n + 3));
                Assert.AreEqual(10, LinkCut.Query(n + 3, n + 0));
                Assert.AreEqual(9, LinkCut.Query(n + 1, n + 3));
                Assert.AreEqual(2, LinkCut.Query(n + 1, n + 1));
            }
            finally { Marshal.FreeHGlobal((nint)n); }
        }

        [Test]
        public void Cut_Middle_SplitsComponents()
        {
            // 0 - 1 - 2 - 3. Cut(1,2). {0,1} and {2,3} separate.
            LctNode* n = MakeNodes(4, 1);
            try
            {
                LinkCut.Link(n + 0, n + 1);
                LinkCut.Link(n + 1, n + 2);
                LinkCut.Link(n + 2, n + 3);
                LinkCut.Cut(n + 1, n + 2);
                Assert.IsTrue(Connected(n + 0, n + 1));
                Assert.IsTrue(Connected(n + 2, n + 3));
                Assert.IsFalse(Connected(n + 0, n + 2));
                Assert.IsFalse(Connected(n + 1, n + 3));
            }
            finally { Marshal.FreeHGlobal((nint)n); }
        }

        [Test]
        public void StarTopology_QueriesToCenter()
        {
            // center 0 connected to 1,2,3. Query(0,k) = 0+k.
            LctNode* n = MakeNodes(4, 0);
            try
            {
                n[1].Value = 10; n[2].Value = 20; n[3].Value = 30;
                LinkCut.Link(n + 1, n + 0);
                LinkCut.Link(n + 2, n + 0);
                LinkCut.Link(n + 3, n + 0);
                Assert.AreEqual(10, LinkCut.Query(n + 0, n + 1));
                Assert.AreEqual(30, LinkCut.Query(n + 0, n + 3));
            }
            finally { Marshal.FreeHGlobal((nint)n); }
        }
    }
}
