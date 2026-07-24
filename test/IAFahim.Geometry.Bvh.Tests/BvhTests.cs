namespace IAFahim.Geometry.Bvh.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class BvhTreeTests
    {
        [Test]
        public void Build_RootBoundsContainTriangle()
        {
            float3* verts = stackalloc float3[3];
            int* indices = stackalloc int[3];
            verts[0] = new float3(0, 0, 0);
            verts[1] = new float3(2, 0, 0);
            verts[2] = new float3(0, 2, 0);
            indices[0] = 0; indices[1] = 1; indices[2] = 2;
            BvhNode* nodes = (BvhNode*)Marshal.AllocHGlobal(32 * sizeof(BvhNode));
            try
            {
                int nodeCount = 0;
                int root = BvhTree.Build(verts, indices, 3, nodes, &nodeCount);
                Assert.IsTrue(nodeCount >= 1);
                Assert.IsTrue(root >= 0 && root < nodeCount);
                BvhNode r = nodes[root];
                Assert.IsTrue(r.Min.x <= 0.01f && r.Min.y <= 0.01f);
                Assert.IsTrue(r.Max.x >= 1.99f && r.Max.y >= 1.99f);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)nodes);
            }
        }

        [Test]
        public void Raycast_HitsTriangle()
        {
            float3* verts = stackalloc float3[3];
            int* indices = stackalloc int[3];
            verts[0] = new float3(0, 0, 0);
            verts[1] = new float3(1, 0, 0);
            verts[2] = new float3(0, 1, 0);
            indices[0] = 0; indices[1] = 1; indices[2] = 2;
            BvhNode* nodes = (BvhNode*)Marshal.AllocHGlobal(32 * sizeof(BvhNode));
            try
            {
                int nodeCount = 0;
                int root = BvhTree.Build(verts, indices, 3, nodes, &nodeCount);
                float3 origin = new float3(0.1f, 0.1f, 1f);
                float3 dir = new float3(0, 0, -1f);
                float dist = 0;
                int tri = -1;
                bool hit = BvhTree.Raycast(nodes, verts, indices, root, origin, dir, &dist, &tri);
                Assert.IsTrue(hit);
                Assert.IsTrue(dist > 0f && dist < 2f);
                Assert.IsTrue(tri >= 0);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)nodes);
            }
        }
    }
}
