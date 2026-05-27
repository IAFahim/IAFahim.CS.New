namespace IAFahim.Math.Spline.Tests
{
    using Unity.Mathematics;
    using NUnit.Framework;

    public sealed unsafe class CubicHermiteTests
    {
        [Test]
        public void Evaluate_Start_ReturnsP0()
        {
            float3 p0 = new float3(0, 0, 0);
            float3 m0 = new float3(1, 0, 0);
            float3 p1 = new float3(1, 0, 0);
            float3 m1 = new float3(1, 0, 0);

            float3 result = CubicHermite.Evaluate(p0, m0, p1, m1, 0.0f);

            Assert.IsTrue(math.all(result == p0));
        }

        [Test]
        public void Evaluate_End_ReturnsP1()
        {
            float3 p0 = new float3(0, 0, 0);
            float3 m0 = new float3(1, 0, 0);
            float3 p1 = new float3(1, 0, 0);
            float3 m1 = new float3(1, 0, 0);

            float3 result = CubicHermite.Evaluate(p0, m0, p1, m1, 1.0f);

            Assert.IsTrue(math.all(result == p1));
        }

        [Test]
        public void EvaluateTangent_Start_IsM0()
        {
            float3 p0 = new float3(0, 0, 0);
            float3 m0 = new float3(1, 0, 0);
            float3 p1 = new float3(1, 0, 0);
            float3 m1 = new float3(1, 0, 0);

            float3 result = CubicHermite.EvaluateTangent(p0, m0, p1, m1, 0.0f);

            Assert.IsTrue(math.abs(result.x - 1.0f) < 0.1f);
        }

        [Test]
        public void IntegrateArcLength_StraightLine_Approximate()
        {
            float3 p0 = new float3(0, 0, 0);
            float3 m0 = new float3(1, 0, 0);
            float3 p1 = new float3(1, 0, 0);
            float3 m1 = new float3(1, 0, 0);

            float length = CubicHermite.IntegrateArcLength(p0, m0, p1, m1, 100);

            Assert.IsTrue(math.abs(length - 1.0f) < 0.1f);
        }
    }

    public sealed unsafe class UniformBSplineTests
    {
        [Test]
        public void Evaluate_InsideRange_ReturnsValidPoint()
        {
            float3 p0 = new float3(0, 0, 0);
            float3 p1 = new float3(0, 1, 0);
            float3 p2 = new float3(1, 1, 0);
            float3 p3 = new float3(1, 0, 0);

            float3 result = UniformBSpline.Evaluate(p0, p1, p2, p3, 0.5f);

            Assert.IsTrue(result.x >= 0.0f && result.x <= 1.0f);
            Assert.IsTrue(result.y >= 0.0f && result.y <= 1.0f);
        }

        [Test]
        public void EvaluateTangent_IsDerivative()
        {
            float3 p0 = new float3(0, 0, 0);
            float3 p1 = new float3(0, 1, 0);
            float3 p2 = new float3(1, 1, 0);
            float3 p3 = new float3(1, 0, 0);

            float3 tangent = UniformBSpline.EvaluateTangent(p0, p1, p2, p3, 0.5f);

            Assert.IsTrue(math.length(tangent) > 0.0f);
        }

        [Test]
        public void UniformSample_ReturnsCorrectCount()
        {
            float3 p0 = new float3(0, 0, 0);
            float3 p1 = new float3(0, 1, 0);
            float3 p2 = new float3(1, 1, 0);
            float3 p3 = new float3(1, 0, 0);
            float3* positions = stackalloc float3[5];
            float3* tangents = stackalloc float3[5];

            UniformBSpline.UniformSample(p0, p1, p2, p3, 5, positions, tangents, 100);

            Assert.IsTrue(positions[0].y >= 0.0f && positions[0].y <= 1.0f);
            Assert.IsTrue(positions[4].x >= 0.5f);
        }

        [Test]
        public void UniformSample_SinglePoint_IsStart()
        {
            float3 p0 = new float3(0, 0, 0);
            float3 p1 = new float3(0, 1, 0);
            float3 p2 = new float3(1, 1, 0);
            float3 p3 = new float3(1, 0, 0);
            float3* positions = stackalloc float3[1];
            float3* tangents = stackalloc float3[1];

            UniformBSpline.UniformSample(p0, p1, p2, p3, 1, positions, tangents, 100);

            Assert.IsTrue(math.all(positions[0] == UniformBSpline.Evaluate(p0, p1, p2, p3, 0.0f)));
        }
    }
}