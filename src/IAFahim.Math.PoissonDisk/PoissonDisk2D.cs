namespace IAFahim.Math.PoissonDisk
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class PoissonDisk2D
    {
        private const int MaxAttempts = 30;

        private const int EmptyCell = -1;

        private const int CellNeighborReach = 2;

        private const float Sqrt2 = 1.41421356f;

        private const float TwoPi = 6.28318530f;

        private const uint StateMultiplier = 747796405u;

        private const uint StateIncrement = 2891336453u;

        private const uint XorFactor = 277803737u;

        private const int HighWordShift = 28;

        private const int LowWordExtraShift = 4;

        private const int FinalShift = 22;

        private const float UintToUnit = 4294967296.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInsideBounds(float2 p, float2 min, float2 max)
            => p.x >= min.x && p.x <= max.x && p.y >= min.y && p.y <= max.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float2 SampleAnnulus(ref uint rng, float2 center, float minDistance)
        {
            float angle = NextFloat(ref rng) * TwoPi;
            float radius = minDistance + NextFloat(ref rng) * minDistance;
            return center + new float2(math.cos(angle), math.sin(angle)) * radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AcceptCandidate(float2* output, float2* active, int* grid, int gridW, int gridH, float cellSize, float2 min, ref int activeCount, ref int pointCount, float2 candidate)
        {
            output[pointCount] = candidate;
            active[activeCount++] = candidate;
            InsertGrid(grid, gridW, gridH, cellSize, min, candidate, pointCount);
            pointCount++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RemoveActiveAt(float2* active, ref int activeCount, int idx)
        {
            activeCount--;
            if (idx < activeCount) active[idx] = active[activeCount];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryPlaceAround(float2 center, float minDistance, float2 min, float2 max, int* grid, int gridW, int gridH, float cellSize, float2* output, float2* active, ref uint rng, ref int activeCount, ref int pointCount)
        {
            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                float2 candidate = SampleAnnulus(ref rng, center, minDistance);
                if (!IsInsideBounds(candidate, min, max)) continue;
                if (IsTooClose(grid, gridW, gridH, cellSize, min, output, pointCount, candidate, minDistance)) continue;
                AcceptCandidate(output, active, grid, gridW, gridH, cellSize, min, ref activeCount, ref pointCount, candidate);
                return true;
            }
            return false;
        }

        public static int Run(float2 min, float2 max, float minDistance, float2* output, int maxPoints, int seed)
        {
            float cellSize = minDistance / Sqrt2;
            int gridW = (int)math.ceil((max.x - min.x) / cellSize);
            int gridH = (int)math.ceil((max.y - min.y) / cellSize);
            if (gridW <= 0 || gridH <= 0) return 0;
            int gridSize = gridW * gridH;
            int* grid = stackalloc int[gridSize];
            for (int i = 0; i < gridSize; i++) grid[i] = EmptyCell;
            float2* active = stackalloc float2[maxPoints];
            int activeCount = 0;
            int pointCount = 0;
            uint rng = (uint)seed;
            float2 first = new float2(
                min.x + NextFloat(ref rng) * (max.x - min.x),
                min.y + NextFloat(ref rng) * (max.y - min.y));
            if (pointCount >= maxPoints) return 0;
            AcceptCandidate(output, active, grid, gridW, gridH, cellSize, min, ref activeCount, ref pointCount, first);
            while (activeCount > 0 && pointCount < maxPoints)
            {
                int activeIdx = (int)(NextFloat(ref rng) * activeCount);
                float2 center = active[activeIdx];
                if (!TryPlaceAround(center, minDistance, min, max, grid, gridW, gridH, cellSize, output, active, ref rng, ref activeCount, ref pointCount))
                    RemoveActiveAt(active, ref activeCount, activeIdx);
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
            for (int dy = -CellNeighborReach; dy <= CellNeighborReach; dy++)
            {
                for (int dx = -CellNeighborReach; dx <= CellNeighborReach; dx++)
                {
                    int nx = gx + dx;
                    int ny = gy + dy;
                    if (nx < 0 || nx >= gridW || ny < 0 || ny >= gridH) continue;
                    int idx = grid[ny * gridW + nx];
                    if (idx < 0) continue;
                    float distSq = math.lengthsq(candidate - points[idx]);
                    if (distSq < minDistSq) return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float NextFloat(ref uint state)
        {
            state = state * StateMultiplier + StateIncrement;
            uint result = ((state >> ((int)(state >> HighWordShift) + LowWordExtraShift)) ^ state) * XorFactor;
            result = (result >> FinalShift) ^ result;
            return (float)result / UintToUnit;
        }
    }
}
