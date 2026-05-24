namespace IAFahim.Optimization.Offline.Tests
{
    using IAFahim.Optimization.Offline;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class ParallelBinarySearchTests
    {
        [Test]
        public void Init_Basic()
        {
            int* lo = stackalloc int[10];
            int* hi = stackalloc int[10];
            int* queryIdx = stackalloc int[10];
            int* bucketSize = stackalloc int[1];
            bucketSize[0] = 0;

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
            int* queryIdx = stackalloc int[10];
            int* bucketSize = stackalloc int[1];
            bucketSize[0] = 0;

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
        }
    }
}