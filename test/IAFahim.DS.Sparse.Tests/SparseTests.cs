namespace IAFahim.DS.Sparse.Tests
{
    using IAFahim.DS.Sparse;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class SparseTests
    {
        [Fact]
        public void SparseTable_BuildAndQuery_RangeMin()
        {
            int* arr = stackalloc int[] { 5, 2, 8, 1, 9, 3 };
            const int n = 6, logN = 3;
            int* log = stackalloc int[n + 1];
            log[1] = 0;
            for (int i = 2; i <= n; i++) log[i] = log[i / 2] + 1;
            int* st = stackalloc int[n * (logN + 1)];
            SparseTableBuild.RunInt32(arr, st, log, n);
            Assert.Equal(1, SparseTableQuery.MinInt32(st, log, 0, 5, n));
            Assert.Equal(1, SparseTableQuery.MinInt32(st, log, 1, 5, n));
            Assert.Equal(1, SparseTableQuery.MinInt32(st, log, 3, 3, n));
        }

        [Fact]
        public void SqrtDecompose_Basic()
        {
            const int n = 16;
            int* arr = stackalloc int[n];
            int* block = stackalloc int[n];
            int blockSize = 0;
            for (int i = 0; i < n; i++) arr[i] = i;
            SqrtDecomposeBuild.Run(arr, block, &blockSize, n);
            int res = SqrtQuery.RangeMin(arr, block, blockSize, 0, 5, n);
            Assert.Equal(0, res);
        }

        [Fact]
        public void DisjointSparse_Basic()
        {
            long* arr = stackalloc long[] { 3, 1, 4, 1, 5, 9, 2, 6 };
            const int n = 8;
            int* blockSize = stackalloc int[1];
            long* st = stackalloc long[n * 4];
            DisjointSparseBuild.RunInt64(arr, st, blockSize, n);
            Assert.Equal(1, DisjointSparseQuery.RangeMinInt64(st, blockSize, 0, 3));
        }
    }
}
