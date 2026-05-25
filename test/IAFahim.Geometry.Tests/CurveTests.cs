namespace IAFahim.Geometry.Tests
{
    using System;
    using IAFahim.Geometry.Curve;
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class CurveTests
    {
        [Test]
        public void CubicBezier_Evaluate_CorrectValues()
        {
            float3 p0 = new float3(0.0f, 0.0f, 0.0f);
            float3 p1 = new float3(1.0f, 0.0f, 0.0f);
            float3 p2 = new float3(1.0f, 1.0f, 0.0f);
            float3 p3 = new float3(2.0f, 1.0f, 0.0f);

            float3 mid = CubicBezier.Evaluate(p0, p1, p2, p3, 0.5f);
            Assert.AreEqual(1.0f, mid.x, 1e-5f);
            Assert.AreEqual(0.5f, mid.y, 1e-5f);
            Assert.AreEqual(0.0f, mid.z, 1e-5f);
        }

        [Test]
        public void CubicBezier_IntegrateArcLength_CorrectValue()
        {
            float3 p0 = new float3(0.0f, 0.0f, 0.0f);
            float3 p1 = new float3(1.0f, 0.0f, 0.0f);
            float3 p2 = new float3(2.0f, 0.0f, 0.0f);
            float3 p3 = new float3(3.0f, 0.0f, 0.0f);

            float len = CubicBezier.IntegrateArcLength(p0, p1, p2, p3);
            Assert.AreEqual(3.0f, len, 1e-3f);
        }

        [Test]
        public void CubicBezier_UniformSample_SpacingIsEqual()
        {
            float3 p0 = new float3(0.0f, 0.0f, 0.0f);
            float3 p1 = new float3(1.0f, 0.0f, 0.0f);
            float3 p2 = new float3(1.0f, 1.0f, 0.0f);
            float3 p3 = new float3(2.0f, 1.0f, 0.0f);

            const int count = 5;
            float3* positions = stackalloc float3[count];
            float3* tangents = stackalloc float3[count];

            CubicBezier.UniformSample(p0, p1, p2, p3, count, positions, tangents);

            float expectedDist = CubicBezier.IntegrateArcLength(p0, p1, p2, p3) / (float)(count - 1);

            for (int i = 0; i < count - 1; i++)
            {
                float dist = math.distance(positions[i], positions[i + 1]);
                Assert.AreEqual(expectedDist, dist, 0.05f);
            }
        }

        [Test]
        public void CatmullRom_Evaluate_CorrectValues()
        {
            float3 p0 = new float3(-1.0f, 0.0f, 0.0f);
            float3 p1 = new float3(0.0f, 0.0f, 0.0f);
            float3 p2 = new float3(1.0f, 0.0f, 0.0f);
            float3 p3 = new float3(2.0f, 0.0f, 0.0f);

            float3 mid = CatmullRom.Evaluate(p0, p1, p2, p3, 0.5f);
            Assert.AreEqual(0.5f, mid.x, 1e-5f);
            Assert.AreEqual(0.0f, mid.y, 1e-5f);
            Assert.AreEqual(0.0f, mid.z, 1e-5f);
        }
    }
}
