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
    }
}