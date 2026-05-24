namespace IAFahim.Unique.Tests
{
    using NUnit.Framework;

    public sealed unsafe class UniqueInt64sTests
    {
        [Test]
        public void EmptyInput_ReturnsZero()
        {
            int count = IAFahim.Unique.UniqueInt64s.Run(null, 0);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void SingleElement_ReturnsOne()
        {
            long* ptr = stackalloc long[] { 42L };
            int count = IAFahim.Unique.UniqueInt64s.Run(ptr, 1);
            Assert.AreEqual(1, count);
            Assert.AreEqual(42L, ptr[0]);
        }

        [Test]
        public void AllUnique_ReturnsLen()
        {
            long* ptr = stackalloc long[] { 1, 2, 3 };
            int count = IAFahim.Unique.UniqueInt64s.Run(ptr, 3);
            Assert.AreEqual(3, count);
        }

        [Test]
        public void AllDuplicates_ReturnsOne()
        {
            long* ptr = stackalloc long[] { 7, 7, 7 };
            int count = IAFahim.Unique.UniqueInt64s.Run(ptr, 3);
            Assert.AreEqual(1, count);
            Assert.AreEqual(7, ptr[0]);
        }

        [Test]
        public void SomeDuplicates_RemovesDuplicates()
        {
            long* ptr = stackalloc long[] { 1, 1, 2, 3, 3 };
            int count = IAFahim.Unique.UniqueInt64s.Run(ptr, 5);
            Assert.AreEqual(3, count);
            Assert.AreEqual(1, ptr[0]);
            Assert.AreEqual(2, ptr[1]);
            Assert.AreEqual(3, ptr[2]);
        }
    }
}