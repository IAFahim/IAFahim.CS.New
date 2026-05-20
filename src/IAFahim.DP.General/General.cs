namespace IAFahim.DP.General
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LagrangeInterpolationDp
    {
        public static double Run(int n, long* y, double x)
        {
            double* fact = stackalloc double[n + 1];
            double* invFact = stackalloc double[n + 1];
            fact[0] = 1;
            for (int i = 1; i <= n; i++) fact[i] = fact[i - 1] * i;
            invFact[n] = 1.0 / fact[n];
            for (int i = n; i > 0; i--) invFact[i - 1] = invFact[i] * i;
            double result = 0;
            for (int i = 0; i <= n; i++)
            {
                double term = y[i];
                term *= invFact[i];
                term *= invFact[n - i];
                if (((n - i) & 1) != 0) term = -term;
                double numer = 1;
                for (int j1 = 0; j1 <= n; j1++)
                {
                    if (j1 != i) numer *= (x - j1);
                }
                term *= numer;
                result += term;
            }
            return result;
        }
    }

    public static unsafe class RerootingDp
    {
        public static void Run(int n, int root, int* head, int* to, int* next, long* dp, long* up, Func<long, long, long> merge)
        {
            Dfs1(root, -1);
            Dfs2(root, -1);

            void Dfs1(int u, int p)
            {
                dp[u] = 1;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (v == p) continue;
                    Dfs1(v, u);
                    dp[u] = merge(dp[u], dp[v] + 1);
                }
            }

            void Dfs2(int u, int p)
            {
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (v == p) continue;
                    up[v] = merge(up[u] + 1, dp[u]);
                    Dfs2(v, u);
                }
            }
        }
    }

    public static unsafe class DigitDp
    {
        public static long Run(string limit, int pos, bool tight, bool isNum, bool started, long* dp)
        {
            if (pos >= limit.Length)
            {
                return isNum && started ? 1 : 0;
            }
            long key = ((tight ? 1 : 0) << 20) | ((isNum ? 1 : 0) << 10) | (started ? 1 : 0);
            if (dp[pos] > 0 && !tight) return dp[pos];
            long result = 0;
            int maxDigit = tight ? limit[pos] - '0' : 9;
            for (int d = 0; d <= maxDigit; d++)
            {
                bool nextTight = tight && (d == maxDigit);
                bool nextStarted = started || (d != 0);
                if (!started && d == 0 && !isNum)
                {
                    result += Run(limit, pos + 1, nextTight, false, false, dp);
                }
                else
                {
                    result += Run(limit, pos + 1, nextTight, true, nextStarted, dp);
                }
            }
            if (!tight) dp[pos] = result;
            return result;
        }
    }

    public static unsafe class ProfileDp
    {
        public static long Run(int m, int n, int* a, long* dp, int* tmp)
        {
            for (int i = 0; i < (1 << m); i++) dp[i] = long.MinValue;
            dp[0] = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    int bit = 1 << j;
                    for (int mask = 0; mask < (1 << m); mask++)
                    {
                        tmp[mask] = dp[mask];
                    }
                    for (int mask = 0; mask < (1 << m); mask++)
                    {
                        if ((mask & bit) != 0)
                        {
                            int nmask = mask ^ bit;
                            tmp[nmask] = Math.Max(tmp[nmask], dp[mask] + a[i * m + j]);
                        }
                    }
                    for (int mask = 0; mask < (1 << m); mask++)
                    {
                        dp[mask] = tmp[mask];
                    }
                }
            }
            return dp[0];
        }
    }

    public static unsafe class BrokenProfileDp
    {
        public static long Run(int m, int n, int* a, long* dp, long* tmp, int* state)
        {
            for (int i = 0; i < (1 << m); i++) dp[i] = long.MinValue;
            dp[0] = 0;
            int pos = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    int bit = 1 << j;
                    for (int mask = 0; mask < (1 << m); mask++)
                    {
                        tmp[mask] = dp[mask];
                    }
                    for (int mask = 0; mask < (1 << m); mask++)
                    {
                        if ((mask & bit) != 0)
                        {
                            int nmask = mask ^ bit;
                            long val = a[i * m + j];
                            tmp[nmask] = Math.Max(tmp[nmask], dp[mask] + val);
                        }
                    }
                    for (int mask = 0; mask < (1 << m); mask++)
                    {
                        dp[mask] = tmp[mask];
                    }
                }
            }
            return dp[0];
        }
    }

    public static unsafe class TreeKnapsack
    {
        public static void Run(int u, int p, int size, int* head, int* to, int* next, int* w, long* v, long* dp, long* tmp)
        {
            size = 1;
            dp[u * 1000] = 0;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v2 = to[e];
                if (v2 == p) continue;
                int subSize = 0;
                Run(v2, u, subSize, head, to, next, w, v, dp, tmp);
                for (int i = size - 1; i >= 0; i--)
                {
                    long val = dp[u * 1000 + i];
                    long val2 = dp[v2 * 1000 + subSize] + v[v2];
                    int w2 = w[v2];
                    if (i + w2 < 1000) dp[u * 1000 + i + w2] = Math.Max(dp[u * 1000 + i + w2], val2);
                }
                size += subSize;
            }
        }
    }

    public static unsafe class SubsetDp
    {
        public static void Run(int n, long* f, long* g)
        {
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < (1 << n); mask++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        g[mask] += f[mask ^ (1 << i)];
                    }
                }
            }
        }

        public static void RunInt(int n, int* f, int* g)
        {
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < (1 << n); mask++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        g[mask] += f[mask ^ (1 << i)];
                    }
                }
            }
        }
    }

    public static unsafe class SosDp
    {
        public static void Run(int n, int* f, int* dp)
        {
            for (int i = 0; i < (1 << n); i++) dp[i] = f[i];
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < (1 << n); mask++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        dp[mask] += dp[mask ^ (1 << i)];
                    }
                }
            }
        }

        public static long RunLong(int n, long* f, long* dp)
        {
            for (int i = 0; i < (1 << n); i++) dp[i] = f[i];
            for (int i = 0; i < n; i++)
            {
                for (int mask = 0; mask < (1 << n); mask++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        dp[mask] += dp[mask ^ (1 << i)];
                    }
                }
            }
            return dp[0];
        }
    }

    public static unsafe class IntervalDp
    {
        public static long Run(int n, int* a, long* dp, long* tmp, long cost(int l, int r))
        {
            for (int i = 0; i < n; i++) dp[i] = 0;
            for (int len = 2; len <= n; len++)
            {
                for (int i = 0; i + len <= n; i++)
                {
                    int j = i + len - 1;
                    long best = long.MaxValue;
                    for (int k = i; k < j; k++)
                    {
                        long val = dp[i * n + k] + dp[(k + 1) * n + j] + cost(i, j);
                        if (val < best) best = val;
                    }
                    dp[i * n + j] = best;
                }
            }
            return dp[0];
        }
    }

    public static unsafe class ProbabilityDp
    {
        public static double Run(int n, double* p, double* dp)
        {
            dp[0] = 1.0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j <= n; j++)
                {
                    dp[j] += dp[i] * p[i];
                }
            }
            return dp[n];
        }
    }

    public static unsafe class ExpectationDp
    {
        public static double Run(int n, double* p, double* dp)
        {
            dp[0] = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j <= n; j++)
                {
                    dp[j] += dp[i] + 1.0;
                    dp[j] *= p[i];
                }
            }
            return dp[n];
        }
    }

    public static unsafe class MinPlusConvolution
    {
        public static void Run(int n, int m, long* a, long* b, long* c, long INF)
        {
            for (int i = 0; i < n + m; i++) c[i] = INF;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    long val = a[i] + b[j];
                    if (val < c[i + j]) c[i + j] = val;
                }
            }
        }
    }

    public static unsafe class MaxPlusConvolution
    {
        public static void Run(int n, int m, long* a, long* b, long* c)
        {
            for (int i = 0; i < n + m; i++) c[i] = long.MinValue;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    long val = a[i] + b[j];
                    if (val > c[i + j]) c[i + j] = val;
                }
            }
        }
    }

    public static unsafe class QuadrangleInequalityDp
    {
        public static long Run(int n, int m, long* dp, long* tmp, long cost(int i, int j), int* opt)
        {
            for (int i = 0; i < n; i++)
            {
                dp[i] = cost(i, i);
                opt[i] = i;
            }
            for (int len = 2; len <= m; len++)
            {
                for (int i = 0; i + len <= n; i++)
                {
                    int j = i + len - 1;
                    long best = long.MaxValue;
                    int bestK = -1;
                    int start = opt[i];
                    int end = (i + 1 < n) ? opt[i + 1] : j;
                    for (int k = start; k <= end; k++)
                    {
                        long val = dp[i * n + k] + dp[k * n + j] + cost(i, j);
                        if (val < best)
                        {
                            best = val;
                            bestK = k;
                        }
                    }
                    tmp[i * n + j] = best;
                    opt[i] = bestK;
                }
            }
            return dp[0];
        }
    }

    public static unsafe class ChtDp
    {
        public static void AddLine(long* lines, int* size, long m, long b, int* ptr)
        {
            int n = *size;
            long* newLines = stackalloc long[2];
            newLines[0] = m;
            newLines[1] = b;
            while (n > 0 && n > *ptr)
            {
                int i = n - 1;
                long m1 = lines[2 * i], b1 = lines[2 * i + 1];
                long m2 = m, b2 = b;
                long x = (b1 - b2) / (m2 - m1);
                if ((b1 - b2) % (m2 - m1) != 0 && ((b1 - b2) < 0) != ((m2 - m1) < 0))
                {
                    x--;
                }
                if (x <= ptr[0])
                {
                    n--;
                }
                else
                {
                    break;
                }
            }
            for (int i = 0; i < 2; i++)
            {
                lines[2 * n + i] = newLines[i];
            }
            *size = n + 1;
        }

        public static long Query(long* lines, int size, int ptr, long x)
        {
            if (size <= 0) return 0;
            int l = 0, r = size - 1;
            while (l < r)
            {
                int mid = (l + r) >> 1;
                if (mid < ptr)
                {
                    l = mid + 1;
                }
                else
                {
                    r = mid;
                }
            }
            return lines[2 * l] * x + lines[2 * l + 1];
        }
    }
}
