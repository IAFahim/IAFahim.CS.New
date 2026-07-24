namespace IAFahim.Math.Noise.Tests
{
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class PerlinNoiseTests
    {
        [Test]
        public void Noise2D_Deterministic()
        {
            float2 p = new float2(0.5f, 0.25f);
            float a = PerlinNoise.Noise2D(p);
            float b = PerlinNoise.Noise2D(p);
            Assert.AreEqual(a, b);
        }

        [Test]
        public void Noise2D_ReferenceSamples()
        {
            float v00 = PerlinNoise.Noise2D(new float2(0f, 0f));
            float v11 = PerlinNoise.Noise2D(new float2(1f, 1f));
            float vHalf = PerlinNoise.Noise2D(new float2(0.5f, 0.5f));
            Assert.AreEqual(0f, v00, 1e-5f);
            Assert.AreEqual(0f, v11, 1e-5f);
            Assert.IsTrue(vHalf >= -1f && vHalf <= 1f);
            float locked = PerlinNoise.Noise2D(new float2(2.3f, 4.7f));
            Assert.AreEqual(locked, PerlinNoise.Noise2D(new float2(2.3f, 4.7f)), 0f);
            Assert.AreEqual(0.12955382f, locked, 1e-4f);
        }

        [Test]
        public void Noise2D_Nan_ReturnsNan()
        {
            float v = PerlinNoise.Noise2D(new float2(float.NaN, 0f));
            Assert.IsTrue(float.IsNaN(v));
        }
    }
}
