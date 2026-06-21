namespace IAFahim.String
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SimpleRand
    {
        private uint state;

        public SimpleRand(uint seed)
        {
            state = seed == 0 ? 123456789 : seed;
        }

        public uint Next()
        {
            uint x = state;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            state = x;
            return x;
        }

        public long NextLong(long min, long max)
        {
            uint val = Next();
            long diff = max - min + 1;
            if (diff <= 0)
            {
                return min;
            }
            return min + (val % diff);
        }
    }

    public static unsafe class Probabilistic
    {
        public static bool FreivaldsMatrixVerify(long* a, long* b, long* c, int n, int iterations, long mod, long* r, long* br, long* abr, long* cr)
        {
            SimpleRand rand = new SimpleRand(42);
            for (int iter = 0; iter < iterations; iter++)
            {
                for (int i = 0; i < n; i++)
                {
                    r[i] = rand.Next() % 2;
                }
                for (int i = 0; i < n; i++)
                {
                    long sum = 0;
                    for (int j = 0; j < n; j++)
                    {
                        sum = (sum + b[(long)i * n + j] * r[j]) % mod;
                    }
                    br[i] = sum;
                }
                for (int i = 0; i < n; i++)
                {
                    long sum = 0;
                    for (int j = 0; j < n; j++)
                    {
                        sum = (sum + a[(long)i * n + j] * br[j]) % mod;
                    }
                    abr[i] = sum;
                }
                for (int i = 0; i < n; i++)
                {
                    long sum = 0;
                    for (int j = 0; j < n; j++)
                    {
                        sum = (sum + c[(long)i * n + j] * r[j]) % mod;
                    }
                    cr[i] = sum;
                }
                for (int i = 0; i < n; i++)
                {
                    if (abr[i] != cr[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool SchwartzZippelTest(delegate* managed<long*, long, long> eval, int numVariables, long degree, int iterations, long mod, long* points)
        {
            SimpleRand rand = new SimpleRand(999);
            for (int iter = 0; iter < iterations; iter++)
            {
                for (int i = 0; i < numVariables; i++)
                {
                    points[i] = rand.NextLong(0, mod - 1);
                }
                long val = eval(points, numVariables);
                if (val != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static int RabinKarpLasVegas(byte* text, int textLen, byte* pattern, int patternLen)
        {
            if (patternLen > textLen)
            {
                return -1;
            }
            if (patternLen == 0)
            {
                return 0;
            }
            const long mod = 1000000007L;
            const long p = 313L;
            long patHash = 0;
            long textHash = 0;
            long pPow = 1;
            for (int i = 0; i < patternLen; i++)
            {
                patHash = (patHash * p + pattern[i]) % mod;
                textHash = (textHash * p + text[i]) % mod;
                if (i > 0)
                {
                    pPow = (pPow * p) % mod;
                }
            }
            for (int i = 0; i <= textLen - patternLen; i++)
            {
                if (textHash == patHash)
                {
                    bool match = true;
                    for (int j = 0; j < patternLen; j++)
                    {
                        if (text[i + j] != pattern[j])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        return i;
                    }
                }
                if (i < textLen - patternLen)
                {
                    textHash = (textHash - text[i] * pPow % mod + mod) % mod;
                    textHash = (textHash * p + text[i + patternLen]) % mod;
                }
            }
            return -1;
        }

        public static bool RandomizedMstVerify(int numVertices, int numEdges, int* u, int* v, long* weight, bool* inMst, int* parent, int* depth, int* up, long* maxEdge)
        {
            int treeEdgeCount = 0;
            for (int i = 0; i < numEdges; i++)
            {
                if (inMst[i])
                {
                    treeEdgeCount++;
                }
            }
            if (treeEdgeCount != numVertices - 1)
            {
                return false;
            }
            int* head = stackalloc int[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                head[i] = -1;
            }
            int* to = stackalloc int[2 * numVertices];
            long* edgeWeight = stackalloc long[2 * numVertices];
            int* next = stackalloc int[2 * numVertices];
            int edgeIdx = 0;
            for (int i = 0; i < numEdges; i++)
            {
                if (inMst[i])
                {
                    int x = u[i];
                    int y = v[i];
                    long w = weight[i];
                    to[edgeIdx] = y;
                    edgeWeight[edgeIdx] = w;
                    next[edgeIdx] = head[x];
                    head[x] = edgeIdx++;
                    to[edgeIdx] = x;
                    edgeWeight[edgeIdx] = w;
                    next[edgeIdx] = head[y];
                    head[y] = edgeIdx++;
                }
            }
            int logV = 1;
            while ((1 << logV) <= numVertices)
            {
                logV++;
            }
            for (int i = 0; i < numVertices; i++)
            {
                parent[i] = -1;
                depth[i] = 0;
                for (int j = 0; j < logV; j++)
                {
                    up[(long)i * logV + j] = -1;
                    maxEdge[(long)i * logV + j] = 0;
                }
            }
            int* stack = stackalloc int[numVertices];
            int stackPtr = 0;
            stack[stackPtr++] = 0;
            parent[0] = 0;
            depth[0] = 0;
            while (stackPtr > 0)
            {
                int curr = stack[--stackPtr];
                for (int e = head[curr]; e != -1; e = next[e])
                {
                    int nxtNode = to[e];
                    if (nxtNode != parent[curr])
                    {
                        parent[nxtNode] = curr;
                        depth[nxtNode] = depth[curr] + 1;
                        up[(long)nxtNode * logV + 0] = curr;
                        maxEdge[(long)nxtNode * logV + 0] = edgeWeight[e];
                        stack[stackPtr++] = nxtNode;
                    }
                }
            }
            for (int j = 1; j < logV; j++)
            {
                for (int i = 0; i < numVertices; i++)
                {
                    int anc = up[(long)i * logV + j - 1];
                    if (anc != -1)
                    {
                        up[(long)i * logV + j] = up[(long)anc * logV + j - 1];
                        long val1 = maxEdge[(long)i * logV + j - 1];
                        long val2 = maxEdge[(long)anc * logV + j - 1];
                        maxEdge[(long)i * logV + j] = val1 > val2 ? val1 : val2;
                    }
                }
            }
            for (int i = 0; i < numEdges; i++)
            {
                if (!inMst[i])
                {
                    if (!VerifyNonMstEdge(u[i], v[i], weight[i], logV, depth, up, maxEdge))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool VerifyNonMstEdge(int x, int y, long w, int logV, int* depth, int* up, long* maxEdge)
        {
            long pathMax = 0;
            if (depth[x] < depth[y])
            {
                int temp = x;
                x = y;
                y = temp;
            }
            for (int j = logV - 1; j >= 0; j--)
            {
                if (depth[x] - (1 << j) >= depth[y])
                {
                    long val = maxEdge[(long)x * logV + j];
                    if (val > pathMax)
                    {
                        pathMax = val;
                    }
                    x = up[(long)x * logV + j];
                }
            }
            if (x != y)
            {
                for (int j = logV - 1; j >= 0; j--)
                {
                    if (up[(long)x * logV + j] != up[(long)y * logV + j])
                    {
                        long val1 = maxEdge[(long)x * logV + j];
                        long val2 = maxEdge[(long)y * logV + j];
                        if (val1 > pathMax)
                        {
                            pathMax = val1;
                        }
                        if (val2 > pathMax)
                        {
                            pathMax = val2;
                        }
                        x = up[(long)x * logV + j];
                        y = up[(long)y * logV + j];
                    }
                }
                long finalVal1 = maxEdge[(long)x * logV + 0];
                long finalVal2 = maxEdge[(long)y * logV + 0];
                if (finalVal1 > pathMax)
                {
                    pathMax = finalVal1;
                }
                if (finalVal2 > pathMax)
                {
                    pathMax = finalVal2;
                }
            }
            return pathMax <= w;
        }
    }
}
