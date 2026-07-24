namespace IAFahim.Geometry.Curve.Tests
{
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class CubicBezierTests
    {
        [Test]
        public void Evaluate_AtEndpoints()
        {
            float3 p0 = new float3(0,0,0), p1 = new float3(1,1,0), p2 = new float3(2,1,0), p3 = new float3(3,0,0);
            float3 a = CubicBezier.Evaluate(p0, p1, p2, p3, 0f);
            float3 b = CubicBezier.Evaluate(p0, p1, p2, p3, 1f);
            Assert.AreEqual(0f, a.x, 1e-5f);
            Assert.AreEqual(0f, a.y, 1e-5f);
            Assert.AreEqual(3f, b.x, 1e-5f);
            Assert.AreEqual(0f, b.y, 1e-5f);
        }

        [Test]
        public void ArcLength_Positive()
        {
            float3 p0 = new float3(0,0,0), p1 = new float3(0,1,0), p2 = new float3(1,1,0), p3 = new float3(1,0,0);
            float len = CubicBezier.IntegrateArcLength(p0, p1, p2, p3);
            Assert.IsTrue(len > 1f);
        }
    }
}
