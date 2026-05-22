namespace IAFahim.Graph.Tree.Tests
{
    using IAFahim.Graph.Tree;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class TreeTests
    {
        private static void AddUndirectedEdge(int* head, int* to, int* next, int* edgeId, int u, int v)
        {
            int e1 = (*edgeId)++;
            to[e1] = v;
            next[e1] = head[u];
            head[u] = e1;

            int e2 = (*edgeId)++;
            to[e2] = u;
            next[e2] = head[v];
            head[v] = e2;
        }

        [Fact]
        public void TreeDiameter_Linear()
        {
            const int n = 5;
            int* head = stackalloc int[n];
            int* to = stackalloc int[(n - 1) * 2 + 1];
            int* next = stackalloc int[(n - 1) * 2 + 1];
            for (int i = 0; i < n; i++) head[i] = 0;
            int edgeId = 1;
            AddUndirectedEdge(head, to, next, &edgeId, 0, 1);
            AddUndirectedEdge(head, to, next, &edgeId, 1, 2);
            AddUndirectedEdge(head, to, next, &edgeId, 2, 3);
            AddUndirectedEdge(head, to, next, &edgeId, 3, 4);
            int d = TreeDiameter.Run(n, 0, head, to, next);
            Assert.Equal(n - 1, d);
        }

        [Fact]
        public void TreeDepth_Basic()
        {
            const int n = 4;
            int* head = stackalloc int[n];
            int* to = stackalloc int[7];
            int* next = stackalloc int[7];
            for (int i = 0; i < n; i++) head[i] = 0;
            int edgeId = 1;
            AddUndirectedEdge(head, to, next, &edgeId, 0, 1);
            AddUndirectedEdge(head, to, next, &edgeId, 0, 2);
            AddUndirectedEdge(head, to, next, &edgeId, 1, 3);
            int* depth = stackalloc int[n];
            for (int i = 0; i < n; i++) depth[i] = -1;
            TreeDepth.Run(n, 0, head, to, next, depth);
            Assert.Equal(0, depth[0]);
            Assert.Equal(1, depth[1]);
            Assert.Equal(1, depth[2]);
            Assert.Equal(2, depth[3]);
        }

        [Fact]
        public void TreeSize_Basic()
        {
            const int n = 5;
            int* head = stackalloc int[n];
            int* to = stackalloc int[9];
            int* next = stackalloc int[9];
            for (int i = 0; i < n; i++) head[i] = 0;
            int edgeId = 1;
            AddUndirectedEdge(head, to, next, &edgeId, 0, 1);
            AddUndirectedEdge(head, to, next, &edgeId, 0, 2);
            AddUndirectedEdge(head, to, next, &edgeId, 1, 3);
            AddUndirectedEdge(head, to, next, &edgeId, 1, 4);
            int* size = stackalloc int[n];
            TreeSize.Run(n, 0, head, to, next, size);
            Assert.Equal(5, size[0]);
            Assert.Equal(3, size[1]);
        }

        [Fact]
        public void LcaBuildAndQuery_Linear()
        {
            const int n = 4;
            int* head = stackalloc int[n];
            int* to = stackalloc int[7];
            int* next = stackalloc int[7];
            for (int i = 0; i < n; i++) head[i] = 0;
            int edgeId = 1;
            AddUndirectedEdge(head, to, next, &edgeId, 0, 1);
            AddUndirectedEdge(head, to, next, &edgeId, 0, 2);
            AddUndirectedEdge(head, to, next, &edgeId, 1, 3);
            int* parent = stackalloc int[n];
            int* depth = stackalloc int[n];
            int logN = 2;
            int* ancestorsFlat = stackalloc int[n * logN];
            int** ancestors = stackalloc int*[n];
            for (int i = 0; i < n; i++) ancestors[i] = &ancestorsFlat[i * logN];
            LcaBuild.Run(n, 0, head, to, next, parent, depth, ancestors, logN);
            int lca = LcaQuery.Run(2, 3, depth, ancestors, logN);
            Assert.Equal(0, lca);
        }

        [Fact]
        public void TreeCenter_SingleChain()
        {
            const int n = 5;
            int* head = stackalloc int[n];
            int* to = stackalloc int[9];
            int* next = stackalloc int[9];
            for (int i = 0; i < n; i++) head[i] = 0;
            int edgeId = 1;
            AddUndirectedEdge(head, to, next, &edgeId, 0, 1);
            AddUndirectedEdge(head, to, next, &edgeId, 1, 2);
            AddUndirectedEdge(head, to, next, &edgeId, 2, 3);
            AddUndirectedEdge(head, to, next, &edgeId, 3, 4);
            int* centers = stackalloc int[n];
            int count = TreeCenter.Run(n, head, to, next, centers);
            Assert.True(count == 1 || count == 2);
        }

        [Fact]
        public void CentroidFind_Basic()
        {
            const int n = 5;
            int* head = stackalloc int[n];
            int* to = stackalloc int[9];
            int* next = stackalloc int[9];
            for (int i = 0; i < n; i++) head[i] = 0;
            int edgeId = 1;
            AddUndirectedEdge(head, to, next, &edgeId, 0, 1);
            AddUndirectedEdge(head, to, next, &edgeId, 0, 2);
            AddUndirectedEdge(head, to, next, &edgeId, 2, 3);
            AddUndirectedEdge(head, to, next, &edgeId, 2, 4);
            bool* removed = (bool*)Marshal.AllocHGlobal(n * sizeof(bool));
            int* size = stackalloc int[n];
            try
            {
                for (int i = 0; i < n; i++) removed[i] = false;
                int c = CentroidFind.Run(n, 0, head, to, next, removed, size);
                Assert.True(c >= 0 && c < n);
            }
            finally { Marshal.FreeHGlobal((nint)removed); }
        }
    }
}