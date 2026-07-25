namespace IAFahim.Graph.Decomposition.Tests
{
    using NUnit.Framework;

    public sealed unsafe class CentroidDecompositionTests
    {
        [Test]
        public void Path3_HasRoot()
        {
            int* head = stackalloc int[3];
            int* to = stackalloc int[8];
            int* next = stackalloc int[8];
            for (int i = 0; i < 3; i++) head[i] = 0;
            int e = 1;
            void Add(int u, int v) { to[e]=v; next[e]=head[u]; head[u]=e++; }
            Add(0,1); Add(1,0); Add(1,2); Add(2,1);
            int* cp = stackalloc int[3];
            CentroidDecomposition.Build(3, head, to, next, cp);
            int roots = 0;
            for (int i = 0; i < 3; i++) if (cp[i] < 0) roots++;
            Assert.AreEqual(1, roots);
        }
    }
}
