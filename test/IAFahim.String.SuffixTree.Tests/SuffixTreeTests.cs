namespace IAFahim.String.SuffixTree.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class SuffixTreeTests
    {
        [Test]
        public void Build_NonEmpty_CreatesNodes()
        {
            const int N = 4;
            int* s = stackalloc int[N + 1];
            s[0] = 1; s[1] = 2; s[2] = 1; s[3] = 3; s[4] = 0;
            SuffixTreeUkkonen.Node* nodes = (SuffixTreeUkkonen.Node*)Marshal.AllocHGlobal(32 * sizeof(SuffixTreeUkkonen.Node));
            SuffixTreeUkkonen.Edge* edges = (SuffixTreeUkkonen.Edge*)Marshal.AllocHGlobal(64 * sizeof(SuffixTreeUkkonen.Edge));
            try
            {
                int nodeCount = 0, edgeCount = 0, last = 0;
                SuffixTreeUkkonen.Build(s, N, nodes, edges, ref nodeCount, ref edgeCount, ref last);
                Assert.IsTrue(nodeCount >= 1);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)nodes);
                Marshal.FreeHGlobal((nint)edges);
            }
        }
    }
}
