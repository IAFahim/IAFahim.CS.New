namespace IAFahim.Math.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

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
            for (int i = n - 1; i >= m - 1; i--)
            {
                if (a[i] == 0) continue;
                long coef = a[i] / b[m - 1];
                for (int j = m - 1; j >= 0; j--)
                    a[i - m + 1 + j] -= coef * b[j];
            }
            int len = m - 1;
            for (int i = 0; i < len; i++) r[i] = a[i];
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
                long inv = ModInverse(i + 1, mod);
                res[i + 1] = a[i] * inv % mod;
            }
            return n + 1;
        }

        private static long ModInverse(long a, long mod)
        {
            long b = mod, x = 0, y = 0;
            long g = ExtGcd(a, b, out x, out y);
            if (g != 1) return 1;
            return (x % b + b) % b;
        }

        private static long ExtGcd(long a, long b, out long x, out long y)
        {
            if (b == 0) { x = 1; y = 0; return a; }
            long x1, y1;
            long g = ExtGcd(b, a % b, out x1, out y1);
            x = y1;
            y = x1 - (a / b) * y1;
            return g;
        }
    }

    public static unsafe class PolynomialInverse
    {
        public static int Run(int n, long* a, long* res, long mod)
        {
            res[0] = ModInverse(a[0], mod);
            int m = 1;
            while (m < n)
            {
                m <<= 1;
                long* temp = stackalloc long[m];
                for (int i = 0; i < Math.Min(n, m); i++) temp[i] = a[i];
                for (int i = Math.Min(n, m); i < m; i++) temp[i] = 0;
                long* buf = stackalloc long[m];
                for (int i = 0; i < m; i++) buf[i] = 0;
                PolynomialMul.Run(m >> 1, res, m >> 1, res, buf);
                for (int i = 0; i < m; i++) buf[i] = (mod - buf[i]) % mod;
                PolynomialMul.Run(m, temp, m, buf, res);
                m = Math.Min(m, n);
            }
            return n;
        }

        private static long ModInverse(long a, long mod)
        {
            long b = mod, x = 0, y = 0;
            long g = ExtGcd(a, b, out x, out y);
            if (g != 1) return 1;
            return (x % b + b) % b;
        }

        private static long ExtGcd(long a, long b, out long x, out long y)
        {
            if (b == 0) { x = 1; y = 0; return a; }
            long x1, y1;
            long g = ExtGcd(b, a % b, out x1, out y1);
            x = y1;
            y = x1 - (a / b) * y1;
            return g;
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
            int invLen = PolynomialInverse.Run(n, a, inv, mod);
            long* buf = stackalloc long[n];
            int bufLen = PolynomialMul.Run(derLen, der, invLen, inv, buf);
            int integralLen = PolynomialIntegral.Run(bufLen, buf, res, mod);
            return n;
        }
    }

    public static unsafe class PolynomialExp
    {
        public static int Run(int n, long* a, long* res, long mod)
        {
            res[0] = 1;
            int m = 1;
            while (m < n)
            {
                m <<= 1;
                long* lnRes = stackalloc long[m];
                int lnLen = PolynomialLog.Run(Math.Min(m, n), res, lnRes, mod);
                long* buf = stackalloc long[m];
                for (int i = 0; i < m; i++) buf[i] = 0;
                for (int i = 0; i < Math.Min(n, m); i++)
                    buf[i] = (a[i] - lnRes[i] + mod) % mod;
                buf[0] = (buf[0] + 1) % mod;
                long* newRes = stackalloc long[m];
                PolynomialMul.Run(Math.Min(m, n), res, Math.Min(m, n), buf, newRes);
                for (int i = 0; i < m; i++) res[i] = newRes[i];
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
                    for (int i = 0; i < len && i < n; i++) res[i] = buf[i] % mod;
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
            if ((a[0] & 1) == 0) return -1;
            res[0] = 1;
            long inv2 = (mod + 1) >> 1;
            int m = 1;
            while (m < n)
            {
                m <<= 1;
                long* inv = stackalloc long[m];
                for (int i = 0; i < m; i++) inv[i] = 0;
                inv[0] = 2;
                long* buf = stackalloc long[m];
                for (int i = 0; i < m; i++) buf[i] = 0;
                PolynomialMul.Run(Math.Min(m, n), res, 1, inv, buf);
                for (int i = 0; i < m; i++) res[i] = buf[i] % mod;
                long* temp = stackalloc long[m];
                for (int i = 0; i < Math.Min(n, m); i++) temp[i] = a[i];
                for (int i = Math.Min(n, m); i < m; i++) temp[i] = 0;
                PolynomialMul.Run(Math.Min(m, n), temp, m, res, buf);
                for (int i = 0; i < m; i++)
                    res[i] = (res[i] + buf[i] * inv2) % mod;
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
                res = (res + a[i] * cur) % mod;
                cur = (cur * x) % mod;
            }
            return res;
        }
    }

    public static unsafe class PolynomialInterpolate
    {
        public static int Run(int n, long* x, long* y, long* res, long mod)
        {
            long* prefix = stackalloc long[n + 1];
            long* suffix = stackalloc long[n + 1];
            prefix[0] = 1;
            for (int i = 0; i < n; i++)
                prefix[i + 1] = (prefix[i] * (mod - x[i])) % mod;
            suffix[n] = 1;
            for (int i = n - 1; i >= 0; i--)
                suffix[i] = (suffix[i + 1] * (mod - x[i])) % mod;
            for (int i = 0; i < n; i++)
            {
                long den = (prefix[i] * suffix[i + 1]) % mod;
                long l = ModInverse(den, mod);
                long* left = stackalloc long[i + 1];
                long* right = stackalloc long[n - i];
                for (int j = 0; j <= i; j++) left[j] = prefix[j];
                for (int j = 0; j < n - i; j++) right[j] = suffix[i + 1 + j];
                long* w = stackalloc long[n];
                PolynomialMul.Run(i + 1, left, n - i, right, w);
                for (int j = 0; j < n; j++)
                    res[j] = (res[j] + y[i] * l % mod * w[j]) % mod;
            }
            return n;
        }

        private static long ModInverse(long a, long mod)
        {
            long b = mod, x = 0, y = 0;
            long g = ExtGcd(a, b, out x, out y);
            if (g != 1) return 1;
            return (x % b + b) % b;
        }

        private static long ExtGcd(long a, long b, out long x, out long y)
        {
            if (b == 0) { x = 1; y = 0; return a; }
            long x1, y1;
            long g = ExtGcd(b, a % b, out x1, out y1);
            x = y1;
            y = x1 - (a / b) * y1;
            return g;
        }
    }

    public static unsafe class LagrangeInterpolate
    {
        public static long Run(long* x, long* y, long n, long t, long mod)
        {
            if (n == 1) return y[0];
            int nn = (int)n;
            long* prefix = stackalloc long[nn + 1];
            long* suffix = stackalloc long[nn + 1];
            prefix[0] = 1;
            for (int i = 0; i < nn; i++)
                prefix[i + 1] = (prefix[i] * (t - x[i] + mod)) % mod;
            suffix[nn] = 1;
            for (int i = nn - 1; i >= 0; i--)
                suffix[i] = (suffix[i + 1] * (t - x[i] + mod)) % mod;
            long res = 0;
            for (int i = 0; i < nn; i++)
            {
                long den = (prefix[i] * suffix[i + 1]) % mod;
                long l = ModInverse(den, mod);
                long term = y[i] % mod * l % mod;
                res = (res + term) % mod;
            }
            return res;
        }

        private static long ModInverse(long a, long mod)
        {
            long b = mod, x = 0, y = 0;
            long g = ExtGcd(a, b, out x, out y);
            if (g != 1) return 1;
            return (x % b + b) % b;
        }

        private static long ExtGcd(long a, long b, out long x, out long y)
        {
            if (b == 0) { x = 1; y = 0; return a; }
            long x1, y1;
            long g = ExtGcd(b, a % b, out x1, out y1);
            x = y1;
            y = x1 - (a / b) * y1;
            return g;
        }
    }
}