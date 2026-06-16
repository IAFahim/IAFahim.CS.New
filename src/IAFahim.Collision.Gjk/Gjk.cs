namespace IAFahim.Collision.Gjk
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class Gjk
    {
        private const float Epsilon = 1e-6f;
        private const int MaxIterations = 32;

        public delegate float3 SupportFunction(float3 direction);

        public static bool Intersect(SupportFunction supportA, SupportFunction supportB)
        {
            float3* simplexA = stackalloc float3[4];
            int count;
            return Intersect(supportA, supportB, simplexA, out count);
        }

        public static bool Intersect(SupportFunction supportA, SupportFunction supportB, float3* outSimplex, out int outCount)
        {
            outCount = 0;
            float3 dir = new float3(1.0f, 0.0f, 0.0f);
            float3* simplexA = stackalloc float3[4];
            int simplexCount = 0;

            float3 support = supportA(dir) - supportB(-dir);

            if (math.lengthsq(support) < Epsilon)
            {
                outSimplex[0] = support;
                outCount = 1;
                return true;
            }

            simplexA[0] = support;
            simplexCount = 1;

            dir = -support;

            for (int iter = 0; iter < MaxIterations; iter++)
            {
                support = supportA(dir) - supportB(-dir);

                if (math.dot(support, dir) < Epsilon)
                {
                    return false;
                }

                simplexA[simplexCount] = support;
                simplexCount++;

                if (DoSimplex(simplexA, ref simplexCount, ref dir))
                {
                    for (int i = 0; i < simplexCount; i++)
                    {
                        outSimplex[i] = simplexA[i];
                    }
                    outCount = simplexCount;
                    return true;
                }
            }

            return false;
        }

        public static float Distance(SupportFunction supportA, SupportFunction supportB)
        {
            float3 dir = new float3(1.0f, 0.0f, 0.0f);

            float3 sA = supportA(dir);
            float3 sB = supportB(-dir);
            float3 w0 = sA - sB;

            if (math.lengthsq(w0) < Epsilon)
            {
                return 0.0f;
            }

            float3* simplex = stackalloc float3[4];
            simplex[0] = w0;
            int count = 1;

            // closest is the current closest point on the simplex to the origin;
            // dir points from that closest point toward the origin.
            float3 closest = w0;
            float closestDistSq = math.lengthsq(w0);
            dir = -w0;

            for (int iter = 0; iter < MaxIterations; iter++)
            {
                sA = supportA(dir);
                sB = supportB(-dir);
                float3 w = sA - sB;

                // Progress test: the support point w lies on the supporting plane
                // of the CSO in direction dir. If it does not extend meaningfully
                // beyond the current closest point along dir, the closest feature
                // is optimal and the search has converged. Using the unnormalized
                // direction, the lack of progress is measured by comparing
                // dot(closest, closest) with dot(w, closest).
                float progress = closestDistSq - math.dot(w, closest);
                if (progress <= Epsilon * closestDistSq + Epsilon)
                {
                    break;
                }

                // Reject a support point already present in the simplex to avoid
                // a degenerate (zero-volume) addition and a stalled search.
                bool duplicate = false;
                for (int i = 0; i < count; i++)
                {
                    if (math.lengthsq(simplex[i] - w) < Epsilon)
                    {
                        duplicate = true;
                        break;
                    }
                }
                if (duplicate)
                {
                    break;
                }

                if (count < 4)
                {
                    simplex[count] = w;
                    count++;
                }

                closest = ComputeClosestPointToOrigin(simplex, ref count);
                closestDistSq = math.lengthsq(closest);

                if (closestDistSq < Epsilon)
                {
                    return 0.0f;
                }

                dir = -closest;
            }

            return math.sqrt(closestDistSq);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DoSimplex(float3* simplex, ref int count, ref float3 dir)
        {
            bool intersected = false;
            if (count == 2)
            {
                intersected = LineCase(simplex, ref count, ref dir);
            }
            else if (count == 3)
            {
                intersected = TriangleCase(simplex, ref count, ref dir);
            }
            else if (count == 4)
            {
                intersected = TetrahedronCase(simplex, ref count, ref dir);
            }

            if (intersected)
            {
                return true;
            }

            if (math.lengthsq(dir) < Epsilon)
            {
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool LineCase(float3* s, ref int count, ref float3 dir)
        {
            float3 b = s[0], a = s[1];
            float3 ab = b - a;
            float3 ao = -a;

            if (math.dot(ab, ao) > 0.0f)
            {
                dir = math.cross(math.cross(ab, ao), ab);
            }
            else
            {
                s[0] = a;
                count = 1;
                dir = ao;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TriangleCase(float3* s, ref int count, ref float3 dir)
        {
            float3 c = s[0], b = s[1], a = s[2];
            float3 ab = b - a;
            float3 ac = c - a;
            float3 ao = -a;
            float3 abc = math.cross(ab, ac);

            if (math.dot(math.cross(abc, ac), ao) > 0.0f)
            {
                if (math.dot(ac, ao) > 0.0f)
                {
                    s[0] = c;
                    s[1] = a;
                    count = 2;
                    dir = math.cross(math.cross(ac, ao), ac);
                }
                else
                {
                    s[0] = b;
                    s[1] = a;
                    count = 2;
                    return LineCase(s, ref count, ref dir);
                }
            }
            else
            {
                if (math.dot(math.cross(ab, abc), ao) > 0.0f)
                {
                    s[0] = b;
                    s[1] = a;
                    count = 2;
                    return LineCase(s, ref count, ref dir);
                }
                else
                {
                    if (math.dot(abc, ao) > 0.0f)
                    {
                        dir = abc;
                    }
                    else
                    {
                        float3 temp = s[0];
                        s[0] = s[1];
                        s[1] = temp;
                        dir = -abc;
                    }
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TetrahedronCase(float3* s, ref int count, ref float3 dir)
        {
            float3 d = s[0], c = s[1], b = s[2], a = s[3];
            float3 ab = b - a;
            float3 ac = c - a;
            float3 ad = d - a;
            float3 ao = -a;

            float3 abc = math.cross(ab, ac);
            float3 acd = math.cross(ac, ad);
            float3 adb = math.cross(ad, ab);

            if (math.dot(abc, ao) > 0.0f)
            {
                s[0] = c;
                s[1] = b;
                s[2] = a;
                count = 3;
                return TriangleCase(s, ref count, ref dir);
            }

            if (math.dot(acd, ao) > 0.0f)
            {
                s[0] = d;
                s[1] = c;
                s[2] = a;
                count = 3;
                return TriangleCase(s, ref count, ref dir);
            }

            if (math.dot(adb, ao) > 0.0f)
            {
                s[0] = b;
                s[1] = d;
                s[2] = a;
                count = 3;
                return TriangleCase(s, ref count, ref dir);
            }

            return true;
        }

        // Returns the closest point on the current simplex to the origin and
        // reduces the simplex (in place, reordering vertices) to the minimal
        // sub-feature (vertex/edge/triangle) that contains the closest point.
        // For count == 4, if the origin is enclosed by the tetrahedron, the
        // closest point is the origin itself (returned as float3.zero).
        private static float3 ComputeClosestPointToOrigin(float3* simplex, ref int count)
        {
            if (count == 1)
            {
                return simplex[0];
            }

            if (count == 2)
            {
                return ClosestPointOnSegment(simplex, ref count);
            }

            if (count == 3)
            {
                return ClosestPointOnTriangle(simplex, ref count);
            }

            return ClosestPointOnTetrahedron(simplex, ref count);
        }

        // Closest point on segment [s0, s1] to the origin, reducing the simplex
        // to a single vertex when the closest feature is an endpoint.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3 ClosestPointOnSegment(float3* simplex, ref int count)
        {
            float3 a = simplex[0];
            float3 b = simplex[1];
            float3 ab = b - a;

            float denom = math.dot(ab, ab);
            if (denom < Epsilon)
            {
                count = 1;
                return a;
            }

            float t = math.dot(-a, ab) / denom;

            if (t <= 0.0f)
            {
                count = 1;
                return a;
            }

            if (t >= 1.0f)
            {
                simplex[0] = b;
                count = 1;
                return b;
            }

            return a + t * ab;
        }

        // Closest point on triangle [s0, s1, s2] to the origin using Voronoi
        // region tests (Ericson, Real-Time Collision Detection). The simplex is
        // reduced to the supporting vertex/edge/face.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3 ClosestPointOnTriangle(float3* simplex, ref int count)
        {
            float3 a = simplex[0];
            float3 b = simplex[1];
            float3 c = simplex[2];

            float3 ab = b - a;
            float3 ac = c - a;
            float3 ap = -a;

            float d1 = math.dot(ab, ap);
            float d2 = math.dot(ac, ap);
            if (d1 <= 0.0f && d2 <= 0.0f)
            {
                count = 1;
                return a;
            }

            float3 bp = -b;
            float d3 = math.dot(ab, bp);
            float d4 = math.dot(ac, bp);
            if (d3 >= 0.0f && d4 <= d3)
            {
                simplex[0] = b;
                count = 1;
                return b;
            }

            float vc = d1 * d4 - d3 * d2;
            if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
            {
                float t = d1 / (d1 - d3);
                // Edge AB.
                count = 2;
                return a + t * ab;
            }

            float3 cp = -c;
            float d5 = math.dot(ab, cp);
            float d6 = math.dot(ac, cp);
            if (d6 >= 0.0f && d5 <= d6)
            {
                simplex[0] = c;
                count = 1;
                return c;
            }

            float vb = d5 * d2 - d1 * d6;
            if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
            {
                float t = d2 / (d2 - d6);
                // Edge AC.
                simplex[1] = c;
                count = 2;
                return a + t * ac;
            }

            float va = d3 * d6 - d5 * d4;
            if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f)
            {
                float t = (d4 - d3) / ((d4 - d3) + (d5 - d6));
                // Edge BC.
                simplex[0] = b;
                simplex[1] = c;
                count = 2;
                return b + t * (c - b);
            }

            // Interior of the face: closest point is the projection of the
            // origin onto the triangle plane.
            float denom = 1.0f / (va + vb + vc);
            float v = vb * denom;
            float w = vc * denom;
            count = 3;
            return a + ab * v + ac * w;
        }

        // Closest point on tetrahedron [s0, s1, s2, s3] to the origin. Tests the
        // origin against each face's outward half-space; if outside, recurses on
        // the closest face. If inside all faces the origin is enclosed and the
        // closest point is the origin itself.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3 ClosestPointOnTetrahedron(float3* simplex, ref int count)
        {
            float3 a = simplex[0];
            float3 b = simplex[1];
            float3 c = simplex[2];
            float3 d = simplex[3];

            float bestDistSq = float.MaxValue;
            int bestI0 = -1, bestI1 = -1, bestI2 = -1;

            // Signed volume (times six) of the tetrahedron. When this is
            // near zero the tetrahedron is degenerate (coplanar/sliver) and
            // the per-face outside tests are unreliable: a flat simplex that
            // does not actually contain the origin would otherwise report no
            // face as outside and be falsely classified as enclosing it.
            float signedVolumeX6 = math.dot(b - a, math.cross(c - a, d - a));
            bool degenerate = math.abs(signedVolumeX6) < Epsilon;

            // Face ABC, outward direction away from D.
            if (degenerate || PointOutsideFace(a, b, c, d))
            {
                EvaluateFace(a, b, c, 0, 1, 2,
                    ref bestDistSq, ref bestI0, ref bestI1, ref bestI2);
            }

            // Face ACD, outward direction away from B.
            if (degenerate || PointOutsideFace(a, c, d, b))
            {
                EvaluateFace(a, c, d, 0, 2, 3,
                    ref bestDistSq, ref bestI0, ref bestI1, ref bestI2);
            }

            // Face ADB, outward direction away from C.
            if (degenerate || PointOutsideFace(a, d, b, c))
            {
                EvaluateFace(a, d, b, 0, 3, 1,
                    ref bestDistSq, ref bestI0, ref bestI1, ref bestI2);
            }

            // Face BDC, outward direction away from A.
            if (degenerate || PointOutsideFace(b, d, c, a))
            {
                EvaluateFace(b, d, c, 1, 3, 2,
                    ref bestDistSq, ref bestI0, ref bestI1, ref bestI2);
            }

            if (bestI0 < 0)
            {
                // Origin is inside a non-degenerate tetrahedron (every face
                // reported the origin on its inner side). The closest point
                // is the origin itself.
                count = 4;
                return float3.zero;
            }

            // Reduce the simplex to the winning face's vertices, then refine on
            // that triangle to capture edge/vertex sub-features.
            float3 v0 = simplex[bestI0];
            float3 v1 = simplex[bestI1];
            float3 v2 = simplex[bestI2];
            simplex[0] = v0;
            simplex[1] = v1;
            simplex[2] = v2;
            count = 3;
            return ClosestPointOnTriangle(simplex, ref count);
        }

        // Evaluates the closest point on a single tetrahedron face (triangle) to
        // the origin and updates the running best. Uses a temporary local
        // simplex so the input array is not disturbed during evaluation.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EvaluateFace(
            float3 a, float3 b, float3 c,
            int i0, int i1, int i2,
            ref float bestDistSq,
            ref int bestI0, ref int bestI1, ref int bestI2)
        {
            float3* tri = stackalloc float3[3];
            tri[0] = a;
            tri[1] = b;
            tri[2] = c;
            int triCount = 3;
            float3 p = ClosestPointOnTriangle(tri, ref triCount);
            float distSq = math.dot(p, p);
            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                bestI0 = i0;
                bestI1 = i1;
                bestI2 = i2;
            }
        }

        // True if the origin lies on the outward side of the plane through
        // (a, b, c), i.e. on the opposite side from the reference point d.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool PointOutsideFace(float3 a, float3 b, float3 c, float3 d)
        {
            float3 normal = math.cross(b - a, c - a);
            float signOrigin = math.dot(normal, -a);
            float signRef = math.dot(normal, d - a);
            // Origin is outside when it sits strictly on the opposite side of
            // the plane from the reference vertex d.
            return signOrigin * signRef < 0.0f;
        }
    }
}
