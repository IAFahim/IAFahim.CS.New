namespace IAFahim.DS.HilbertOrder.Tests
{
    using IAFahim.DS.HilbertOrder;
    using NUnit.Framework;

    public sealed unsafe class HilbertOrderTests
    {
        [Test]
        public void Encode_SamePointSameOrder()
        {
            long ord1 = HilbertOrder.Encode(5, 5, 10);
            long ord2 = HilbertOrder.Encode(5, 5, 10);
            Assert.AreEqual(ord1, ord2);
        }

        [Test]
        public void Encode_DifferentPointsDifferentOrder()
        {
            long ord1 = HilbertOrder.Encode(0, 0, 10);
            long ord2 = HilbertOrder.Encode(1, 0, 10);
            Assert.AreNotEqual(ord1, ord2);
        }

        [Test]
        public void Encode_Bounds()
        {
            long maxOrd = 1L << 20;
            long ord = HilbertOrder.Encode(1023, 1023, 10);
            Assert.IsTrue(ord >= 0 && ord < maxOrd);
        }

        [Test]
        public void Encode_UniqueAfterSwapped()
        {
            long ord1 = HilbertOrder.SwappedEncode(3, 7, 10);
            long ord2 = HilbertOrder.SwappedEncode(7, 3, 10);
            Assert.AreEqual(ord1, ord2);
        }

        [Test]
        public void Encode_BorderValues()
        {
            long ord = HilbertOrder.Encode(0, 1023, 10);
            Assert.IsTrue(ord >= 0);
            ord = HilbertOrder.Encode(1023, 0, 10);
            Assert.IsTrue(ord >= 0);
        }

        [Test]
        public void Encode_FullGridUniqueAndInRange()
        {
            const int LogN = 4;
            const int Side = 1 << LogN;
            const int Cells = Side * Side;
            bool* seen = stackalloc bool[Cells];
            for (int i = 0; i < Cells; i++) seen[i] = false;
            for (int x = 0; x < Side; x++)
            {
                for (int y = 0; y < Side; y++)
                {
                    long d = HilbertOrder.Encode(x, y, LogN);
                    Assert.IsTrue(d >= 0 && d < Cells);
                    Assert.IsFalse(seen[(int)d]);
                    seen[(int)d] = true;
                }
            }
            for (int i = 0; i < Cells; i++) Assert.IsTrue(seen[i]);
        }

        [Test]
        public void Run_Rot0_MatchesEncode()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Assert.AreEqual(HilbertOrder.Encode(x, y, 3), HilbertOrder.Run(x, y, 3, 0));
                }
            }
        }
    }

    public sealed unsafe class GilbertOrderTests
    {
        [Test]
        public void Encode_Basic()
        {
            long ord = GilbertOrder.Encode(0, 0, 4, 4);
            Assert.IsTrue(ord >= 0);
        }

        [Test]
        public void Encode_DifferentPoints()
        {
            long ord1 = GilbertOrder.Encode(0, 0, 8, 8);
            long ord2 = GilbertOrder.Encode(1, 1, 8, 8);
            Assert.AreNotEqual(ord1, ord2);
        }

        [Test]
        public void Encode_ZeroWidth()
        {
            long ord = GilbertOrder.Encode(0, 0, 0, 5);
            Assert.AreEqual(0, ord);
        }

        [Test]
        public void Encode_SquarePowerOfTwo_MatchesHilbert()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Assert.AreEqual(HilbertOrder.Encode(x, y, 3), GilbertOrder.Encode(x, y, 8, 8));
                }
            }
        }
    }

    public sealed unsafe class BlockOrderTests
    {
        [Test]
        public void Encode_Basic()
        {
            long ord = BlockOrder.Encode(0, 10, 4);
            Assert.IsTrue(ord >= 0);
        }

        [Test]
        public void Encode_MultipleBlocks()
        {
            long ord1 = BlockOrder.Encode(0, 3, 2);
            long ord2 = BlockOrder.Encode(4, 7, 2);
            Assert.IsTrue(ord2 > ord1);
        }

        [Test]
        public void Decode_RoundTripsR_EvenAndOddBlocks()
        {
            int l = 0, r = 0;
            long codeEven = BlockOrder.Encode(0, 12, 4);
            BlockOrder.Decode(codeEven, 100, 4, &l, &r);
            Assert.AreEqual(0, l);
            Assert.AreEqual(12, r);

            long codeOdd = BlockOrder.Encode(5, 17, 4);
            BlockOrder.Decode(codeOdd, 100, 4, &l, &r);
            Assert.AreEqual(4, l);
            Assert.AreEqual(17, r);
        }
    }
}