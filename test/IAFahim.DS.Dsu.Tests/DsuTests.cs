namespace IAFahim.DS.Dsu.Tests
{
    using IAFahim.DS.Dsu;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class DsuTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            fixed (int* parent = new int[0])
            fixed (int* size = new int[0])
            {
                DsuInit.Run(0, parent, size);
                Assert.True(true);
            }
        }

        [Fact]
        public void SingleElement_ParentIsSelf()
        {
            int* parent = stackalloc int[1];
            int* size = stackalloc int[1];
            DsuInit.Run(1, parent, size);
            Assert.Equal(0, DsuFind.Run(0, parent));
            Assert.Equal(1, DsuSize.Run(0, size));
        }

        [Fact]
        public void UnionFind_Basic()
        {
            const int n = 5;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            DsuInit.Run(n, parent, size);

            DsuUnion.Run(0, 1, parent, size);
            DsuUnion.Run(1, 2, parent, size);
            Assert.True(DsuSame.Run(0, 2, parent));
            Assert.Equal(3, DsuSize.Run(0, size));

            Assert.False(DsuSame.Run(0, 3, parent));
            Assert.False(DsuSame.Run(2, 4, parent));

            DsuUnion.Run(3, 4, parent, size);
            Assert.True(DsuSame.Run(3, 4, parent));

            DsuUnion.Run(2, 3, parent, size);
            Assert.True(DsuSame.Run(0, 4, parent));
            Assert.Equal(5, DsuSize.Run(0, size));
        }

        [Fact]
        public void SelfLoop_NoChange()
        {
            int* parent = stackalloc int[3];
            int* size = stackalloc int[3];
            DsuInit.Run(3, parent, size);
            DsuUnion.Run(0, 0, parent, size);
            Assert.Equal(0, DsuFind.Run(0, parent));
        }

        [Fact]
        public void DsuParity_Basic()
        {
            const int n = 4;
            int* parent = stackalloc int[n];
            int* diff = stackalloc int[n];
            DsuParity.DsuInit.Run(n, parent, diff);

            DsuParity.Union.Run(0, 1, 1, parent, diff);
            DsuParity.Union.Run(1, 2, 1, parent, diff);
            DsuParity.Union.Run(2, 3, 1, parent, diff);

            Assert.Equal(1, DsuParity.Find.Run(0, parent, diff) ^ DsuParity.Find.Run(3, parent, diff));
            Assert.Equal(0, DsuParity.Find.Run(0, parent, diff) ^ DsuParity.Find.Run(2, parent, diff));
        }

        [Fact]
        public void DsuBipartite_Basic()
        {
            const int n = 3;
            int* parent = stackalloc int[n];
            bool* bipartite = stackalloc bool[n];
            DsuBipartite.DsuInit.Run(n, parent, bipartite);

            Assert.True(DsuBipartite.Add.Run(0, 1, parent, bipartite));
            Assert.True(DsuBipartite.Add.Run(1, 2, parent, bipartite));

            Assert.False(DsuBipartite.Add.Run(0, 2, parent, bipartite));
        }

        [Fact]
        public void DsuRollback_SnapshotRestore()
        {
            const int n = 5;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            DsuInit.Run(n, parent, size);

            long snap1 = DsuRollback.Snapshot.Run(parent);
            DsuUnion.Run(0, 1, parent, size);
            DsuUnion.Run(2, 3, parent, size);

            long snap2 = DsuRollback.Snapshot.Run(parent);
            DsuUnion.Run(0, 2, parent, size);

            Assert.True(DsuSame.Run(0, 3, parent));

            DsuRollback.Run(snap2, parent, size);
            Assert.False(DsuSame.Run(0, 2, parent));
            Assert.True(DsuSame.Run(0, 1, parent));

            DsuRollback.Run(snap1, parent, size);
            Assert.False(DsuSame.Run(0, 1, parent));
        }
    }
}