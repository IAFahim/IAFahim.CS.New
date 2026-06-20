namespace IAFahim.Math.PotentialField
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class PotentialField3D
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Attractive(float3 position, float3 target, float strength)
        {
            float3 delta = target - position;
            float dist = math.length(delta);

            if (dist < 1e-6f)
            {
                return float3.zero;
            }

            return strength * delta / dist;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Repulsive(float3 position, float3 obstacle, float radius, float strength)
        {
            float3 delta = position - obstacle;
            float dist = math.length(delta);

            if (dist > radius || dist < 1e-6f)
            {
                return float3.zero;
            }

            float magnitude = strength * (1.0f / dist - 1.0f / radius) / (dist * dist);
            return magnitude * delta / dist;
        }

        public static void ComputeGradient(
            float3 position,
            float3* attractors, int attractorCount, float attractStrength,
            float3* repulsors, int repulsorCount, float repulsorRadius, float repulsorStrength,
            out float3 gradient)
        {
            gradient = float3.zero;

            for (int i = 0; i < attractorCount; i++)
            {
                gradient += Attractive(position, attractors[i], attractStrength);
            }

            for (int i = 0; i < repulsorCount; i++)
            {
                gradient += Repulsive(position, repulsors[i], repulsorRadius, repulsorStrength);
            }
        }

        public static int GradientDescent(
            float3 start,
            float3* attractors, int attractorCount, float attractStrength,
            float3* repulsors, int repulsorCount, float repulsorRadius, float repulsorStrength,
            float stepSize, float tolerance, int maxSteps,
            float3* path)
        {
            float3 pos = start;
            int stepCount = 0;
            if (maxSteps <= 0)
            {
                return 0;
            }
            path[stepCount++] = pos;

            for (int i = 0; i < maxSteps - 1; i++)
            {
                ComputeGradient(pos, attractors, attractorCount, attractStrength,
                    repulsors, repulsorCount, repulsorRadius, repulsorStrength,
                    out float3 grad);

                float gradLen = math.length(grad);

                if (gradLen < tolerance)
                {
                    break;
                }

                float3 dir = grad / gradLen;
                pos = pos + dir * stepSize;
                path[stepCount++] = pos;

                if (attractorCount > 0)
                {
                    float distToTargetSq = math.lengthsq(pos - attractors[0]);
                    if (distToTargetSq < tolerance * tolerance)
                    {
                        break;
                    }
                }
            }

            return stepCount;
        }
    }
}
