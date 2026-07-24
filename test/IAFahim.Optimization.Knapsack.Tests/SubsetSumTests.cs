namespace IAFahim.Optimization.Knapsack.Tests
{
    using NUnit.Framework;

    public sealed unsafe class SubsetSumTests
    {
        [Test]
        public void EmptyTargetZero_True()
        {
            Assert.IsTrue(SubsetSum.Can(null, 0, 0));
        }

        [Test]
        public void EmptyTargetPositive_False()
        {
            Assert.IsFalse(SubsetSum.Can(null, 0, 5));
        }

        [Test]
        public void SmallTarget_BitsetPath()
        {
            long* w = stackalloc long[3];
            w[0] = 3; w[1] = 5; w[2] = 7;
            Assert.IsTrue(SubsetSum.Can(w, 3, 8));
            Assert.IsTrue(SubsetSum.Can(w, 3, 12));
            Assert.IsFalse(SubsetSum.Can(w, 3, 4));
            Assert.IsTrue(SubsetSum.Can(w, 3, 0));
        }

        [Test]
        public void LargeTarget_MultiWordPath()
        {
            long* w = stackalloc long[4];
            w[0] = 40; w[1] = 50; w[2] = 60; w[3] = 70;
            Assert.IsTrue(SubsetSum.Can(w, 4, 110));
            Assert.IsTrue(SubsetSum.Can(w, 4, 220));
            Assert.IsFalse(SubsetSum.Can(w, 4, 45));
        }

        [Test]
        public void TargetExactly63_SingleWord()
        {
            long* w = stackalloc long[2];
            w[0] = 30;
            w[1] = 33;
            Assert.IsTrue(SubsetSum.Can(w, 2, 63));
            Assert.IsFalse(SubsetSum.Can(w, 2, 62));
        }
    }
}
