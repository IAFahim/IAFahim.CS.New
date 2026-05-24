namespace IAFahim.Search.MeetInMiddle.Tests
{
    using NUnit.Framework;

    public sealed unsafe class MeetInMiddleTests
    {
        [Test]
        public void SubsetSumCount()
        {
            int* values = stackalloc int[] { 1, 2, 3 };
            int count = MeetInMiddle.SubsetSumCount(values, 3, 5);
            Assert.IsTrue(count > 0);
        }

        [Test]
        public void HasSubsetSum_True()
        {
            int* values = stackalloc int[] { 1, 2, 3, 4 };
            bool result = MeetInMiddle.HasSubsetSum(values, 4, 7);
            Assert.IsTrue(result);
        }

        [Test]
        public void HasSubsetSum_False()
        {
            int* values = stackalloc int[] { 1, 2, 3 };
            bool result = MeetInMiddle.HasSubsetSum(values, 3, 10);
            Assert.IsFalse(result);
        }
    }
}