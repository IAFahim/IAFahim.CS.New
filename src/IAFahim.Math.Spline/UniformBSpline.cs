namespace IAFahim.Math.Spline
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class UniformBSpline
    {
        private const float One = 1.0f;
        private const float Two = 2.0f;
        private const float Three = 3.0f;
        private const float Four = 4.0f;
        private const float Six = 6.0f;
        private const float Nine = 9.0f;
        private const float Twelve = 12.0f;
        private const float OneSixth = 0.16666667f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Evaluate(float3 p0, float3 p1, float3 p2, float3 p3, float t)
        {
            float tt = t * t;
            float ttt = tt * t;

            float b0 = -ttt + Three * tt - Three * t + One;
            float b1 = Three * ttt - Six * tt + Four;
            float b2 = -Three * ttt + Three * tt + Three * t + One;
            float b3 = ttt;

            return OneSixth * (b0 * p0 + b1 * p1 + b2 * p2 + b3 * p3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 EvaluateTangent(float3 p0, float3 p1, float3 p2, float3 p3, float t)
        {
            float tt = t * t;

            float db0 = -Three * tt + Two * Three * t - Three;
            float db1 = Nine * tt - Twelve * t;
            float db2 = -Three * Three * tt + Two * Three * t + Three;
            float db3 = Three * tt;

            return OneSixth * (db0 * p0 + db1 * p1 + db2 * p2 + db3 * p3);
        }

        public static void UniformSample(float3 p0, float3 p1, float3 p2, float3 p3,
            int count, float3* outPositions, float3* outTangents, int lutSamples)
        {
            if (count <= 0)
            {
                return;
            }

            if (count == 1)
            {
                outPositions[0] = Evaluate(p0, p1, p2, p3, 0.0f);
                outTangents[0] = SafeNormalize(EvaluateTangent(p0, p1, p2, p3, 0.0f));
                return;
            }

            float* lut = stackalloc float[lutSamples + 1];
            lut[0] = 0.0f;
            float3 prev = Evaluate(p0, p1, p2, p3, 0.0f);
            float step = One / (float)lutSamples;

            for (int i = 1; i <= lutSamples; i++)
            {
                float t = (float)i * step;
                float3 curr = Evaluate(p0, p1, p2, p3, t);
                lut[i] = lut[i - 1] + math.distance(curr, prev);
                prev = curr;
            }

            float totalLength = lut[lutSamples];
            outPositions[0] = Evaluate(p0, p1, p2, p3, 0.0f);
            outTangents[0] = SafeNormalize(EvaluateTangent(p0, p1, p2, p3, 0.0f));

            float targetStep = totalLength / (float)(count - 1);

            for (int i = 1; i < count - 1; i++)
            {
                float targetDist = (float)i * targetStep;
                float t = FindT(lut, targetDist, step, lutSamples);
                outPositions[i] = Evaluate(p0, p1, p2, p3, t);
                outTangents[i] = SafeNormalize(EvaluateTangent(p0, p1, p2, p3, t));
            }

            outPositions[count - 1] = Evaluate(p0, p1, p2, p3, 1.0f);
            outTangents[count - 1] = SafeNormalize(EvaluateTangent(p0, p1, p2, p3, 1.0f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float FindT(float* lut, float targetDist, float step, int lutSamples)
        {
            int low = 0;
            int high = lutSamples;

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
            float factor = segmentLength > 1e-6f ? (targetDist - d0) / segmentLength : 0.0f;
            return ((float)low + factor) * step;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3 SafeNormalize(float3 v)
        {
            float len = math.length(v);
            return len > 1e-6f ? v / len : new float3(0.0f, 0.0f, 1.0f);
        }
    }
}
