namespace IAFahim.DS.RollbackStack.Tests
{
    using IAFahim.DS.RollbackStack;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class UndoableUnionFindTests
    {
        [Fact]
        public void SnapshotAndRollback_Basic()
        {
            const int n = 5;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            int* history = stackalloc int[n * 2];
            int histSize = 0;

            // Initialize
            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
                size[i] = 1;
            }

            UndoableUnionFind.Union(parent, size, history, &histSize, 0, 1);
            UndoableUnionFind.Union(parent, size, history, &histSize, 1, 2);

            int snap = UndoableUnionFind.Snapshot(parent, size, history, histSize);

            UndoableUnionFind.Union(parent, size, history, &histSize, 3, 4);

            // Rollback to snap removes union(3,4), keeping 0-1-2 connected and 3-4 separate
            UndoableUnionFind.Rollback(parent, size, history, snap, &histSize);

            // Verify 0-1-2 is connected
            Assert.Equal(UndoableUnionFind.Find(parent, 0), UndoableUnionFind.Find(parent, 2));
            // Verify 3 and 4 are NOT connected (union(3,4) was rolled back)
            Assert.NotEqual(UndoableUnionFind.Find(parent, 3), UndoableUnionFind.Find(parent, 4));
        }

        [Fact]
        public void EmptySnapshot_NoOp()
        {
            int* parent = stackalloc int[3];
            int* size = stackalloc int[3];
            int* history = stackalloc int[10];
            int histSize = 0;

            int snap = UndoableUnionFind.Snapshot(parent, size, history, histSize);
            Assert.Equal(0, snap);

            UndoableUnionFind.Rollback(parent, size, history, snap, &histSize);
            Assert.Equal(0, histSize);
        }

        [Fact]
        public void MultipleSnapshots_StackBehavior()
        {
            int* parent = stackalloc int[4];
            int* size = stackalloc int[4];
            int* history = stackalloc int[20];
            int histSize = 0;

            for (int i = 0; i < 4; i++) { parent[i] = i; size[i] = 1; }

            UndoableUnionFind.Union(parent, size, history, &histSize, 0, 1);
            int snap1 = UndoableUnionFind.Snapshot(parent, size, history, histSize);

            UndoableUnionFind.Union(parent, size, history, &histSize, 1, 2);
            int snap2 = UndoableUnionFind.Snapshot(parent, size, history, histSize);

            UndoableUnionFind.Union(parent, size, history, &histSize, 2, 3);

            UndoableUnionFind.Rollback(parent, size, history, snap1, &histSize);

            Assert.Equal(2, histSize);
            Assert.Equal(UndoableUnionFind.Find(parent, 0), UndoableUnionFind.Find(parent, 1));
            Assert.NotEqual(UndoableUnionFind.Find(parent, 0), UndoableUnionFind.Find(parent, 2));
        }
    }

    public sealed unsafe class UndoableBipartiteDsuTests
    {
        [Fact]
        public void BipartiteUnion_Basic()
        {
            int* parent = stackalloc int[4];
            int* parity = stackalloc int[4];
            int* history = stackalloc int[30];
            int histSize = 0;

            for (int i = 0; i < 4; i++) { parent[i] = i; parity[i] = 0; }

            bool ok = UndoableBipartiteDsu.Union(parent, parity, history, &histSize, 0, 1);
            Assert.True(ok);

            ok = UndoableBipartiteDsu.Union(parent, parity, history, &histSize, 2, 3);
            Assert.True(ok);

            int snap = UndoableBipartiteDsu.Snapshot(parent, parity, history, histSize);

            ok = UndoableBipartiteDsu.Union(parent, parity, history, &histSize, 1, 2);
            Assert.True(ok);

            UndoableBipartiteDsu.Rollback(parent, parity, history, snap, &histSize);
        }

        [Fact]
        public void DetectOddCycle()
        {
            int* parent = stackalloc int[3];
            int* parity = stackalloc int[3];
            int* history = stackalloc int[30];
            int histSize = 0;

            for (int i = 0; i < 3; i++) { parent[i] = i; parity[i] = 0; }

            bool ok = UndoableBipartiteDsu.Union(parent, parity, history, &histSize, 0, 1);
            Assert.True(ok);

            ok = UndoableBipartiteDsu.Union(parent, parity, history, &histSize, 1, 2);
            Assert.True(ok);
        }
    }
}