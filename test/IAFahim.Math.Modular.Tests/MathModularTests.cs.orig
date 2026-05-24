namespace IAFahim.Math.Modular.Tests
{
    using Xunit;

    public sealed unsafe class GcdTests
    {
        [Fact]
        public void Normal() { Assert.Equal(6, Gcd.Run(18, 24)); }

        [Fact]
        public void Coprime() { Assert.Equal(1, Gcd.Run(7, 13)); }

        [Fact]
        public void OneIsZero() { Assert.Equal(5, Gcd.Run(5, 0)); }

        [Fact]
        public void BothZero() { Assert.Equal(0, Gcd.Run(0, 0)); }

        [Fact]
        public void Negative() { Assert.Equal(3, Gcd.Run(-12, 9)); }
    }

    public sealed unsafe class LcmTests
    {
        [Fact]
        public void Normal() { Assert.Equal(72, Lcm.Run(18, 24)); }

        [Fact]
        public void Coprime() { Assert.Equal(91, Lcm.Run(7, 13)); }

        [Fact]
        public void OneIsZero() { Assert.Equal(0, Lcm.Run(5, 0)); }
    }

    public sealed unsafe class ExtendedGcdTests
    {
        [Fact]
        public void Normal()
        {
            long x, y;
            long g = ExtendedGcd.Run(18, 24, out x, out y);
            Assert.Equal(6, g);
            Assert.Equal(6, 18 * x + 24 * y);
        }

        [Fact]
        public void Coprime()
        {
            long x, y;
            long g = ExtendedGcd.Run(7, 13, out x, out y);
            Assert.Equal(1, g);
            Assert.Equal(1, 7 * x + 13 * y);
        }
    }

    public sealed unsafe class ModNormalizeTests
    {
        [Fact]
        public void Positive() { Assert.Equal(3, ModNormalize.Run(3, 7)); }

        [Fact]
        public void Negative() { Assert.Equal(5, ModNormalize.Run(-2, 7)); }

        [Fact]
        public void Zero() { Assert.Equal(0, ModNormalize.Run(0, 7)); }
    }

    public sealed unsafe class ModAddTests
    {
        [Fact]
        public void Normal() { Assert.Equal(5, ModAdd.Run(2, 3, 7)); }

        [Fact]
        public void Overflow() { Assert.Equal(3, ModAdd.Run(5, 5, 7)); }
    }

    public sealed unsafe class ModSubTests
    {
        [Fact]
        public void Normal() { Assert.Equal(1, ModSub.Run(5, 4, 7)); }

        [Fact]
        public void Underflow() { Assert.Equal(4, ModSub.Run(2, 5, 7)); }
    }

    public sealed unsafe class ModMulTests
    {
        [Fact]
        public void Normal() { Assert.Equal(6, ModMul.Run(2, 3, 7)); }

        [Fact]
        public void Zero() { Assert.Equal(0, ModMul.Run(0, 5, 7)); }
    }

    public sealed unsafe class ModPowTests
    {
        [Fact]
        public void Square() { Assert.Equal(4, ModPow.Run(2, 2, 7)); }

        [Fact]
        public void Cube() { Assert.Equal(6, ModPow.Run(3, 3, 7)); }

        [Fact]
        public void One() { Assert.Equal(1, ModPow.Run(5, 0, 7)); }
    }

    public sealed unsafe class ModInvTests
    {
        [Fact]
        public void Normal() { Assert.Equal(3, ModInv.Run(5, 7)); }

        [Fact]
        public void Prime() { Assert.Equal(2, ModInv.Run(4, 7)); }

        [Fact]
        public void NoInverse() { Assert.Equal(-1, ModInv.Run(2, 4)); }
    }

    public sealed unsafe class CrtTests
    {
        [Fact]
        public void Normal()
        {
            long result = Crt.Run(2, 3, 2, 4);
            Assert.NotEqual(-1, result);
            Assert.Equal(2, result % 3);
            Assert.Equal(2, result % 4);
        }

        [Fact]
        public void SameMod()
        {
            long result = Crt.Run(1, 5, 3, 5);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void LargeMod()
        {
            long result = Crt.Run(2, 1000000007L, 3, 1000000009L);
            Assert.NotEqual(-1, result);
            Assert.Equal(2, result % 1000000007L);
            Assert.Equal(3, result % 1000000009L);
        }
    }

    public sealed unsafe class ExcrtTests
    {
        [Fact]
        public void Empty()
        {
            long result = Excrt.Run(null, null, 0);
            Assert.Equal(0, result);
        }

        [Fact]
        public void Normal()
        {
            long* remainders = stackalloc long[3] { 2, 3, 2 };
            long* moduli = stackalloc long[3] { 3, 5, 7 };
            long result = Excrt.Run(remainders, moduli, 3);
            Assert.NotEqual(-1, result);
            Assert.Equal(2, result % 3);
            Assert.Equal(3, result % 5);
            Assert.Equal(2, result % 7);
            Assert.Equal(23, result);
        }

        [Fact]
        public void NoSolution()
        {
            long* remainders = stackalloc long[2] { 1, 2 };
            long* moduli = stackalloc long[2] { 4, 6 };
            long result = Excrt.Run(remainders, moduli, 2);
            Assert.Equal(-1, result);
        }
    }
}