namespace IAFahim.Graph.Connectivity.Tests
{
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class ConnectivityTests
    {
        [Fact]
        public void IncrementalConnectivity_Basic()
        {
            const int N = 10;
            int* parent = stackalloc int[N];
            int* size = stackalloc int[N];
            
            IncrementalConnectivity.Init(parent, size, N);
            Assert.False(IncrementalConnectivity.Connected(parent, 0, 1));
            
            Assert.True(IncrementalConnectivity.Union(parent, size, 0, 1));
            Assert.True(IncrementalConnectivity.Connected(parent, 0, 1));
            Assert.False(IncrementalConnectivity.Union(parent, size, 0, 1)); // Already connected
            
            IncrementalConnectivity.Union(parent, size, 1, 2);
            Assert.True(IncrementalConnectivity.Connected(parent, 0, 2));
        }

        [Fact]
        public void OfflineDynamicMst_Rollback()
        {
            const int N = 10;
            int* parent = stackalloc int[N];
            int* size = stackalloc int[N];
            RollbackOp* history = stackalloc RollbackOp[100];
            int historyCount = 0;
            
            OfflineDynamicMst.Init(parent, size, N);
            OfflineDynamicMst.Union(parent, size, 0, 1, history, ref historyCount);
            OfflineDynamicMst.Union(parent, size, 1, 2, history, ref historyCount);
            
            Assert.Equal(OfflineDynamicMst.Find(parent, 0), OfflineDynamicMst.Find(parent, 2));
            
            OfflineDynamicMst.Rollback(parent, size, history, ref historyCount, 1);
            Assert.NotEqual(OfflineDynamicMst.Find(parent, 0), OfflineDynamicMst.Find(parent, 2));
            Assert.Equal(OfflineDynamicMst.Find(parent, 0), OfflineDynamicMst.Find(parent, 1));
        }

        [Fact]
        public void DynamicTransitiveClosure_Basic()
        {
            const int N = 3;
            byte* reach = stackalloc byte[N * N];
            DynamicTransitiveClosure.Init(reach, N);
            
            Assert.True(DynamicTransitiveClosure.CanReach(reach, N, 0, 0));
            Assert.False(DynamicTransitiveClosure.CanReach(reach, N, 0, 1));
            
            DynamicTransitiveClosure.AddEdge(reach, N, 0, 1);
            DynamicTransitiveClosure.AddEdge(reach, N, 1, 2);
            
            Assert.True(DynamicTransitiveClosure.CanReach(reach, N, 0, 2));
            Assert.False(DynamicTransitiveClosure.CanReach(reach, N, 2, 0));
        }
    }
}
