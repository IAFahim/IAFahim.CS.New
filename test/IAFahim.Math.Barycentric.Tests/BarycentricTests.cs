namespace IAFahim.Math.Barycentric.Tests
{
    using Unity.Mathematics;
    using NUnit.Framework;

    public sealed unsafe class BarycentricCoordsTests
    {
        [Test]
        public void Compute_TriangleVertex_ReturnsCorrectWeights()
        {
            float3 a = new float3(0, 0, 0);
            float3 b = new float3(1, 0, 0);
            float3 c = new float3(0, 1, 0);
            float3 bary = BarycentricCoords.Compute(a, b, c, a);
            Assert.IsTrue(bary.x >= 0.99f);
            Assert.IsTrue(bary.y < 0.01f);
            Assert.IsTrue(bary.z < 0.01f);
        }

        [Test]
        public void Compute_Centroid_ReturnsEqualWeights()
        {
            float3 a = new float3(0, 0, 0);
            float3 b = new float3(1, 0, 0);
            float3 c = new float3(0, 1, 0);
            float3 bary = BarycentricCoords.Compute(a, b, c, new float3(1.0f / 3.0f, 1.0f / 3.0f, 0));
            Assert.IsTrue(bary.x > 0.3f && bary.x < 0.35f);
            Assert.IsTrue(bary.y > 0.3f && bary.y < 0.35f);
            Assert.IsTrue(bary.z > 0.3f && bary.z < 0.35f);
        }

        [Test]
        public void Interpolate_TriangleCentroid_ReturnsCentroid()
        {
            float3 a = new float3(0, 0, 0);
            float3 b = new float3(2, 0, 0);
            float3 c = new float3(0, 2, 0);
            float3 bary = new float3(1.0f / 3.0f);
            float3 result = BarycentricCoords.Interpolate(a, b, c, bary);
            Assert.IsTrue(math.abs(result.x - 2.0f / 3.0f) < 0.01f);
            Assert.IsTrue(math.abs(result.y - 2.0f / 3.0f) < 0.01f);
        }

        [Test]
        public void IsInside_TriangleInside_ReturnsTrue()
        {
            float3 bary = new float3(0.3f, 0.3f, 0.4f);
            Assert.IsTrue(BarycentricCoords.IsInside(bary));
        }

        [Test]
        public void IsInside_Outside_ReturnsFalse()
        {
            float3 bary = new float3(-0.1f, 0.5f, 0.6f);
            Assert.IsFalse(BarycentricCoords.IsInside(bary));
        }

        [Test]
        public void ProjectOntoTriangle_Outside_ProjectsInside()
        {
            float3 a = new float3(0, 0, 0);
            float3 b = new float3(1, 0, 0);
            float3 c = new float3(0, 1, 0);
            float3 outside = new float3(-0.5f, -0.5f, 0);
            float3 projected = BarycentricCoords.ProjectOntoTriangle(a, b, c, outside);
            Assert.IsTrue(projected.x >= 0.0f);
            Assert.IsTrue(projected.y >= 0.0f);
        }

        [Test]
        public void Compute2D_SameAs3D()
        {
            float2 a = new float2(0, 0);
            float2 b = new float2(1, 0);
            float2 c = new float2(0, 1);
            float2 bary = BarycentricCoords.Compute2D(a, b, c, new float2(0.25f, 0.25f));
            Assert.IsTrue(bary.x >= 0.0f && bary.x <= 1.0f);
            Assert.IsTrue(bary.y >= 0.0f && bary.y <= 1.0f);
            Assert.IsTrue((bary.x + bary.y) <= 1.0f);
        }
    }
}