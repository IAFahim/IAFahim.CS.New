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
            float3 centerOfMass;
            float3 restCenter;
            ComputeCentersOfMass(
                positions, restPositions, invMasses, count, out centerOfMass, out restCenter);

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

        // Computes the mass-weighted centers of the live and rest configurations in a single
        // pass. Both share the same invMasses (hence the same per-particle mass and totalMass),
        // so fusing the two former passes removes one full stream over the data and N divisions.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ComputeCentersOfMass(
            float3* positions, float3* restPositions, float* invMasses, int count,
            out float3 center, out float3 restCenter)
        {
            float3 com = float3.zero;
            float3 restCom = float3.zero;
            float totalMass = 0.0f;

            for (int i = 0; i < count; i++)
            {
                float mass = invMasses[i] > 1e-8f ? 1.0f / invMasses[i] : 0.0f;
                com += positions[i] * mass;
                restCom += restPositions[i] * mass;
                totalMass += mass;
            }

            if (totalMass > 1e-8f)
            {
                float invTotalMass = 1.0f / totalMass;
                com *= invTotalMass;
                restCom *= invTotalMass;
            }

            center = com;
            restCenter = restCom;
        }

        private const int RotationExtractionIterations = 16;
        private const float RotationExtractionEpsilon = 1e-9f;

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

            return ExtractRotation(apq);
        }

        // Extracts the rotational part R of the cross-covariance matrix a (Apq) via the robust
        // iterative quaternion algorithm of Mueller et al. 2016, "A Robust Method to Extract the
        // Rotational Part of Deformations". This is the polar factor R = a * (a^T * a)^{-1/2} and,
        // unlike a direct polar/Higham iteration, stays stable for singular or rank-deficient a.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3x3 ExtractRotation(float3x3 a)
        {
            quaternion q = quaternion.identity;

            for (int iteration = 0; iteration < RotationExtractionIterations; iteration++)
            {
                float3x3 r = new float3x3(q);

                // Columns of r are the rotated basis vectors; columns of a are the targets.
                float3 numerator =
                    math.cross(r.c0, a.c0) +
                    math.cross(r.c1, a.c1) +
                    math.cross(r.c2, a.c2);

                float denominator = math.abs(
                    math.dot(r.c0, a.c0) +
                    math.dot(r.c1, a.c1) +
                    math.dot(r.c2, a.c2)) + RotationExtractionEpsilon;

                float3 omega = numerator / denominator;

                float angle = math.length(omega);
                if (angle < RotationExtractionEpsilon)
                {
                    break;
                }

                float3 axis = omega / angle;
                q = math.normalize(math.mul(quaternion.AxisAngle(axis, angle), q));
            }

            return new float3x3(q);
        }
    }
}
