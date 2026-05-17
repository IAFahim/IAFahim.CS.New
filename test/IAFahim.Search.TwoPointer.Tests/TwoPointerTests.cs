namespace IAFahim.Search.TwoPointer.Tests
{
    using Xunit;

    public sealed unsafe class TwoPointersTests
    {
        [Fact]
        public void CountPairsWithSum()
        {
            int* a = stackalloc int[] { 1, 2, 4 };
            int* b = stackalloc int[] { 1, 3, 5 };
            int count = TwoPointers.CountPairsWithSum(a, 3, b, 3, 5);
            Assert.Equal(2, count);
        }

        [Fact]
        public void HasPairWithSum_Found()
        {
            int* a = stackalloc int[] { 1, 2, 4 };
            int* b = stackalloc int[] { 1, 3, 5 };
            bool found = TwoPointers.HasPairWithSum(a, 3, b, 3, 5);
            Assert.True(found);
        }

        [Fact]
        public void HasPairWithSum_NotFound()
        {
            int* a = stackalloc int[] { 1, 2, 4 };
            int* b = stackalloc int[] { 1, 3 };
            bool found = TwoPointers.HasPairWithSum(a, 3, b, 2, 10);
            Assert.False(found);
        }
    }
}