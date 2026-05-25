namespace IAFahim.Geometry.Curve
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class CatmullRom
    {
        private const float Half = 0.5f;
        private const float Zero = 0.0f;
        private const float One = 1.0f;
        private const float Two = 2.0f;
        private const float Three = 3.0f;
        private const float Four = 4.0f;
        private const float Five = 5.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Evaluate(float3 p0, float3 p1, float3 p2, float3 p3, float t)
        {
            float tt = t * t;
            float ttt = tt * t;

            float3 a = Two * p1;
            float3 b = p2 - p0;
            float3 c = Two * p0 - Five * p1 + Four * p2 - p3;
            float3 d = p3 - p0 + Three * (p1 - p2);

            float3 p = Half * (a + b * t + c * tt + d * ttt);
            return p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 EvaluateTangent(float3 p0, float3 p1, float3 p2, float3 p3, float t)
        {
            float tt = t * t;

            float3 b = p2 - p0;
            float3 c = Two * p0 - Five * p1 + Four * p2 - p3;
            float3 d = p3 - p0 + Three * (p1 - p2);

            float3 tangent = Half * (b + Two * c * t + Three * d * tt);
            return tangent;
        }
    }
}
