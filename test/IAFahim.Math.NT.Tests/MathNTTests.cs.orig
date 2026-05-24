namespace IAFahim.Math.NT.Tests
{
    using IAFahim.Math.NT;
    using Xunit;

    public sealed unsafe class MathNTTests
    {
        [Fact]
        public void Gcd_Basic()
        {
            // We use long GCD from IAFahim.Math.NT if it exists, or modular one.
            // PollardRho and others have a private GCD.
            // Let's use the one from IAFahim.Math.Modular for testing here.
            Assert.Equal(6, IAFahim.Math.Modular.Gcd.Run(18, 12));
            Assert.Equal(6, IAFahim.Math.Modular.Gcd.Run(12, 18));
            Assert.Equal(7, IAFahim.Math.Modular.Gcd.Run(0, 7));
            Assert.Equal(7, IAFahim.Math.Modular.Gcd.Run(7, 0));
            Assert.Equal(1, IAFahim.Math.Modular.Gcd.Run(1, 1));
        }

        [Fact]
        public void MillerRabin_Basic()
        {
            Assert.True(MillerRabin.Run(2));
            Assert.True(MillerRabin.Run(3));
            Assert.True(MillerRabin.Run(17));
            Assert.True(MillerRabin.Run(997));
            Assert.False(MillerRabin.Run(4));
            Assert.False(MillerRabin.Run(15));
        }

        [Fact]
        public void Factorize_Basic()
        {
            long* prime = stackalloc long[10];
            int cnt = Factorize.Run(12, prime);
            Assert.True(cnt >= 2);
        }

        [Fact]
        public void Phi_Basic()
        {
            Assert.Equal(1, Phi.Run(1));
            Assert.Equal(1, Phi.Run(2));
            Assert.Equal(2, Phi.Run(3));
            Assert.Equal(4, Phi.Run(8));
            Assert.Equal(6, Phi.Run(7));
        }

        [Fact]
        public void BitCount_Basic()
        {
            Assert.Equal(1, BitCount.Run(1));
            Assert.Equal(1, BitCount.Run(2));
            Assert.Equal(2, BitCount.Run(3));
            Assert.Equal(1, BitCount.Run(8));
            Assert.Equal(4, BitCount.Run(15));
        }

        [Fact]
        public void HighestBit_Basic()
        {
            Assert.Equal(1, HighestBit.Run(1));
            Assert.Equal(2, HighestBit.Run(3)); // Highest bit of 3 (0011) is 2 (0010)
            Assert.Equal(8, HighestBit.Run(15));
            Assert.Equal(16, HighestBit.Run(17));
        }

        [Fact]
        public void Mobius_Basic()
        {
            Assert.Equal(1, Mobius.Run(1));
            Assert.Equal(-1, Mobius.Run(2));
            Assert.Equal(-1, Mobius.Run(3));
            Assert.Equal(0, Mobius.Run(4));
            Assert.Equal(-1, Mobius.Run(5));
            Assert.Equal(1, Mobius.Run(6));
        }
    }
}
