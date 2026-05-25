namespace IAFahim.Geometry.Triangulation
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class EarClipping
    {
        private const float Zero = 0.0f;
        private const float One = 1.0f;
        private const float Epsilon = 1e-6f;

        public static void Triangulate(
            float2* vertices, 
            int outerCount, 
            int* holeStarts, 
            int* holeCounts, 
            int holeCount, 
            int* outTriangles, 
            out int outTriangleCount)
        {
            outTriangleCount = 0;
            if (outerCount < 3 || holeCount < 0)
            {
                return;
            }

            int totalVertices = outerCount;
            for (int h = 0; h < holeCount; h++)
            {
                if (holeCounts[h] < 0 || holeStarts[h] < 0)
                {
                    return;
                }
                totalVertices += holeCounts[h];
            }

            int maxMergedSize = totalVertices + 2 * holeCount;
            int* mergedIndices = stackalloc int[maxMergedSize];
            int mergedCount = outerCount;

            for (int i = 0; i < outerCount; i++)
            {
                mergedIndices[i] = i;
            }

            for (int h = 0; h < holeCount; h++)
            {
                int holeStart = holeStarts[h];
                int holeSize = holeCounts[h];
                MergeHole(vertices, mergedIndices, &mergedCount, holeStart, holeSize);
            }

            TriangulatePolygon(vertices, mergedIndices, mergedCount, outTriangles, out outTriangleCount);
        }

        private static void MergeHole(float2* vertices, int* mergedIndices, int* mergedCount, int holeStart, int holeSize)
        {
            int bestOuter = -1;
            int bestHole = -1;
            float minDistSq = float.MaxValue;

            for (int i = 0; i < *mergedCount; i++)
            {
                int oIdx = mergedIndices[i];
                float2 oPt = vertices[oIdx];

                for (int j = 0; j < holeSize; j++)
                {
                    int hIdx = holeStart + j;
                    float2 hPt = vertices[hIdx];

                    float distSq = math.distancesq(oPt, hPt);
                    if (distSq < minDistSq)
                    {
                        if (IsValidBridge(vertices, mergedIndices, *mergedCount, holeStart, holeSize, oPt, hPt))
                        {
                            minDistSq = distSq;
                            bestOuter = i;
                            bestHole = j;
                        }
                    }
                }
            }

            if (bestOuter != -1 && bestHole != -1)
            {
                int outerIdx = mergedIndices[bestOuter];
                int holeIdx = holeStart + bestHole;

                int insertPos = bestOuter + 1;
                int shiftCount = *mergedCount - insertPos;

                for (int k = *mergedCount - 1; k >= insertPos; k--)
                {
                    mergedIndices[k + holeSize + 2] = mergedIndices[k];
                }

                mergedIndices[insertPos] = outerIdx;
                mergedIndices[insertPos + 1] = holeIdx;

                for (int k = 0; k < holeSize; k++)
                {
                    int idx = holeStart + (bestHole + k) % holeSize;
                    mergedIndices[insertPos + 2 + k] = idx;
                }

                *mergedCount += holeSize + 2;
            }
        }

        private static bool IsValidBridge(
            float2* vertices, 
            int* mergedIndices, 
            int mergedCount, 
            int holeStart, 
            int holeSize, 
            float2 oPt, 
            float2 hPt)
        {
            for (int i = 0; i < mergedCount; i++)
            {
                int next = (i + 1) % mergedCount;
                float2 a = vertices[mergedIndices[i]];
                float2 b = vertices[mergedIndices[next]];

                if (LineSegmentsIntersect(oPt, hPt, a, b))
                {
                    return false;
                }
            }

            for (int j = 0; j < holeSize; j++)
            {
                int next = (j + 1) % holeSize;
                float2 a = vertices[holeStart + j];
                float2 b = vertices[holeStart + next];

                if (LineSegmentsIntersect(oPt, hPt, a, b))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool LineSegmentsIntersect(float2 p1, float2 q1, float2 p2, float2 q2)
        {
            float d1 = Direction(p2, q2, p1);
            float d2 = Direction(p2, q2, q1);
            float d3 = Direction(p1, q1, p2);
            float d4 = Direction(p1, q1, q2);

            if (((d1 > Zero && d2 < Zero) || (d1 < Zero && d2 > Zero)) &&
                ((d3 > Zero && d4 < Zero) || (d3 < Zero && d4 > Zero)))
            {
                return true;
            }

            if (d1 == Zero && OnSegment(p2, q2, p1)) return true;
            if (d2 == Zero && OnSegment(p2, q2, q1)) return true;
            if (d3 == Zero && OnSegment(p1, q1, p2)) return true;
            if (d4 == Zero && OnSegment(p1, q1, q2)) return true;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Direction(float2 pi, float2 pj, float2 pk)
        {
            return (pj.x - pi.x) * (pk.y - pi.y) - (pj.y - pi.y) * (pk.x - pi.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool OnSegment(float2 pi, float2 pj, float2 pk)
        {
            return pk.x >= math.min(pi.x, pj.x) && pk.x <= math.max(pi.x, pj.x) &&
                   pk.y >= math.min(pi.y, pj.y) && pk.y <= math.max(pi.y, pj.y);
        }

        private static void TriangulatePolygon(
            float2* vertices, 
            int* mergedIndices, 
            int mergedCount, 
            int* outTriangles, 
            out int outTriangleCount)
        {
            outTriangleCount = 0;
            if (mergedCount < 3)
            {
                return;
            }

            int* activeIndices = stackalloc int[mergedCount];
            for (int i = 0; i < mergedCount; i++)
            {
                activeIndices[i] = mergedIndices[i];
            }

            int activeCount = mergedCount;
            bool isCcw = IsCounterClockwise(vertices, activeIndices, activeCount);

            int iterations = 0;
            int maxIterations = activeCount * activeCount;

            while (activeCount > 3 && iterations < maxIterations)
            {
                iterations++;
                bool earFound = false;

                for (int i = 0; i < activeCount; i++)
                {
                    int prev = (i - 1 + activeCount) % activeCount;
                    int next = (i + 1) % activeCount;

                    int prevIdx = activeIndices[prev];
                    int currIdx = activeIndices[i];
                    int nextIdx = activeIndices[next];

                    if (IsEar(vertices, activeIndices, activeCount, prevIdx, currIdx, nextIdx, isCcw))
                    {
                        outTriangles[outTriangleCount++] = prevIdx;
                        outTriangles[outTriangleCount++] = currIdx;
                        outTriangles[outTriangleCount++] = nextIdx;

                        for (int k = i; k < activeCount - 1; k++)
                        {
                            activeIndices[k] = activeIndices[k + 1];
                        }
                        activeCount--;
                        earFound = true;
                        break;
                    }
                }

                if (!earFound)
                {
                    break;
                }
            }

            if (activeCount == 3)
            {
                outTriangles[outTriangleCount++] = activeIndices[0];
                outTriangles[outTriangleCount++] = activeIndices[1];
                outTriangles[outTriangleCount++] = activeIndices[2];
            }
        }

        private static bool IsEar(
            float2* vertices, 
            int* activeIndices, 
            int activeCount, 
            int prevIdx, 
            int currIdx, 
            int nextIdx, 
            bool isCcw)
        {
            float2 a = vertices[prevIdx];
            float2 b = vertices[currIdx];
            float2 c = vertices[nextIdx];

            float dir = Direction(a, b, c);
            if (isCcw && dir <= Zero) return false;
            if (!isCcw && dir >= Zero) return false;

            for (int i = 0; i < activeCount; i++)
            {
                int idx = activeIndices[i];
                if (idx == prevIdx || idx == currIdx || idx == nextIdx)
                {
                    continue;
                }

                float2 p = vertices[idx];
                if (PointInTriangle(p, a, b, c))
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool PointInTriangle(float2 p, float2 a, float2 b, float2 c)
        {
            float d1 = Direction(p, a, b);
            float d2 = Direction(p, b, c);
            float d3 = Direction(p, c, a);

            bool hasNeg = (d1 < -Epsilon) || (d2 < -Epsilon) || (d3 < -Epsilon);
            bool hasPos = (d1 > Epsilon) || (d2 > Epsilon) || (d3 > Epsilon);

            return !(hasNeg && hasPos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsCounterClockwise(float2* vertices, int* indices, int count)
        {
            float area = Zero;
            for (int i = 0; i < count; i++)
            {
                int next = (i + 1) % count;
                float2 currPt = vertices[indices[i]];
                float2 nextPt = vertices[indices[next]];
                area += (nextPt.x - currPt.x) * (nextPt.y + currPt.y);
            }
            return area < Zero;
        }
    }
}
