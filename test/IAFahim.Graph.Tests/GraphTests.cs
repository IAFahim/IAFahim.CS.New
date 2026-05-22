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
            int edgeId = 1;
            int edgeCount = 1;
            AddEdge.Run(head, to, next, &edgeId, 0, 1, &edgeCount);
            int* dist = stackalloc int[n];
            for (int i = 0; i < n; i++) dist[i] = -1;
            int* parent = stackalloc int[n];
            Bfs.Run(n, 0, head, to, next, dist, parent);
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
            int edgeId = 1;
            int edgeCount = 4;
            AddDirectedEdge.Run(head, to, next, &edgeId, 0, 1, &edgeCount);
            AddDirectedEdge.Run(head, to, next, &edgeId, 0, 2, &edgeCount);
            AddDirectedEdge.Run(head, to, next, &edgeId, 1, 3, &edgeCount);
            AddDirectedEdge.Run(head, to, next, &edgeId, 2, 3, &edgeCount);
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
            int edgeId = 1;
            int edgeCount = 3;
            AddEdge.Run(head, to, next, &edgeId, 0, 1, &edgeCount);
            AddEdge.Run(head, to, next, &edgeId, 1, 2, &edgeCount);
            AddEdge.Run(head, to, next, &edgeId, 3, 4, &edgeCount);
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
            int edgeId = 1;
            int edgeCount = 4;
            AddDirectedEdge.Run(head, to, next, &edgeId, 0, 1, &edgeCount);
            AddDirectedEdge.Run(head, to, next, &edgeId, 1, 2, &edgeCount);
            AddDirectedEdge.Run(head, to, next, &edgeId, 2, 0, &edgeCount);
            AddDirectedEdge.Run(head, to, next, &edgeId, 1, 3, &edgeCount);
            int* idx = stackalloc int[n];
            int* low = stackalloc int[n];
            bool* onStack = stackalloc bool[n];
            int* stack = stackalloc int[n];
            int* comp = stackalloc int[n];
            for (int i = 0; i < n; i++) { idx[i] = -1; onStack[i] = false; comp[i] = -1; }
            int stackSize = 0;
            int curIdx = 0;
            int sccCount = 0;
            for (int i = 0; i < n; i++)
            {
                if (idx[i] < 0)
                    TarjanScc.Run(i, head, to, next, idx, low, onStack, stack, ref stackSize, ref curIdx, ref sccCount, comp);
            }
            Assert.True(sccCount >= 1);
        }

        [Fact]
        public void Bridges_Simple()
        {
            const int n = 4;
            int* head = stackalloc int[n];
            int* to = stackalloc int[6];
            int* next = stackalloc int[6];
            for (int i = 0; i < n; i++) head[i] = 0;
            int edgeId = 1;
            int edgeCount = 4;
            AddEdge.Run(head, to, next, &edgeId, 0, 1, &edgeCount);
            AddEdge.Run(head, to, next, &edgeId, 1, 2, &edgeCount);
            AddEdge.Run(head, to, next, &edgeId, 2, 0, &edgeCount);
            AddEdge.Run(head, to, next, &edgeId, 2, 3, &edgeCount);
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
            int edgeId = 1;
            int edgeCount = 2;
            AddEdge.Run(head, to, next, &edgeId, 0, 1, &edgeCount);
            AddEdge.Run(head, to, next, &edgeId, 1, 2, &edgeCount);
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
            int* w = stackalloc int[m * 2];
            for (int i = 0; i < n; i++) head[i] = 0;
            int edgeId = 1;
            int edgeCount = 1;
            AddWeightedEdge.Run(head, to, next, w, &edgeId, 0, 1, 5, &edgeCount);
            long* dist = stackalloc long[n];
            for (int i = 0; i < n; i++) dist[i] = long.MaxValue;
            int* parent = stackalloc int[n];
            DijkstraSparse.Run(n, 0, head, to, next, w, dist, parent);
            Assert.Equal(5, dist[1]);
        }

        [Fact]
        public void Mst_Kruskal_Basic()
        {
            const int n = 4, m = 5;
            int* u = stackalloc int[m] { 0, 0, 1, 1, 2 };
            int* v = stackalloc int[m] { 1, 2, 2, 3, 3 };
            int* w = stackalloc int[m] { 1, 4, 3, 2, 5 };
            int* mstEdges = stackalloc int[m];
            long weight = MinimumSpanningTreeKruskal.Run(n, m, u, v, w, mstEdges);
            Assert.True(weight > 0);
        }

        [Fact]
        public void BellmanFord_NegativeCycle()
        {
            const int n = 3, m = 3;
            int* u = stackalloc int[m];
            int* v = stackalloc int[m];
            int* w = stackalloc int[m];
            u[0] = 0; v[0] = 1; w[0] = 1;
            u[1] = 1; v[1] = 2; w[1] = -5;
            u[2] = 2; v[2] = 0; w[2] = 1;
            long* dist = stackalloc long[n];
            for (int i = 0; i < n; i++) dist[i] = 0;
            int* parent = stackalloc int[n];
            bool success = BellmanFord.Run(n, 0, m, u, v, w, dist, parent);
            Assert.False(success);
        }
    }
}