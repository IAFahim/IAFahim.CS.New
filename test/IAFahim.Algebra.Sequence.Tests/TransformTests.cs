namespace IAFahim.Algebra.Sequence.Tests
{
    using NUnit.Framework;

    public sealed unsafe class TransformTests
    {
        private const int MOD = 1000000007;

        [Test]
        public void Binomial_AllOnes_GivesPowersOfTwo()
        {
            // Binomial transform of [1,1,1,...] = [2^0, 2^1, 2^2, ...].
            long* a = stackalloc long[5] { 1, 1, 1, 1, 1 };
            long* b = stackalloc long[5];
            Transform.Binomial(a, 5, MOD, b);
            Assert.AreEqual(1, b[0]);
            Assert.AreEqual(2, b[1]);
            Assert.AreEqual(4, b[2]);
            Assert.AreEqual(8, b[3]);
            Assert.AreEqual(16, b[4]);
        }

        [Test]
        public void InverseBinomial_AllOnes_GivesUnitImpulse()
        {
            // Inverse binomial of [1,1,1,...] = [1,0,0,...]. (inverse transform undoes
            // binomial([1,0,0,...]) = [1,1,1,...])
            long* a = stackalloc long[5] { 1, 1, 1, 1, 1 };
            long* b = stackalloc long[5];
            Transform.InverseBinomial(a, 5, MOD, b);
            Assert.AreEqual(1, b[0]);
            Assert.AreEqual(0, b[1]);
            Assert.AreEqual(0, b[2]);
            Assert.AreEqual(0, b[3]);
            Assert.AreEqual(0, b[4]);
        }

        [Test]
        public void SetPartition_BellNumbers_1_1_2_5_15()
        {
            Assert.AreEqual(1, Transform.SetPartition(0, MOD));
            Assert.AreEqual(1, Transform.SetPartition(1, MOD));
            Assert.AreEqual(2, Transform.SetPartition(2, MOD));
            Assert.AreEqual(5, Transform.SetPartition(3, MOD));
            Assert.AreEqual(15, Transform.SetPartition(4, MOD));
            Assert.AreEqual(52, Transform.SetPartition(5, MOD));
        }

        [Test]
        public void CayleyCount_LabeledTrees()
        {
            // Cayley's formula: number of labeled trees on n vertices = n^(n-2).
            Assert.AreEqual(1, Transform.CayleyCount(1, MOD));
            Assert.AreEqual(1, Transform.CayleyCount(2, MOD));
            Assert.AreEqual(3, Transform.CayleyCount(3, MOD));
            Assert.AreEqual(16, Transform.CayleyCount(4, MOD));
            Assert.AreEqual(125, Transform.CayleyCount(5, MOD));
        }
    }
}
