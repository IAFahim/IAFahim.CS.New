namespace IAFahim.Graph.Flow.Tests
{
    using IAFahim.Graph.Flow;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class FlowTests
    {
        [Fact]
        public void DinicMaxFlow_Simple()
        {
            const int n = 4;
            int* head = stackalloc int[n];
            int* to = stackalloc int[8];
            int* next = stackalloc int[8];
            int* cap = stackalloc int[8];
            for (int i = 0; i < n; i++) head[i] = 0;
            int edgeId = 0;
            DinicBfs.Run(n, 0, 0, head, to, next, cap, null, null, null, null);

            for (int i = 0; i < 8; i++) { to[i] = -1; next[i] = 0; cap[i] = 0; }
            head[0] = 1; to[1] = 1; next[1] = 0; cap[1] = 10;
            head[1] = 3; to[3] = 1; next[3] = 0; cap[3] = 0;
            head[0] = 5; to[5] = 2; next[5] = 1; cap[5] = 5;
            head[1] = 7; to[7] = 2; next[7] = 3; cap[7] = 0;
            head[1] = 9; to[9] = 3; next[9] = 7; cap[9] = 7;
            head[2] = 11; to[11] = 3; next[11] = 0; cap[11] = 0;
            head[2] = 13; to[13] = 3; next[13] = 11; cap[13] = 8;
            head[3] = 15; to[15] = 3; next[15] = 13; cap[15] = 0;

            long flow = DinicMaxFlow.Run(n, 0, 3, head, to, next, cap);
            Assert.True(flow >= 0);
        }

        [Fact]
        public void MinCostMaxFlow_Basic()
        {
            const int n = 3;
            int* head = stackalloc int[n];
            int* to = stackalloc int[6];
            int* next = stackalloc int[6];
            int* cost = stackalloc int[6];
            int* cap = stackalloc int[6];
            for (int i = 0; i < n; i++) head[i] = 0;
            long flow = 0, minCost = 0;
            var result = MinCostMaxFlow.Run(n, 0, 2, head, to, next, cost, cap);
            Assert.True(result.flow >= 0);
        }

        [Fact]
        public void MinCut_Basic()
        {
            const int n = 3;
            int* head = stackalloc int[n];
            int* to = stackalloc int[6];
            int* next = stackalloc int[6];
            int* cap = stackalloc int[6];
            int* flow = stackalloc int[6];
            for (int i = 0; i < 6; i++) { flow[i] = 0; cap[i] = 0; }
            bool* visited = stackalloc bool[n];
            int cut = MinCut.Run(n, 0, 2, head, to, next, cap, flow, visited);
            Assert.True(cut >= 0);
        }

        [Fact]
        public void EdmondsKarp_Empty()
        {
            const int n = 2;
            int* head = stackalloc int[n];
            int* to = stackalloc int[4];
            int* next = stackalloc int[4];
            int* cap = stackalloc int[4];
            int* flow = stackalloc int[4];
            for (int i = 0; i < n; i++) head[i] = 0;
            for (int i = 0; i < 4; i++) { flow[i] = 0; cap[i] = 0; }
            long f = EdmondsKarp.Run(n, 0, 1, head, to, next, cap, flow);
            Assert.Equal(0, f);
        }
    }
}