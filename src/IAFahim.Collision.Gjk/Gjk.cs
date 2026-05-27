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

            dir = -w0;

            float3* simplex = stackalloc float3[4];
            simplex[0] = w0;
            int count = 1;

            for (int iter = 0; iter < MaxIterations; iter++)
            {
                sA = supportA(dir);
                sB = supportB(-dir);
                float3 w = sA - sB;

                float dw = math.dot(w, dir);

                if (dw > -Epsilon && dw < Epsilon)
                {
                    return 0.0f;
                }

                if (dw < 0.0f)
                {
                    break;
                }

                if (count < 4)
                {
                    simplex[count] = w;
                    count++;
                }

                dir = -ComputeClosestPointToOrigin(simplex, count);
            }

            return ComputeDistanceToOrigin(simplex, count);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ComputeDistanceToOrigin(float3* simplex, int count)
        {
            if (count == 1)
            {
                return math.length(simplex[0]);
            }

            float3 closest = ComputeClosestPointToOrigin(simplex, count);
            return math.length(closest);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3 ComputeClosestPointToOrigin(float3* simplex, int count)
        {
            if (count == 1)
            {
                return simplex[0];
            }

            if (count == 2)
            {
                return ClosestPointOnSegment(simplex[0], simplex[1], float3.zero);
            }

            return float3.zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3 ClosestPointOnSegment(float3 a, float3 b, float3 p)
        {
            float3 ab = b - a;
            float t = math.dot(p - a, ab) / math.dot(ab, ab);
            t = math.clamp(t, 0.0f, 1.0f);
            return a + t * ab;
        }
    }
}
