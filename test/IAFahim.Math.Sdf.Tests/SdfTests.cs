namespace IAFahim.Math.Sdf.Tests
{
    using System.Runtime.InteropServices;
    using Unity.Mathematics;
    using NUnit.Framework;

    public sealed unsafe class SdfPrimitiveTests
    {
        [Test]
        public void Sphere_Center_ReturnsNegativeRadius()
        {
            float d = SdfPrimitive.Sphere(float3.zero, 1.0f);
            Assert.IsTrue(d < 0.0f);
            Assert.IsTrue(math.abs(d + 1.0f) < 1e-6f);
        }

        [Test]
        public void Sphere_Outside_ReturnsPositive()
        {
            float d = SdfPrimitive.Sphere(new float3(5, 0, 0), 1.0f);
            Assert.IsTrue(d > 0.0f);
            Assert.IsTrue(math.abs(d - 4.0f) < 1e-4f);
        }

        [Test]
        public void Box_Inside_ReturnsNegative()
        {
            float d = SdfPrimitive.Box(float3.zero, new float3(1.0f));
            Assert.IsTrue(d < 0.0f);
        }

        [Test]
        public void Box_Outside_ReturnsPositive()
        {
            float d = SdfPrimitive.Box(new float3(5, 0, 0), new float3(1.0f));
            Assert.IsTrue(d > 0.0f);
        }

        [Test]
        public void Plane_Horizontal_ReturnsYDistance()
        {
            float d = SdfPrimitive.Plane(new float3(0, 3, 0), new float3(0, 1, 0), 0.0f);
            Assert.IsTrue(math.abs(d - 3.0f) < 1e-6f);
        }

        [Test]
        public void Capsule_Inside_ReturnsNegative()
        {
            float d = SdfPrimitive.Capsule(new float3(0, 0.2f, 1.5f), new float3(0, 0, 1), new float3(0, 0, 2), 0.5f);
            Assert.IsTrue(d < 0.0f);
        }

        [Test]
        public void Torus_HasCorrectDistancePattern()
        {
            float d = SdfPrimitive.Torus(float3.zero, 1.0f, 0.3f);
            Assert.IsTrue(d > 0.0f);
            float dOuter = SdfPrimitive.Torus(new float3(3, 0, 0), 1.0f, 0.3f);
            Assert.IsTrue(dOuter > 0.0f);
            Assert.IsTrue(dOuter > d);
        }

        [Test]
        public void Cylinder_Vertical_ReturnsRadialDistance()
        {
            float d = SdfPrimitive.Cylinder(new float3(2, 0, 0), 1.0f, 1.0f);
            Assert.IsTrue(d > 0.0f);
            Assert.IsTrue(math.abs(d - 1.0f) < 1e-4f);
        }
    }

    public sealed unsafe class SdfBooleanTests
    {
        [Test]
        public void Union_ReturnsMinimum()
        {
            float d1 = 1.0f, d2 = -1.0f;
            float result = SdfBoolean.Union(d1, d2);
            Assert.IsTrue(math.abs(result - (-1.0f)) < 1e-6f);
        }

        [Test]
        public void Intersection_ReturnsMaximum()
        {
            float d1 = 1.0f, d2 = -1.0f;
            float result = SdfBoolean.Intersection(d1, d2);
            Assert.IsTrue(math.abs(result - 1.0f) < 1e-6f);
        }

        [Test]
        public void Difference_KeepsFirst()
        {
            float d1 = 2.0f, d2 = -1.0f;
            float result = SdfBoolean.Difference(d1, d2);
            Assert.IsTrue(math.abs(result - 2.0f) < 1e-6f);
        }

        [Test]
        public void SmoothUnion_SmoothsBlend()
        {
            float d1 = 0.1f, d2 = 0.2f;
            float result = SdfBoolean.SmoothUnion(d1, d2, 0.5f);
            Assert.IsTrue(result < d1 && result < d2);
        }
    }

    public sealed unsafe class SdfRayMarchTests
    {
        [Test]
        public void March_SphereHit_FindsSurface()
        {
            float t;
            float3 hit;
            bool hitResult = SdfRayMarch.March(
                p => SdfPrimitive.Sphere(p, 1.0f),
                new float3(0, 0, -5),
                new float3(0, 0, 1),
                10.0f, 100,
                out t, out hit);
            Assert.IsTrue(hitResult);
            Assert.IsTrue(t > 0.0f);
            Assert.IsTrue(t < 6.0f);
        }

        [Test]
        public void March_Miss_ReturnsFalse()
        {
            float t;
            float3 hit;
            bool hitResult = SdfRayMarch.March(
                p => SdfPrimitive.Sphere(p, 1.0f),
                new float3(0, 0, -5),
                new float3(0, 1, 0),
                10.0f, 100,
                out t, out hit);
            Assert.IsFalse(hitResult);
        }

        [Test]
        public void EstimateNormal_AtOrigin_ApproximateGradient()
        {
            float3 n = SdfRayMarch.EstimateNormal(p => SdfPrimitive.Sphere(p, 1.0f), float3.zero);
            float len = math.length(n);
            Assert.IsTrue(math.abs(len - 1.0f) < 0.1f);
        }
    }

    public sealed unsafe class SdfTransformTests
    {
        [Test]
        public void Translate_MovesPoint()
        {
            float3 p = new float3(5, 0, 0);
            float3 result = SdfTransform.Translate(p, new float3(3, 0, 0));
            Assert.IsTrue(math.abs(result.x - 2.0f) < 1e-6f);
        }

        [Test]
        public void Scale_Uniform_ScalesCorrectly()
        {
            float3 p = new float3(2, 0, 0);
            float3 result = SdfTransform.Scale(p, new float3(2.0f));
            Assert.IsTrue(math.abs(result.x - 1.0f) < 1e-6f);
        }

        [Test]
        public void MirrorX_FoldsNegativeSide()
        {
            float3 p = new float3(-3, 2, 0);
            float3 result = SdfTransform.MirrorX(p);
            Assert.IsTrue(result.x >= 0.0f);
            Assert.AreEqual(p.y, result.y);
            Assert.AreEqual(p.z, result.z);
        }
    }
}