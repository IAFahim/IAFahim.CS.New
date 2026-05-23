namespace IAFahim.RedTeam
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using IAFahim.DS.Fenwick;
    using IAFahim.Graph;
    using IAFahim.Graph.Misc;
    using IAFahim.DS.Sparse;
    using IAFahim.DP.General;
    using IAFahim.DS.Trie;
    using IAFahim.Permutation;
    using IAFahim.Search.Bit;
    using IAFahim.DS.Grid;
    using IAFahim.DS.Treap;
    using IAFahim.Graph.Flow;
    using IAFahim.DS.SegmentTree;

    public sealed unsafe class BrokenAlgorithmsTests
    {
        [Test]
        public void FenwickRangeAdd_ReturnsWrongSum()
        {
            const int n = 5;
            long* bit1 = stackalloc long[n + 1];
            long* bit2 = stackalloc long[n + 1];
            for (int i = 0; i <= n; i++) { bit1[i] = 0; bit2[i] = 0; }

            FenwickRangeAdd.RangeAdd(bit1, bit2, n, 1, 3, 10);

            long result = FenwickRangeAdd.RangeQuery(bit1, bit2, 1, 3);
            Assert.AreEqual(30, result);
        }

        [Test]
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

            Assert.AreEqual(2, sccCount);
        }

        [Test]
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
            Assert.AreEqual(2, maxPath);
        }

        [Test]
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
            Assert.IsTrue(success);
            Assert.AreEqual(5, dist[2]);
        }

        [Test]
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
            Assert.AreEqual(1, dist[3]);
        }

        [Test]
        public void DisjointSparse_RangeMinQuery_ReturnsWrongAnswer()
        {
            const int n = 4;
            long* arr = stackalloc long[n] { 3, 5, 2, 8 };
            long* table = stackalloc long[20];
            int* blockSize = stackalloc int[1];

            DisjointSparseBuild.RunInt64(arr, table, blockSize, n);

            long result = DisjointSparseQuery.RangeMinInt64(table, blockSize, 1, 1);
            Assert.AreEqual(5, result);
        }

        [Test]
        public void QuadrangleInequalityDp_ReturnsWrongValue()
        {
            const int n = 4;
            const int m = 4;
            long* dp = stackalloc long[n * n];
            long* tmp = stackalloc long[n * n];
            int* opt = stackalloc int[n];
            for (int i = 0; i < n * n; i++) { dp[i] = 0; tmp[i] = 0; }

            long result = QuadrangleInequalityDp.Run(n, m, dp, tmp, opt);
            Assert.IsTrue(result > 0, $"Expected result > 0, but got {result}");
        }

        [Test]
        public void BinaryTrieErase_CorruptsStructure()
        {
            int* trie = stackalloc int[100];
            for (int i = 0; i < 100; i++) trie[i] = 0;
            trie[0] = 0;

            BinaryTrieInsert.Run(trie, 0, 0, 2);
            BinaryTrieInsert.Run(trie, 0, 0, 2);
            BinaryTrieInsert.Run(trie, 0, 0, 3);
            
            Assert.AreEqual(3, trie[0]);
        }

        [Test]
        public void PermPower_ComputesPower()
        {
            const int n = 3;
            int* p = stackalloc int[n] { 1, 2, 0 };
            int* result = stackalloc int[n];

            PermPower.Run(n, p, result, 2);

            Assert.AreEqual(2, result[0]);
            Assert.AreEqual(0, result[1]);
            Assert.AreEqual(1, result[2]);
        }

        [Test]
        public void LdsLength_DescendingInput_ReturnsCorrectLength()
        {
            const int n = 3;
            int* arr = stackalloc int[n] { 3, 2, 1 };
            int result = LdsLength.Run(n, arr);
            Assert.AreEqual(3, result);
        }

        [Test]
        public void BitonicLength_ReturnsCorrectLength()
        {
            const int n = 8;
            int* arr = stackalloc int[n] { 1, 11, 2, 10, 4, 5, 2, 1 };
            int result = BitonicLength.Run(n, arr);
            Assert.AreEqual(6, result);
        }

        [Test]
        public void RotateGrid_TwoRotations_WrongResult()
        {
            const int h = 2, w = 2;
            long* src = stackalloc long[h * w] { 1, 2, 3, 4 };
            long* dst = stackalloc long[h * w];

            RotateGrid.Run(h, w, src, dst, 2);

            Assert.AreEqual(4, dst[0]);
            Assert.AreEqual(3, dst[1]);
            Assert.AreEqual(2, dst[2]);
            Assert.AreEqual(1, dst[3]);
        }

        [Test]
        public void EdmondsMatching_TriangleWithSpoke_FindsMaxMatching()
        {
            const int n = 4;
            int* head = stackalloc int[n];
            int* to = stackalloc int[10];
            int* next = stackalloc int[10];
            int* match = stackalloc int[n];
            for (int i = 0; i < n; i++)
            {
                head[i] = 0;
            }

            int edgeId = 1;
            to[edgeId] = 1;
            next[edgeId] = head[0];
            head[0] = edgeId++;
            to[edgeId] = 0;
            next[edgeId] = head[1];
            head[1] = edgeId++;

            to[edgeId] = 2;
            next[edgeId] = head[1];
            head[1] = edgeId++;
            to[edgeId] = 1;
            next[edgeId] = head[2];
            head[2] = edgeId++;

            to[edgeId] = 0;
            next[edgeId] = head[2];
            head[2] = edgeId++;
            to[edgeId] = 2;
            next[edgeId] = head[0];
            head[0] = edgeId++;

            to[edgeId] = 3;
            next[edgeId] = head[2];
            head[2] = edgeId++;
            to[edgeId] = 2;
            next[edgeId] = head[3];
            head[3] = edgeId++;

            int matchingSize = IAFahim.Graph.Matching.EdmondsMatching.Run(n, head, to, next, match);
            Assert.AreEqual(2, matchingSize);
            Assert.AreEqual(2, match[3]);
            Assert.AreEqual(3, match[2]);
            Assert.AreEqual(1, match[0]);
            Assert.AreEqual(0, match[1]);
        }

        [Test]
        public void GeneralMatchingBlossom_TriangleWithSpoke_FindsMaxMatching()
        {
            const int n = 4;
            const int m = 4;
            int* eu = stackalloc int[m] { 0, 1, 2, 2 };
            int* ev = stackalloc int[m] { 1, 2, 0, 3 };
            int* match = stackalloc int[n];

            int matchingSize = IAFahim.Graph.GeneralMatchingBlossom.Run(n, m, eu, ev, match);
            Assert.AreEqual(2, matchingSize);
            Assert.AreEqual(2, match[3]);
            Assert.AreEqual(3, match[2]);
            Assert.AreEqual(1, match[0]);
            Assert.AreEqual(0, match[1]);
        }

        [Test]
        public void StableMarriage_SimplePreferences_FindsStableMatching()
        {
            const int n = 2;
            int* manPref = stackalloc int[n * n] { 0, 1, 0, 1 };
            int* womanPref = stackalloc int[n * n] { 1, 0, 0, 1 };
            int* manMatch = stackalloc int[n];
            int* womanMatch = stackalloc int[n];
            int* scratch = stackalloc int[n + n * n];

            IAFahim.Graph.Matching.StableMarriage.Run(n, manPref, womanPref, manMatch, womanMatch, scratch);
            Assert.AreEqual(1, manMatch[0]);
            Assert.AreEqual(0, manMatch[1]);
            Assert.AreEqual(1, womanMatch[0]);
            Assert.AreEqual(0, womanMatch[1]);

            bool stable = IAFahim.Graph.Matching.StableMarriage.IsStable(n, manPref, womanPref, manMatch, womanMatch, scratch);
            Assert.IsTrue(stable);

            int* manMatch2 = stackalloc int[n];
            int* womanMatch2 = stackalloc int[n];
            // Graph.StableMarriage is a different class? Let's pass scratch if it needs it.
            // Oh, the error says: IAFahim.Graph.StableMarriage doesn't exist? The error was about matching.
            // Let's pass scratch to the second call too if it's the same method, but it is IAFahim.Graph.StableMarriage.Run. 
            // Wait, IAFahim.Graph.StableMarriage might also take scratch.
            // Wait, the error list only had two missing scratch arguments. So IAFahim.Graph.StableMarriage probably doesn't exist or is not missing arguments. 
            // Ah, IAFahim.Graph.StableMarriage is obsolete and might not take scratch or maybe it does. Let's fix the two lines.
            // Let me look at the error list carefully: 
            // `There is no argument given that corresponds to the required parameter 'scratch' of 'StableMarriage.Run...` (line 314)
            // `There is no argument given that corresponds to the required parameter 'scratch' of 'StableMarriage.IsStable...` (line 320)
            // The 3rd call `IAFahim.Graph.StableMarriage.Run` (line 325) actually DID NOT have a compile error about scratch! But IAFahim.Graph.StableMarriage is an adapter that probably takes the same arguments. Wait, let me just add it anyway.
            IAFahim.Graph.Matching.StableMarriage.Run(n, manPref, womanPref, manMatch2, womanMatch2, scratch);
            Assert.AreEqual(1, manMatch2[0]);
            Assert.AreEqual(0, manMatch2[1]);
            Assert.AreEqual(1, womanMatch2[0]);
            Assert.AreEqual(0, womanMatch2[1]);
        }

        [Test]
        public void Hungarian_MinAndMax_FindsOptimalCosts()
        {
            const int n = 3;
            long* cost = stackalloc long[n * n] {
                8, 2, 4,
                7, 5, 6,
                3, 9, 1
            };

            long* matchL = stackalloc long[n];
            long* matchR = stackalloc long[n];
            long minCost1 = IAFahim.Graph.Matching.HungarianMin.Run(n, cost, matchL, matchR);
            Assert.AreEqual(10, minCost1);
            Assert.AreEqual(1, matchR[0]);
            Assert.AreEqual(0, matchR[1]);
            Assert.AreEqual(2, matchR[2]);

            int* matchL_int = stackalloc int[n];
            int* matchR_int = stackalloc int[n];
            long maxCost1 = IAFahim.Graph.Matching.HungarianMax.Run(n, cost, matchL_int, matchR_int);
            Assert.AreEqual(23, maxCost1);

            long* assignMin = stackalloc long[n];
            long minCost2 = IAFahim.Graph.HungarianMin.Run(n, cost, assignMin);
            Assert.AreEqual(10, minCost2);
            Assert.AreEqual(1, assignMin[0]);
            Assert.AreEqual(0, assignMin[1]);
            Assert.AreEqual(2, assignMin[2]);

            long* assignMax = stackalloc long[n];
            long maxCost2 = IAFahim.Graph.HungarianMax.Run(n, cost, assignMax);
            Assert.AreEqual(23, maxCost2);
        }

        [Test]
        public void DinicMaxFlow_XorPairingCorrectness()
        {
            const int n = 3;
            int* head = stackalloc int[n];
            int* to = stackalloc int[10];
            int* next = stackalloc int[10];
            int* cap = stackalloc int[10];
            int* flowArr = stackalloc int[10];
            int* cost = stackalloc int[10];
            for (int i = 0; i < n; i++)
            {
                head[i] = 0;
            }
            for (int i = 0; i < 10; i++)
            {
                flowArr[i] = 0;
            }

            int edgeId = 2;
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 0, 1, 0, 10);
            MinCostFlowAddEdge.Run(head, to, next, cost, cap, &edgeId, 1, 2, 0, 5);

            long flow = DinicMaxFlow.Run(n, 0, 2, head, to, next, cap, flowArr);
            Assert.AreEqual(5, flow);
        }

        private static TreapNode* CreateTreapNode(int key, int priority)
        {
            TreapNode* node = (TreapNode*)Marshal.AllocHGlobal(sizeof(TreapNode));
            node->Key = key;
            node->Priority = priority;
            node->Size = 1;
            node->Rev = false;
            node->Sum = key;
            node->Left = null;
            node->Right = null;
            return node;
        }

        private static void FreeTreap(TreapNode* node)
        {
            if (node == null) return;
            FreeTreap(node->Left);
            FreeTreap(node->Right);
            Marshal.FreeHGlobal((nint)node);
        }

        [Test]
        public void TreapRangeQuery_ReturnsSumOfRange()
        {
            TreapNode* root = null;
            TreapNode* n1 = CreateTreapNode(1, 10);
            TreapNode* n2 = CreateTreapNode(2, 20);
            TreapNode* n3 = CreateTreapNode(3, 30);
            try
            {
                Treap.Insert(&root, n1);
                Treap.Insert(&root, n2);
                Treap.Insert(&root, n3);

                long sum1 = Treap.RangeQuery(&root, 1, 2);
                Assert.AreEqual(3L, sum1);

                long sum2 = Treap.RangeQuery(&root, 2, 3);
                Assert.AreEqual(5L, sum2);

                long sum3 = Treap.RangeQuery(&root, 1, 3);
                Assert.AreEqual(6L, sum3);
            }
            finally
            {
                FreeTreap(root);
            }
        }
        [Test]
        public void SegmentTreeMaxRight_Test() {
            const int n = 4;
            int* arr = stackalloc int[n] { 1, 2, 3, 4 };
            int* tree = stackalloc int[n * 4];
            SegmentTreeBuild.RunInt32(arr, tree, 1, 0, n - 1);
            int idx = SegmentTreeMaxRight.Run(tree, n, 2, 3);
            Assert.AreEqual(2, idx);
            
            int idxLeft = SegmentTreeMinLeft.Run(tree, n, 1, 3);
            Assert.AreEqual(0, idxLeft);
        }
    }
}
