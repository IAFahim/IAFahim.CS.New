namespace IAFahim.Graph.SCC.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class TarjanSccTests
    {
        [Test]
        public void TarjanScc_Basic()
        {
            const int N = 4;
            // 0 -> 1 -> 2 -> 0, and 2 -> 3
            int* head = stackalloc int[N];
            for (int i = 0; i < N; i++) head[i] = 0;

            int* next = stackalloc int[5];
            int* to = stackalloc int[5];
            int e = 1;

            void AddEdge(int u, int v)
            {
                to[e] = v; next[e] = head[u]; head[u] = e++;
            }
            
            AddEdge(0, 1);
            AddEdge(1, 2);
            AddEdge(2, 0);
            AddEdge(2, 3);
            
            int* tin = stackalloc int[N];
            int* low = stackalloc int[N];
            int* stack = stackalloc int[N];
            byte* inStack = stackalloc byte[N];
            int* sccId = stackalloc int[N];
            int sccCount = 0;
            
            TarjanScc.Find(N, head, next, to, tin, low, stack, inStack, sccId, ref sccCount);
            
            Assert.AreEqual(2, sccCount);
            Assert.AreEqual(sccId[0], sccId[1]);
            Assert.AreEqual(sccId[1], sccId[2]);
            Assert.AreNotEqual(sccId[0], sccId[3]);
        }
    }
}
