namespace IAFahim.Physics.Xpbd
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class BendingConstraint
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Solve(float3* p0, float3* p1, float3* p2,
            float invMass0, float invMass1, float invMass2,
            float restAngle, float compliance, float dt)
        {
            float3 d1 = *p1 - *p0;
            float3 d2 = *p2 - *p0;

            float len1 = math.length(d1);
            float len2 = math.length(d2);

            if (len1 < 1e-8f || len2 < 1e-8f)
            {
                return;
            }

            float3 n1 = d1 / len1;
            float3 n2 = d2 / len2;

            float cosAngle = math.clamp(math.dot(n1, n2), -1.0f, 1.0f);
            float angle = math.acos(cosAngle);

            float c = angle - restAngle;

            float alpha = compliance / (dt * dt);

            float3 grad0 = (-n2 + cosAngle * n1) / len1
                          + (-n1 + cosAngle * n2) / len2;
            float3 grad1 = (n2 - cosAngle * n1) / len1;
            float3 grad2 = (n1 - cosAngle * n2) / len2;

            float w = invMass0 * math.dot(grad0, grad0)
                    + invMass1 * math.dot(grad1, grad1)
                    + invMass2 * math.dot(grad2, grad2);

            if (w + alpha < 1e-12f)
            {
                return;
            }

            float lambda = -c / (w + alpha);

            *p0 = *p0 + invMass0 * lambda * grad0;
            *p1 = *p1 + invMass1 * lambda * grad1;
            *p2 = *p2 + invMass2 * lambda * grad2;
        }
    }
}
