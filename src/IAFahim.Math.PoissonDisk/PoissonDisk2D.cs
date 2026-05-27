namespace IAFahim.Math.PoissonDisk
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class PoissonDisk2D
    {
        private const int MaxAttempts = 30;

        public static int Run(float2 min, float2 max, float minDistance, float2* output, int maxPoints, int seed)
        {
            float cellSize = minDistance / 1.41421356f;
            int gridW = (int)math.ceil((max.x - min.x) / cellSize);
            int gridH = (int)math.ceil((max.y - min.y) / cellSize);

            if (gridW <= 0 || gridH <= 0)
            {
                return 0;
            }

            int gridSize = gridW * gridH;
            int* grid = stackalloc int[gridSize];
            for (int i = 0; i < gridSize; i++)
            {
                grid[i] = -1;
            }

            float2* active = stackalloc float2[maxPoints];
            int activeCount = 0;
            int pointCount = 0;

            uint rng = (uint)seed;
            float2 first = new float2(
                min.x + NextFloat(ref rng) * (max.x - min.x),
                min.y + NextFloat(ref rng) * (max.y - min.y));

            if (pointCount >= maxPoints)
            {
                return 0;
            }

            output[pointCount] = first;
            active[activeCount++] = first;
            InsertGrid(grid, gridW, gridH, cellSize, min, first, pointCount);
            pointCount++;

            while (activeCount > 0 && pointCount < maxPoints)
            {
                int activeIdx = (int)(NextFloat(ref rng) * activeCount);
                float2 center = active[activeIdx];
                bool found = false;

                for (int attempt = 0; attempt < MaxAttempts; attempt++)
                {
                    float angle = NextFloat(ref rng) * 6.28318530f;
                    float radius = minDistance + NextFloat(ref rng) * minDistance;
                    float2 candidate = center + new float2(math.cos(angle), math.sin(angle)) * radius;

                    if (candidate.x < min.x || candidate.x > max.x || candidate.y < min.y || candidate.y > max.y)
                    {
                        continue;
                    }

                    if (IsTooClose(grid, gridW, gridH, cellSize, min, output, pointCount, candidate, minDistance))
                    {
                        continue;
                    }

                    output[pointCount] = candidate;
                    active[activeCount++] = candidate;
                    InsertGrid(grid, gridW, gridH, cellSize, min, candidate, pointCount);
                    pointCount++;
                    found = true;
                    break;
                }

                if (!found)
                {
                    activeCount--;
                    if (activeIdx < activeCount)
                    {
                        active[activeIdx] = active[activeCount];
                    }
                }
            }

            return pointCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InsertGrid(int* grid, int gridW, int gridH, float cellSize, float2 min, float2 point, int index)
        {
            int gx = (int)((point.x - min.x) / cellSize);
            int gy = (int)((point.y - min.y) / cellSize);
            gx = math.clamp(gx, 0, gridW - 1);
            gy = math.clamp(gy, 0, gridH - 1);
            grid[gy * gridW + gx] = index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsTooClose(int* grid, int gridW, int gridH, float cellSize, float2 min, float2* points, int pointCount, float2 candidate, float minDist)
        {
            int gx = (int)((candidate.x - min.x) / cellSize);
            int gy = (int)((candidate.y - min.y) / cellSize);
            float minDistSq = minDist * minDist;

            for (int dy = -2; dy <= 2; dy++)
            {
                for (int dx = -2; dx <= 2; dx++)
                {
                    int nx = gx + dx;
                    int ny = gy + dy;
                    if (nx < 0 || nx >= gridW || ny < 0 || ny >= gridH)
                    {
                        continue;
                    }

                    int idx = grid[ny * gridW + nx];
                    if (idx < 0)
                    {
                        continue;
                    }

                    float distSq = math.lengthsq(candidate - points[idx]);
                    if (distSq < minDistSq)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float NextFloat(ref uint state)
        {
            state = state * 747796405u + 2891336453u;
            uint result = ((state >> ((int)(state >> 28) + 4)) ^ state) * 277803737u;
            result = (result >> 22) ^ result;
            return (float)result / 4294967295.0f;
        }
    }
}
