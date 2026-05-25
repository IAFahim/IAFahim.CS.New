namespace IAFahim.Geometry.Curve
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class CubicBezier
    {
        private const int SampleCount = 128;
        private const float One = 1.0f;
        private const float Zero = 0.0f;
        private const float Two = 2.0f;
        private const float Three = 3.0f;
        private const float Six = 6.0f;
        private const float Epsilon = 1e-6f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Evaluate(float3 p0, float3 p1, float3 p2, float3 p3, float t)
        {
            float u = One - t;
            float uu = u * u;
            float uuu = uu * u;
            float tt = t * t;
            float ttt = tt * t;

            float3 p = uuu * p0 + Three * uu * t * p1 + Three * u * tt * p2 + ttt * p3;
            return p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 EvaluateTangent(float3 p0, float3 p1, float3 p2, float3 p3, float t)
        {
            float u = One - t;
            float uu = u * u;
            float tt = t * t;

            float3 tangent = Three * uu * (p1 - p0) + Six * u * t * (p2 - p1) + Three * tt * (p3 - p2);
            return tangent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float IntegrateArcLength(float3 p0, float3 p1, float3 p2, float3 p3)
        {
            float length = Zero;
            float step = One / (float)SampleCount;
            float3 prev = p0;

            for (int i = 1; i <= SampleCount; i++)
            {
                float t = (float)i * step;
                float3 curr = Evaluate(p0, p1, p2, p3, t);
                length += math.distance(curr, prev);
                prev = curr;
            }

            return length;
        }

        public static void UniformSample(float3 p0, float3 p1, float3 p2, float3 p3, int count, float3* outPositions, float3* outTangents)
        {
            if (count <= 0)
            {
                return;
            }

            if (count == 1)
            {
                outPositions[0] = p0;
                outTangents[0] = SafeNormalize(EvaluateTangent(p0, p1, p2, p3, Zero), new float3(Zero, Zero, One));
                return;
            }

            float* lut = stackalloc float[SampleCount + 1];
            lut[0] = Zero;
            float3 prev = p0;
            float step = One / (float)SampleCount;

            for (int i = 1; i <= SampleCount; i++)
            {
                float t = (float)i * step;
                float3 curr = Evaluate(p0, p1, p2, p3, t);
                lut[i] = lut[i - 1] + math.distance(curr, prev);
                prev = curr;
            }

            float totalLength = lut[SampleCount];
            outPositions[0] = p0;
            outTangents[0] = SafeNormalize(EvaluateTangent(p0, p1, p2, p3, Zero), new float3(Zero, Zero, One));

            float targetStep = totalLength / (float)(count - 1);

            for (int i = 1; i < count - 1; i++)
            {
                float targetDist = (float)i * targetStep;
                float t = FindT(lut, targetDist, step);
                outPositions[i] = Evaluate(p0, p1, p2, p3, t);
                outTangents[i] = SafeNormalize(EvaluateTangent(p0, p1, p2, p3, t), new float3(Zero, Zero, One));
            }

            outPositions[count - 1] = p3;
            outTangents[count - 1] = SafeNormalize(EvaluateTangent(p0, p1, p2, p3, One), new float3(Zero, Zero, One));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float FindT(float* lut, float targetDist, float step)
        {
            int low = 0;
            int high = SampleCount;

            while (low < high - 1)
            {
                int mid = (low + high) >> 1;
                if (lut[mid] < targetDist)
                {
                    low = mid;
                }
                else
                {
                    high = mid;
                }
            }

            float d0 = lut[low];
            float d1 = lut[high];
            float segmentLength = d1 - d0;
            float factor = segmentLength > Zero ? (targetDist - d0) / segmentLength : Zero;
            float t = ((float)low + factor) * step;
            return t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3 SafeNormalize(float3 v, float3 fallback)
        {
            float len = math.length(v);
            return len > Epsilon ? v / len : fallback;
        }
    }
}
