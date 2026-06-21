namespace IAFahim.Collision.Gjk
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class Epa
    {
        private const float Epsilon = 1e-6f;

        private const int MaxIterations = 128;

        private const int MaxFaces = 256;

        private const int EdgesPerFace = 3;

        private const int TetrahedronVertexCount = 4;

        private const int RefineContinue = 0;

        private const int RefineConverged = 1;

        private const int RefineFailed = 2;

        private const float DirectionThresholdRatio = 0.9f;

        private const float PerturbationFactor = 0.1f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TryAddHorizonEdge(int u, int v, int currentFace, int faceCount, bool* visible, int* faceA, int* faceB, int* faceC, int* horizonA, int* horizonB, ref int horizonCount)
        {
            if (IsHorizonEdge(u, v, currentFace, faceCount, visible, faceA, faceB, faceC))
            {
                horizonA[horizonCount] = u;
                horizonB[horizonCount] = v;
                horizonCount++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ExpandPolytope(float3 newSupport, float3* vertices, ref int vertexCount, float3* faceNormals, float* faceDistances, int* faceA, int* faceB, int* faceC, ref int faceCount)
        {
            int newIdx = vertexCount;
            vertices[vertexCount] = newSupport;
            vertexCount++;

            bool* visible = stackalloc bool[MaxFaces];
            for (int f = 0; f < faceCount; f++)
                visible[f] = math.dot(faceNormals[f], newSupport) - faceDistances[f] > Epsilon;

            int* horizonA = stackalloc int[MaxFaces * EdgesPerFace];
            int* horizonB = stackalloc int[MaxFaces * EdgesPerFace];
            int horizonCount = 0;

            for (int f = 0; f < faceCount; f++)
            {
                if (!visible[f]) continue;
                int aIdx = faceA[f], bIdx = faceB[f], cIdx = faceC[f];
                TryAddHorizonEdge(aIdx, bIdx, f, faceCount, visible, faceA, faceB, faceC, horizonA, horizonB, ref horizonCount);
                TryAddHorizonEdge(bIdx, cIdx, f, faceCount, visible, faceA, faceB, faceC, horizonA, horizonB, ref horizonCount);
                TryAddHorizonEdge(cIdx, aIdx, f, faceCount, visible, faceA, faceB, faceC, horizonA, horizonB, ref horizonCount);
            }

            for (int f = faceCount - 1; f >= 0; f--)
                if (visible[f]) RemoveFace(f, faceNormals, faceDistances, faceA, faceB, faceC, ref faceCount);

            for (int i = 0; i < horizonCount; i++)
                AddFace(vertices, horizonA[i], horizonB[i], newIdx, faceNormals, faceDistances, faceA, faceB, faceC, ref faceCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int RefineOnce(Gjk.SupportFunction supportA, Gjk.SupportFunction supportB, float3* vertices, ref int vertexCount, float3* faceNormals, float* faceDistances, int* faceA, int* faceB, int* faceC, ref int faceCount, out float3 outNormal, out float outDepth)
        {
            outNormal = float3.zero;
            outDepth = 0.0f;
            int closestFace = FindClosestFace(faceDistances, faceCount);
            if (closestFace < 0) return RefineFailed;
            float minDist = faceDistances[closestFace];
            float3 faceNormal = faceNormals[closestFace];
            float3 newSupport = supportA(faceNormal) - supportB(-faceNormal);
            float supDist = math.dot(newSupport, faceNormal);
            if (supDist - minDist < Epsilon)
            {
                outNormal = faceNormal;
                outDepth = minDist;
                return RefineConverged;
            }
            ExpandPolytope(newSupport, vertices, ref vertexCount, faceNormals, faceDistances, faceA, faceB, faceC, ref faceCount);
            return RefineContinue;
        }

        public static float PenetrationDepth(Gjk.SupportFunction supportA, Gjk.SupportFunction supportB,
            float3* initialSimplex, int simplexCount, out float3 normal, out float depth)
        {
            normal = float3.zero;
            depth = 0.0f;

            float3* vertices = stackalloc float3[MaxIterations + TetrahedronVertexCount];
            int vertexCount = simplexCount;
            for (int i = 0; i < simplexCount; i++) vertices[i] = initialSimplex[i];

            if (vertexCount < TetrahedronVertexCount)
            {
                ExpandSimplex(supportA, supportB, vertices, ref vertexCount);
            }

            float3* faceNormals = stackalloc float3[MaxFaces];
            float* faceDistances = stackalloc float[MaxFaces];
            int* faceA = stackalloc int[MaxFaces];
            int* faceB = stackalloc int[MaxFaces];
            int* faceC = stackalloc int[MaxFaces];
            int faceCount = 0;

            AddFace(vertices, 0, 1, 2, faceNormals, faceDistances, faceA, faceB, faceC, ref faceCount);
            AddFace(vertices, 0, 3, 1, faceNormals, faceDistances, faceA, faceB, faceC, ref faceCount);
            AddFace(vertices, 0, 2, 3, faceNormals, faceDistances, faceA, faceB, faceC, ref faceCount);
            AddFace(vertices, 1, 3, 2, faceNormals, faceDistances, faceA, faceB, faceC, ref faceCount);

            for (int iter = 0; iter < MaxIterations; iter++)
            {
                int status = RefineOnce(supportA, supportB, vertices, ref vertexCount, faceNormals, faceDistances, faceA, faceB, faceC, ref faceCount, out float3 iterNormal, out float iterDepth);
                if (status == RefineFailed) return 0.0f;
                if (status == RefineConverged) { normal = iterNormal; depth = iterDepth; return depth; }
            }

            return 0.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsHorizonEdge(int u, int v, int currentFace, int faceCount, bool* visible, int* fa, int* fb, int* fc)
        {
            for (int i = 0; i < faceCount; i++)
            {
                if (i == currentFace) continue;
                int a = fa[i];
                int b = fb[i];
                int c = fc[i];
                bool containsU = a == u || b == u || c == u;
                bool containsV = a == v || b == v || c == v;
                if (containsU && containsV) return !visible[i];
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddFace(float3* vertices, int a, int b, int c,
            float3* normals, float* distances, int* fa, int* fb, int* fc, ref int faceCount)
        {
            if (faceCount >= MaxFaces) return;
            float3 ab = vertices[b] - vertices[a];
            float3 ac = vertices[c] - vertices[a];
            float3 n = math.cross(ab, ac);
            float len = math.length(n);
            if (len < Epsilon) return;
            n = n / len;
            float d = math.dot(n, vertices[a]);
            if (d < 0.0f) { n = -n; d = -d; }
            int idx = faceCount;
            normals[idx] = n;
            distances[idx] = d;
            fa[idx] = a;
            fb[idx] = b;
            fc[idx] = c;
            faceCount++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindClosestFace(float* distances, int faceCount)
        {
            if (faceCount == 0) return -1;
            int closest = 0;
            float minDist = distances[0];
            for (int i = 1; i < faceCount; i++)
                if (distances[i] < minDist) { minDist = distances[i]; closest = i; }
            return closest;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RemoveFace(int idx, float3* normals, float* distances,
            int* fa, int* fb, int* fc, ref int faceCount)
        {
            int last = faceCount - 1;
            if (idx < last)
            {
                normals[idx] = normals[last];
                distances[idx] = distances[last];
                fa[idx] = fa[last];
                fb[idx] = fb[last];
                fc[idx] = fc[last];
            }
            faceCount--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ExpandSimplex(Gjk.SupportFunction supportA, Gjk.SupportFunction supportB,
            float3* vertices, ref int vertexCount)
        {
            if (vertexCount == 1)
            {
                float3 a = vertices[0];
                float3 dir = math.lengthsq(a) < Epsilon ? new float3(1.0f, 0.0f, 0.0f) : -a;
                vertices[1] = supportA(dir) - supportB(-dir);
                vertexCount = 2;
            }

            if (vertexCount == 2)
            {
                float3 a = vertices[0], b = vertices[1];
                float3 ab = b - a;
                float3 dir = new float3(1.0f, 0.0f, 0.0f);
                if (math.abs(ab.x) > DirectionThresholdRatio * math.length(ab))
                {
                    dir = new float3(0.0f, 1.0f, 0.0f);
                }
                dir = math.cross(ab, dir) + ab * PerturbationFactor;
                vertices[2] = supportA(dir) - supportB(-dir);
                vertexCount = 3;
            }

            if (vertexCount == 3)
            {
                float3 a = vertices[0], b = vertices[1], c = vertices[2];
                float3 ab = b - a;
                float3 ac = c - a;
                float3 dir = math.cross(ab, ac);
                if (math.lengthsq(dir) < Epsilon)
                {
                    dir = new float3(0.0f, 0.0f, 1.0f);
                }
                dir = dir + (ab + ac) * PerturbationFactor;
                vertices[3] = supportA(dir) - supportB(-dir);
                vertexCount = 4;
            }
        }
    }
}
