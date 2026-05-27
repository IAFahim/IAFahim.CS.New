namespace IAFahim.Collision.Gjk.Tests
{
    using Unity.Mathematics;
    using NUnit.Framework;

    public sealed unsafe class GjkTests
    {
        [Test]
        public void Intersect_SphereSphere_Overlapping_ReturnsTrue()
        {
            bool result = Gjk.Intersect(
                d => MinkowskiDifference.SphereSupport(d, new float3(0, 0, 0), 1.0f),
                d => MinkowskiDifference.SphereSupport(d, new float3(1, 0, 0), 1.0f));
            Assert.IsTrue(result);
        }

        [Test]
        public void Intersect_SphereSphere_Separated_ReturnsFalse()
        {
            bool result = Gjk.Intersect(
                d => MinkowskiDifference.SphereSupport(d, new float3(0, 0, 0), 1.0f),
                d => MinkowskiDifference.SphereSupport(d, new float3(5, 0, 0), 1.0f));
            Assert.IsFalse(result);
        }

        [Test]
        public void Intersect_BoxBox_Overlap_ReturnsTrue()
        {
            bool result = Gjk.Intersect(
                d => MinkowskiDifference.BoxSupport(d, new float3(0, 0, 0), new float3(1.0f)),
                d => MinkowskiDifference.BoxSupport(d, new float3(1, 0, 0), new float3(1.0f)));
            Assert.IsTrue(result);
        }

        [Test]
        public void Distance_SphereSphere_Overlapping_ReturnsZero()
        {
            float dist = Gjk.Distance(
                d => MinkowskiDifference.SphereSupport(d, new float3(0, 0, 0), 1.0f),
                d => MinkowskiDifference.SphereSupport(d, new float3(0.5f, 0, 0), 1.0f));
            Assert.IsTrue(dist < 1e-6f);
        }

        [Test]
        public void Distance_SphereSphere_Separated_ReturnsDistance()
        {
            float dist = Gjk.Distance(
                d => MinkowskiDifference.SphereSupport(d, new float3(0, 0, 0), 1.0f),
                d => MinkowskiDifference.SphereSupport(d, new float3(4, 0, 0), 1.0f));
            Assert.IsTrue(dist > 1.0f);
            Assert.IsTrue(dist < 3.0f);
        }

        [Test]
        public void Intersect_SphereSphere_AtOrigin_ReturnsTrue()
        {
            bool result = Gjk.Intersect(
                d => MinkowskiDifference.SphereSupport(d, new float3(0, 0, 0), 1.0f),
                d => MinkowskiDifference.SphereSupport(d, new float3(0, 0, 0), 1.0f));
            Assert.IsTrue(result);
        }
    }

    public sealed unsafe class EpaTests
    {
        [Test]
        public void PenetrationDepth_OverlappingSpheres_ReturnsPositive()
        {
            float3* simplexA = stackalloc float3[4];
            int count;

            bool intersected = Gjk.Intersect(
                d => MinkowskiDifference.SphereSupport(d, new float3(0, 0, 0), 1.0f),
                d => MinkowskiDifference.SphereSupport(d, new float3(1.2f, 0, 0), 0.5f),
                simplexA, out count);

            Assert.IsTrue(intersected);

            float3 normal;
            float depth;
            float result = Epa.PenetrationDepth(
                d => MinkowskiDifference.SphereSupport(d, new float3(0, 0, 0), 1.0f),
                d => MinkowskiDifference.SphereSupport(d, new float3(1.2f, 0, 0), 0.5f),
                simplexA, count, out normal, out depth);

            Assert.IsTrue(result > 0.0f);
            Assert.IsTrue(depth > 0.0f);
        }
    }
}