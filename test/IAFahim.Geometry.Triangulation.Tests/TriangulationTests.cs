namespace IAFahim.Geometry.Triangulation.Tests
{
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class EarClippingTests
    {
        [Test]
        public void Triangle_OneTriangle()
        {
            float2* v = stackalloc float2[3];
            v[0] = new float2(0, 0);
            v[1] = new float2(1, 0);
            v[2] = new float2(0, 1);
            int* tris = stackalloc int[9];
            int count;
            EarClipping.Triangulate(v, 3, null, null, 0, tris, out count);
            Assert.AreEqual(3, count);
            Assert.IsTrue(tris[0] >= 0 && tris[0] < 3);
            Assert.IsTrue(tris[1] >= 0 && tris[1] < 3);
            Assert.IsTrue(tris[2] >= 0 && tris[2] < 3);
        }

        [Test]
        public void Quad_TwoTriangles()
        {
            float2* v = stackalloc float2[4];
            v[0] = new float2(0, 0);
            v[1] = new float2(1, 0);
            v[2] = new float2(1, 1);
            v[3] = new float2(0, 1);
            int* tris = stackalloc int[12];
            int count;
            EarClipping.Triangulate(v, 4, null, null, 0, tris, out count);
            Assert.AreEqual(6, count);
            Assert.AreEqual(0, count % 3);
        }
    }
}
