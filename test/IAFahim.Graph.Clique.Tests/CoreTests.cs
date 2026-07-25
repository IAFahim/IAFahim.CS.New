namespace IAFahim.Graph.Clique.Tests
{
    using NUnit.Framework;

    public sealed unsafe class BronKerboschTests
    {
        [Test]
        public void Triangle_OneMaximalCliqueSize3()
        {
            const int n = 3;
            byte* adj = stackalloc byte[n * n];
            for (int i = 0; i < n * n; i++) adj[i] = 0;
            adj[0*n+1]=adj[1*n+0]=1;
            adj[1*n+2]=adj[2*n+1]=1;
            adj[0*n+2]=adj[2*n+0]=1;
            ulong* cl = stackalloc ulong[8];
            int c = BronKerbosch.EnumerateMaximal(adj, n, cl, 8);
            Assert.AreEqual(1, c);
            Assert.AreEqual(7UL, cl[0]);
            Assert.AreEqual(3, BronKerbosch.MaximumSize(adj, n));
        }

        [Test]
        public void Empty_Zero()
        {
            Assert.AreEqual(0, BronKerbosch.MaximumSize(null, 0));
        }
    }
}
