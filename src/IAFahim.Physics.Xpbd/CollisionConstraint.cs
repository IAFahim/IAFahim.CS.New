namespace IAFahim.Physics.Xpbd
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class CollisionConstraint
    {
        private const float Zero = 0.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SolvePlane(
            float3* pos, float3* vel, float invMass,
            float3 planeNormal, float planeDistance, float restitution, float friction, float dt)
        {
            float d = math.dot(*pos, planeNormal) + planeDistance;

            if (d >= Zero)
            {
                return;
            }

            float3 vn = math.dot(*vel, planeNormal) * planeNormal;
            float3 vt = *vel - vn;

            *pos = *pos - d * planeNormal;

            if (math.dot(vn, planeNormal) < Zero)
            {
                *vel = -restitution * vn + (1.0f - friction) * vt;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SolveSphere(
            float3* posA, float3* posB, float3* velA, float3* velB,
            float invMassA, float invMassB, float radiusA, float radiusB,
            float restitution, float friction)
        {
            float3 delta = *posB - *posA;
            float dist = math.length(delta);
            float minDist = radiusA + radiusB;

            if (dist >= minDist || dist < 1e-8f)
            {
                return;
            }

            float3 n = delta / dist;
            float overlap = minDist - dist;

            float totalInvMass = invMassA + invMassB;
            if (totalInvMass < 1e-12f)
            {
                return;
            }

            *posA = *posA - n * overlap * (invMassA / totalInvMass);
            *posB = *posB + n * overlap * (invMassB / totalInvMass);

            float3 relVel = *velB - *velA;
            float vn = math.dot(relVel, n);

            if (vn > Zero)
            {
                return;
            }

            float jmag = -(1.0f + restitution) * vn / totalInvMass;
            float3 impulse = jmag * n;
            *velA = *velA - impulse * invMassA;
            *velB = *velB + impulse * invMassB;

            float3 vnA = math.dot(*velA, n) * n;
            float3 vtA = *velA - vnA;
            float3 vnB = math.dot(*velB, n) * n;
            float3 vtB = *velB - vnB;

            *velA = vnA + (1.0f - friction) * vtA;
            *velB = vnB + (1.0f - friction) * vtB;
        }
    }
}
