namespace IAFahim.Math.Quaternion
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class SwingTwistDecomposition
    {
        private const float Epsilon = 1e-6f;
        private const float One = 1.0f;
        private const float Two = 2.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(quaternion q, float3 twistAxis, out quaternion swing, out quaternion twist)
        {
            float3 qv = new float3(q.value.x, q.value.y, q.value.z);
            float dot = math.dot(qv, twistAxis);

            float3 twistPart = dot * twistAxis;
            twist = new quaternion(twistPart.x, twistPart.y, twistPart.z, q.value.w);

            float twistLen = math.length(new float3(twist.value.x, twist.value.y, twist.value.z));
            if (twistLen < Epsilon)
            {
                twist = quaternion.identity;
                swing = q;
                return;
            }

            float twistMag = math.sqrt(twistLen * twistLen + twist.value.w * twist.value.w);
            if (twistMag > Epsilon)
            {
                float inv = One / twistMag;
                twist = new quaternion(
                    twist.value.x * inv,
                    twist.value.y * inv,
                    twist.value.z * inv,
                    twist.value.w * inv);
            }

            swing = math.mul(q, math.conjugate(twist));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float TwistAngle(quaternion q, float3 twistAxis)
        {
            float3 qv = new float3(q.value.x, q.value.y, q.value.z);
            float dot = math.dot(qv, twistAxis);
            float3 projection = dot * twistAxis;

            float twistLen = math.length(projection);
            return Two * math.atan2(twistLen, q.value.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FromTwistAngle(float angle, float3 twistAxis)
        {
            float halfAngle = angle * 0.5f;
            float s = math.sin(halfAngle);
            return new quaternion(
                twistAxis.x * s,
                twistAxis.y * s,
                twistAxis.z * s,
                math.cos(halfAngle));
        }
    }
}
