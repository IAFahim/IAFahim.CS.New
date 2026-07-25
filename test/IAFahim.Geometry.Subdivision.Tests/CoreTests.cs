namespace IAFahim.Geometry.Subdivision.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class PointQuadtreeTests
    {
        [Test]
        public void Build_QueryCount_MatchesBruteExact()
        {
            const int N = 20;
            double* xs = stackalloc double[N];
            double* ys = stackalloc double[N];
            for (int i = 0; i < N; i++)
            {
                xs[i] = (i % 5) + 0.25;
                ys[i] = (i / 5) + 0.25;
            }
            int nodeCap = 256;
            int memBytes = PointQuadtree.NodeBytes(nodeCap);
            byte* mem = (byte*)Marshal.AllocHGlobal(memBytes);
            int* pointIdx = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                int nodeCount = 0;
                int root = PointQuadtree.Build(xs, ys, N, 0, 5, 0, 5, mem, nodeCap, pointIdx, N, &nodeCount);
                Assert.IsTrue(root >= 0);
                Assert.IsTrue(nodeCount >= 1);

                int qt = PointQuadtree.QueryCount(xs, ys, mem, nodeCap, nodeCount, pointIdx, root, 0, 2, 0, 2);
                int brute = PointQuadtree.RangeCount(xs, ys, N, 0, 2, 0, 2);
                Assert.AreEqual(brute, qt);
                Assert.AreEqual(4, qt); // points (0.25,0.25)(1.25,0.25)(0.25,1.25)(1.25,1.25)
            }
            finally
            {
                Marshal.FreeHGlobal((nint)mem);
                Marshal.FreeHGlobal((nint)pointIdx);
            }
        }

        [Test]
        public void SubdivideBox_MidpointsExact()
        {
            double* boxes = stackalloc double[16];
            PointQuadtree.SubdivideBox(0, 2, 0, 2, boxes);
            Assert.AreEqual(0, boxes[0]);
            Assert.AreEqual(1, boxes[1]);
            Assert.AreEqual(1, boxes[2]);
            Assert.AreEqual(2, boxes[3]);
        }
    }
}
