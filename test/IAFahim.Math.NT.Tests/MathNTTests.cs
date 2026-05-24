namespace IAFahim.Math.NT.Tests
{
    using IAFahim.Math.NT;
    using NUnit.Framework;

    public sealed unsafe class MathNTTests
    {
        [Test]
        public void Gcd_Basic()
        {
            // We use long GCD from IAFahim.Math.NT if it exists, or modular one.
            // PollardRho and others have a private GCD.
            // Let's use the one from IAFahim.Math.Modular for testing here.
            Assert.AreEqual(6, IAFahim.Math.Modular.Gcd.Run(18, 12));
            Assert.AreEqual(6, IAFahim.Math.Modular.Gcd.Run(12, 18));
            Assert.AreEqual(7, IAFahim.Math.Modular.Gcd.Run(0, 7));
            Assert.AreEqual(7, IAFahim.Math.Modular.Gcd.Run(7, 0));
            Assert.AreEqual(1, IAFahim.Math.Modular.Gcd.Run(1, 1));
        }

        [Test]
        public void MillerRabin_Basic()
        {
            Assert.IsTrue(MillerRabin.Run(2));
            Assert.IsTrue(MillerRabin.Run(3));
            Assert.IsTrue(MillerRabin.Run(17));
            Assert.IsTrue(MillerRabin.Run(997));
            Assert.IsFalse(MillerRabin.Run(4));
            Assert.IsFalse(MillerRabin.Run(15));
        }

        [Test]
        public void Factorize_Basic()
        {
            long* prime = stackalloc long[10];
            int cnt = Factorize.Run(12, prime);
            Assert.IsTrue(cnt >= 2);
        }

        [Test]
        public void Phi_Basic()
        {
            Assert.AreEqual(1, Phi.Run(1));
            Assert.AreEqual(1, Phi.Run(2));
            Assert.AreEqual(2, Phi.Run(3));
            Assert.AreEqual(4, Phi.Run(8));
            Assert.AreEqual(6, Phi.Run(7));
        }

        [Test]
        public void BitCount_Basic()
        {
            Assert.AreEqual(1, BitCount.Run(1));
            Assert.AreEqual(1, BitCount.Run(2));
            Assert.AreEqual(2, BitCount.Run(3));
            Assert.AreEqual(1, BitCount.Run(8));
            Assert.AreEqual(4, BitCount.Run(15));
        }

        [Test]
        public void HighestBit_Basic()
        {
            Assert.AreEqual(1, HighestBit.Run(1));
            Assert.AreEqual(2, HighestBit.Run(3)); // Highest bit of 3 (0011) is 2 (0010)
            Assert.AreEqual(8, HighestBit.Run(15));
            Assert.AreEqual(16, HighestBit.Run(17));
        }

        [Test]
        public void Mobius_Basic()
        {
            Assert.AreEqual(1, Mobius.Run(1));
            Assert.AreEqual(-1, Mobius.Run(2));
            Assert.AreEqual(-1, Mobius.Run(3));
            Assert.AreEqual(0, Mobius.Run(4));
            Assert.AreEqual(-1, Mobius.Run(5));
            Assert.AreEqual(1, Mobius.Run(6));
        }
    }
}
