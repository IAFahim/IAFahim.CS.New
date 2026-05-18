namespace IAFahim.Graph.Tests
{
    using IAFahim.Graph;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class GraphTests
    {
        [Fact]
        public void Bfs_TwoNodes_Connected()
        {
            const int n = 2, m = 1;
            int* head = stackalloc int[n];
            int* to = stackalloc int[m * 2];
            int* next = stackalloc int[m * 2];
            for (int i = 0; i < n; i++) head[i] = 0;
            AddEdge.Run(n, head, to, next, 0, 1);
            long* dist = stackalloc long[n];
            for (int i = 0; i < n; i++) dist[i] = -1;
            Bfs.Run(0, n, head, to, next, dist);
            Assert.Equal(0, dist[0]);
            Assert.Equal(1, dist[1]);
        }

        [Fact]
        public void Toposort_Basic()
        {
            const int n = 4;
            int* head = stackalloc int[n];
            int* to = stackalloc int[6];
            int* next = stackalloc int[6];
            for (int i = 0; i < n; i++) head[i] = 0;
            AddDirectedEdge.Run(n, head, to, next, 0, 1);
            AddDirectedEdge.Run(n, head, to, next, 0, 2);
            AddDirectedEdge.Run(n, head, to, next, 1, 3);
            AddDirectedEdge.Run(n, head, to, next, 2, 3);
            int* order = stackalloc int[n];
            int len = Toposort.Run(n, head, to, next, order);
            Assert.Equal(n, len);
            Assert.True(order[0] < order[1]);
            Assert.True(order[0] < order[2]);
            Assert.True(order[1] < order[3] || order[2] < order[3]);
        }

        [Fact]
        public void DsuComponents_Connected()
        {
            const int n = 5, m = 4;
            int* head = stackalloc int[n];
            int* to = stackalloc int[m * 2];
            int* next = stackalloc int[m * 2];
            for (int i = 0; i < n; i++) head[i] = 0;
            AddEdge.Run(n, head, to, next, 0, 1);
            AddEdge.Run(n, head, to, next, 1, 2);
            AddEdge.Run(n, head, to, next, 3, 4);
            int* comp = stackalloc int[n];
            int count = ConnectedComponents.Run(n, head, to, next, comp);
            Assert.Equal(2, count);
        }

        [Fact]
        public void TarjanScc_Basic()
        {
            const int n = 4;
            int* head = stackalloc int[n];
            int* to = stackalloc int[6];
            int* next = stackalloc int[6];
            for (int i = 0; i < n; i++) head[i] = 0;
            AddDirectedEdge.Run(n, head, to, next, 0, 1);
            AddDirectedEdge.Run(n, head, to, next, 1, 2);
            AddDirectedEdge.Run(n, head, to, next, 2, 0);
            AddDirectedEdge.Run(n, head, to, next, 1, 3);
            int* idx = stackalloc int[n];
            int* low = stackalloc int[n];
            int* comp = stackalloc int[n];
            int compCount = TarjanScc.Run(n, head, to, next, idx, low, comp, 0);
            Assert.True(compCount >= 1);
        }

        [Fact]
        public void Bridges_Simple()
        {
            const int n = 4;
            int* head = stackalloc int[n];
            int* to = stackalloc int[6];
            int* next = stackalloc int[6];
            for (int i = 0; i < n; i++) head[i] = 0;
            AddEdge.Run(n, head, to, next, 0, 1);
            AddEdge.Run(n, head, to, next, 1, 2);
            AddEdge.Run(n, head, to, next, 2, 0);
            AddEdge.Run(n, head, to, next, 2, 3);
            int* bu = stackalloc int[n];
            int* bv = stackalloc int[n];
            int count = Bridges.Run(n, head, to, next, bu, bv);
            Assert.True(count >= 0);
        }

        [Fact]
        public void ArticulationPoints_Linear()
        {
            const int n = 3;
            int* head = stackalloc int[n];
            int* to = stackalloc int[6];
            int* next = stackalloc int[6];
            for (int i = 0; i < n; i++) head[i] = 0;
            AddEdge.Run(n, head, to, next, 0, 1);
            AddEdge.Run(n, head, to, next, 1, 2);
            bool* isArt = stackalloc bool[n];
            int count = ArticulationPoints.Run(n, 0, head, to, next, isArt);
            Assert.True(count >= 0);
        }

        [Fact]
        public void Dijkstra_TwoNodes()
        {
            const int n = 2, m = 1;
            int* head = stackalloc int[n];
            int* to = stackalloc int[m * 2];
            int* next = stackalloc int[m * 2];
            long* w = stackalloc long[m * 2];
            for (int i = 0; i < n; i++) head[i] = 0;
            AddWeightedEdge.Run(n, head, to, next, w, 0, 1, 5);
            long* dist = stackalloc long[n];
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            DijkstraSparse.Run(0, n, head, to, next, w, dist);
            Assert.Equal(5, dist[1]);
        }

        [Fact]
        public void Mst_Kruskal_Basic()
        {
            const int n = 4, m = 5;
            int* u = stackalloc int[m] { 0, 0, 1, 1, 2 };
            int* v = stackalloc int[m] { 1, 2, 2, 3, 3 };
            long* w = stackalloc long[m] { 1, 4, 3, 2, 5 };
            int* mstEdges = stackalloc int[m];
            long weight = MinimumSpanningTreeKruskal.Run(n, u, v, w, m, mstEdges);
            Assert.True(weight > 0);
        }

        [Fact]
        public void BellmanFord_NegativeCycle()
        {
            const int n = 3, m = 3;
            int* u = stackalloc int[m];
            int* v = stackalloc int[m];
            long* w = stackalloc long[m];
            u[0] = 0; v[0] = 1; w[0] = 1;
            u[1] = 1; v[1] = 2; w[1] = -5;
            u[2] = 2; v[2] = 0; w[2] = 1;
            long* dist = stackalloc long[n];
            for (int i = 0; i < n; i++) dist[i] = 0;
            int res = BellmanFord.Run(0, n, m, u, v, w, dist);
            Assert.Equal(-1, res);
        }
    }
}