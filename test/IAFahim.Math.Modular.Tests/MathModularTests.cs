namespace IAFahim.Math.Modular.Tests
{
    using NUnit.Framework;

    public sealed unsafe class GcdTests
    {
        [Test]
        public void Normal() { Assert.AreEqual(6, Gcd.Run(18, 24)); }

        [Test]
        public void Coprime() { Assert.AreEqual(1, Gcd.Run(7, 13)); }

        [Test]
        public void OneIsZero() { Assert.AreEqual(5, Gcd.Run(5, 0)); }

        [Test]
        public void BothZero() { Assert.AreEqual(0, Gcd.Run(0, 0)); }

        [Test]
        public void Negative() { Assert.AreEqual(3, Gcd.Run(-12, 9)); }
    }

    public sealed unsafe class LcmTests
    {
        [Test]
        public void Normal() { Assert.AreEqual(72, Lcm.Run(18, 24)); }

        [Test]
        public void Coprime() { Assert.AreEqual(91, Lcm.Run(7, 13)); }

        [Test]
        public void OneIsZero() { Assert.AreEqual(0, Lcm.Run(5, 0)); }
    }

    public sealed unsafe class ExtendedGcdTests
    {
        [Test]
        public void Normal()
        {
            long x, y;
            long g = ExtendedGcd.Run(18, 24, out x, out y);
            Assert.AreEqual(6, g);
            Assert.AreEqual(6, 18 * x + 24 * y);
        }

        [Test]
        public void Coprime()
        {
            long x, y;
            long g = ExtendedGcd.Run(7, 13, out x, out y);
            Assert.AreEqual(1, g);
            Assert.AreEqual(1, 7 * x + 13 * y);
        }
    }

    public sealed unsafe class ModNormalizeTests
    {
        [Test]
        public void Positive() { Assert.AreEqual(3, ModNormalize.Run(3, 7)); }

        [Test]
        public void Negative() { Assert.AreEqual(5, ModNormalize.Run(-2, 7)); }

        [Test]
        public void Zero() { Assert.AreEqual(0, ModNormalize.Run(0, 7)); }
    }

    public sealed unsafe class ModAddTests
    {
        [Test]
        public void Normal() { Assert.AreEqual(5, ModAdd.Run(2, 3, 7)); }

        [Test]
        public void Overflow() { Assert.AreEqual(3, ModAdd.Run(5, 5, 7)); }
    }

    public sealed unsafe class ModSubTests
    {
        [Test]
        public void Normal() { Assert.AreEqual(1, ModSub.Run(5, 4, 7)); }

        [Test]
        public void Underflow() { Assert.AreEqual(4, ModSub.Run(2, 5, 7)); }
    }

    public sealed unsafe class ModMulTests
    {
        [Test]
        public void Normal() { Assert.AreEqual(6, ModMul.Run(2, 3, 7)); }

        [Test]
        public void Zero() { Assert.AreEqual(0, ModMul.Run(0, 5, 7)); }
    }

    public sealed unsafe class ModPowTests
    {
        [Test]
        public void Square() { Assert.AreEqual(4, ModPow.Run(2, 2, 7)); }

        [Test]
        public void Cube() { Assert.AreEqual(6, ModPow.Run(3, 3, 7)); }

        [Test]
        public void One() { Assert.AreEqual(1, ModPow.Run(5, 0, 7)); }
    }

    public sealed unsafe class ModInvTests
    {
        [Test]
        public void Normal() { Assert.AreEqual(3, ModInv.Run(5, 7)); }

        [Test]
        public void Prime() { Assert.AreEqual(2, ModInv.Run(4, 7)); }

        [Test]
        public void NoInverse() { Assert.AreEqual(-1, ModInv.Run(2, 4)); }
    }

    public sealed unsafe class CrtTests
    {
        [Test]
        public void Normal()
        {
            long result = Crt.Run(2, 3, 2, 4);
            Assert.AreNotEqual(-1, result);
            Assert.AreEqual(2, result % 3);
            Assert.AreEqual(2, result % 4);
        }

        [Test]
        public void SameMod()
        {
            long result = Crt.Run(1, 5, 3, 5);
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void LargeMod()
        {
            long result = Crt.Run(2, 1000000007L, 3, 1000000009L);
            Assert.AreNotEqual(-1, result);
            Assert.AreEqual(2, result % 1000000007L);
            Assert.AreEqual(3, result % 1000000009L);
        }
    }

    public sealed unsafe class ExcrtTests
    {
        [Test]
        public void Empty()
        {
            long result = Excrt.Run(null, null, 0);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Normal()
        {
            long* remainders = stackalloc long[3] { 2, 3, 2 };
            long* moduli = stackalloc long[3] { 3, 5, 7 };
            long result = Excrt.Run(remainders, moduli, 3);
            Assert.AreNotEqual(-1, result);
            Assert.AreEqual(2, result % 3);
            Assert.AreEqual(3, result % 5);
            Assert.AreEqual(2, result % 7);
            Assert.AreEqual(23, result);
        }

        [Test]
        public void NoSolution()
        {
            long* remainders = stackalloc long[2] { 1, 2 };
            long* moduli = stackalloc long[2] { 4, 6 };
            long result = Excrt.Run(remainders, moduli, 2);
            Assert.AreEqual(-1, result);
        }
    }
}