namespace IAFahim.DS.HilbertOrder.Tests
{
    using IAFahim.DS.HilbertOrder;
    using Xunit;

    public sealed unsafe class HilbertOrderTests
    {
        [Fact]
        public void Encode_SamePointSameOrder()
        {
            long ord1 = HilbertOrder.Encode(5, 5, 10);
            long ord2 = HilbertOrder.Encode(5, 5, 10);
            Assert.Equal(ord1, ord2);
        }

        [Fact]
        public void Encode_DifferentPointsDifferentOrder()
        {
            long ord1 = HilbertOrder.Encode(0, 0, 10);
            long ord2 = HilbertOrder.Encode(1, 0, 10);
            Assert.NotEqual(ord1, ord2);
        }

        [Fact]
        public void Encode_Bounds()
        {
            long maxOrd = 1L << 20;
            long ord = HilbertOrder.Encode(1023, 1023, 10);
            Assert.True(ord >= 0 && ord < maxOrd);
        }

        [Fact]
        public void Encode_UniqueAfterSwapped()
        {
            long ord1 = HilbertOrder.SwappedEncode(3, 7, 10);
            long ord2 = HilbertOrder.SwappedEncode(7, 3, 10);
            Assert.Equal(ord1, ord2);
        }

        [Fact]
        public void Encode_BorderValues()
        {
            long ord = HilbertOrder.Encode(0, 1023, 10);
            Assert.True(ord >= 0);
            ord = HilbertOrder.Encode(1023, 0, 10);
            Assert.True(ord >= 0);
        }
    }

    public sealed unsafe class GilbertOrderTests
    {
        [Fact]
        public void Encode_Basic()
        {
            long ord = GilbertOrder.Encode(0, 0, 4, 4);
            Assert.True(ord >= 0);
        }

        [Fact]
        public void Encode_DifferentPoints()
        {
            long ord1 = GilbertOrder.Encode(0, 0, 8, 8);
            long ord2 = GilbertOrder.Encode(1, 1, 8, 8);
            Assert.NotEqual(ord1, ord2);
        }

        [Fact]
        public void Encode_ZeroWidth()
        {
            long ord = GilbertOrder.Encode(0, 0, 0, 5);
            Assert.Equal(0, ord);
        }
    }

    public sealed unsafe class BlockOrderTests
    {
        [Fact]
        public void Encode_Basic()
        {
            long ord = BlockOrder.Encode(0, 10, 4);
            Assert.True(ord >= 0);
        }

        [Fact]
        public void Encode_MultipleBlocks()
        {
            long ord1 = BlockOrder.Encode(0, 3, 2);
            long ord2 = BlockOrder.Encode(4, 7, 2);
            Assert.True(ord2 > ord1);
        }

        [Fact]
        public void Decode_Basic()
        {
            int l = 0, r = 0;
            long code = BlockOrder.Encode(5, 12, 4);
            BlockOrder.Decode(code, 100, 4, &l, &r);
            Assert.True(l >= 0);
            Assert.True(r >= l);
        }
    }
}