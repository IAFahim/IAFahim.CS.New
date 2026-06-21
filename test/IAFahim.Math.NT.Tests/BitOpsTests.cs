namespace IAFahim.Math.NT.Tests
{
    using NUnit.Framework;

    public sealed class BitOpsTests
    {
        [Test]
        public void BitCount_KnownValues()
        {
            Assert.AreEqual(0, BitCount.Run(0));
            Assert.AreEqual(1, BitCount.Run(1));
            Assert.AreEqual(1, BitCount.Run(8));
            Assert.AreEqual(3, BitCount.Run(7));
            Assert.AreEqual(4, BitCount.Run(15));
            Assert.AreEqual(2, BitCount.Run(0b1010));
            Assert.AreEqual(16, BitCount.Run(0x55555555)); // alternating bits, 16 set in 32-bit
            // long overload
            Assert.AreEqual(0, BitCount.Run(0L));
            Assert.AreEqual(32, BitCount.Run((long)0x5555555555555555UL));
        }

        [Test]
        public void BitLength_KnownValues()
        {
            Assert.AreEqual(0, BitLength.Run(0));
            Assert.AreEqual(1, BitLength.Run(1));
            Assert.AreEqual(2, BitLength.Run(2));
            Assert.AreEqual(2, BitLength.Run(3));
            Assert.AreEqual(3, BitLength.Run(4));
            Assert.AreEqual(5, BitLength.Run(20));  // 20 = 0b10100
            Assert.AreEqual(5, BitLength.Run(31));
            Assert.AreEqual(6, BitLength.Run(32));
        }

        [Test]
        public void HighestBit_KnownValues()
        {
            Assert.AreEqual(0, HighestBit.Run(0));
            Assert.AreEqual(1, HighestBit.Run(1));
            Assert.AreEqual(2, HighestBit.Run(2));
            Assert.AreEqual(2, HighestBit.Run(3));
            Assert.AreEqual(4, HighestBit.Run(5));
            Assert.AreEqual(16, HighestBit.Run(20));
            Assert.AreEqual(16, HighestBit.Run(31));
            Assert.AreEqual(32, HighestBit.Run(32));
        }

        [Test]
        public void LowestBit_KnownValues()
        {
            Assert.AreEqual(0, LowestBit.Run(0));
            Assert.AreEqual(1, LowestBit.Run(1));
            Assert.AreEqual(2, LowestBit.Run(2));
            Assert.AreEqual(1, LowestBit.Run(3));
            Assert.AreEqual(4, LowestBit.Run(20));  // 20 = 0b10100
            Assert.AreEqual(8, LowestBit.Run(24));  // 24 = 0b11000
        }

        [Test]
        public void NextBit_StrictlyGreater()
        {
            // NextBit(x) = smallest power of 2 strictly greater than x.
            Assert.AreEqual(0, NextBit.Run(0));  // code returns 0 for x==0
            Assert.AreEqual(2, NextBit.Run(1));
            Assert.AreEqual(4, NextBit.Run(3));
            Assert.AreEqual(8, NextBit.Run(5));
            Assert.AreEqual(32, NextBit.Run(16));  // power of 2: next is 2x
            Assert.AreEqual(32, NextBit.Run(20));
        }

        [Test]
        public void PrevBit_StrictlyLess()
        {
            // PrevBit(x) = largest power of 2 strictly less than x.
            Assert.AreEqual(0, PrevBit.Run(0));
            Assert.AreEqual(0, PrevBit.Run(1));
            Assert.AreEqual(1, PrevBit.Run(2));
            Assert.AreEqual(2, PrevBit.Run(3));
            Assert.AreEqual(4, PrevBit.Run(5));
            Assert.AreEqual(8, PrevBit.Run(16));   // power of 2: prev is half
            Assert.AreEqual(16, PrevBit.Run(20));
            Assert.AreEqual(16, PrevBit.Run(17));
        }

        [Test]
        public void BitReverse_KnownValues()
        {
            Assert.AreEqual(0, BitReverse.Run(0));
            Assert.AreEqual(unchecked((int)0x80000000), BitReverse.Run(1));  // bit 0 -> bit 31
            Assert.AreEqual(1, BitReverse.Run(unchecked((int)0x80000000)));
            Assert.AreEqual(unchecked((int)0x40000000), BitReverse.Run(2));  // bit 1 -> bit 30
        }
    }
}
