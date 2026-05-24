namespace IAFahim.DS.SegmentTree.Tests
{
    using IAFahim.DS.SegmentTree;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class SegmentTreeTests
    {
        [Test]
        public void EmptyInput_NoOp()
        {
            int* seg = stackalloc int[4];
            for (int i = 0; i < 4; i++) seg[i] = 0;
            Assert.AreEqual(0, seg[0]);
        }

        [Test]
        public void BuildAndQuery_SingleElement()
        {
            const int n = 1;
            int* arr = stackalloc int[n];
            int* seg = stackalloc int[n * 4];
            arr[0] = 42;
            SegmentTreeBuild.RunInt32(arr, seg, 1, 0, n - 1);
            Assert.AreEqual(42, SegmentTreeQuery.RunInt32(seg, 1, 0, n - 1, 0, n - 1));
        }

        [Test]
        public void PointUpdate_AndQuery()
        {
            const int n = 8;
            int* arr = stackalloc int[n];
            int* seg = stackalloc int[n * 4];
            for (int i = 0; i < n; i++) arr[i] = i;
            SegmentTreeBuild.RunInt32(arr, seg, 1, 0, n - 1);
            SegmentTreeSet.RunInt32(seg, 1, 0, n - 1, 3, 100);
            Assert.AreEqual(100, SegmentTreeQuery.RunInt32(seg, 1, 0, n - 1, 3, 3));
            Assert.AreEqual(106, SegmentTreeQuery.RunInt32(seg, 1, 0, n - 1, 2, 4));
        }

        [Test]
        public void RangeQuery_AllElements()
        {
            const int n = 10;
            int* arr = stackalloc int[n];
            int* seg = stackalloc int[n * 4];
            for (int i = 0; i < n; i++) arr[i] = i + 1;
            SegmentTreeBuild.RunInt32(arr, seg, 1, 0, n - 1);
            Assert.AreEqual(55, SegmentTreeQuery.RunInt32(seg, 1, 0, n - 1, 0, n - 1));
        }

        [Test]
        public void LazySegment_RangeAdd()
        {
            const int n = 8;
            int* arr = stackalloc int[n];
            int* seg = stackalloc int[n * 4 + 2];
            int* lazy = stackalloc int[n * 4 + 2];
            for (int i = 0; i < n; i++) arr[i] = 1;
            for (int i = 0; i < n * 4 + 2; i++) { seg[i] = 0; lazy[i] = 0; }
            LazySegmentBuild.RunInt32(arr, seg, 1, 0, n - 1);
            LazySegmentUpdate.RangeAddInt32(seg, lazy, 1, 0, n - 1, 0, 3, 5);
            int q = LazySegmentQuery.RangeSumInt32(seg, lazy, 1, 0, n - 1, 0, 3);
            Assert.AreEqual(24, q);
        }

        [Test]
        public void PersistentSegment_Basic()
        {
            const int n = 10;
            int* arr = stackalloc int[n];
            for (int i = 0; i < n; i++) arr[i] = i;

            int maxNodes = n * 40;
            int* lc = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int* rc = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int* tree = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));

            try
            {
                for (int i = 0; i < maxNodes; i++) { lc[i] = rc[i] = tree[i] = 0; }
                int* roots = stackalloc int[n + 2];
                roots[0] = PersistentSegmentBuild.RunInt32(arr, roots, lc, rc, tree, 0, 0, n - 1);
                roots[1] = PersistentSegmentUpdate.RunInt32(tree, lc, rc, roots[0], 0, n - 1, 5, 100);

                int orig = PersistentSegmentQuery.RunInt32(tree, lc, rc, roots[0], 0, n - 1, 5, 5);
                int updated = PersistentSegmentQuery.RunInt32(tree, lc, rc, roots[1], 0, n - 1, 5, 5);
                Assert.AreEqual(5, orig);
                Assert.AreEqual(100, updated);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)lc);
                Marshal.FreeHGlobal((nint)rc);
                Marshal.FreeHGlobal((nint)tree);
            }
        }

        [Test]
        public void DualSegment_Basic()
        {
            const int n = 5;
            int* seg = stackalloc int[n + 1];
            for (int i = 0; i <= n; i++) seg[i] = 0;
            DualSegmentApply.RangeAddInt32(seg, 1, 10);
            Assert.AreEqual(10, seg[1]);
        }
    }
}