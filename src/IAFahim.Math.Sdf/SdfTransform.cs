namespace IAFahim.Math.Sdf
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class SdfTransform
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Translate(float3 p, float3 offset)
        {
            return p - offset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 RotateX(float3 p, float angle)
        {
            float c = math.cos(angle), s = math.sin(angle);
            return new float3(p.x, c * p.y - s * p.z, s * p.y + c * p.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 RotateY(float3 p, float angle)
        {
            float c = math.cos(angle), s = math.sin(angle);
            return new float3(c * p.x + s * p.z, p.y, -s * p.x + c * p.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 RotateZ(float3 p, float angle)
        {
            float c = math.cos(angle), s = math.sin(angle);
            return new float3(c * p.x - s * p.y, s * p.x + c * p.y, p.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Scale(float3 p, float3 scale)
        {
            return p / scale;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ScaleDistance(float d, float3 scale)
        {
            return d * math.cmin(scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Repeat(float3 p, float3 cellSize)
        {
            return p - cellSize * math.round(p / cellSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 MirrorX(float3 p)
        {
            p.x = math.abs(p.x);
            return p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 MirrorY(float3 p)
        {
            p.y = math.abs(p.y);
            return p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 MirrorZ(float3 p)
        {
            p.z = math.abs(p.z);
            return p;
        }
    }
}
