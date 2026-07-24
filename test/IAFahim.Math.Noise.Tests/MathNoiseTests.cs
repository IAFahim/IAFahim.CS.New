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
            Assert.IsTrue(a >= -1.5f && a <= 1.5f);
        }
    }
}
