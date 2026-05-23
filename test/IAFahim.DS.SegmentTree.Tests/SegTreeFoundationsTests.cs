namespace IAFahim.DS.SegmentTree.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class SegTreeFoundationsTests
    {
        [Test]
        public void PersistentLazySeg_EmptyInput_NoOp()
        {
            int* lc = stackalloc int[100];
            int* rc = stackalloc int[100];
            long* sum = stackalloc long[100];
            long* lazy = stackalloc long[100];
            int* cnt = stackalloc int[100];
            int alloc = 0;
            for (int i = 0; i < 100; i++) { lc[i] = rc[i] = 0; sum[i] = lazy[i] = 0; cnt[i] = 0; }
            long result = PersistentLazySegmentQuery.Run(0, 0, 9, 0, 9, 0, lc, rc, sum, lazy, cnt);
            Assert.AreEqual(0L, result);
        }



        [Test]
        public void ChairmanTree_Kth()
        {
            const int N = 5;
            int* arr = stackalloc int[N];
            arr[0] = 3; arr[1] = 1; arr[2] = 4; arr[3] = 1; arr[4] = 5;

            int maxNodes = N * 40;
            int* roots = (int*)Marshal.AllocHGlobal((N + 1) * sizeof(int));
            int* lc = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int* rc = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int* cnt = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            long* sumArr = (long*)Marshal.AllocHGlobal(maxNodes * sizeof(long));
            int alloc = 0;

            try
            {
                for (int i = 0; i < maxNodes; i++) { lc[i] = rc[i] = cnt[i] = 0; sumArr[i] = 0; }
                for (int i = 0; i <= N; i++) roots[i] = 0;

                ChairmanTreeBuild.Run(arr, N, 1, 5, roots, lc, rc, cnt, sumArr, &alloc);

                int kth = ChairmanTreeKth.Run(roots[0], roots[N], 1, 5, 2, lc, rc, cnt);
                Assert.IsTrue(kth >= 1 && kth <= 5);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)roots);
                Marshal.FreeHGlobal((nint)lc);
                Marshal.FreeHGlobal((nint)rc);
                Marshal.FreeHGlobal((nint)cnt);
                Marshal.FreeHGlobal((nint)sumArr);
            }
        }

        [Test]
        public void MergeableSegTree_UpdateAndQuery()
        {
            int maxNodes = 200;
            int* lc = stackalloc int[maxNodes];
            int* rc = stackalloc int[maxNodes];
            int* sum = stackalloc int[maxNodes];
            int alloc = 0;
            for (int i = 0; i < maxNodes; i++) { lc[i] = rc[i] = sum[i] = 0; }

            int root1 = 0;
            int root2 = 0;
            MergeableSegmentTreeUpdate.Run(&root1, 0, 99, 10, 1, lc, rc, sum, &alloc);
            MergeableSegmentTreeUpdate.Run(&root2, 0, 99, 10, 2, lc, rc, sum, &alloc);

            int merged = MergeableSegmentTreeMerge.Run(root1, root2, lc, rc, sum, &alloc);
            int q = MergeableSegmentTreeQuery.Run(merged, 0, 99, 10, 10, lc, rc, sum);
            Assert.AreEqual(3, q);
        }

        [Test]
        public void MergeSortTree_CountLess()
        {
            const int N = 5;
            int* arr = stackalloc int[N];
            arr[0] = 3; arr[1] = 1; arr[2] = 4; arr[3] = 1; arr[4] = 5;

            int poolCap = N * 20;
            int* pool = (int*)Marshal.AllocHGlobal(poolCap * sizeof(int));
            int** nodes = (int**)Marshal.AllocHGlobal(4 * N * sizeof(nint));
            int* sizes = stackalloc int[4 * N];
            int poolUsed = 0;

            try
            {
                for (int i = 0; i < poolCap; i++) pool[i] = 0;
                for (int i = 0; i < 4 * N; i++) { nodes[i] = null; sizes[i] = 0; }

                MergeSortTreeBuild.Run(arr, N, nodes, sizes, pool, ref poolUsed);
                int cnt = MergeSortTreeCountLess.Run(N, 0, N - 1, 4, nodes, sizes);
                Assert.AreEqual(3, cnt);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)pool);
                Marshal.FreeHGlobal((nint)nodes);
            }
        }

        [Test]
        public void SegTreeOnTreeMerge_Basic()
        {
            int maxNodes = 200;
            int* lc = stackalloc int[maxNodes];
            int* rc = stackalloc int[maxNodes];
            int* sum = stackalloc int[maxNodes];
            int alloc = 0;
            for (int i = 0; i < maxNodes; i++) { lc[i] = rc[i] = sum[i] = 0; }

            int r1 = 0;
            int r2 = 0;
            MergeableSegmentTreeUpdate.Run(&r1, 0, 99, 5, 1, lc, rc, sum, &alloc);
            MergeableSegmentTreeUpdate.Run(&r2, 0, 99, 5, 2, lc, rc, sum, &alloc);

            int merged = SegmentTreeOnTreeMerge.Run(r1, r2, 0, 99, lc, rc, sum, &alloc);
            int q = MergeableSegmentTreeQuery.Run(merged, 0, 99, 5, 5, lc, rc, sum);
            Assert.AreEqual(3, q);
        }
    }
}
