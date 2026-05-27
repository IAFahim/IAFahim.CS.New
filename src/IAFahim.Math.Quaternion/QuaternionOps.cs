namespace IAFahim.Math.Quaternion
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class QuaternionOps
    {
        private const float Half = 0.5f;
        private const float One = 1.0f;
        private const float Two = 2.0f;
        private const float RadToDeg = 57.2957795f;
        private const float DegToRad = 0.0174532925f;
        private const float Epsilon = 1e-6f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FromAxisAngle(float3 axis, float angleRadians)
        {
            float len = math.length(axis);
            if (len < Epsilon)
            {
                return quaternion.identity;
            }

            float3 n = axis / len;
            float halfAngle = angleRadians * Half;
            float s = math.sin(halfAngle);
            return new quaternion(n.x * s, n.y * s, n.z * s, math.cos(halfAngle));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToAxisAngle(quaternion q, out float3 axis, out float angle)
        {
            float w = math.clamp(q.value.w, -One, One);
            float sinHalf = math.sqrt(One - w * w);

            if (sinHalf < Epsilon)
            {
                axis = new float3(0.0f, One, 0.0f);
                angle = 0.0f;
                return;
            }

            axis = new float3(q.value.x, q.value.y, q.value.z) / sinHalf;
            angle = Two * math.acos(w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion FromEuler(float3 eulerRadians)
        {
            float3 half = eulerRadians * Half;
            float3 c = math.cos(half);
            float3 s = math.sin(half);

            return new quaternion(
                s.x * c.y * c.z - c.x * s.y * s.z,
                c.x * s.y * c.z + s.x * c.y * s.z,
                c.x * c.y * s.z - s.x * s.y * c.z,
                c.x * c.y * c.z + s.x * s.y * s.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 ToEuler(quaternion q)
        {
            float4 v = q.value;
            float sinRCosP = Two * (v.w * v.x + v.y * v.z);
            float cosRCosP = One - Two * (v.x * v.x + v.y * v.y);
            float roll = math.atan2(sinRCosP, cosRCosP);

            float sinP = Two * (v.w * v.y - v.z * v.x);
            float pitch = math.abs(sinP) >= One
                ? math.select(-math.PI * Half, math.PI * Half, sinP > 0.0f)
                : math.asin(sinP);

            float sinYCosP = Two * (v.w * v.z + v.x * v.y);
            float cosYCosP = One - Two * (v.y * v.y + v.z * v.z);
            float yaw = math.atan2(sinYCosP, cosYCosP);

            return new float3(roll, pitch, yaw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion LookRotation(float3 forward, float3 up)
        {
            float3 f = math.normalizesafe(forward);
            float3 r = math.normalizesafe(math.cross(up, f));
            float3 u = math.cross(f, r);

            if (math.lengthsq(r) < Epsilon)
            {
                return quaternion.identity;
            }

            float m00 = r.x, m01 = u.x, m02 = f.x;
            float m10 = r.y, m11 = u.y, m12 = f.y;
            float m20 = r.z, m21 = u.z, m22 = f.z;

            float trace = m00 + m11 + m22;

            if (trace > Epsilon)
            {
                float s = math.sqrt(trace + One) * Two;
                return new quaternion(
                    (m21 - m12) / s,
                    (m02 - m20) / s,
                    (m10 - m01) / s,
                    s * Half);
            }

            if (m00 > m11 && m00 > m22)
            {
                float s = math.sqrt(One + m00 - m11 - m22) * Two;
                return new quaternion(
                    s * Half,
                    (m10 + m01) / s,
                    (m02 + m20) / s,
                    (m21 - m12) / s);
            }

            if (m11 > m22)
            {
                float s = math.sqrt(One + m11 - m00 - m22) * Two;
                return new quaternion(
                    (m10 + m01) / s,
                    s * Half,
                    (m21 + m12) / s,
                    (m02 - m20) / s);
            }

            {
                float s = math.sqrt(One + m22 - m00 - m11) * Two;
                return new quaternion(
                    (m02 + m20) / s,
                    (m21 + m12) / s,
                    s * Half,
                    (m10 - m01) / s);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 RotateVector(quaternion q, float3 v)
        {
            float3 u = new float3(q.value.x, q.value.y, q.value.z);
            float s = q.value.w;
            float3 t = Two * math.cross(u, v);
            return v + s * t + math.cross(u, t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion Conjugate(quaternion q)
        {
            return new quaternion(-q.value.x, -q.value.y, -q.value.z, q.value.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(quaternion a, quaternion b)
        {
            return math.dot(a.value, b.value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Length(quaternion q)
        {
            return math.length(q.value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion Normalize(quaternion q)
        {
            float len = math.length(q.value);
            if (len < Epsilon)
            {
                return quaternion.identity;
            }

            float inv = One / len;
            return new quaternion(q.value.x * inv, q.value.y * inv, q.value.z * inv, q.value.w * inv);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleBetween(quaternion a, quaternion b)
        {
            float d = math.clamp(math.dot(a.value, b.value), -One, One);
            if (d < 0.0f)
            {
                d = -d;
            }

            return Two * math.acos(d);
        }
    }
}
