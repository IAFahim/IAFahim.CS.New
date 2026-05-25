namespace IAFahim.Geometry.Tests
{
    using System;
    using IAFahim.Geometry.Triangulation;
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class TriangulationTests
    {
        [Test]
        public void EarClipping_Triangle_GeneratesOneTriangle()
        {
            float2* vertices = stackalloc float2[3];
            vertices[0] = new float2(0.0f, 0.0f);
            vertices[1] = new float2(1.0f, 0.0f);
            vertices[2] = new float2(0.0f, 1.0f);

            int* triangles = stackalloc int[9];
            int triangleCount;

            EarClipping.Triangulate(vertices, 3, null, null, 0, triangles, out triangleCount);

            Assert.AreEqual(3, triangleCount);
            Assert.AreEqual(0, triangles[0]);
            Assert.AreEqual(1, triangles[1]);
            Assert.AreEqual(2, triangles[2]);
        }

        [Test]
        public void EarClipping_Square_GeneratesTwoTriangles()
        {
            float2* vertices = stackalloc float2[4];
            vertices[0] = new float2(0.0f, 0.0f);
            vertices[1] = new float2(1.0f, 0.0f);
            vertices[2] = new float2(1.0f, 1.0f);
            vertices[3] = new float2(0.0f, 1.0f);

            int* triangles = stackalloc int[12];
            int triangleCount;

            EarClipping.Triangulate(vertices, 4, null, null, 0, triangles, out triangleCount);

            Assert.AreEqual(6, triangleCount);
        }

        [Test]
        public void EarClipping_SquareWithHole_Triangulates()
        {
            float2* vertices = stackalloc float2[8];
            // Outer Square (CCW)
            vertices[0] = new float2(0.0f, 0.0f);
            vertices[1] = new float2(3.0f, 0.0f);
            vertices[2] = new float2(3.0f, 3.0f);
            vertices[3] = new float2(0.0f, 3.0f);
            // Inner Square (CW)
            vertices[4] = new float2(1.0f, 1.0f);
            vertices[5] = new float2(1.0f, 2.0f);
            vertices[6] = new float2(2.0f, 2.0f);
            vertices[7] = new float2(2.0f, 1.0f);

            int* holeStarts = stackalloc int[1];
            holeStarts[0] = 4;
            int* holeCounts = stackalloc int[1];
            holeCounts[0] = 4;

            int* triangles = stackalloc int[48];
            int triangleCount;

            EarClipping.Triangulate(vertices, 4, holeStarts, holeCounts, 1, triangles, out triangleCount);

            Assert.IsTrue(triangleCount > 0);
            Assert.AreEqual(0, triangleCount % 3);
        }
    }
}
