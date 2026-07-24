namespace IAFahim.Geometry.Mesh.Tests
{
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class MeshProjectionTests
    {
        [Test]
        public void RecalculateNormals_UnitZTriangle()
        {
            float3* verts = stackalloc float3[3];
            int* indices = stackalloc int[3];
            float3* normals = stackalloc float3[3];
            verts[0] = new float3(0, 0, 0);
            verts[1] = new float3(1, 0, 0);
            verts[2] = new float3(0, 1, 0);
            indices[0] = 0; indices[1] = 1; indices[2] = 2;
            normals[0] = normals[1] = normals[2] = new float3(0, 0, 0);
            MeshProjection.RecalculateNormals(verts, 3, indices, 3, normals);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(0f, normals[i].x, 1e-4f);
                Assert.AreEqual(0f, normals[i].y, 1e-4f);
                Assert.AreEqual(1f, normals[i].z, 1e-4f);
                Assert.AreEqual(1f, math.length(normals[i]), 1e-4f);
            }
        }
    }
}
