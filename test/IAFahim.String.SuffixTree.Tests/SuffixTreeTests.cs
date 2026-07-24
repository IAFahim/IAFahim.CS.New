namespace IAFahim.String.SuffixTree.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class SuffixTreeTests
    {
        [Test]
        public void Build_Banana_HasLeavesAndLinks()
        {
            int[] arr = { 1, 2, 3, 2, 3, 4, 0 };
            const int N = 6;
            int* s = stackalloc int[N + 1];
            for (int i = 0; i <= N; i++) s[i] = arr[i];
            SuffixTreeUkkonen.Node* nodes = (SuffixTreeUkkonen.Node*)Marshal.AllocHGlobal(64 * sizeof(SuffixTreeUkkonen.Node));
            SuffixTreeUkkonen.Edge* edges = (SuffixTreeUkkonen.Edge*)Marshal.AllocHGlobal(128 * sizeof(SuffixTreeUkkonen.Edge));
            try
            {
                int nodeCount = 0, edgeCount = 0, last = 0;
                SuffixTreeUkkonen.Build(s, N, nodes, edges, ref nodeCount, ref edgeCount, ref last);
                Assert.IsTrue(nodeCount >= N);
                Assert.IsTrue(edgeCount >= 1);
                Assert.AreEqual(-1, nodes[0].Link);
                int leafish = 0;
                for (int i = 1; i < nodeCount; i++)
                {
                    if (nodes[i].FirstEdge < 0) leafish++;
                }
                Assert.IsTrue(leafish >= 1);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)nodes);
                Marshal.FreeHGlobal((nint)edges);
            }
        }

        [Test]
        public void Build_Empty_OnlyRoot()
        {
            SuffixTreeUkkonen.Node* nodes = (SuffixTreeUkkonen.Node*)Marshal.AllocHGlobal(4 * sizeof(SuffixTreeUkkonen.Node));
            SuffixTreeUkkonen.Edge* edges = (SuffixTreeUkkonen.Edge*)Marshal.AllocHGlobal(4 * sizeof(SuffixTreeUkkonen.Edge));
            try
            {
                int nodeCount = 0, edgeCount = 0, last = 0;
                SuffixTreeUkkonen.Build(null, 0, nodes, edges, ref nodeCount, ref edgeCount, ref last);
                Assert.AreEqual(1, nodeCount);
                Assert.AreEqual(0, edgeCount);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)nodes);
                Marshal.FreeHGlobal((nint)edges);
            }
        }
    }
}
