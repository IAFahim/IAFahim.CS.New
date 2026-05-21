namespace IAFahim.DS.Dsu.Tests
{
    using IAFahim.DS.Dsu;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class DsuRollbackTests
    {
        [Fact]
        public void Snapshot_EmptyHistory_ReturnsZero()
        {
            int* history = stackalloc int[10];
            int snap = DsuRollbackSnapshot.Run(history, 0);
            Assert.Equal(0, snap);
        }

        [Fact]
        public void Snapshot_NonEmptyHistory_ReturnsCurrentSize()
        {
            int* history = stackalloc int[10];
            int* histSize = stackalloc int[1];
            *histSize = 3;
            int snap = DsuRollbackSnapshot.Run(history, *histSize);
            Assert.Equal(3, snap);
        }

        [Fact]
        public void Snapshot_PartialHistory_ReturnsPartialSize()
        {
            int* history = stackalloc int[10];
            int snap = DsuRollbackSnapshot.Run(history, 5);
            Assert.Equal(5, snap);
        }

        [Fact]
        public void Rollback_AfterSingleUnion_RestoresState()
        {
            const int n = 5;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            int* history = stackalloc int[n * 3];
            int* histSize = stackalloc int[1];
            *histSize = 0;
            DsuInit.Run(parent, size, n);

            DsuRollbackUnion.Run(parent, size, history, histSize, 0, 1);
            Assert.True(DsuSame.Run(parent, 0, 1));

            int snap = DsuRollbackSnapshot.Run(history, *histSize);
            Assert.Equal(3, snap);

            DsuRollback.Run(parent, size, history, 0, histSize);
            Assert.False(DsuSame.Run(parent, 0, 1));
            Assert.Equal(1, DsuSize.Run(parent, size, 0));
        }

        [Fact]
        public void Rollback_AfterMultipleUnions_RestoresCorrectly()
        {
            const int n = 6;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            int* history = stackalloc int[n * 3];
            int* histSize = stackalloc int[1];
            *histSize = 0;
            DsuInit.Run(parent, size, n);

            DsuRollbackUnion.Run(parent, size, history, histSize, 0, 1);
            DsuRollbackUnion.Run(parent, size, history, histSize, 2, 3);
            DsuRollbackUnion.Run(parent, size, history, histSize, 0, 2);
            Assert.True(DsuSame.Run(parent, 0, 3));
            Assert.Equal(4, DsuSize.Run(parent, size, 0));

            int snap = DsuRollbackSnapshot.Run(history, *histSize);
            Assert.Equal(9, snap);

            DsuRollback.Run(parent, size, history, 0, histSize);
            Assert.False(DsuSame.Run(parent, 0, 1));
            Assert.False(DsuSame.Run(parent, 2, 3));
            Assert.Equal(1, DsuSize.Run(parent, size, 0));
            Assert.Equal(1, DsuSize.Run(parent, size, 2));
        }

        [Fact]
        public void Rollback_TargetSameAsCurrent_NoChange()
        {
            const int n = 3;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            int* history = stackalloc int[n * 3];
            int* histSize = stackalloc int[1];
            *histSize = 0;
            DsuInit.Run(parent, size, n);

            DsuRollbackUnion.Run(parent, size, history, histSize, 0, 1);
            int target = *histSize;

            DsuRollback.Run(parent, size, history, target, histSize);
            Assert.True(DsuSame.Run(parent, 0, 1));
        }

        [Fact]
        public void Rollback_PartialRollback_KeepsSomeChanges()
        {
            const int n = 5;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            int* history = stackalloc int[n * 3];
            int* histSize = stackalloc int[1];
            *histSize = 0;
            DsuInit.Run(parent, size, n);

            DsuRollbackUnion.Run(parent, size, history, histSize, 0, 1);
            int snap1 = DsuRollbackSnapshot.Run(history, *histSize);
            DsuRollbackUnion.Run(parent, size, history, histSize, 2, 3);
            int snap2 = DsuRollbackSnapshot.Run(history, *histSize);
            DsuRollbackUnion.Run(parent, size, history, histSize, 0, 2);

            Assert.True(DsuSame.Run(parent, 0, 3));
            Assert.Equal(4, DsuSize.Run(parent, size, 0));

            DsuRollback.Run(parent, size, history, snap2, histSize);
            Assert.True(DsuSame.Run(parent, 0, 1));
            Assert.True(DsuSame.Run(parent, 2, 3));
            Assert.False(DsuSame.Run(parent, 0, 2));
        }

        [Fact]
        public void Rollback_AllowsMultipleSavepoints_IndependentRestores()
        {
            const int n = 8;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            int* history = stackalloc int[n * 3];
            int* histSize = stackalloc int[1];
            *histSize = 0;
            DsuInit.Run(parent, size, n);

            DsuRollbackUnion.Run(parent, size, history, histSize, 0, 1);
            int snap1 = DsuRollbackSnapshot.Run(history, *histSize);

            DsuRollbackUnion.Run(parent, size, history, histSize, 2, 3);
            DsuRollbackUnion.Run(parent, size, history, histSize, 3, 4);
            int snap2 = DsuRollbackSnapshot.Run(history, *histSize);

            DsuRollbackUnion.Run(parent, size, history, histSize, 5, 6);
            DsuRollbackUnion.Run(parent, size, history, histSize, 6, 7);
            int snap3 = DsuRollbackSnapshot.Run(history, *histSize);

            Assert.True(DsuSame.Run(parent, 0, 4));
            Assert.True(DsuSame.Run(parent, 5, 7));

            DsuRollback.Run(parent, size, history, snap2, histSize);
            Assert.True(DsuSame.Run(parent, 0, 4));
            Assert.False(DsuSame.Run(parent, 5, 6));

            DsuRollback.Run(parent, size, history, snap1, histSize);
            Assert.False(DsuSame.Run(parent, 0, 1));
            Assert.False(DsuSame.Run(parent, 2, 3));

            DsuRollback.Run(parent, size, history, snap3, histSize);
            Assert.True(DsuSame.Run(parent, 5, 7));
        }

        [Fact]
        public void Rollback_DuplicateUnions_CountsCorrectly()
        {
            const int n = 4;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            int* history = stackalloc int[n * 3];
            int* histSize = stackalloc int[1];
            *histSize = 0;
            DsuInit.Run(parent, size, n);

            DsuRollbackUnion.Run(parent, size, history, histSize, 0, 1);
            DsuRollbackUnion.Run(parent, size, history, histSize, 1, 2);
            DsuRollbackUnion.Run(parent, size, history, histSize, 2, 3);

            DsuRollback.Run(parent, size, history, 0, histSize);
            Assert.Equal(1, DsuSize.Run(parent, size, 0));
            Assert.Equal(1, DsuSize.Run(parent, size, 2));
            Assert.Equal(1, DsuSize.Run(parent, size, 3));
        }

        [Fact]
        public void Undo_AfterUnion_ReversesLast()
        {
            const int n = 4;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            DsuInit.Run(parent, size, n);

            DsuUndo.Run(parent, size, 0, 1);
            Assert.False(DsuSame.Run(parent, 0, 1));
        }

        [Fact]
        public void BipartiteAdd_SameParityConstraint_KeepsGraphBipartite()
        {
            const int n = 4;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            int* parity = stackalloc int[n];
            int* history = stackalloc int[n * 6];
            int* histSize = stackalloc int[1];
            *histSize = 0;
            DsuInit.Run(parent, size, n);
            for (int i = 0; i < n; i++) parity[i] = 0;

            bool r1 = DsuBipartiteAdd.Run(parent, parity, size, history, histSize, 0, 1);
            bool r2 = DsuBipartiteAdd.Run(parent, parity, size, history, histSize, 1, 2);
            bool r3 = DsuBipartiteAdd.Run(parent, parity, size, history, histSize, 2, 3);

            Assert.True(r1);
            Assert.True(r2);
            Assert.True(r3);
        }

        [Fact]
        public void BipartiteAdd_OddCycle_ReturnsFalse()
        {
            const int n = 3;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            int* parity = stackalloc int[n];
            int* history = stackalloc int[n * 6];
            int* histSize = stackalloc int[1];
            *histSize = 0;
            DsuInit.Run(parent, size, n);
            for (int i = 0; i < n; i++) parity[i] = 0;

            bool r1 = DsuBipartiteAdd.Run(parent, parity, size, history, histSize, 0, 1);
            bool r2 = DsuBipartiteAdd.Run(parent, parity, size, history, histSize, 1, 2);
            bool r3 = DsuBipartiteAdd.Run(parent, parity, size, history, histSize, 2, 0);

            Assert.True(r1);
            Assert.True(r2);
            Assert.False(r3);
        }

        [Fact]
        public void ParityFind_AfterParityUnion_CorrectParity()
        {
            const int n = 3;
            int* parent = stackalloc int[n];
            int* parity = stackalloc int[n];
            DsuInit.Run(parent, parity, n);
            for (int i = 0; i < n; i++) parity[i] = 0;

            DsuParityUnion.Run(parent, parity, 0, 1, 1);
            Assert.True(DsuParityFind.Run(parent, parity, 0) != DsuParityFind.Run(parent, parity, 1));

            DsuParityUnion.Run(parent, parity, 1, 2, 1);
            Assert.True(DsuParityFind.Run(parent, parity, 0) == DsuParityFind.Run(parent, parity, 2));
        }

        [Fact]
        public void ParityUnion_InconsistentParity_ReturnsFalse()
        {
            const int n = 3;
            int* parent = stackalloc int[n];
            int* parity = stackalloc int[n];
            DsuInit.Run(parent, parity, n);
            for (int i = 0; i < n; i++) parity[i] = 0;

            DsuParityUnion.Run(parent, parity, 0, 1, 1);
            bool r = DsuParityUnion.Run(parent, parity, 0, 1, 1);
            Assert.False(r);
        }

        [Fact]
        public void SmallToLargeMerge_InitializesAllNegative()
        {
            const int n = 5;
            int* parent = stackalloc int[n];
            int* heavy = stackalloc int[n];
            SmallToLargeMerge.Run(parent, heavy, n);
            for (int i = 0; i < n; i++)
                Assert.Equal(-1, heavy[i]);
        }
    }
}