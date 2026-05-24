namespace IAFahim.Search.TwoPointer.Tests
{
    using NUnit.Framework;

    public sealed unsafe class TwoPointersTests
    {
        [Test]
        public void CountPairsWithSum()
        {
            int* a = stackalloc int[] { 1, 2, 4 };
            int* b = stackalloc int[] { 1, 3, 5 };
            int count = TwoPointers.CountPairsWithSum(a, 3, b, 3, 5);
            Assert.AreEqual(2, count);
        }

        [Test]
        public void HasPairWithSum_Found()
        {
            int* a = stackalloc int[] { 1, 2, 4 };
            int* b = stackalloc int[] { 1, 3, 5 };
            bool found = TwoPointers.HasPairWithSum(a, 3, b, 3, 5);
            Assert.IsTrue(found);
        }

        [Test]
        public void HasPairWithSum_NotFound()
        {
            int* a = stackalloc int[] { 1, 2, 4 };
            int* b = stackalloc int[] { 1, 3 };
            bool found = TwoPointers.HasPairWithSum(a, 3, b, 2, 10);
            Assert.IsFalse(found);
        }
    }
}