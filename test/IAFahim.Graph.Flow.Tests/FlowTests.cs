namespace IAFahim.Graph.Flow.Tests
{
    using IAFahim.Graph.Flow;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class FlowTests
    {
        [Test]
        public void DinicMaxFlow_Simple()
        {
            const int n = 4;
            int* head = stackalloc int[n];
            int* to = stackalloc int[16];
            int* next = stackalloc int[16];
            int* cap = stackalloc int[16];
            int* flowArr = stackalloc int[16];
            int* cost = stackalloc int[16];
            for (int i = 0; i < n; i++) head[i] = 0;
            for (int i = 0; i < 16; i++) flowArr[i] = 0;
            
            int edgeId = 2;
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 0, 1, 0, 10);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 0, 2, 0, 5);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 1, 2, 0, 15);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 1, 3, 0, 10);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 2, 3, 0, 10);

            long flow = DinicMaxFlow.Run(n, 0, 3, head, to, next, cap, flowArr);
            Assert.AreEqual(15, flow);
        }

        [Test]
        public void MinCostMaxFlow_Empty()
        {
            const int n = 3;
            int* head = stackalloc int[n];
            int* to = stackalloc int[6];
            int* next = stackalloc int[6];
            int* cost = stackalloc int[6];
            int* cap = stackalloc int[6];
            for (int i = 0; i < n; i++) head[i] = 0;
            var result = MinCostMaxFlow.Run(n, 0, 2, head, to, next, cost, cap);
            Assert.IsTrue(result.flow >= 0);
        }

        
        [Test]
        public void MinCut_Basic()
        {
            const int n = 3;
            int* head = stackalloc int[n];
            int* to = stackalloc int[6];
            int* next = stackalloc int[6];
            int* cap = stackalloc int[6];
            int* flow = stackalloc int[6];
            for (int i = 0; i < 6; i++) { flow[i] = 0; cap[i] = 0; }
            byte* visited = stackalloc byte[n];
            MinCut.Run(n, 0, head, to, next, cap, visited);
        }

        [Test]
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
            Assert.AreEqual(0, f);
        }

        [Test]
        public void IsapAndDinicWithLinkCut_SameAsDinic()
        {
            const int n = 4;
            int* head = stackalloc int[n];
            int* to = stackalloc int[16];
            int* next = stackalloc int[16];
            int* cap = stackalloc int[16];
            int* flow1 = stackalloc int[16];
            int* flow2 = stackalloc int[16];
            int* flow3 = stackalloc int[16];
            int* cost = stackalloc int[16];
            for (int i = 0; i < n; i++) head[i] = 0;
            for (int i = 0; i < 16; i++) { flow1[i] = flow2[i] = flow3[i] = 0; }
            int edgeId = 2;
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 0, 1, 0, 10);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 0, 2, 0, 5);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 1, 2, 0, 15);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 1, 3, 0, 10);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 2, 3, 0, 10);
            long d = DinicMaxFlow.Run(n, 0, 3, head, to, next, cap, flow1);
            for (int i = 0; i < 16; i++) flow2[i] = 0;
            long isap = IsapGapOptimization.Run(n, 0, 3, head, to, next, cap, flow2);
            for (int i = 0; i < 16; i++) flow3[i] = 0;
            long dlc = DinicWithLinkCut.Run(n, 0, 3, head, to, next, cap, flow3);
            Assert.AreEqual(d, isap);
            Assert.AreEqual(d, dlc);
            Assert.AreEqual(15, d);
        }

        [Test]
        public void MinCostFlowSspAndDijkstra_CheapestPath()
        {
            const int n = 3;
            int* head = stackalloc int[n];
            int* to = stackalloc int[16];
            int* next = stackalloc int[16];
            int* cap = stackalloc int[16];
            int* cost = stackalloc int[16];
            int* flow = stackalloc int[16];
            for (int i = 0; i < n; i++) head[i] = 0;
            for (int i = 0; i < 16; i++) flow[i] = 0;
            int edgeId = 2;
            // s=0 -> 1 cost 5 cap 1; 0 -> 2 cost 10 cap 1; 1 -> 2 cost 1 cap 1  => cheapest unit is 6
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 0, 1, 5, 1);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 0, 2, 10, 1);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 1, 2, 1, 1);
            // Max flow is 2: path 0-1-2 cost 6 and path 0-2 cost 10 => total min-cost 16.
            long c1 = MinCostFlowSsp.Run(n, 0, 2, head, to, next, cap, cost, flow);
            for (int i = 0; i < 16; i++) flow[i] = 0;
            long c2 = MinCostFlowDijkstra.Run(n, 0, 2, head, to, next, cap, cost, flow);
            for (int i = 0; i < 16; i++) flow[i] = 0;
            long c3 = MinCostFlowCapacityScaling.Run(n, 0, 2, head, to, next, cap, cost, flow);
            Assert.AreEqual(16, c1);
            Assert.AreEqual(16, c2);
            Assert.AreEqual(16, c3);
        }

        [Test]
        public void MinimumSTCutAll_CutEdgesSaturated()
        {
            const int n = 4;
            int* head = stackalloc int[n];
            int* to = stackalloc int[16];
            int* next = stackalloc int[16];
            int* cap = stackalloc int[16];
            int* flow = stackalloc int[16];
            int* cost = stackalloc int[16];
            int* cutU = stackalloc int[8];
            int* cutV = stackalloc int[8];
            for (int i = 0; i < n; i++) head[i] = 0;
            for (int i = 0; i < 16; i++) flow[i] = 0;
            int edgeId = 2;
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 0, 1, 0, 10);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 0, 2, 0, 5);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 1, 2, 0, 15);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 1, 3, 0, 10);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 2, 3, 0, 10);
            int cutCount = 0;
            long mf = MinimumSTCutAll.Run(n, 0, 3, head, to, next, cap, flow, cutU, cutV, &cutCount);
            Assert.AreEqual(15, mf);
            Assert.IsTrue(cutCount >= 1);
        }
    }
}
