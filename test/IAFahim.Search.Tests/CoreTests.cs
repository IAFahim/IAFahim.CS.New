namespace IAFahim.Search.Tests
{
    using NUnit.Framework;

    public sealed unsafe class BinarySearchTests
    {
        [Test]
        public void LowerUpperFind_Sorted()
        {
            int* a = stackalloc int[5] { 1, 3, 3, 7, 9 };
            Assert.AreEqual(1, BinarySearch.LowerBound(a, 5, 3));
            Assert.AreEqual(3, BinarySearch.UpperBound(a, 5, 3));
            Assert.AreEqual(3, BinarySearch.Find(a, 5, 7));
            Assert.AreEqual(-1, BinarySearch.Find(a, 5, 2));
            Assert.IsTrue(BinarySearch.TryFind(a, 5, 9, out int ix));
            Assert.AreEqual(4, ix);
        }
    }
}
