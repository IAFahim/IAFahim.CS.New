namespace IAFahim.Search.MeetInMiddle.Tests
{
    using Xunit;

    public sealed unsafe class MeetInMiddleTests
    {
        [Fact]
        public void SubsetSumCount()
        {
            int* values = stackalloc int[] { 1, 2, 3 };
            int count = MeetInMiddle.SubsetSumCount(values, 3, 5);
            Assert.True(count > 0);
        }

        [Fact]
        public void HasSubsetSum_True()
        {
            int* values = stackalloc int[] { 1, 2, 3, 4 };
            bool result = MeetInMiddle.HasSubsetSum(values, 4, 7);
            Assert.True(result);
        }

        [Fact]
        public void HasSubsetSum_False()
        {
            int* values = stackalloc int[] { 1, 2, 3 };
            bool result = MeetInMiddle.HasSubsetSum(values, 3, 10);
            Assert.False(result);
        }
    }
}