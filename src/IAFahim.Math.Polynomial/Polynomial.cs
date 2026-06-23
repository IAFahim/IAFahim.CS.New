namespace IAFahim.Math.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PolyModArith
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ModInverse(long a, long mod)
        {
            long b = mod, x = 0, y = 0;
            long g = ExtGcd(a, b, out x, out y);
            if (g != 1) return 1;
            return (x % b + b) % b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ExtGcd(long a, long b, out long x, out long y)
        {
            if (b == 0) { x = 1; y = 0; return a; }
            long x1, y1;
            long g = ExtGcd(b, a % b, out x1, out y1);
            x = y1;
            y = x1 - (a / b) * y1;
            return g;
        }
    }

    public static unsafe class PolynomialAdd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, long* a, int m, long* b, long* res)
        {
            int len = n > m ? n : m;
            int i = 0;
            for (; i < Math.Min(n, m); i++)
                res[i] = a[i] + b[i];
            for (; i < n; i++) res[i] = a[i];
            for (; i < m; i++) res[i] = b[i];
            return len;
        }
    }

    public static unsafe class PolynomialSub
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, long* a, int m, long* b, long* res)
        {
            int len = n > m ? n : m;
            int i = 0;
            for (; i < Math.Min(n, m); i++)
                res[i] = a[i] - b[i];
            for (; i < n; i++) res[i] = a[i];
            for (; i < m; i++) res[i] = -b[i];
            return len;
        }
    }

    public static unsafe class PolynomialMul
    {
        public static int Run(int n, long* a, int m, long* b, long* res)
        {
            for (int i = 0; i < n + m - 1; i++) res[i] = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    res[i + j] += a[i] * b[j];
            return n + m - 1;
        }

        public static int RunMod(int n, long* a, int m, long* b, long* res, long mod)
        {
            for (int i = 0; i < n + m - 1; i++) res[i] = 0;
            for (int i = 0; i < n; i++)
            {
                long ai = a[i] % mod;
                for (int j = 0; j < m; j++)
                    res[i + j] = (res[i + j] + ai * (b[j] % mod)) % mod;
            }
            return n + m - 1;
        }
    }

    public static unsafe class PolynomialDiv
    {
        public static int Run(int n, long* a, int m, long* b, long* q, long* r)
        {
            if (n < m)
            {
                for (int i = 0; i < n; i++) r[i] = a[i];
                return n;
            }
            for (int i = 0; i < n - m + 1; i++) q[i] = 0;
            for (int i = 0; i < n; i++) r[i] = a[i];
            for (int i = n - 1; i >= m - 1; i--)
            {
                if (r[i] == 0) continue;
                long coef = r[i] / b[m - 1];
                q[i - m + 1] = coef;
                for (int j = m - 1; j >= 0; j--)
                    r[i - m + 1 + j] -= coef * b[j];
            }
            return n - m + 1;
        }
    }

    public static unsafe class PolynomialMod
    {
        public static int Run(int n, long* a, int m, long* b, long* r)
        {
            if (n < m)
            {
                for (int i = 0; i < n; i++) r[i] = a[i];
                return n;
            }
            long* q = stackalloc long[n];
            for (int i = 0; i < n; i++) q[i] = a[i];
            for (int i = n - 1; i >= m - 1; i--)
            {
                if (q[i] == 0) continue;
                long coef = q[i] / b[m - 1];
                for (int j = m - 1; j >= 0; j--)
                    q[i - m + 1 + j] -= coef * b[j];
            }
            int len = m - 1;
            for (int i = 0; i < len; i++) r[i] = q[i];
            return len;
        }
    }

    public static unsafe class PolynomialDerivative
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, long* a, long* res)
        {
            if (n <= 1) { res[0] = 0; return 1; }
            for (int i = 1; i < n; i++)
                res[i - 1] = a[i] * i;
            return n - 1;
        }
    }

    public static unsafe class PolynomialIntegral
    {
        public static int Run(int n, long* a, long* res, long mod)
        {
            res[0] = 0;
            for (int i = 0; i < n; i++)
            {
            long inv = PolyModArith.ModInverse(i + 1, mod);
            res[i + 1] = a[i] % mod * inv % mod;
            }
            return n + 1;
        }
    }

    public static unsafe class PolynomialInverse
    {
        public static int Run(int n, long* a, long* res, long mod)
        {
            res[0] = PolyModArith.ModInverse(a[0], mod);
            int sz = 1;
            while (sz < n) sz <<= 1;
            long* tmp = stackalloc long[sz * 6];
            int m = 1;
            while (m < n)
            {
                m <<= 1;
                int cur = Math.Min(m, n);
                long* fa = tmp;
                long* fb = tmp + m;
                long* fc = tmp + 2 * m;
                long* fr = tmp + 4 * m;
                for (int i = 0; i < cur; i++) fa[i] = a[i];
                for (int i = cur; i < m; i++) fa[i] = 0;
                for (int i = 0; i < m / 2; i++) fb[i] = res[i];
                for (int i = m / 2; i < m; i++) fb[i] = 0;
                for (int i = 0; i < 2 * m; i++) fc[i] = 0;
                PolynomialMul.RunMod(m, fa, m / 2, fb, fc, mod);
                for (int i = 0; i < m; i++) fc[i] = (mod - fc[i] % mod) % mod;
                fc[0] = (fc[0] + 2) % mod;
                for (int i = m; i < 2 * m; i++) fc[i] = 0;
                for (int i = 0; i < 2 * m; i++) fr[i] = 0;
                PolynomialMul.RunMod(m / 2, fb, m, fc, fr, mod);
                for (int i = 0; i < cur; i++) res[i] = fr[i] % mod;
            }
            return n;
        }
    }

    public static unsafe class PolynomialLog
    {
        public static int Run(int n, long* a, long* res, long mod)
        {
            if (a[0] != 1) return -1;
            long* der = stackalloc long[n];
            int derLen = PolynomialDerivative.Run(n, a, der);
            long* inv = stackalloc long[n];
            PolynomialInverse.Run(n, a, inv, mod);
            long* buf = stackalloc long[2 * n];
            for (int i = 0; i < 2 * n; i++) buf[i] = 0;
            PolynomialMul.RunMod(derLen, der, n, inv, buf, mod);
            long* bufTrunc = stackalloc long[n];
            for (int i = 0; i < n - 1; i++) bufTrunc[i] = buf[i] % mod;
            PolynomialIntegral.Run(n - 1, bufTrunc, res, mod);
            for (int i = 0; i < n; i++) res[i] = (res[i] % mod + mod) % mod;
            return n;
        }
    }

    public static unsafe class PolynomialExp
    {
        public static int Run(int n, long* a, long* res, long mod)
        {
            if (n == 0) return 0;
            res[0] = 1;
            for (int i = 1; i < n; i++) res[i] = 0;
            int m = 1;
            long* lnBuf = stackalloc long[n];
            long* diff = stackalloc long[n];
            long* newRes = stackalloc long[2 * n];
            while (m < n)
            {
                m <<= 1;
                int cur = Math.Min(m, n);
                for (int i = 0; i < cur; i++) lnBuf[i] = 0;
                PolynomialLog.Run(cur, res, lnBuf, mod);
                for (int i = 0; i < cur; i++)
                    diff[i] = ((i < n ? a[i] : 0) - lnBuf[i] % mod + mod) % mod;
                diff[0] = (diff[0] + 1) % mod;
                for (int i = 0; i < 2 * n; i++) newRes[i] = 0;
                PolynomialMul.RunMod(Math.Min(m / 2, n), res, cur, diff, newRes, mod);
                for (int i = 0; i < cur; i++) res[i] = newRes[i] % mod;
            }
            return n;
        }
    }

    public static unsafe class PolynomialPow
    {
        public static int Run(int n, long* a, long k, long* res, long mod)
        {
            if (k == 0)
            {
                res[0] = 1;
                return 1;
            }
            long* temp = stackalloc long[n];
            for (int i = 0; i < n; i++) temp[i] = a[i];
            res[0] = 1;
            int len = 1;
            while (k > 0)
            {
                if ((k & 1) == 1)
                {
                    long* buf = stackalloc long[n * 2];
                    len = PolynomialMul.Run(len, res, n, temp, buf);
                    if (len > n) len = n;
                    for (int i = 0; i < len; i++) res[i] = buf[i] % mod;
                }
                long* sqr = stackalloc long[n * 2];
                int sqrLen = PolynomialMul.Run(n, temp, n, temp, sqr);
                for (int i = 0; i < sqrLen && i < n; i++) temp[i] = sqr[i] % mod;
                k >>= 1;
            }
            return len;
        }
    }

    public static unsafe class PolynomialSqrt
    {
        public static int Run(int n, long* a, long* res, long mod)
        {
            if ((a[0] & 1) == 0 && a[0] != 1) return -1;
            res[0] = 1;
            long inv2 = (mod + 1) >> 1;
            long* inv = stackalloc long[n];
            long* tmp = stackalloc long[2 * n];
            int m = 1;
            while (m < n)
            {
                m <<= 1;
                int cur = Math.Min(m, n);
                for (int i = 0; i < cur; i++) inv[i] = 0;
                PolynomialInverse.Run(cur, res, inv, mod);
                for (int i = 0; i < 2 * n; i++) tmp[i] = 0;
                long* aTrunc = stackalloc long[cur];
                for (int i = 0; i < cur; i++) aTrunc[i] = a[i] % mod;
                PolynomialMul.RunMod(cur, aTrunc, cur, inv, tmp, mod);
                for (int i = 0; i < cur; i++)
                    res[i] = (res[i] + tmp[i]) % mod * inv2 % mod;
            }
            return n;
        }
    }

    public static unsafe class PolynomialEval
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long* a, long x, long mod)
        {
            long res = 0, cur = 1;
            for (int i = 0; i < n; i++)
            {
                res = (res + a[i] % mod * cur) % mod;
                cur = cur * x % mod;
            }
            return res;
        }
    }

    public static unsafe class PolynomialInterpolate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildVandermonde(long* x, int n, long mod, long* vand)
        {
            for (int i = 0; i <= n; i++) vand[i] = 0;
            vand[0] = 1;
            for (int i = 0; i < n; i++)
            {
                long negXi = (mod - x[i] % mod) % mod;
                for (int j = i + 1; j >= 1; j--)
                    vand[j] = (vand[j - 1] + vand[j] * negXi) % mod;
                vand[0] = vand[0] * negXi % mod;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EvalBasisAt(long xi, long* vand, int n, long* wi, long mod)
        {
            wi[n - 1] = vand[n];
            for (int j = n - 2; j >= 0; j--)
                wi[j] = (vand[j + 1] + wi[j + 1] * xi) % mod;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AccumulateWeights(long* res, long* wi, int n, long* x, long* y, int i, long mod)
        {
            long xi = x[i] % mod;
            long den = 1;
            for (int j = 0; j < n; j++)
            {
                if (j == i) continue;
                den = den * ((xi - x[j] % mod + mod) % mod) % mod;
            }
            long inv = PolyModArith.ModInverse(den, mod);
            long coef = y[i] % mod * inv % mod;
            for (int j = 0; j < n; j++)
                res[j] = (res[j] + coef * wi[j]) % mod;
        }

        public static int Run(int n, long* x, long* y, long* res, long mod)
        {
            for (int i = 0; i < n; i++) res[i] = 0;
            if (n == 0) return 0;
            long* vand = stackalloc long[n + 1];
            BuildVandermonde(x, n, mod, vand);
            long* wi = stackalloc long[n];
            for (int i = 0; i < n; i++)
            {
                long xi = x[i] % mod;
                EvalBasisAt(xi, vand, n, wi, mod);
                AccumulateWeights(res, wi, n, x, y, i, mod);
            }
            return n;
        }
    }

    public static unsafe class LagrangeInterpolate
    {
        public static long Run(long* x, long* y, long n, long t, long mod)
        {
            if (n == 1) return y[0] % mod;
            int nn = (int)n;
            long* prefix = stackalloc long[nn + 1];
            long* suffix = stackalloc long[nn + 1];
            prefix[0] = 1;
            for (int i = 0; i < nn; i++)
                prefix[i + 1] = prefix[i] * ((t - x[i] % mod + mod) % mod) % mod;
            suffix[nn] = 1;
            for (int i = nn - 1; i >= 0; i--)
                suffix[i] = suffix[i + 1] * ((t - x[i] % mod + mod) % mod) % mod;
            long res = 0;
            for (int i = 0; i < nn; i++)
            {
                long num = prefix[i] * suffix[i + 1] % mod;
                long* denTerms = stackalloc long[nn - 1];
                int dc = 0;
                for (int j = 0; j < nn; j++)
                {
                    if (j == i) continue;
                    denTerms[dc++] = (x[i] - x[j] % mod + mod) % mod;
                }
                long den = 1;
                for (int j = 0; j < dc; j++) den = den * denTerms[j] % mod;
                long inv = PolyModArith.ModInverse(den, mod);
                long term = y[i] % mod * num % mod * inv % mod;
                res = (res + term) % mod;
            }
            return res;
        }
    }
}