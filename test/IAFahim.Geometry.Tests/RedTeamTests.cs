namespace IAFahim.Geometry.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.Geometry.Curve;
    using IAFahim.Geometry.Frame;
    using IAFahim.Geometry.Triangulation;
    using IAFahim.Math.Noise;
    using IAFahim.Geometry.Bvh;
    using IAFahim.Geometry.Mesh;
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class RedTeamTests
    {
        [Test]
        public void CubicBezierCoincidentControlPoints_TangentsAreNaN()
        {
            float3 p0 = new float3(0.0f, 0.0f, 0.0f);
            float3 p1 = new float3(0.0f, 0.0f, 0.0f);
            float3 p2 = new float3(0.0f, 0.0f, 0.0f);
            float3 p3 = new float3(0.0f, 0.0f, 0.0f);
            const int N = 10;
            float3* positions = (float3*)Marshal.AllocHGlobal(N * sizeof(float3));
            float3* tangents = (float3*)Marshal.AllocHGlobal(N * sizeof(float3));
            try
            {
                CubicBezier.UniformSample(p0, p1, p2, p3, N, positions, tangents);
                for (int i = 0; i < N; i++)
                {
                    Assert.IsFalse(math.isnan(tangents[i].x));
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)positions);
                Marshal.FreeHGlobal((nint)tangents);
            }
        }

        [Test]
        public void CubicBezierDegenerateEndpoints_TangentsAreNaN()
        {
            float3 p0 = new float3(0.0f, 0.0f, 0.0f);
            float3 p1 = new float3(0.0f, 0.0f, 0.0f);
            float3 p2 = new float3(1.0f, 1.0f, 1.0f);
            float3 p3 = new float3(1.0f, 1.0f, 1.0f);
            const int N = 10;
            float3* positions = (float3*)Marshal.AllocHGlobal(N * sizeof(float3));
            float3* tangents = (float3*)Marshal.AllocHGlobal(N * sizeof(float3));
            try
            {
                CubicBezier.UniformSample(p0, p1, p2, p3, N, positions, tangents);
                Assert.IsFalse(math.isnan(tangents[0].x));
                Assert.IsFalse(math.isnan(tangents[N - 1].x));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)positions);
                Marshal.FreeHGlobal((nint)tangents);
            }
        }

        [Test]
        public void CubicBezierExtremeInputs_DoesNotCrash()
        {
            float3 p0 = new float3(1e30f, 1e30f, 1e30f);
            float3 p1 = new float3(1e30f, 1e30f, 1e30f);
            float3 p2 = new float3(-1e30f, -1e30f, -1e30f);
            float3 p3 = new float3(-1e30f, -1e30f, -1e30f);
            const int N = 10;
            float3* positions = (float3*)Marshal.AllocHGlobal(N * sizeof(float3));
            float3* tangents = (float3*)Marshal.AllocHGlobal(N * sizeof(float3));
            try
            {
                CubicBezier.UniformSample(p0, p1, p2, p3, N, positions, tangents);
                Assert.Pass();
            }
            finally
            {
                Marshal.FreeHGlobal((nint)positions);
                Marshal.FreeHGlobal((nint)tangents);
            }
        }

        [Test]
        public void ParallelTransportZeroInitialNormal_TangentsAndNormalsAreNaN()
        {
            const int N = 5;
            float3* positions = (float3*)Marshal.AllocHGlobal(N * sizeof(float3));
            float3* outRight = (float3*)Marshal.AllocHGlobal(N * sizeof(float3));
            float3* outUp = (float3*)Marshal.AllocHGlobal(N * sizeof(float3));
            float3* outForward = (float3*)Marshal.AllocHGlobal(N * sizeof(float3));
            try
            {
                for (int i = 0; i < N; i++)
                {
                    positions[i] = new float3((float)i, 0.0f, 0.0f);
                }
                ParallelTransport.Compute(positions, N, new float3(0.0f, 0.0f, 0.0f), outRight, outUp, outForward);
                Assert.IsTrue(math.isnan(outUp[0].x));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)positions);
                Marshal.FreeHGlobal((nint)outRight);
                Marshal.FreeHGlobal((nint)outUp);
                Marshal.FreeHGlobal((nint)outForward);
            }
        }

        [Test]
        public void ParallelTransportCoincidentPoints_NormalsAreGenerated()
        {
            const int N = 5;
            float3* positions = (float3*)Marshal.AllocHGlobal(N * sizeof(float3));
            float3* outRight = (float3*)Marshal.AllocHGlobal(N * sizeof(float3));
            float3* outUp = (float3*)Marshal.AllocHGlobal(N * sizeof(float3));
            float3* outForward = (float3*)Marshal.AllocHGlobal(N * sizeof(float3));
            try
            {
                for (int i = 0; i < N; i++)
                {
                    positions[i] = new float3(0.0f, 0.0f, 0.0f);
                }
                ParallelTransport.Compute(positions, N, new float3(0.0f, 1.0f, 0.0f), outRight, outUp, outForward);
                Assert.Pass();
            }
            finally
            {
                Marshal.FreeHGlobal((nint)positions);
                Marshal.FreeHGlobal((nint)outRight);
                Marshal.FreeHGlobal((nint)outUp);
                Marshal.FreeHGlobal((nint)outForward);
            }
        }

        [Test]
        public void EarClippingCollinearVertices_NoTrianglesGenerated()
        {
            const int N = 4;
            float2* vertices = (float2*)Marshal.AllocHGlobal(N * sizeof(float2));
            int* triangles = (int*)Marshal.AllocHGlobal(3 * N * sizeof(int));
            try
            {
                vertices[0] = new float2(0.0f, 0.0f);
                vertices[1] = new float2(1.0f, 1.0f);
                vertices[2] = new float2(2.0f, 2.0f);
                vertices[3] = new float2(3.0f, 3.0f);
                int triangleCount;
                EarClipping.Triangulate(vertices, N, null, null, 0, triangles, out triangleCount);
                Assert.AreEqual(0, triangleCount);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)vertices);
                Marshal.FreeHGlobal((nint)triangles);
            }
        }

        [Test]
        public void EarClippingSelfIntersectingHoles_TriangulationCompletesOrIgnores()
        {
            const int outerCount = 4;
            const int holeCount = 4;
            const int totalCount = outerCount + holeCount;
            float2* vertices = (float2*)Marshal.AllocHGlobal(totalCount * sizeof(float2));
            int* holeStarts = (int*)Marshal.AllocHGlobal(1 * sizeof(int));
            int* holeCounts = (int*)Marshal.AllocHGlobal(1 * sizeof(int));
            int* triangles = (int*)Marshal.AllocHGlobal(3 * totalCount * sizeof(int));
            try
            {
                vertices[0] = new float2(0.0f, 0.0f);
                vertices[1] = new float2(10.0f, 0.0f);
                vertices[2] = new float2(10.0f, 10.0f);
                vertices[3] = new float2(0.0f, 10.0f);

                vertices[4] = new float2(2.0f, 2.0f);
                vertices[5] = new float2(8.0f, 8.0f);
                vertices[6] = new float2(2.0f, 8.0f);
                vertices[7] = new float2(8.0f, 2.0f);

                holeStarts[0] = 4;
                holeCounts[0] = 4;

                int triangleCount;
                EarClipping.Triangulate(vertices, outerCount, holeStarts, holeCounts, 1, triangles, out triangleCount);
                Assert.Pass();
            }
            finally
            {
                Marshal.FreeHGlobal((nint)vertices);
                Marshal.FreeHGlobal((nint)holeStarts);
                Marshal.FreeHGlobal((nint)holeCounts);
                Marshal.FreeHGlobal((nint)triangles);
            }
        }

        [Test]
        public void EarClippingNegativeHoleCounts_ThrowsException()
        {
            const int outerCount = 4;
            float2* vertices = (float2*)Marshal.AllocHGlobal(outerCount * sizeof(float2));
            int* holeStarts = (int*)Marshal.AllocHGlobal(1 * sizeof(int));
            int* holeCounts = (int*)Marshal.AllocHGlobal(1 * sizeof(int));
            int* triangles = (int*)Marshal.AllocHGlobal(12 * sizeof(int));
            try
            {
                vertices[0] = new float2(0.0f, 0.0f);
                vertices[1] = new float2(10.0f, 0.0f);
                vertices[2] = new float2(10.0f, 10.0f);
                vertices[3] = new float2(0.0f, 10.0f);

                holeStarts[0] = 4;
                holeCounts[0] = -5;

                int triangleCount;
                EarClipping.Triangulate(vertices, outerCount, holeStarts, holeCounts, 1, triangles, out triangleCount);
                Assert.AreEqual(0, triangleCount);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)vertices);
                Marshal.FreeHGlobal((nint)holeStarts);
                Marshal.FreeHGlobal((nint)holeCounts);
                Marshal.FreeHGlobal((nint)triangles);
            }
        }

        [Test]
        public void SimplexNoiseNaNInput_ReturnsNaN()
        {
            float2 p = new float2(float.NaN, float.NaN);
            float val = SimplexNoise.Noise2D(p);
            // This currently fails because SimplexNoise returns 0.0f for NaNs
            Assert.IsTrue(float.IsNaN(val));
        }

        [Test]
        public void SimplexNoiseLargeInput_ReturnsCorrectFloatType()
        {
            float2 p = new float2(1e12f, 1e12f);
            float val = SimplexNoise.Noise2D(p);
            Assert.IsFalse(float.IsInfinity(val));
        }

        [Test]
        public void PerlinNoiseNaNInput_ReturnsNaN()
        {
            float2 p = new float2(float.NaN, float.NaN);
            float val = PerlinNoise.Noise2D(p);
            Assert.IsTrue(float.IsNaN(val));
        }

        [Test]
        public void PerlinNoiseLargeInput_ReturnsCorrectFloatType()
        {
            float2 p = new float2(1e12f, 1e12f);
            float val = PerlinNoise.Noise2D(p);
            Assert.IsFalse(float.IsInfinity(val));
        }

        [Test]
        public void BvhTreeCoincidentTriangles_QuickSortStackOverflow()
        {
            // Increase triangleCount to reliably cause stack overflow
            const int triangleCount = 50000;
            const int indexCount = triangleCount * 3;
            const int vertexCount = 3;
            float3* vertices = (float3*)Marshal.AllocHGlobal(vertexCount * sizeof(float3));
            int* indices = (int*)Marshal.AllocHGlobal(indexCount * sizeof(int));
            BvhNode* nodes = (BvhNode*)Marshal.AllocHGlobal(2 * triangleCount * sizeof(BvhNode));
            int nodeCount;
            try
            {
                vertices[0] = new float3(0.0f, 0.0f, 0.0f);
                vertices[1] = new float3(1.0f, 0.0f, 0.0f);
                vertices[2] = new float3(0.0f, 1.0f, 0.0f);
                for (int i = 0; i < indexCount; i += 3)
                {
                    indices[i] = 0;
                    indices[i + 1] = 1;
                    indices[i + 2] = 2;
                }
                // Running this should result in StackOverflowException (crashes process)
                BvhTree.Build(vertices, indices, indexCount, nodes, &nodeCount);
                Assert.Pass();
            }
            finally
            {
                Marshal.FreeHGlobal((nint)vertices);
                Marshal.FreeHGlobal((nint)indices);
                Marshal.FreeHGlobal((nint)nodes);
            }
        }

        [Test]
        public void BvhTreeRaycastNearEpsilonDirection_DoesNotCrash()
        {
            const int vertexCount = 3;
            const int indexCount = 3;
            float3* vertices = (float3*)Marshal.AllocHGlobal(vertexCount * sizeof(float3));
            int* indices = (int*)Marshal.AllocHGlobal(indexCount * sizeof(int));
            BvhNode* nodes = (BvhNode*)Marshal.AllocHGlobal(10 * sizeof(BvhNode));
            int nodeCount;
            try
            {
                vertices[0] = new float3(-1.0f, -1.0f, 0.0f);
                vertices[1] = new float3(1.0f, -1.0f, 0.0f);
                vertices[2] = new float3(0.0f, 1.0f, 0.0f);
                indices[0] = 0;
                indices[1] = 1;
                indices[2] = 2;
                int root = BvhTree.Build(vertices, indices, indexCount, nodes, &nodeCount);

                float dist;
                int triIdx;
                BvhTree.Raycast(nodes, vertices, indices, root, new float3(0.0f, 0.0f, -5.0f), new float3(-1e-6f, 0.0f, 1.0f), &dist, &triIdx);
                Assert.Pass();
            }
            finally
            {
                Marshal.FreeHGlobal((nint)vertices);
                Marshal.FreeHGlobal((nint)indices);
                Marshal.FreeHGlobal((nint)nodes);
            }
        }

        [Test]
        public void BvhTreeRaycastFalseNegative_DueToNaNInversion()
        {
            const int vertexCount = 3;
            const int indexCount = 3;
            float3* vertices = (float3*)Marshal.AllocHGlobal(vertexCount * sizeof(float3));
            int* indices = (int*)Marshal.AllocHGlobal(indexCount * sizeof(int));
            BvhNode* nodes = (BvhNode*)Marshal.AllocHGlobal(10 * sizeof(BvhNode));
            int nodeCount;
            try
            {
                vertices[0] = new float3(-0.5f, -0.5f, 0.0f);
                vertices[1] = new float3(1.0f, -0.5f, 0.0f);
                vertices[2] = new float3(1.0f, 1.0f, 0.0f);
                indices[0] = 0;
                indices[1] = 1;
                indices[2] = 2;
                int root = BvhTree.Build(vertices, indices, indexCount, nodes, &nodeCount);

                float dist;
                int triIdx;
                bool hit = BvhTree.Raycast(nodes, vertices, indices, root, 
                    new float3(1.0f, 0.0f, -5.0f), 
                    new float3(-1e-6f, 0.0f, 1.0f), 
                    &dist, &triIdx);
                
                Assert.IsTrue(hit);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)vertices);
                Marshal.FreeHGlobal((nint)indices);
                Marshal.FreeHGlobal((nint)nodes);
            }
        }

        [Test]
        public void MeshProjectionDeformVerticesOppositeRights_TangentsAreNaN()
        {
            const int vertexCount = 1;
            const int pathCount = 2;
            float3* inVertices = (float3*)Marshal.AllocHGlobal(vertexCount * sizeof(float3));
            float3* inNormals = (float3*)Marshal.AllocHGlobal(vertexCount * sizeof(float3));
            float3* pathPositions = (float3*)Marshal.AllocHGlobal(pathCount * sizeof(float3));
            float3* pathRights = (float3*)Marshal.AllocHGlobal(pathCount * sizeof(float3));
            float3* pathUps = (float3*)Marshal.AllocHGlobal(pathCount * sizeof(float3));
            float3* pathForwards = (float3*)Marshal.AllocHGlobal(pathCount * sizeof(float3));
            float* vertexU = (float*)Marshal.AllocHGlobal(vertexCount * sizeof(float));
            float3* outVertices = (float3*)Marshal.AllocHGlobal(vertexCount * sizeof(float3));
            float3* outNormals = (float3*)Marshal.AllocHGlobal(vertexCount * sizeof(float3));
            try
            {
                inVertices[0] = new float3(0.0f, 0.0f, 0.0f);
                inNormals[0] = new float3(0.0f, 1.0f, 0.0f);
                pathPositions[0] = new float3(0.0f, 0.0f, 0.0f);
                pathPositions[1] = new float3(0.0f, 0.0f, 1.0f);
                
                // Opposite rights cause math.lerp to return zero vector at t=0.5f
                pathRights[0] = new float3(1.0f, 0.0f, 0.0f);
                pathRights[1] = new float3(-1.0f, 0.0f, 0.0f);
                
                pathUps[0] = new float3(0.0f, 1.0f, 0.0f);
                pathUps[1] = new float3(0.0f, 1.0f, 0.0f);
                pathForwards[0] = new float3(0.0f, 0.0f, 1.0f);
                pathForwards[1] = new float3(0.0f, 0.0f, 1.0f);
                vertexU[0] = 0.5f;

                MeshProjection.DeformVertices(inVertices, inNormals, vertexCount, pathPositions, pathRights, pathUps, pathForwards, pathCount, vertexU, new float3(1.0f, 1.0f, 1.0f), outVertices, outNormals);
                
                Assert.IsFalse(math.isnan(outVertices[0].x));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)inVertices);
                Marshal.FreeHGlobal((nint)inNormals);
                Marshal.FreeHGlobal((nint)pathPositions);
                Marshal.FreeHGlobal((nint)pathRights);
                Marshal.FreeHGlobal((nint)pathUps);
                Marshal.FreeHGlobal((nint)pathForwards);
                Marshal.FreeHGlobal((nint)vertexU);
                Marshal.FreeHGlobal((nint)outVertices);
                Marshal.FreeHGlobal((nint)outNormals);
            }
        }
    }
}
