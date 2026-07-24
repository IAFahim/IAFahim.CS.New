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
            int* dst = stackalloc int[8];
            int count = EnumerateSupersets.Run(2, 7, dst);
            Assert.AreEqual(4, count);
            bool[] seen = new bool[8];
            for (int i = 0; i < count; i++) seen[dst[i]] = true;
            Assert.IsTrue(seen[2]);
            Assert.IsTrue(seen[3]);
            Assert.IsTrue(seen[6]);
            Assert.IsTrue(seen[7]);
        }

        [Test]
        public void FreeZero_SingleSuperset()
        {
            int* dst = stackalloc int[1];
            int count = EnumerateSupersets.Run(7, 7, dst);
            Assert.AreEqual(1, count);
            Assert.AreEqual(7, dst[0]);
        }

        [Test]
        public void AllFree_PowersOfTwo()
        {
            int* dst = stackalloc int[8];
            int count = EnumerateSupersets.Run(0, 7, dst);
            Assert.AreEqual(8, count);
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

        [Test]
        public void NextWithSamePopCount_AndEnumerateUntil()
        {
            int n = EnumerateMasks.NextWithSamePopCount(0b0101);
            Assert.IsTrue(n > 0b0101);
            int* dst = stackalloc int[4];
            int c = EnumerateSubsets.EnumerateUntil(7, dst, 3);
            Assert.AreEqual(3, c);
        }
    }
}