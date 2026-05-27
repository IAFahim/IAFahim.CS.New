namespace IAFahim.Physics.Xpbd
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class ShapeMatchingConstraint
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Solve(
            float3* positions, float3* restPositions, float* invMasses,
            int count, float compliance, float dt, float3* deltas)
        {
            float3 centerOfMass = ComputeCenterOfMass(positions, invMasses, count);
            float3 restCenter = ComputeCenterOfMass(restPositions, invMasses, count);

            float3x3 rotation = ComputeOptimalRotation(
                positions, restPositions, invMasses, count, centerOfMass, restCenter);

            float alpha = compliance / (dt * dt);

            for (int i = 0; i < count; i++)
            {
                float3 restOffset = restPositions[i] - restCenter;
                float3 target = centerOfMass + math.mul(rotation, restOffset);

                float w = 1.0f / (invMasses[i] + alpha);

                deltas[i] = (target - positions[i]) * math.min(1.0f, w);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3 ComputeCenterOfMass(float3* positions, float* invMasses, int count)
        {
            float3 com = float3.zero;
            float totalMass = 0.0f;

            for (int i = 0; i < count; i++)
            {
                float mass = invMasses[i] > 1e-8f ? 1.0f / invMasses[i] : 0.0f;
                com += positions[i] * mass;
                totalMass += mass;
            }

            if (totalMass > 1e-8f)
            {
                com /= totalMass;
            }

            return com;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3x3 ComputeOptimalRotation(
            float3* positions, float3* restPositions, float* invMasses,
            int count, float3 center, float3 restCenter)
        {
            float3x3 apq = new float3x3(0.0f);

            for (int i = 0; i < count; i++)
            {
                float mass = invMasses[i] > 1e-8f ? 1.0f / invMasses[i] : 0.0f;
                float3 p = positions[i] - center;
                float3 q = restPositions[i] - restCenter;

                apq.c0.x += mass * p.x * q.x;
                apq.c0.y += mass * p.y * q.x;
                apq.c0.z += mass * p.z * q.x;
                apq.c1.x += mass * p.x * q.y;
                apq.c1.y += mass * p.y * q.y;
                apq.c1.z += mass * p.z * q.y;
                apq.c2.x += mass * p.x * q.z;
                apq.c2.y += mass * p.y * q.z;
                apq.c2.z += mass * p.z * q.z;
            }

            float3x3 s = new float3x3(
                apq.c0.x * apq.c0.x + apq.c0.y * apq.c0.y + apq.c0.z * apq.c0.z,
                apq.c0.x * apq.c1.x + apq.c0.y * apq.c1.y + apq.c0.z * apq.c1.z,
                apq.c0.x * apq.c2.x + apq.c0.y * apq.c2.y + apq.c0.z * apq.c2.z,
                apq.c1.x * apq.c0.x + apq.c1.y * apq.c0.y + apq.c1.z * apq.c0.z,
                apq.c1.x * apq.c1.x + apq.c1.y * apq.c1.y + apq.c1.z * apq.c1.z,
                apq.c1.x * apq.c2.x + apq.c1.y * apq.c2.y + apq.c1.z * apq.c2.z,
                apq.c2.x * apq.c0.x + apq.c2.y * apq.c0.y + apq.c2.z * apq.c0.z,
                apq.c2.x * apq.c1.x + apq.c2.y * apq.c1.y + apq.c2.z * apq.c1.z,
                apq.c2.x * apq.c2.x + apq.c2.y * apq.c2.y + apq.c2.z * apq.c2.z);

            return apq;
        }
    }
}
