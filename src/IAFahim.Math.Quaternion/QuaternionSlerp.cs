namespace IAFahim.Math.Quaternion
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class QuaternionSlerp
    {
        private const float One = 1.0f;
        private const float Epsilon = 1e-6f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion Run(quaternion from, quaternion to, float t)
        {
            float d = math.dot(from.value, to.value);

            if (d < 0.0f)
            {
                to = new quaternion(-to.value.x, -to.value.y, -to.value.z, -to.value.w);
                d = -d;
            }

            if (d > One - Epsilon)
            {
                return Normalize(LerpUnsafe(from, to, t));
            }

            float theta0 = math.acos(d);
            float theta = theta0 * t;
            float sinTheta = math.sin(theta);
            float sinTheta0 = math.sin(theta0);

            float s0 = math.cos(theta) - d * sinTheta / sinTheta0;
            float s1 = sinTheta / sinTheta0;

            return new quaternion(
                s0 * from.value.x + s1 * to.value.x,
                s0 * from.value.y + s1 * to.value.y,
                s0 * from.value.z + s1 * to.value.z,
                s0 * from.value.w + s1 * to.value.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static quaternion LerpUnsafe(quaternion a, quaternion b, float t)
        {
            float oneMinusT = One - t;
            return new quaternion(
                oneMinusT * a.value.x + t * b.value.x,
                oneMinusT * a.value.y + t * b.value.y,
                oneMinusT * a.value.z + t * b.value.z,
                oneMinusT * a.value.w + t * b.value.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static quaternion Normalize(quaternion q)
        {
            float len = math.length(q.value);
            if (len < Epsilon)
            {
                return quaternion.identity;
            }

            float inv = One / len;
            return new quaternion(q.value.x * inv, q.value.y * inv, q.value.z * inv, q.value.w * inv);
        }
    }
}
