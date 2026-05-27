namespace IAFahim.Math.Sdf
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class SdfPrimitive
    {
        private const float Zero = 0.0f;
        private const float One = 1.0f;
        private const float Two = 2.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sphere(float3 p, float radius)
        {
            return math.length(p) - radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Box(float3 p, float3 halfExtents)
        {
            float3 q = math.abs(p) - halfExtents;
            return math.length(math.max(q, Zero)) + math.min(math.max(q.x, math.max(q.y, q.z)), Zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Capsule(float3 p, float3 a, float3 b, float radius)
        {
            float3 pa = p - a, ba = b - a;
            float h = math.clamp(math.dot(pa, ba) / math.dot(ba, ba), Zero, One);
            return math.length(pa - ba * h) - radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Torus(float3 p, float majorRadius, float minorRadius)
        {
            float2 q = new float2(math.length(new float2(p.x, p.z)) - majorRadius, p.y);
            return math.length(q) - minorRadius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cylinder(float3 p, float radius, float halfHeight)
        {
            float2 d = math.abs(new float2(math.length(new float2(p.x, p.z)), p.y)) - new float2(radius, halfHeight);
            return math.min(math.max(d.x, d.y), Zero) + math.length(math.max(d, Zero));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Plane(float3 p, float3 n, float distance)
        {
            return math.dot(p, n) + distance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ellipsoid(float3 p, float3 radii)
        {
            float3 scaled = p / radii;
            float k0 = math.length(scaled);
            float3 k1V = scaled / radii;
            float k1 = math.length(k1V);
            return k0 * (k0 - One) / k1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cone(float3 p, float2 dimensions)
        {
            float2 q = new float2(math.length(new float2(p.x, p.z)), p.y);
            float2 tip = q - dimensions;
            float ca = new float2(dimensions.x, -dimensions.y).x;
            float cb = new float2(dimensions.x, -dimensions.y).y;
            float d = math.max(tip.x, tip.y);
            float side = math.dot(q - dimensions * math.clamp(math.dot(q, dimensions) / math.dot(dimensions, dimensions), Zero, One), dimensions);
            return math.length(math.max(new float2(d, -side), Zero)) + math.min(d, Zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Octahedron(float3 p, float size)
        {
            float3 q = math.abs(p);
            return (q.x + q.y + q.z - size) * 0.57735027f;
        }
    }
}
