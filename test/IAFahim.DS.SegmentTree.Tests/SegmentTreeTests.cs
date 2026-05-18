namespace IAFahim.DS.SegmentTree.Tests
{
    using IAFahim.DS.SegmentTree;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class SegmentTreeTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            int* seg = stackalloc int[1];
            long* lazy = stackalloc long[1];
            SegmentTreeBuild.Run<int>(null, 0, seg);
            Assert.Equal(0, seg[0]);
        }

        [Fact]
        public void BuildAndQuery_SingleElement()
        {
            const int n = 1;
            int* arr = stackalloc int[n];
            int* seg = stackalloc int[n * 4];
            arr[0] = 42;
            SegmentTreeBuild.Run(arr, n, seg);
            Assert.Equal(42, SegmentTreeQuery.Run(seg, 0, 0, n - 1, 0, n - 1));
        }

        [Fact]
        public void PointUpdate_AndQuery()
        {
            const int n = 8;
            int* arr = stackalloc int[n];
            int* seg = stackalloc int[n * 4];
            for (int i = 0; i < n; i++) arr[i] = i;
            SegmentTreeBuild.Run(arr, n, seg);

            SegmentTreeSet.Run(seg, 0, 0, n - 1, 3, 100, 0);
            Assert.Equal(100, SegmentTreeQuery.Run(seg, 0, 0, n - 1, 3, 3));
            Assert.Equal(106, SegmentTreeQuery.Run(seg, 0, 0, n - 1, 2, 4));
        }

        [Fact]
        public void RangeQuery_AllElements()
        {
            const int n = 10;
            int* arr = stackalloc int[n];
            int* seg = stackalloc int[n * 4];
            for (int i = 0; i < n; i++) arr[i] = i + 1;
            SegmentTreeBuild.Run(arr, n, seg);
            Assert.Equal(55, SegmentTreeQuery.Run(seg, 0, 0, n - 1, 0, n - 1));
        }

        [Fact]
        public void LazySegment_Basic()
        {
            const int n = 8;
            int* arr = stackalloc int[n];
            int* seg = stackalloc int[n * 4];
            long* lazy = stackalloc long[n * 4];
            for (int i = 0; i < n; i++) arr[i] = 1;
            LazySegmentBuild.Run(arr, n, seg);

            LazySegmentUpdate.Run(seg, lazy, 0, 0, n - 1, 0, 3, 5, 0);
            Assert.Equal(5, seg[0]);
        }

        [Fact]
        public void PersistentSegment_Basic()
        {
            const int n = 10;
            int* arr = stackalloc int[n];
            for (int i = 0; i < n; i++) arr[i] = i;
            int* root = stackalloc int[1];
            *root = PersistentSegmentBuild.Run(arr, n, 0, 0, n - 1);

            int* newRoot = stackalloc int[1];
            *newRoot = PersistentSegmentUpdate.Run(*root, 0, 0, n - 1, 5, 100, 0);
            Assert.Equal(5, PersistentSegmentQuery.Run(*root, 0, 0, n - 1, 5, 5));
            Assert.Equal(100, PersistentSegmentQuery.Run(*newRoot, 0, 0, n - 1, 5, 5));
            Assert.Equal(3, PersistentSegmentQuery.Run(*newRoot, 0, 0, n - 1, 3, 3));
        }

        [Fact]
        public void DualSegment_Basic()
        {
            const int n = 5;
            long* seg = stackalloc long[n];
            long* lazy = stackalloc long[n];
            for (int i = 0; i < n; i++) seg[i] = i;
            DualSegmentApply.Run(seg, lazy, 0, 1, 3);
            Assert.Equal(1, seg[0]);
        }
    }
}