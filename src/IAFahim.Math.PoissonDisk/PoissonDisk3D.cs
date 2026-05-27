namespace IAFahim.Math.PoissonDisk
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class PoissonDisk3D
    {
        private const int MaxAttempts = 30;

        public static int Run(float3 min, float3 max, float minDistance, float3* output, int maxPoints, int seed)
        {
            float cellSize = minDistance / 1.73205081f;
            int gridW = (int)math.ceil((max.x - min.x) / cellSize);
            int gridH = (int)math.ceil((max.y - min.y) / cellSize);
            int gridD = (int)math.ceil((max.z - min.z) / cellSize);

            if (gridW <= 0 || gridH <= 0 || gridD <= 0)
            {
                return 0;
            }

            int gridSize = gridW * gridH * gridD;
            int* grid = stackalloc int[gridSize];
            for (int i = 0; i < gridSize; i++)
            {
                grid[i] = -1;
            }

            float3* active = stackalloc float3[maxPoints];
            int activeCount = 0;
            int pointCount = 0;

            uint rng = (uint)seed;
            float3 first = new float3(
                min.x + NextFloat(ref rng) * (max.x - min.x),
                min.y + NextFloat(ref rng) * (max.y - min.y),
                min.z + NextFloat(ref rng) * (max.z - min.z));

            if (pointCount >= maxPoints)
            {
                return 0;
            }

            output[pointCount] = first;
            active[activeCount++] = first;
            InsertGrid(grid, gridW, gridH, gridD, cellSize, min, first, pointCount);
            pointCount++;

            while (activeCount > 0 && pointCount < maxPoints)
            {
                int activeIdx = (int)(NextFloat(ref rng) * activeCount);
                float3 center = active[activeIdx];
                bool found = false;

                for (int attempt = 0; attempt < MaxAttempts; attempt++)
                {
                    float theta = NextFloat(ref rng) * 6.28318530f;
                    float phi = math.acos(2.0f * NextFloat(ref rng) - 1.0f);
                    float radius = minDistance + NextFloat(ref rng) * minDistance;

                    float sinPhi = math.sin(phi);
                    float3 dir = new float3(
                        sinPhi * math.cos(theta),
                        sinPhi * math.sin(theta),
                        math.cos(phi));

                    float3 candidate = center + dir * radius;

                    if (candidate.x < min.x || candidate.x > max.x ||
                        candidate.y < min.y || candidate.y > max.y ||
                        candidate.z < min.z || candidate.z > max.z)
                    {
                        continue;
                    }

                    if (IsTooClose(grid, gridW, gridH, gridD, cellSize, min, output, pointCount, candidate, minDistance))
                    {
                        continue;
                    }

                    output[pointCount] = candidate;
                    active[activeCount++] = candidate;
                    InsertGrid(grid, gridW, gridH, gridD, cellSize, min, candidate, pointCount);
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
        private static void InsertGrid(int* grid, int gridW, int gridH, int gridD, float cellSize, float3 min, float3 point, int index)
        {
            int gx = (int)((point.x - min.x) / cellSize);
            int gy = (int)((point.y - min.y) / cellSize);
            int gz = (int)((point.z - min.z) / cellSize);
            gx = math.clamp(gx, 0, gridW - 1);
            gy = math.clamp(gy, 0, gridH - 1);
            gz = math.clamp(gz, 0, gridD - 1);
            grid[gz * gridW * gridH + gy * gridW + gx] = index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsTooClose(int* grid, int gridW, int gridH, int gridD, float cellSize, float3 min, float3* points, int pointCount, float3 candidate, float minDist)
        {
            int gx = (int)((candidate.x - min.x) / cellSize);
            int gy = (int)((candidate.y - min.y) / cellSize);
            int gz = (int)((candidate.z - min.z) / cellSize);
            float minDistSq = minDist * minDist;

            for (int dz = -2; dz <= 2; dz++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    for (int dx = -2; dx <= 2; dx++)
                    {
                        int nx = gx + dx;
                        int ny = gy + dy;
                        int nz = gz + dz;
                        if (nx < 0 || nx >= gridW || ny < 0 || ny >= gridH || nz < 0 || nz >= gridD)
                        {
                            continue;
                        }

                        int idx = grid[nz * gridW * gridH + ny * gridW + nx];
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
