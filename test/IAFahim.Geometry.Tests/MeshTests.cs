namespace IAFahim.Geometry.Tests
{
    using System;
    using IAFahim.Geometry.Mesh;
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class MeshTests
    {
        [Test]
        public void MeshProjection_DeformVertices_CorrectPosition()
        {
            float3* inVertices = stackalloc float3[1];
            inVertices[0] = new float3(1.0f, 0.0f, 0.0f); // 1 unit to the right local

            float3* inNormals = stackalloc float3[1];
            inNormals[0] = new float3(1.0f, 0.0f, 0.0f);

            float3* pathPositions = stackalloc float3[2];
            pathPositions[0] = new float3(0.0f, 0.0f, 0.0f);
            pathPositions[1] = new float3(0.0f, 0.0f, 10.0f);

            float3* pathRights = stackalloc float3[2];
            pathRights[0] = new float3(1.0f, 0.0f, 0.0f);
            pathRights[1] = new float3(1.0f, 0.0f, 0.0f);

            float3* pathUps = stackalloc float3[2];
            pathUps[0] = new float3(0.0f, 1.0f, 0.0f);
            pathUps[1] = new float3(0.0f, 1.0f, 0.0f);

            float3* pathForwards = stackalloc float3[2];
            pathForwards[0] = new float3(0.0f, 0.0f, 1.0f);
            pathForwards[1] = new float3(0.0f, 0.0f, 1.0f);

            float* vertexU = stackalloc float[1];
            vertexU[0] = 0.5f; // Middle of the path

            float3* outVertices = stackalloc float3[1];
            float3* outNormals = stackalloc float3[1];

            MeshProjection.DeformVertices(
                inVertices, 
                inNormals, 
                1, 
                pathPositions, 
                pathRights, 
                pathUps, 
                pathForwards, 
                2, 
                vertexU, 
                new float3(1.0f, 1.0f, 1.0f), 
                outVertices, 
                outNormals);

            // Expected position: middle of path (0, 0, 5) + 1 unit to the right (1, 0, 0) = (1, 0, 5)
            Assert.AreEqual(1.0f, outVertices[0].x, 1e-5f);
            Assert.AreEqual(0.0f, outVertices[0].y, 1e-5f);
            Assert.AreEqual(5.0f, outVertices[0].z, 1e-5f);

            // Expected normal: (1, 0, 0)
            Assert.AreEqual(1.0f, outNormals[0].x, 1e-5f);
            Assert.AreEqual(0.0f, outNormals[0].y, 1e-5f);
            Assert.AreEqual(0.0f, outNormals[0].z, 1e-5f);
        }

        [Test]
        public void MeshProjection_RecalculateNormals_CorrectDirection()
        {
            float3* vertices = stackalloc float3[3];
            vertices[0] = new float3(0.0f, 0.0f, 0.0f);
            vertices[1] = new float3(1.0f, 0.0f, 0.0f);
            vertices[2] = new float3(0.0f, 0.0f, 1.0f);

            int* indices = stackalloc int[3];
            indices[0] = 0;
            indices[1] = 2;
            indices[2] = 1;

            float3* outNormals = stackalloc float3[3];

            MeshProjection.RecalculateNormals(vertices, 3, indices, 3, outNormals);

            // Normal of CCW triangle on XZ plane looking up (0, 1, 0)
            Assert.AreEqual(0.0f, outNormals[0].x, 1e-5f);
            Assert.AreEqual(1.0f, outNormals[0].y, 1e-5f);
            Assert.AreEqual(0.0f, outNormals[0].z, 1e-5f);
        }
    }
}
