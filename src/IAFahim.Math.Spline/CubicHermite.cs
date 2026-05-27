namespace IAFahim.Math.Spline
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class CubicHermite
    {
        private const float Zero = 0.0f;
        private const float One = 1.0f;
        private const float Two = 2.0f;
        private const float Three = 3.0f;
        private const float Four = 4.0f;
        private const float Six = 6.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Evaluate(float3 p0, float3 m0, float3 p1, float3 m1, float t)
        {
            float tt = t * t;
            float ttt = tt * t;

            float h00 = Two * ttt - Three * tt + One;
            float h10 = ttt - Two * tt + t;
            float h01 = -Two * ttt + Three * tt;
            float h11 = ttt - tt;

            return h00 * p0 + h10 * m0 + h01 * p1 + h11 * m1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 EvaluateTangent(float3 p0, float3 m0, float3 p1, float3 m1, float t)
        {
            float tt = t * t;

            float dh00 = Six * tt - Six * t;
            float dh10 = Three * tt - Four * t + One;
            float dh01 = -Six * tt + Six * t;
            float dh11 = Three * tt - Two * t;

            return dh00 * p0 + dh10 * m0 + dh01 * p1 + dh11 * m1;
        }

        public static float IntegrateArcLength(float3 p0, float3 m0, float3 p1, float3 m1, int sampleCount)
        {
            float length = Zero;
            float step = One / (float)sampleCount;
            float3 prev = p0;

            for (int i = 1; i <= sampleCount; i++)
            {
                float t = (float)i * step;
                float3 curr = Evaluate(p0, m0, p1, m1, t);
                length += math.distance(curr, prev);
                prev = curr;
            }

            return length;
        }
    }
}
