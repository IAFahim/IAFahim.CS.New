namespace IAFahim.Graph.TreeDecomposition.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using IAFahim.Graph.TreeDecomposition;

    public sealed unsafe class TreeDecompositionTests
    {
        private static void AddEdge(int* head, int* to, int* next, ref int edgeCount, int u, int v)
        {
            to[edgeCount] = v;
            next[edgeCount] = head[u];
            head[u] = edgeCount++;

            to[edgeCount] = u;
            next[edgeCount] = head[v];
            head[v] = edgeCount++;
        }

        [Test]
        public void Hld_Basic()
        {
            const int N = 5;
            int* head = stackalloc int[N];
            int* to = stackalloc int[2 * N];
            int* next = stackalloc int[2 * N];
            int edgeCount = 1;

            for (int i = 0; i < N; i++) head[i] = 0;

            // Edges: 0-1, 1-2, 1-3, 3-4
            AddEdge(head, to, next, ref edgeCount, 0, 1);
            AddEdge(head, to, next, ref edgeCount, 1, 2);
            AddEdge(head, to, next, ref edgeCount, 1, 3);
            AddEdge(head, to, next, ref edgeCount, 3, 4);

            int* parent = stackalloc int[N];
            int* depth = stackalloc int[N];
            int* heavy = stackalloc int[N];
            int* size = stackalloc int[N];
            int* headChain = stackalloc int[N];
            int* pos = stackalloc int[N];
            int curPos = 0;

            HeavyLightDecomposition.TreePathDecompose(
                N, 0, head, to, next,
                parent, depth, heavy, size,
                headChain, pos, ref curPos);

            // Segment Tree for sums
            const int ST_SIZE = 4 * N;
            long* tree = stackalloc long[ST_SIZE];
            long* lazyAdd = stackalloc long[ST_SIZE];
            long* lazyAssign = stackalloc long[ST_SIZE];
            byte* hasAssign = stackalloc byte[ST_SIZE];
            long* initVals = stackalloc long[N];

            for (int i = 0; i < N; i++) initVals[i] = 0;
            for (int i = 0; i < ST_SIZE; i++)
            {
                tree[i] = 0;
                lazyAdd[i] = 0;
                lazyAssign[i] = 0;
                hasAssign[i] = 0;
            }

            HeavyLightDecomposition.BuildSumTree(tree, 1, 0, N - 1, initVals);

            // PathAdd: add 5 to path 2-4 (path is 2-1-3-4)
            HeavyLightDecomposition.PathAdd(2, 4, 5, tree, lazyAdd, lazyAssign, hasAssign, headChain, pos, parent, depth, N);
            
            // PathQuery: sum on path 2-4 should be 5 * 4 = 20
            Assert.AreEqual(20, HeavyLightDecomposition.PathSumQuery(2, 4, tree, lazyAdd, lazyAssign, hasAssign, headChain, pos, parent, depth, N));

            // PathAssign: assign 3 to path 1-4 (path is 1-3-4)
            HeavyLightDecomposition.PathAssign(1, 4, 3, tree, lazyAdd, lazyAssign, hasAssign, headChain, pos, parent, depth, N);
            // Now nodes 1, 3, 4 have value 3. Node 2 still has value 5. Node 0 has value 0.
            // PathQuery 2-4 (2-1-3-4): 5 + 3 + 3 + 3 = 14.
            Assert.AreEqual(14, HeavyLightDecomposition.PathSumQuery(2, 4, tree, lazyAdd, lazyAssign, hasAssign, headChain, pos, parent, depth, N));

            // Segment Tree for max subarray
            HldSegNode* saTree = stackalloc HldSegNode[ST_SIZE];
            long* saLazyAssign = stackalloc long[ST_SIZE];
            byte* saHasAssign = stackalloc byte[ST_SIZE];
            long* saInitVals = stackalloc long[N];

            // Initialize values: node 0: -2, node 1: 5, node 2: -1, node 3: 4, node 4: -3
            // In pos array: we must put the values at their pos[node] positions!
            saInitVals[pos[0]] = -2;
            saInitVals[pos[1]] = 5;
            saInitVals[pos[2]] = -1;
            saInitVals[pos[3]] = 4;
            saInitVals[pos[4]] = -3;

            for (int i = 0; i < ST_SIZE; i++)
            {
                saLazyAssign[i] = 0;
                saHasAssign[i] = 0;
            }

            HeavyLightDecomposition.BuildMaxSubarrayTree(saTree, 1, 0, N - 1, saInitVals);

            // PathMaxSubarray on path 2-4 (path values: 2 is -1, 1 is 5, 3 is 4, 4 is -3)
            // Sequence from 2 to 4: [-1, 5, 4, -3]
            // Max subarray sum is 5 + 4 = 9.
            HldSegNode res = HeavyLightDecomposition.PathMaxSubarray(2, 4, saTree, saLazyAssign, saHasAssign, headChain, pos, parent, depth, N);
            Assert.AreEqual(9, res.Ans);
        }

        // --- TREE MO'S ALGORITHM TESTS ---
        private struct MoContext
        {
            public int CurrentUniqueCount;
            public int* Counts;
            public int* Values;
            public int* Results;
        }

        private static void MoAdd(int node, void* ctxPtr)
        {
            MoContext* ctx = (MoContext*)ctxPtr;
            int val = ctx->Values[node];
            if (ctx->Counts[val] == 0)
            {
                ctx->CurrentUniqueCount++;
            }
            ctx->Counts[val]++;
        }

        private static void MoRemove(int node, void* ctxPtr)
        {
            MoContext* ctx = (MoContext*)ctxPtr;
            int val = ctx->Values[node];
            ctx->Counts[val]--;
            if (ctx->Counts[val] == 0)
            {
                ctx->CurrentUniqueCount--;
            }
        }

        private static void MoQueryCallback(int queryId, int queryIndex, void* ctxPtr)
        {
            MoContext* ctx = (MoContext*)ctxPtr;
            ctx->Results[queryId] = ctx->CurrentUniqueCount;
        }

        [Test]
        public void MoOnTree_Basic()
        {
            const int N = 5;
            int* head = stackalloc int[N];
            int* to = stackalloc int[2 * N];
            int* next = stackalloc int[2 * N];
            int edgeCount = 1;

            for (int i = 0; i < N; i++) head[i] = 0;

            // Edges: 0-1, 1-2, 1-3, 3-4
            AddEdge(head, to, next, ref edgeCount, 0, 1);
            AddEdge(head, to, next, ref edgeCount, 1, 2);
            AddEdge(head, to, next, ref edgeCount, 1, 3);
            AddEdge(head, to, next, ref edgeCount, 3, 4);

            int* euler = stackalloc int[2 * N];
            int* inTime = stackalloc int[N];
            int* outTime = stackalloc int[N];
            int timer = 0;
            MoAlgorithmOnTree.BuildEulerTour(0, -1, head, to, next, euler, ref timer, inTime, outTime);

            int* values = stackalloc int[N];
            // Node values (colors): 0: A(0), 1: B(1), 2: A(0), 3: C(2), 4: B(1)
            values[0] = 0;
            values[1] = 1;
            values[2] = 0;
            values[3] = 2;
            values[4] = 1;

            const int Q = 2;
            MoQuery* queries = stackalloc MoQuery[Q];
            // Query 0: path 2 to 4. LCA is 1. Since LCA != 2 and LCA != 4, interval is [outTime[u], inTime[v]]
            int u0 = 2, v0 = 4;
            if (inTime[u0] > inTime[v0]) { int tmp = u0; u0 = v0; v0 = tmp; }
            queries[0].Id = 0;
            queries[0].L = outTime[u0];
            queries[0].R = inTime[v0];
            queries[0].Lca = 1;
            queries[0].BlockId = queries[0].L / 2; // block size 2

            // Query 1: path 0 to 2. LCA is 0. Since LCA == 0, interval is [inTime[u], inTime[v]]
            int u1 = 0, v1 = 2;
            if (inTime[u1] > inTime[v1]) { int tmp = u1; u1 = v1; v1 = tmp; }
            queries[1].Id = 1;
            queries[1].L = inTime[u1];
            queries[1].R = inTime[v1];
            queries[1].Lca = -1;
            queries[1].BlockId = queries[1].L / 2;

            MoAlgorithmOnTree.SortQueries(queries, Q);

            int* counts = stackalloc int[10];
            for (int i = 0; i < 10; i++) counts[i] = 0;
            int* results = stackalloc int[Q];

            MoContext ctx;
            ctx.CurrentUniqueCount = 0;
            ctx.Counts = counts;
            ctx.Values = values;
            ctx.Results = results;

            // Setup unmanaged function pointers
            delegate*<int, void*, void> addPtr = &MoAdd;
            delegate*<int, void*, void> removePtr = &MoRemove;
            delegate*<int, int, void*, void> queryPtr = &MoQueryCallback;

            int* depth = stackalloc int[N];
            int* parent = stackalloc int[N];
            for (int i = 0; i < N; i++) { depth[i] = 0; parent[i] = -1; }

            MoAlgorithmOnTree.TreeMoQuery(
                N, Q, euler, inTime, outTime, depth, parent,
                queries, addPtr, removePtr, queryPtr, &ctx, 2);

            Assert.AreEqual(3, results[0]);
            Assert.AreEqual(2, results[1]);
        }
    }
}
