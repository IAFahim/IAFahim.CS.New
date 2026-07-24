namespace IAFahim.Optimization.Offline.Tests
{
    using System.Runtime.InteropServices;
    using IAFahim.Optimization.Offline;
    using NUnit.Framework;

    public sealed unsafe class ParallelBinarySearchTests
    {
        [Test]
        public void Init_Basic()
        {
            int* lo = stackalloc int[10];
            int* hi = stackalloc int[10];

            ParallelBinarySearch.Init(lo, hi, 10);

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(0, lo[i]);
                Assert.AreEqual(-1, hi[i]);
            }
        }

        [Test]
        public void InitWithRange_Basic()
        {
            int* lo = stackalloc int[10];
            int* hi = stackalloc int[10];

            ParallelBinarySearch.InitWithRange(lo, hi, 10, 1, 100);

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(1, lo[i]);
                Assert.AreEqual(100, hi[i]);
            }
        }

        [Test]
        public void Mid_Basic()
        {
            Assert.AreEqual(50, ParallelBinarySearch.Mid(0, 100));
            Assert.AreEqual(5, ParallelBinarySearch.Mid(0, 10));
            Assert.AreEqual(5, ParallelBinarySearch.Mid(10, 0));
        }

        [Test]
        public void GroupByMid_EmptyActive_ReturnsZero()
        {
            int* lo = stackalloc int[] { 3, 5 };
            int* hi = stackalloc int[] { 3, 4 };
            int* queryIdx = stackalloc int[] { 0, 1 };
            int* buckets = stackalloc int[2];
            int* bucketSize = stackalloc int[1];
            bucketSize[0] = -1;

            int active = ParallelBinarySearch.GroupByMid(lo, hi, queryIdx, bucketSize, 2, buckets);

            Assert.AreEqual(0, active);
            Assert.AreEqual(0, bucketSize[0]);
        }

        [Test]
        public void GroupByMid_SortsActiveByMid_SkipsFinished()
        {
            int* lo = stackalloc int[] { 0, 0, 8, 1 };
            int* hi = stackalloc int[] { 10, 4, 8, 3 };
            int* queryIdx = stackalloc int[] { 0, 1, 2, 3 };
            int* buckets = stackalloc int[4];
            int* bucketSize = stackalloc int[1];
            bucketSize[0] = 0;

            int active = ParallelBinarySearch.GroupByMid(lo, hi, queryIdx, bucketSize, 4, buckets);

            Assert.AreEqual(3, active);
            Assert.AreEqual(3, bucketSize[0]);
            Assert.AreEqual(2, ParallelBinarySearch.Mid(lo[buckets[0]], hi[buckets[0]]));
            Assert.AreEqual(2, ParallelBinarySearch.Mid(lo[buckets[1]], hi[buckets[1]]));
            Assert.AreEqual(5, ParallelBinarySearch.Mid(lo[buckets[2]], hi[buckets[2]]));
            Assert.AreEqual(1, buckets[0]);
            Assert.AreEqual(3, buckets[1]);
            Assert.AreEqual(0, buckets[2]);
        }

        [Test]
        public void GroupByMid_NoCollisionDistinctMids()
        {
            int* lo = stackalloc int[] { 0, 0 };
            int* hi = stackalloc int[] { 2, 0 };
            int* queryIdx = stackalloc int[] { 0, 1 };
            int* buckets = stackalloc int[2];
            int* bucketSize = stackalloc int[1];
            bucketSize[0] = 0;

            int active = ParallelBinarySearch.GroupByMid(lo, hi, queryIdx, bucketSize, 2, buckets);

            Assert.AreEqual(1, active);
            Assert.AreEqual(0, buckets[0]);
            Assert.AreEqual(1, ParallelBinarySearch.Mid(lo[0], hi[0]));
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PrefixCtx
    {
        public int* Updates;
        public int* Need;
        public int Sum;
    }

    public sealed unsafe class DivideConquerAnswerTests
    {
        private static void Apply(void* context, int v)
        {
            PrefixCtx* c = (PrefixCtx*)context;
            c->Sum += c->Updates[v];
        }

        private static void Undo(void* context, int v)
        {
            PrefixCtx* c = (PrefixCtx*)context;
            c->Sum -= c->Updates[v];
        }

        private static bool Check(void* context, int q)
        {
            PrefixCtx* c = (PrefixCtx*)context;
            return c->Sum >= c->Need[q];
        }

        [Test]
        public void Solve_EmptyQueries_NoOp()
        {
            int* answers = stackalloc int[1];
            answers[0] = -1;
            int* queryIdx = stackalloc int[1];
            DivideConquerAnswer.Solve(answers, 0, 4, queryIdx, 0, null, &Apply, &Undo, &Check);
            Assert.AreEqual(-1, answers[0]);
        }

        [Test]
        public void Solve_PrefixSumEarliestTime()
        {
            const int nTimes = 5;
            const int nQueries = 4;
            int* updates = stackalloc int[] { 1, 2, 3, 4, 5 };
            int* need = stackalloc int[] { 6, 1, 15, 7 };
            int* answers = stackalloc int[nQueries];
            int* queryIdx = stackalloc int[nQueries];
            for (int i = 0; i < nQueries; i++)
            {
                answers[i] = -1;
                queryIdx[i] = i;
            }

            PrefixCtx ctx;
            ctx.Updates = updates;
            ctx.Need = need;
            ctx.Sum = 0;

            DivideConquerAnswer.Solve(
                answers, 0, nTimes - 1, queryIdx, nQueries, &ctx, &Apply, &Undo, &Check);

            Assert.AreEqual(2, answers[0]);
            Assert.AreEqual(0, answers[1]);
            Assert.AreEqual(4, answers[2]);
            Assert.AreEqual(3, answers[3]);
            Assert.AreEqual(0, ctx.Sum);
        }

        [Test]
        public void Solve_SinglePointRange()
        {
            int* updates = stackalloc int[] { 10 };
            int* need = stackalloc int[] { 1, 10 };
            int* answers = stackalloc int[2];
            int* queryIdx = stackalloc int[] { 0, 1 };
            answers[0] = -1;
            answers[1] = -1;

            PrefixCtx ctx;
            ctx.Updates = updates;
            ctx.Need = need;
            ctx.Sum = 0;

            DivideConquerAnswer.Solve(answers, 0, 0, queryIdx, 2, &ctx, &Apply, &Undo, &Check);

            Assert.AreEqual(0, answers[0]);
            Assert.AreEqual(0, answers[1]);
        }
    }

    public sealed unsafe class OfflineKthNumberTests
    {
        [Test]
        public void BuildPersistentSegTree_Basic()
        {
            const int maxNodes = 100;
            int* leftChild = stackalloc int[maxNodes];
            int* rightChild = stackalloc int[maxNodes];
            int* sum = stackalloc int[maxNodes];
            int* version = stackalloc int[1];
            int* allocCnt = stackalloc int[1];
            *allocCnt = 0;
            version[0] = 0;

            int root = OfflineKthNumber.BuildPersistentSegTree(
                leftChild, rightChild, sum, version, 0, 0, 10, 5, allocCnt);

            Assert.IsTrue(root > 0);
        }

        [Test]
        public void QueryKth_Basic()
        {
            const int maxNodes = 100;
            int* leftChild = stackalloc int[maxNodes];
            int* rightChild = stackalloc int[maxNodes];
            int* sum = stackalloc int[maxNodes];
            int* version = stackalloc int[1];
            int* allocCnt = stackalloc int[1];
            *allocCnt = 0;
            version[0] = 0;

            int root = OfflineKthNumber.BuildPersistentSegTree(
                leftChild, rightChild, sum, version, 0, 0, 10, 5, allocCnt);

            int kth = OfflineKthNumber.QueryKth(
                leftChild, rightChild, sum, root, 0, 10, 1);
            Assert.IsTrue(kth >= 0 && kth <= 10);
        }

        [Test]
        public void EmptyTree_QueryDoesNotCrash()
        {
            const int maxNodes = 10;
            int* leftChild = stackalloc int[maxNodes];
            int* rightChild = stackalloc int[maxNodes];
            int* sum = stackalloc int[maxNodes];
            Assert.IsTrue(leftChild != null);
            Assert.IsTrue(rightChild != null);
            Assert.IsTrue(sum != null);
        }
    }

    public sealed unsafe class Cdq3DDominanceTests
    {
        private static void BitAdd(int* bit, int i, int v)
        {
            const int maxZ = 8;
            for (int x = i; x <= maxZ; x += x & -x) bit[x] += v;
        }

        private static int BitSum(int* bit, int i)
        {
            int s = 0;
            for (int x = i; x > 0; x -= x & -x) s += bit[x];
            return s;
        }

        [Test]
        public void Process_CountsThreeDDominance()
        {
            int* x = stackalloc int[] { 1, 2, 3, 4 };
            int* y = stackalloc int[] { 2, 3, 1, 4 };
            int* z = stackalloc int[] { 3, 1, 2, 4 };
            int n = 4;
            int* idx = stackalloc int[n];
            int* tmp = stackalloc int[n];
            int* count = stackalloc int[n];
            int* bit = stackalloc int[9];
            for (int i = 0; i < n; i++) idx[i] = i;
            for (int i = 0; i < n; i++) count[i] = 0;
            for (int i = 0; i <= 8; i++) bit[i] = 0;

            Cdq3DDominance.SortByX(x, y, z, idx, tmp, 0, n - 1);
            Cdq3DDominance.Process(x, y, z, idx, tmp, count, 0, n - 1, bit, 8,
                &BitAdd, &BitSum);

            Assert.AreEqual(0, count[0]);
            Assert.AreEqual(0, count[1]);
            Assert.AreEqual(0, count[2]);
            Assert.AreEqual(3, count[3]);
        }

        [Test]
        public void Process_ChainDominance()
        {
            int* x = stackalloc int[] { 1, 2, 3 };
            int* y = stackalloc int[] { 1, 2, 3 };
            int* z = stackalloc int[] { 1, 2, 3 };
            int n = 3;
            int* idx = stackalloc int[n];
            int* tmp = stackalloc int[n];
            int* count = stackalloc int[n];
            int* bit = stackalloc int[9];
            for (int i = 0; i < n; i++)
            {
                idx[i] = i;
                count[i] = 0;
            }
            for (int i = 0; i <= 8; i++) bit[i] = 0;

            Cdq3DDominance.SortByX(x, y, z, idx, tmp, 0, n - 1);
            Cdq3DDominance.Process(x, y, z, idx, tmp, count, 0, n - 1, bit, 8,
                &BitAdd, &BitSum);

            Assert.AreEqual(0, count[0]);
            Assert.AreEqual(1, count[1]);
            Assert.AreEqual(2, count[2]);
        }
    }
}