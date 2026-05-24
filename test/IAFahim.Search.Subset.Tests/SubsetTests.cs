namespace IAFahim.Search.Subset.Tests
{
    using NUnit.Framework;

    public sealed unsafe class EnumerateSubsetsTests
    {
        [Test]
        public void Count()
        {
            Assert.AreEqual(8, EnumerateSubsets.Count(7));
        }

        [Test]
        public void Run()
        {
            int* dst = stackalloc int[8];
            EnumerateSubsets.Run(7, dst);
            Assert.AreEqual(7, dst[0]);
            Assert.AreEqual(0, dst[7]);
        }
    }

    public sealed unsafe class EnumerateSupersetsTests
    {
        [Test]
        public void Run()
        {
            int* dst = stackalloc int[4];
            int count = EnumerateSupersets.Run(2, 7, dst);
            Assert.AreEqual(3, count);
        }
    }

    public sealed unsafe class EnumerateMasksTests
    {
        [Test]
        public void Count()
        {
            Assert.AreEqual(8, EnumerateMasks.Count(3));
        }

        [Test]
        public void CountPopBits()
        {
            Assert.AreEqual(3, EnumerateMasks.CountPopBits(7));
            Assert.AreEqual(1, EnumerateMasks.CountPopBits(4));
            Assert.AreEqual(0, EnumerateMasks.CountPopBits(0));
        }
    }
}