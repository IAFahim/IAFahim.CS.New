namespace IAFahim.Geometry.MarchingCubes.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class MarchingSquaresTests
    {
        [Test]
        public void Contour_StepField_ExactInterpolatedEndpoints()
        {
            // 2x2 field: bottom zeros, top ones -> isolevel 0.5 crosses vertical mid-edges
            // v00=0 v10=0 v01=1 v11=1  code = 8|4 = 12 -> segment left-edge to right-edge
            // left edge (0,0)-(0,1): v 0->1, t=0.5 -> (0, 0.5)
            // right edge (1,0)-(1,1): v 0->1, t=0.5 -> (1, 0.5)
            float* v = stackalloc float[4] { 0, 0, 1, 1 };
            // layout y*w+x: (0,0)=0 (1,0)=0 (0,1)=1 (1,1)=1
            v[0] = 0; v[1] = 0; v[2] = 1; v[3] = 1;
            float* segs = stackalloc float[16];
            int c = MarchingSquares.Contour(v, 2, 2, 0.5f, segs, 4);
            Assert.AreEqual(1, c);
            // endpoints (0,0.5) and (1,0.5) order may swap
            bool ok =
                (Near(segs[0], 0) && Near(segs[1], 0.5f) && Near(segs[2], 1) && Near(segs[3], 0.5f)) ||
                (Near(segs[0], 1) && Near(segs[1], 0.5f) && Near(segs[2], 0) && Near(segs[3], 0.5f));
            Assert.IsTrue(ok, $"seg=({segs[0]},{segs[1]})-({segs[2]},{segs[3]})");
        }

        private static bool Near(float a, float b) => Math.Abs(a - b) < 1e-4f;
    }

    public sealed unsafe class MarchingCubesTests
    {
        [Test]
        public void PolygonizeCube_SingleCorner_OneTriangle()
        {
            float* vals = stackalloc float[8];
            for (int i = 0; i < 8; i++) vals[i] = 0;
            vals[0] = 1;
            float* tris = stackalloc float[27];
            int n = MarchingCubes.PolygonizeCube(vals, 0.5f, tris, 3);
            Assert.AreEqual(1, n);
            AssertVertexOnTriangle(0.5f, 0, 0, tris, n);
            AssertVertexOnTriangle(0, 0, 0.5f, tris, n);
            AssertVertexOnTriangle(0, 0.5f, 0, tris, n);
        }

        [Test]
        public void PolygonizeCube_Empty_ZeroTriangles()
        {
            float* vals = stackalloc float[8];
            for (int i = 0; i < 8; i++) vals[i] = 0;
            float* tris = stackalloc float[9];
            Assert.AreEqual(0, MarchingCubes.PolygonizeCube(vals, 0.5f, tris, 1));
        }

        [Test]
        public void PolygonizeCube_Case5AndCase7_NonZeroTriangles()
        {
            // case 5 = bits 0+2; case 7 = bits 0+1+2 — must not silent-zero with full table
            Assert.IsTrue(MarchingCubes.TriangleCount(5) > 0);
            Assert.IsTrue(MarchingCubes.TriangleCount(7) > 0);
            float* vals = stackalloc float[8];
            float* tris = stackalloc float[45];
            SetCubeBits(vals, 5);
            int n5 = MarchingCubes.PolygonizeCube(vals, 0.5f, tris, 5);
            Assert.AreEqual(MarchingCubes.TriangleCount(5), n5);
            Assert.IsTrue(n5 >= 1);
            SetCubeBits(vals, 7);
            int n7 = MarchingCubes.PolygonizeCube(vals, 0.5f, tris, 5);
            Assert.AreEqual(MarchingCubes.TriangleCount(7), n7);
            Assert.IsTrue(n7 >= 1);
        }

        [Test]
        public void FullTable_NonTrivialConfigs_ProduceTriangles()
        {
            // Every cubeIndex with EdgeTable!=0 must have >=1 triangle in TriTable
            // and PolygonizeCube must return that count (not silent zero).
            float* vals = stackalloc float[8];
            float* tris = stackalloc float[45];
            int silentZero = 0;
            int withEdges = 0;
            for (int ci = 0; ci < 256; ci++)
            {
                int expected = MarchingCubes.TriangleCount(ci);
                if (expected == 0) continue;
                withEdges++;
                SetCubeBits(vals, ci);
                int n = MarchingCubes.PolygonizeCube(vals, 0.5f, tris, 5);
                if (n == 0) silentZero++;
                Assert.AreEqual(expected, n, $"cubeIndex={ci}");
            }
            Assert.IsTrue(withEdges > 200, $"withEdges={withEdges}");
            Assert.AreEqual(0, silentZero);
        }

        private static void SetCubeBits(float* vals, int cubeIndex)
        {
            for (int i = 0; i < 8; i++)
                vals[i] = ((cubeIndex >> i) & 1) != 0 ? 1f : 0f;
        }

        private static void AssertVertexOnTriangle(float x, float y, float z, float* tris, int n)
        {
            for (int t = 0; t < n; t++)
            {
                for (int k = 0; k < 3; k++)
                {
                    int o = t * 9 + k * 3;
                    if (Math.Abs(tris[o] - x) < 1e-4f && Math.Abs(tris[o + 1] - y) < 1e-4f && Math.Abs(tris[o + 2] - z) < 1e-4f)
                        return;
                }
            }
            Assert.Fail($"missing vertex ({x},{y},{z})");
        }
    }
}
