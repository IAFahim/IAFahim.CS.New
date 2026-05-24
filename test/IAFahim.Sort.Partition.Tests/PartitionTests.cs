namespace IAFahim.Sort.Partition.Tests
{
    using NUnit.Framework;

    public sealed unsafe class PartitionTests
    {
        [Test]
        public void Run()
        {
            int* ptr = stackalloc int[] { 5, 3, 1, 4, 1, 9 };
            int pivotIdx = Partition.Run(ptr, 6, 4);
            Assert.AreEqual(1, ptr[pivotIdx]);
            for (int i = 0; i < pivotIdx; i++)
                Assert.IsTrue(ptr[i] < ptr[pivotIdx]);
            for (int i = pivotIdx + 1; i < 6; i++)
                Assert.IsTrue(ptr[i] >= ptr[pivotIdx]);
        }

        [Test]
        public void InvalidPivotIdx()
        {
            int* ptr = stackalloc int[] { 1, 2, 3 };
            int result = Partition.Run(ptr, 3, 10);
            Assert.AreEqual(-1, result);
        }
    }

    public sealed unsafe class NthElementTests
    {
        [Test]
        public void TryGetNthElement_Found()
        {
            int* ptr = stackalloc int[] { 3, 1, 4, 1, 5, 9 };
            int val;
            bool found = Partition.TryGetNthElement(ptr, 6, 2, out val);
            Assert.IsTrue(found);
            Assert.AreEqual(3, val);
        }

        [Test]
        public void TryGetNthElement_NotFound()
        {
            int* ptr = stackalloc int[] { 1, 2, 3 };
            int val;
            bool found = Partition.TryGetNthElement(ptr, 3, 10, out val);
            Assert.IsFalse(found);
        }
    }
}