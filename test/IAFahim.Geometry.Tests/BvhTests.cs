namespace IAFahim.Geometry.Tests
{
    using System;
    using IAFahim.Geometry.Bvh;
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class BvhTests
    {
        [Test]
        public void BvhTree_Raycast_IntersectsCorrectTriangle()
        {
            float3* vertices = stackalloc float3[4];
            vertices[0] = new float3(0.0f, 0.0f, 0.0f);
            vertices[1] = new float3(1.0f, 0.0f, 0.0f);
            vertices[2] = new float3(1.0f, 0.0f, 1.0f);
            vertices[3] = new float3(0.0f, 0.0f, 1.0f);

            int* indices = stackalloc int[6];
            // Triangle 0
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            // Triangle 1
            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;

            BvhNode* nodes = stackalloc BvhNode[4];
            int nodeCount;
            int root = BvhTree.Build(vertices, indices, 6, nodes, &nodeCount);

            Assert.IsTrue(root >= 0);
            Assert.IsTrue(nodeCount > 0);

            // Ray that should intersect Triangle 0 at centroid
            float3 origin = new float3(0.6f, 5.0f, 0.3f);
            float3 direction = new float3(0.0f, -1.0f, 0.0f);

            float dist;
            int triIdx;
            bool hit = BvhTree.Raycast(nodes, vertices, indices, root, origin, direction, &dist, &triIdx);

            Assert.IsTrue(hit);
            Assert.AreEqual(5.0f, dist, 1e-5f);
            Assert.AreEqual(0, triIdx);

            // Ray that should miss completely
            float3 missOrigin = new float3(5.0f, 5.0f, 5.0f);
            float3 missDirection = new float3(0.0f, -1.0f, 0.0f);

            bool hitMiss = BvhTree.Raycast(nodes, vertices, indices, root, missOrigin, missDirection, &dist, &triIdx);
            Assert.IsFalse(hitMiss);
        }
    }
}
