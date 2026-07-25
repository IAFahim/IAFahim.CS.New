namespace IAFahim.DS.RollbackSeg.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class RollbackSegCoreTests
    {
        [Test]
        public void RangeAdd_PartialQuery_MatchesBrute_AndRollback()
        {
            const int N = 16;
            const int TreeSize = 64;
            long* arr = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            long* tree = (long*)Marshal.AllocHGlobal(TreeSize * sizeof(long));
            long* lazy = (long*)Marshal.AllocHGlobal(TreeSize * sizeof(long));
            int* histNode = (int*)Marshal.AllocHGlobal(4096 * sizeof(int));
            long* histVal = (long*)Marshal.AllocHGlobal(4096 * sizeof(long));
            byte* histType = (byte*)Marshal.AllocHGlobal(4096);
            int* top = (int*)Marshal.AllocHGlobal(sizeof(int));
            long* brute = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            try
            {
                for (int i = 0; i < N; i++) { arr[i] = i + 1; brute[i] = i + 1; }
                for (int i = 0; i < TreeSize; i++) { tree[i] = 0; lazy[i] = 0; }
                *top = 0;
                RollbackSegBuild.RunInt64(arr, tree, 1, 0, N - 1);

                int cp0 = RollbackSegRollback.GetCheckpoint(top);
                RollbackSegUpdate.RangeAddInt64(tree, lazy, histNode, histVal, histType, top, 1, 0, N - 1, 2, 7, 10);
                for (int i = 2; i <= 7; i++) brute[i] += 10;

                Assert.AreEqual(BruteSum(brute, 0, N - 1), RollbackSegQuery.RangeSumInt64(tree, lazy, 1, 0, N - 1, 0, N - 1));
                Assert.AreEqual(BruteSum(brute, 3, 5), RollbackSegQuery.RangeSumInt64(tree, lazy, 1, 0, N - 1, 3, 5));
                Assert.AreEqual(BruteSum(brute, 0, 1), RollbackSegQuery.RangeSumInt64(tree, lazy, 1, 0, N - 1, 0, 1));

                int cp1 = RollbackSegRollback.GetCheckpoint(top);
                RollbackSegUpdate.RangeAddInt64(tree, lazy, histNode, histVal, histType, top, 1, 0, N - 1, 0, N - 1, 3);
                for (int i = 0; i < N; i++) brute[i] += 3;
                Assert.AreEqual(BruteSum(brute, 4, 10), RollbackSegQuery.RangeSumInt64(tree, lazy, 1, 0, N - 1, 4, 10));

                RollbackSegRollback.Run(tree, lazy, histNode, histVal, histType, top, cp1);
                for (int i = 0; i < N; i++) brute[i] -= 3;
                Assert.AreEqual(BruteSum(brute, 0, N - 1), RollbackSegQuery.RangeSumInt64(tree, lazy, 1, 0, N - 1, 0, N - 1));

                RollbackSegRollback.Run(tree, lazy, histNode, histVal, histType, top, cp0);
                for (int i = 0; i < N; i++) brute[i] = i + 1;
                Assert.AreEqual(BruteSum(brute, 0, N - 1), RollbackSegQuery.RangeSumInt64(tree, lazy, 1, 0, N - 1, 0, N - 1));
                Assert.AreEqual(0, *top);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)arr);
                Marshal.FreeHGlobal((nint)tree);
                Marshal.FreeHGlobal((nint)lazy);
                Marshal.FreeHGlobal((nint)histNode);
                Marshal.FreeHGlobal((nint)histVal);
                Marshal.FreeHGlobal((nint)histType);
                Marshal.FreeHGlobal((nint)top);
                Marshal.FreeHGlobal((nint)brute);
            }
        }

        [Test]
        public void UndoLast_RestoresOneHistorySlot()
        {
            const int N = 4;
            long* arr = stackalloc long[4] { 1, 2, 3, 4 };
            long* tree = stackalloc long[16];
            long* lazy = stackalloc long[16];
            int* histNode = stackalloc int[64];
            long* histVal = stackalloc long[64];
            byte* histType = stackalloc byte[64];
            int top = 0;
            for (int i = 0; i < 16; i++) { tree[i] = 0; lazy[i] = 0; }
            RollbackSegBuild.RunInt64(arr, tree, 1, 0, N - 1);
            long before = RollbackSegQuery.RangeSumInt64(tree, lazy, 1, 0, N - 1, 0, N - 1);
            RollbackSegUpdate.RangeAddInt64(tree, lazy, histNode, histVal, histType, &top, 1, 0, N - 1, 0, N - 1, 5);
            Assert.AreEqual(before + 5 * N, RollbackSegQuery.RangeSumInt64(tree, lazy, 1, 0, N - 1, 0, N - 1));
            while (top > 0)
                RollbackSegRollback.UndoLast(tree, lazy, histNode, histVal, histType, &top);
            Assert.AreEqual(before, RollbackSegQuery.RangeSumInt64(tree, lazy, 1, 0, N - 1, 0, N - 1));
        }

        private static long BruteSum(long* a, int l, int r)
        {
            long s = 0;
            for (int i = l; i <= r; i++) s += a[i];
            return s;
        }
    }
}
