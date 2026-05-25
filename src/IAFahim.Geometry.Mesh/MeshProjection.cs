namespace IAFahim.Geometry.Mesh
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class MeshProjection
    {
        private const float Zero = 0.0f;
        private const float One = 1.0f;
        private const float Epsilon = 1e-6f;

        public static void DeformVertices(
            float3* inVertices, 
            float3* inNormals, 
            int vertexCount, 
            float3* pathPositions, 
            float3* pathRights, 
            float3* pathUps, 
            float3* pathForwards, 
            int pathCount,
            float* vertexU, // normalized parameter [0, 1] along the path for each vertex
            float3 scale, 
            float3* outVertices, 
            float3* outNormals)
        {
            if (pathCount <= 0 || vertexCount <= 0)
            {
                return;
            }

            for (int i = 0; i < vertexCount; i++)
            {
                float val = vertexU[i];
                float u = math.isnan(val) ? Zero : math.clamp(val, Zero, One);
                float floatIdx = u * (float)(pathCount - 1);
                int idx0 = (int)math.floor(floatIdx);
                int idx1 = math.min(idx0 + 1, pathCount - 1);
                float t = floatIdx - (float)idx0;

                float3 p0 = pathPositions[idx0];
                float3 p1 = pathPositions[idx1];
                float3 r0 = pathRights[idx0];
                float3 r1 = pathRights[idx1];
                float3 u0 = pathUps[idx0];
                float3 u1 = pathUps[idx1];
                float3 f0 = pathForwards[idx0];
                float3 f1 = pathForwards[idx1];

                float3 p = math.lerp(p0, p1, t);
                float3 r = SafeNormalize(math.lerp(r0, r1, t), new float3(One, Zero, Zero));
                float3 upVec = SafeNormalize(math.lerp(u0, u1, t), new float3(Zero, One, Zero));
                float3 f = SafeNormalize(math.lerp(f0, f1, t), new float3(Zero, Zero, One));

                float3 vLocal = inVertices[i];
                float3 vDeformed = p + vLocal.x * r * scale.x + vLocal.y * upVec * scale.y;
                outVertices[i] = vDeformed;

                if (inNormals != null && outNormals != null)
                {
                    float3 nLocal = inNormals[i];
                    float3 nDeformed = nLocal.x * r + nLocal.y * upVec + nLocal.z * f;
                    float len = math.length(nDeformed);
                    outNormals[i] = len > Epsilon ? nDeformed / len : upVec;
                }
            }
        }

        public static void RecalculateNormals(
            float3* vertices, 
            int vertexCount, 
            int* indices, 
            int indexCount, 
            float3* outNormals)
        {
            if (vertexCount <= 0)
            {
                return;
            }

            for (int i = 0; i < vertexCount; i++)
            {
                outNormals[i] = new float3(Zero, Zero, Zero);
            }

            int triangleCount = indexCount / 3;
            for (int i = 0; i < triangleCount; i++)
            {
                int idx0 = indices[i * 3];
                int idx1 = indices[i * 3 + 1];
                int idx2 = indices[i * 3 + 2];

                float3 v0 = vertices[idx0];
                float3 v1 = vertices[idx1];
                float3 v2 = vertices[idx2];

                float3 edge1 = v1 - v0;
                float3 edge2 = v2 - v0;
                float3 faceNormal = math.cross(edge1, edge2);

                outNormals[idx0] += faceNormal;
                outNormals[idx1] += faceNormal;
                outNormals[idx2] += faceNormal;
            }

            for (int i = 0; i < vertexCount; i++)
            {
                float len = math.length(outNormals[i]);
                outNormals[i] = len > Epsilon ? outNormals[i] / len : new float3(Zero, One, Zero);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3 SafeNormalize(float3 v, float3 fallback)
        {
            float len = math.length(v);
            return len > Epsilon ? v / len : fallback;
        }
    }
}
