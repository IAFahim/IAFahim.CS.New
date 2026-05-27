namespace IAFahim.Math.PotentialField
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class PotentialField2D
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 Attractive(float2 position, float2 target, float strength)
        {
            float2 delta = target - position;
            float dist = math.length(delta);

            if (dist < 1e-6f)
            {
                return float2.zero;
            }

            return strength * delta / dist;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 Repulsive(float2 position, float2 obstacle, float radius, float strength)
        {
            float2 delta = position - obstacle;
            float dist = math.length(delta);

            if (dist > radius || dist < 1e-6f)
            {
                return float2.zero;
            }

            float magnitude = strength * (1.0f / dist - 1.0f / radius) / (dist * dist);
            return magnitude * delta / dist;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 Tangential(float2 position, float2 obstacle, float radius, float strength)
        {
            float2 delta = position - obstacle;
            float dist = math.length(delta);

            if (dist > radius || dist < 1e-6f)
            {
                return float2.zero;
            }

            float2 tangent = new float2(-delta.y, delta.x);
            return strength * tangent / dist;
        }

        public static void ComputeGradient(
            float2 position,
            float2* attractors, int attractorCount, float attractStrength,
            float2* repulsors, int repulsorCount, float repulsorRadius, float repulsorStrength,
            float2* tangentials, int tangentialCount, float tangentialRadius, float tangentialStrength,
            out float2 gradient)
        {
            gradient = float2.zero;

            for (int i = 0; i < attractorCount; i++)
            {
                gradient += Attractive(position, attractors[i], attractStrength);
            }

            for (int i = 0; i < repulsorCount; i++)
            {
                gradient += Repulsive(position, repulsors[i], repulsorRadius, repulsorStrength);
            }

            for (int i = 0; i < tangentialCount; i++)
            {
                gradient += Tangential(position, tangentials[i], tangentialRadius, tangentialStrength);
            }
        }

        public static int GradientDescent(
            float2 start,
            float2* attractors, int attractorCount, float attractStrength,
            float2* repulsors, int repulsorCount, float repulsorRadius, float repulsorStrength,
            float2* tangentials, int tangentialCount, float tangentialRadius, float tangentialStrength,
            float stepSize, float tolerance, int maxSteps,
            float2* path)
        {
            float2 pos = start;
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
                    tangentials, tangentialCount, tangentialRadius, tangentialStrength,
                    out float2 grad);

                float gradLen = math.length(grad);

                if (gradLen < tolerance)
                {
                    break;
                }

                float2 dir = grad / gradLen;
                pos = pos + dir * stepSize;
                path[stepCount++] = pos;

                if (attractorCount > 0)
                {
                    float distToTarget = math.length(pos - attractors[0]);
                    if (distToTarget < tolerance)
                    {
                        break;
                    }
                }
            }

            return stepCount;
        }
    }
}
