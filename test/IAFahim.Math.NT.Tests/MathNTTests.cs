namespace IAFahim.Math.NT.Tests
{
    using IAFahim.Math.NT;
    using Xunit;

    public sealed unsafe class MathNTTests
    {
        [Fact]
        public void Gcd_Basic()
        {
            Assert.Equal(6, Gcd.Run(18, 12));
            Assert.Equal(6, Gcd.Run(12, 18));
            Assert.Equal(7, Gcd.Run(0, 7));
            Assert.Equal(7, Gcd.Run(7, 0));
            Assert.Equal(1, Gcd.Run(1, 1));
        }

        [Fact]
        public void IsPrime_SmallNumbers()
        {
            Assert.False(IsPrime.Run(0));
            Assert.False(IsPrime.Run(1));
            Assert.True(IsPrime.Run(2));
            Assert.True(IsPrime.Run(3));
            Assert.False(IsPrime.Run(4));
            Assert.True(IsPrime.Run(5));
            Assert.False(IsPrime.Run(9));
            Assert.True(IsPrime.Run(17));
            Assert.False(IsPrime.Run(25));
        }

        [Fact]
        public void MillerRabin_Basic()
        {
            Assert.True(MillerRabin.IsPrime(2));
            Assert.True(MillerRabin.IsPrime(3));
            Assert.True(MillerRabin.IsPrime(17));
            Assert.True(MillerRabin.IsPrime(997));
            Assert.False(MillerRabin.IsPrime(4));
            Assert.False(MillerRabin.IsPrime(15));
        }

        [Fact]
        public void Factorize_Basic()
        {
            int* prime = stackalloc int[10];
            int* exp = stackalloc int[10];
            int cnt = Factorize.Run(12, prime, exp);
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
            Assert.Equal(2, HighestBit.Run(3));
            Assert.Equal(8, HighestBit.Run(15));
            Assert.Equal(16, HighestBit.Run(17));
        }

        [Fact]
        public void FloorSum_Basic()
        {
            long result = FloorSum.Run(10, 1, 1, 0, 100);
            Assert.True(result >= 0);
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