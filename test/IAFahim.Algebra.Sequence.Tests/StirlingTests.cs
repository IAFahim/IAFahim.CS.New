namespace IAFahim.Algebra.Sequence.Tests
{
    using NUnit.Framework;

    public sealed unsafe class StirlingTests
    {
        private const int MOD = 1000000007;

        // Signed Stirling first kind s(n,k). n=3 row: s(3,1)=2, s(3,2)=-3, s(3,3)=1.
        [Test]
        public void First_Signed_n3()
        {
            Assert.AreEqual(2, Stirling.First(3, 1, MOD));
            Assert.AreEqual(MOD - 3, Stirling.First(3, 2, MOD));
            Assert.AreEqual(1, Stirling.First(3, 3, MOD));
        }

        [Test]
        public void First_n4()
        {
            // s(4,1)=-6, s(4,2)=11, s(4,3)=-6, s(4,4)=1.
            Assert.AreEqual(MOD - 6, Stirling.First(4, 1, MOD));
            Assert.AreEqual(11, Stirling.First(4, 2, MOD));
            Assert.AreEqual(MOD - 6, Stirling.First(4, 3, MOD));
            Assert.AreEqual(1, Stirling.First(4, 4, MOD));
        }

        [Test]
        public void Second_n3_131()
        {
            // S(3,1)=1, S(3,2)=3, S(3,3)=1.
            Assert.AreEqual(1, Stirling.Second(3, 1, MOD));
            Assert.AreEqual(3, Stirling.Second(3, 2, MOD));
            Assert.AreEqual(1, Stirling.Second(3, 3, MOD));
        }

        [Test]
        public void Second_n4_1761()
        {
            // S(4,1)=1, S(4,2)=7, S(4,3)=6, S(4,4)=1.
            Assert.AreEqual(1, Stirling.Second(4, 1, MOD));
            Assert.AreEqual(7, Stirling.Second(4, 2, MOD));
            Assert.AreEqual(6, Stirling.Second(4, 3, MOD));
            Assert.AreEqual(1, Stirling.Second(4, 4, MOD));
        }

        [Test]
        public void Second_SumOverK_EqualsBellNumber()
        {
            // Sum of S(n,k) over k=1..n = Bell(n). Bell(4)=15.
            long sum = 0;
            for (int k = 1; k <= 4; k++) sum = (sum + Stirling.Second(4, k, MOD)) % MOD;
            Assert.AreEqual(15, sum);
        }

        [Test]
        public void FirstRow_FillsCorrectLength()
        {
            long* row = stackalloc long[6];
            Stirling.FirstRow(5, MOD, row);
            Assert.AreEqual(1, row[5]);
        }

        [Test]
        public void SecondRow_FillsCorrectLength()
        {
            long* row = stackalloc long[6];
            Stirling.SecondRow(5, MOD, row);
            Assert.AreEqual(1, row[1]);
            Assert.AreEqual(1, row[5]);
        }
    }
}
