namespace IAFahim.RedTeam
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;
    using IAFahim.DS.Fenwick;
    using IAFahim.Graph;
    using IAFahim.Graph.Misc;
    using IAFahim.DS.Sparse;
    using IAFahim.DP.General;
    using IAFahim.DS.Trie;
    using IAFahim.Permutation;
    using IAFahim.Search.Bit;
    using IAFahim.DS.Grid;

    public sealed unsafe class BrokenAlgorithmsTests
    {
        [Fact]
        public void FenwickRangeAdd_ReturnsWrongSum()
        {
            const int n = 5;
            long* bit1 = stackalloc long[n + 1];
            long* bit2 = stackalloc long[n + 1];
            for (int i = 0; i <= n; i++) { bit1[i] = 0; bit2[i] = 0; }

            FenwickRangeAdd.RangeAdd(bit1, bit2, n, 1, 3, 10);

            long result = FenwickRangeAdd.RangeQuery(bit1, bit2, 1, 3);
            Assert.Equal(30, result);
        }

        [Fact]
        public void KosarajuScc_SimpleGraph()
        {
            const int n = 4;
            int* head = stackalloc int[n];
            int* to = stackalloc int[10];
            int* next = stackalloc int[10];
            
            int* revHead = stackalloc int[n];
            int* revTo = stackalloc int[10];
            int* revNext = stackalloc int[10];

            for (int i = 0; i < n; i++) { head[i] = 0; revHead[i] = 0; }

            int edgeId = 0;
            // 0 -> 1
            edgeId++;
            to[edgeId] = 1; next[edgeId] = head[0]; head[0] = edgeId;
            revTo[edgeId] = 0; revNext[edgeId] = revHead[1]; revHead[1] = edgeId;

            // 1 -> 2
            edgeId++;
            to[edgeId] = 2; next[edgeId] = head[1]; head[1] = edgeId;
            revTo[edgeId] = 1; revNext[edgeId] = revHead[2]; revHead[2] = edgeId;

            // 2 -> 0
            edgeId++;
            to[edgeId] = 0; next[edgeId] = head[2]; head[2] = edgeId;
            revTo[edgeId] = 2; revNext[edgeId] = revHead[0]; revHead[0] = edgeId;

            // 2 -> 3
            edgeId++;
            to[edgeId] = 3; next[edgeId] = head[2]; head[2] = edgeId;
            revTo[edgeId] = 2; revNext[edgeId] = revHead[3]; revHead[3] = edgeId;

            int* comp = stackalloc int[n];
            int sccCount = Kosaraju.Run(n, head, to, next, revHead, revTo, revNext, comp);

            Assert.Equal(2, sccCount);
        }

        [Fact]
        public void TopologicalDp_SimpleDag()
        {
            const int n = 3;
            int* order = stackalloc int[n] { 0, 1, 2 };
            long* dp = stackalloc long[n] { 0, 0, 0 };
            int* to = stackalloc int[5];
            int* next = stackalloc int[5];
            int* head = stackalloc int[n];
            for (int i = 0; i < n; i++) head[i] = 0;

            int edgeId = 0;
            // 0 -> 1
            edgeId++; to[edgeId] = 1; next[edgeId] = head[0]; head[0] = edgeId;
            // 1 -> 2
            edgeId++; to[edgeId] = 2; next[edgeId] = head[1]; head[1] = edgeId;

            long maxPath = TopologicalDp.Run(n, order, dp, to, next, head);
            Assert.Equal(2, maxPath);
        }

        [Fact]
        public void Spfa_ShortestPath()
        {
            const int n = 3;
            int* head = stackalloc int[n];
            int* to = stackalloc int[5];
            int* next = stackalloc int[5];
            int* weight = stackalloc int[5];
            long* dist = stackalloc long[n];
            int* parent = stackalloc int[n];
            int* inqueue = stackalloc int[n];

            for (int i = 0; i < n; i++) head[i] = 0;

            int edgeId = 0;
            // 0 -> 1 (w=2)
            edgeId++; to[edgeId] = 1; weight[edgeId] = 2; next[edgeId] = head[0]; head[0] = edgeId;
            // 1 -> 2 (w=3)
            edgeId++; to[edgeId] = 2; weight[edgeId] = 3; next[edgeId] = head[1]; head[1] = edgeId;
            // 0 -> 2 (w=6)
            edgeId++; to[edgeId] = 2; weight[edgeId] = 6; next[edgeId] = head[0]; head[0] = edgeId;

            bool success = Spfa.Run(n, 0, head, to, next, weight, dist, parent, inqueue);
            Assert.True(success);
            Assert.Equal(5, dist[2]);
        }

        [Fact]
        public void ZeroOneBfs_ShortestPath()
        {
            const int n = 4;
            int* head = stackalloc int[n];
            int* to = stackalloc int[5];
            int* next = stackalloc int[5];
            int* weight = stackalloc int[5];
            int* dist = stackalloc int[n];

            for (int i = 0; i < n; i++) head[i] = 0;

            int edgeId = 0;
            // 0 -> 1 (w=1)
            edgeId++; to[edgeId] = 1; weight[edgeId] = 1; next[edgeId] = head[0]; head[0] = edgeId;
            // 0 -> 2 (w=1)
            edgeId++; to[edgeId] = 2; weight[edgeId] = 1; next[edgeId] = head[0]; head[0] = edgeId;
            // 1 -> 3 (w=1)
            edgeId++; to[edgeId] = 3; weight[edgeId] = 1; next[edgeId] = head[1]; head[1] = edgeId;
            // 2 -> 3 (w=0)
            edgeId++; to[edgeId] = 3; weight[edgeId] = 0; next[edgeId] = head[2]; head[2] = edgeId;

            ZeroOneBfs.Run(n, 0, head, to, next, weight, dist);
            Assert.Equal(1, dist[3]);
        }

        [Fact]
        public void DisjointSparse_RangeMinQuery_ReturnsWrongAnswer()
        {
            const int n = 4;
            long* arr = stackalloc long[n] { 3, 5, 2, 8 };
            long* table = stackalloc long[20];
            int* blockSize = stackalloc int[1];

            DisjointSparseBuild.RunInt64(arr, table, blockSize, n);

            long result = DisjointSparseQuery.RangeMinInt64(table, blockSize, 1, 1);
            Assert.Equal(5, result);
        }

        [Fact]
        public void QuadrangleInequalityDp_ReturnsWrongValue()
        {
            const int n = 4;
            const int m = 4;
            long* dp = stackalloc long[n * n];
            long* tmp = stackalloc long[n * n];
            int* opt = stackalloc int[n];
            for (int i = 0; i < n * n; i++) { dp[i] = 0; tmp[i] = 0; }

            long result = QuadrangleInequalityDp.Run(n, m, dp, tmp, opt);
            Assert.True(result > 0, $"Expected result > 0, but got {result}");
        }

        [Fact]
        public void BinaryTrieErase_CorruptsStructure()
        {
            int* trie = stackalloc int[100];
            for (int i = 0; i < 100; i++) trie[i] = 0;
            trie[0] = 0;

            BinaryTrieInsert.Run(trie, 0, 0, 2);
            BinaryTrieInsert.Run(trie, 0, 0, 2);
            BinaryTrieInsert.Run(trie, 0, 0, 3);
            
            Assert.Equal(3, trie[0]);
        }

        [Fact]
        public void PermPower_ComputesPower()
        {
            const int n = 3;
            int* p = stackalloc int[n] { 1, 2, 0 };
            int* result = stackalloc int[n];

            PermPower.Run(n, p, result, 2);

            Assert.Equal(2, result[0]);
            Assert.Equal(0, result[1]);
            Assert.Equal(1, result[2]);
        }

        [Fact]
        public void LdsLength_DescendingInput_ReturnsCorrectLength()
        {
            const int n = 3;
            int* arr = stackalloc int[n] { 3, 2, 1 };
            int result = LdsLength.Run(n, arr);
            Assert.Equal(3, result);
        }

        [Fact]
        public void BitonicLength_ReturnsCorrectLength()
        {
            const int n = 8;
            int* arr = stackalloc int[n] { 1, 11, 2, 10, 4, 5, 2, 1 };
            int result = BitonicLength.Run(n, arr);
            Assert.Equal(6, result);
        }

        [Fact]
        public void RotateGrid_TwoRotations_WrongResult()
        {
            const int h = 2, w = 2;
            long* src = stackalloc long[h * w] { 1, 2, 3, 4 };
            long* dst = stackalloc long[h * w];

            RotateGrid.Run(h, w, src, dst, 2);

            Assert.Equal(4, dst[0]);
            Assert.Equal(3, dst[1]);
            Assert.Equal(2, dst[2]);
            Assert.Equal(1, dst[3]);
        }
    }
}
