namespace IAFahim.DS.Dsu.Tests
{
    using IAFahim.DS.Dsu;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class DsuRollbackTests
    {
        [Test]
        public void Snapshot_EmptyHistory_ReturnsZero()
        {
            int* history = stackalloc int[10];
            int snap = DsuRollbackSnapshot.Run(history, 0);
            Assert.AreEqual(0, snap);
        }

        [Test]
        public void Snapshot_NonEmptyHistory_ReturnsCurrentSize()
        {
            int* history = stackalloc int[10];
            int* histSize = stackalloc int[1];
            *histSize = 3;
            int snap = DsuRollbackSnapshot.Run(history, *histSize);
            Assert.AreEqual(3, snap);
        }

        [Test]
        public void Snapshot_PartialHistory_ReturnsPartialSize()
        {
            int* history = stackalloc int[10];
            int snap = DsuRollbackSnapshot.Run(history, 5);
            Assert.AreEqual(5, snap);
        }

        [Test]
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
            Assert.IsTrue(DsuSame.Run(parent, 0, 1));

            int snap = DsuRollbackSnapshot.Run(history, *histSize);
            Assert.AreEqual(3, snap);

            DsuRollback.Run(parent, size, history, 0, histSize);
            Assert.IsFalse(DsuSame.Run(parent, 0, 1));
            Assert.AreEqual(1, DsuSize.Run(parent, size, 0));
        }

        [Test]
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
            Assert.IsTrue(DsuSame.Run(parent, 0, 3));
            Assert.AreEqual(4, DsuSize.Run(parent, size, 0));

            int snap = DsuRollbackSnapshot.Run(history, *histSize);
            Assert.AreEqual(9, snap);

            DsuRollback.Run(parent, size, history, 0, histSize);
            Assert.IsFalse(DsuSame.Run(parent, 0, 1));
            Assert.IsFalse(DsuSame.Run(parent, 2, 3));
            Assert.AreEqual(1, DsuSize.Run(parent, size, 0));
            Assert.AreEqual(1, DsuSize.Run(parent, size, 2));
        }

        [Test]
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
            Assert.IsTrue(DsuSame.Run(parent, 0, 1));
        }

        [Test]
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

            Assert.IsTrue(DsuSame.Run(parent, 0, 3));
            Assert.AreEqual(4, DsuSize.Run(parent, size, 0));

            DsuRollback.Run(parent, size, history, snap2, histSize);
            Assert.IsTrue(DsuSame.Run(parent, 0, 1));
            Assert.IsTrue(DsuSame.Run(parent, 2, 3));
            Assert.IsFalse(DsuSame.Run(parent, 0, 2));
        }

        [Test]
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

            Assert.IsTrue(DsuSame.Run(parent, 2, 4));
            Assert.IsTrue(DsuSame.Run(parent, 5, 7));

            DsuRollback.Run(parent, size, history, snap2, histSize);
            Assert.IsTrue(DsuSame.Run(parent, 2, 4));
            Assert.IsFalse(DsuSame.Run(parent, 5, 6));

            DsuRollback.Run(parent, size, history, snap1, histSize);
            Assert.IsTrue(DsuSame.Run(parent, 0, 1));
            Assert.IsFalse(DsuSame.Run(parent, 2, 3));

            DsuRollback.Run(parent, size, history, 0, histSize);
            Assert.IsFalse(DsuSame.Run(parent, 0, 1));
        }

        [Test]
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
            Assert.AreEqual(1, DsuSize.Run(parent, size, 0));
            Assert.AreEqual(1, DsuSize.Run(parent, size, 2));
            Assert.AreEqual(1, DsuSize.Run(parent, size, 3));
        }

        [Ignore("Broken by AI - DsuUndo was removed")]
        [Test]
        public void Undo_AfterUnion_ReversesLast()
        {
            const int n = 4;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            DsuInit.Run(parent, size, n);

            DsuUndo.Run(parent, size, 0, 1);
            Assert.IsFalse(DsuSame.Run(parent, 0, 1));
        }

        [Test]
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

            Assert.IsTrue(r1);
            Assert.IsTrue(r2);
            Assert.IsTrue(r3);
        }

        [Test]
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

            Assert.IsTrue(r1);
            Assert.IsTrue(r2);
            Assert.IsFalse(r3);
        }

        [Test]
        public void ParityFind_AfterParityUnion_CorrectParity()
        {
            const int n = 3;
            int* parent = stackalloc int[n];
            int* parity = stackalloc int[n];
            DsuInit.Run(parent, parity, n);
            for (int i = 0; i < n; i++) parity[i] = 0;

            DsuParityUnion.Run(parent, parity, 0, 1, 1);
            DsuParityFind.Run(parent, parity, 0);
            DsuParityFind.Run(parent, parity, 1);
            Assert.IsTrue((parity[0] ^ parity[1]) == 1);

            DsuParityUnion.Run(parent, parity, 1, 2, 1);
            DsuParityFind.Run(parent, parity, 0);
            DsuParityFind.Run(parent, parity, 2);
            Assert.IsTrue((parity[0] ^ parity[2]) == 0);
        }

        [Test]
        public void ParityUnion_InconsistentParity_ReturnsFalse()
        {
            const int n = 3;
            int* parent = stackalloc int[n];
            int* parity = stackalloc int[n];
            DsuInit.Run(parent, parity, n);
            for (int i = 0; i < n; i++) parity[i] = 0;

            DsuParityUnion.Run(parent, parity, 0, 1, 1);
            bool r = DsuParityUnion.Run(parent, parity, 0, 1, 0);
            Assert.IsFalse(r);
        }

        [Test]
        public void SmallToLargeMerge_InitializesAllNegative()
        {
            const int n = 5;
            int* parent = stackalloc int[n];
            int* heavy = stackalloc int[n];
            SmallToLargeMerge.Run(parent, heavy, n);
            for (int i = 0; i < n; i++)
                Assert.AreEqual(-1, heavy[i]);
        }
    }
}