namespace IAFahim.Geometry.Tests
{
    using System;
    using IAFahim.Math.Noise;
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class NoiseTests
    {
        [Test]
        public void SimplexNoise_Noise2D_ReturnsBoundedValues()
        {
            for (float x = -10.0f; x <= 10.0f; x += 2.5f)
            {
                for (float y = -10.0f; y <= 10.0f; y += 2.5f)
                {
                    float val = SimplexNoise.Noise2D(new float2(x, y));
                    Assert.IsTrue(val >= -1.0f && val <= 1.0f);
                }
            }
        }

        [Test]
        public void PerlinNoise_Noise2D_ReturnsBoundedValues()
        {
            for (float x = -10.0f; x <= 10.0f; x += 2.5f)
            {
                for (float y = -10.0f; y <= 10.0f; y += 2.5f)
                {
                    float val = PerlinNoise.Noise2D(new float2(x, y));
                    Assert.IsTrue(val >= -1.0f && val <= 1.0f);
                }
            }
        }
    }
}
