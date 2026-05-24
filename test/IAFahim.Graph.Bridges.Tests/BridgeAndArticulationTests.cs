namespace IAFahim.Graph.Bridges.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class BridgeAndArticulationTests
    {
        [Test]
        public void Tarjan_BridgeAndArticulation_Basic()
        {
            const int N = 4;
            // 0-1, 1-2, 2-0, 2-3
            int* head = stackalloc int[N];
            for (int i = 0; i < N; i++) head[i] = -1;
            
            int* next = stackalloc int[8];
            int* to = stackalloc int[8];
            int e = 0;
            
            void AddEdge(int u, int v)
            {
                to[e] = v; next[e] = head[u]; head[u] = e++;
                to[e] = u; next[e] = head[v]; head[v] = e++;
            }
            
            AddEdge(0, 1);
            AddEdge(1, 2);
            AddEdge(2, 0);
            AddEdge(2, 3);
            
            int* tin = stackalloc int[N];
            int* low = stackalloc int[N];
            byte* isArt = stackalloc byte[N];
            int* bridgesU = stackalloc int[4];
            int* bridgesV = stackalloc int[4];
            int bridgeCount = 0;
            
            BridgeAndArticulation.Find(N, head, next, to, tin, low, isArt, bridgesU, bridgesV, ref bridgeCount);
            
            Assert.AreEqual(1, bridgeCount);
            Assert.IsTrue((bridgesU[0] == 2 && bridgesV[0] == 3) || (bridgesU[0] == 3 && bridgesV[0] == 2));
            
            Assert.AreEqual(0, isArt[0]);
            Assert.AreEqual(0, isArt[1]);
            Assert.AreEqual(1, isArt[2]);
            Assert.AreEqual(0, isArt[3]);
        }
    }
}
