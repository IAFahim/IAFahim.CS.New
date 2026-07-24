namespace IAFahim.DS.FixedCollections.Tests
{
    using NUnit.Framework;

    public sealed unsafe class FixedBitMaskTests
    {
        [Test]
        public void SetIsSet_RoundTrip()
        {
            FixedBitMask<ulong> mask = default;
            mask.Reset();
            Assert.IsFalse(mask.IsSet(3));
            mask.Set(3, true);
            Assert.IsTrue(mask.IsSet(3));
            mask.Set(3, false);
            Assert.IsFalse(mask.IsSet(3));
        }
    }
}
