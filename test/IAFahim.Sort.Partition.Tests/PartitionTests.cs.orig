namespace IAFahim.Sort.Partition.Tests
{
    using Xunit;

    public sealed unsafe class PartitionTests
    {
        [Fact]
        public void Run()
        {
            int* ptr = stackalloc int[] { 5, 3, 1, 4, 1, 9 };
            int pivotIdx = Partition.Run(ptr, 6, 4);
            Assert.Equal(1, ptr[pivotIdx]);
            for (int i = 0; i < pivotIdx; i++)
                Assert.True(ptr[i] < ptr[pivotIdx]);
            for (int i = pivotIdx + 1; i < 6; i++)
                Assert.True(ptr[i] >= ptr[pivotIdx]);
        }

        [Fact]
        public void InvalidPivotIdx()
        {
            int* ptr = stackalloc int[] { 1, 2, 3 };
            int result = Partition.Run(ptr, 3, 10);
            Assert.Equal(-1, result);
        }
    }

    public sealed unsafe class NthElementTests
    {
        [Fact]
        public void TryGetNthElement_Found()
        {
            int* ptr = stackalloc int[] { 3, 1, 4, 1, 5, 9 };
            int val;
            bool found = Partition.TryGetNthElement(ptr, 6, 2, out val);
            Assert.True(found);
            Assert.Equal(3, val);
        }

        [Fact]
        public void TryGetNthElement_NotFound()
        {
            int* ptr = stackalloc int[] { 1, 2, 3 };
            int val;
            bool found = Partition.TryGetNthElement(ptr, 3, 10, out val);
            Assert.False(found);
        }
    }
}