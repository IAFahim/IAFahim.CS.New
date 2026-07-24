namespace IAFahim.Geometry.Bvh.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class BvhTreeTests
    {
        [Test]
        public void Build_SingleTriangle()
        {
            float3* verts = stackalloc float3[3];
            int* indices = stackalloc int[3];
            verts[0] = new float3(0, 0, 0);
            verts[1] = new float3(1, 0, 0);
            verts[2] = new float3(0, 1, 0);
            indices[0] = 0; indices[1] = 1; indices[2] = 2;
            BvhNode* nodes = (BvhNode*)Marshal.AllocHGlobal(16 * sizeof(BvhNode));
            try
            {
                int nodeCount = 0;
                int root = BvhTree.Build(verts, indices, 3, nodes, &nodeCount);
                Assert.IsTrue(nodeCount >= 1);
                Assert.IsTrue(root >= 0);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)nodes);
            }
        }
    }
}
