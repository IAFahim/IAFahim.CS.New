namespace IAFahim.Graph.Misc.Tests
{
    using NUnit.Framework;

    public sealed unsafe class TransitiveClosureTests
    {
        [Test]
        public void Chain_Closes()
        {
            const int N = 3;
            int* adj = stackalloc int[N*N];
            int* closure = stackalloc int[N*N];
            for (int i=0;i<N*N;i++) adj[i]=0;
            adj[0*N+1]=1; adj[1*N+2]=1;
            TransitiveClosure.Run(N, adj, closure);
            Assert.AreEqual(1, closure[0*N+1]);
            Assert.AreEqual(1, closure[1*N+2]);
            Assert.AreEqual(1, closure[0*N+2]);
        }
    }
}
