namespace IAFahim.DS.SpatialMap.Tests
{
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed class SpatialMapUtilityTests
    {
        [Test]
        public void Quantized_Origin_IsHalfSize()
        {
            // position 0 maps to halfSize cell when step=1
            int2 half = new int2(10, 10);
            int2 q = SpatialMapUtility.Quantized(new float2(0, 0), 1f, half);
            Assert.AreEqual(10, q.x);
            Assert.AreEqual(10, q.y);
        }

        [Test]
        public void Quantized_BelowWorld_IsNegative()
        {
            int2 half = new int2(5, 5);
            int2 q = SpatialMapUtility.Quantized(new float2(-6, -6), 1f, half);
            Assert.IsTrue(q.x < 0 || q.y < 0);
        }

        [Test]
        public void Quantized_AboveWorld_ExceedsWidth()
        {
            int2 half = new int2(5, 5);
            int width = half.x * 2;
            int2 q = SpatialMapUtility.Quantized(new float2(6, 6), 1f, half);
            Assert.IsTrue(q.x >= width || q.y >= width);
        }

        [Test]
        public void Hash_RoundTrip_InBounds()
        {
            int2 q = new int2(3, 4);
            int width = 16;
            int h = SpatialMapUtility.Hash(q, width);
            Assert.AreEqual(4 * 16 + 3, h);
        }

        [Test]
        public void SpatialMap3_Quantized_LowerAndUpperBounds()
        {
            int3 half = new int3(4, 4, 4);
            int3 qLo = SpatialMapUtility3.Quantized(new float3(-5, 0, 0), 1f, half);
            Assert.IsTrue(qLo.x < 0);
            int3 qHi = SpatialMapUtility3.Quantized(new float3(5, 0, 0), 1f, half);
            Assert.IsTrue(qHi.x >= half.x * 2);
        }

        [Test]
        public void SpatialKeyedMap_Quantized_MatchesUtility()
        {
            int2 half = new int2(8, 8);
            float2 p = new float2(1.5f, -2.5f);
            int2 a = SpatialKeyedMap.Quantized(p, 1f, half);
            int2 b = SpatialMapUtility.Quantized(p, 1f, half);
            Assert.AreEqual(b.x, a.x);
            Assert.AreEqual(b.y, a.y);
        }

        [Test]
        public void Hex_IsWithinBounds_RejectsOutside()
        {
            int2 min = new int2(0, 0);
            int2 size = new int2(4, 4);
            Assert.IsTrue(SpatialHexMap.IsWithinBounds(new int2(1, 1), min, size));
            Assert.IsFalse(SpatialHexMap.IsWithinBounds(new int2(-1, 0), min, size));
            Assert.IsFalse(SpatialHexMap.IsWithinBounds(new int2(4, 0), min, size));
        }

        [Test]
        public void Hex_Quantized_CenterNearZero()
        {
            int2 q = SpatialHexMap.Quantized(new float2(0, 0), 1f);
            Assert.AreEqual(0, q.x);
            Assert.AreEqual(0, q.y);
        }
    }
}
