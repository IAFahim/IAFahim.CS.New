namespace IAFahim.Math.Polynomial.Fps
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FormalPowerSeriesInverse
    {
        public static int Run(int n, long* a, long* res, long mod)
        {
            res[0] = ModInverse(a[0], mod);
            int len = 1;
            while (len < n)
            {
                len <<= 1;
                long* temp = stackalloc long[len];
                for (int i = 0; i < Math.Min(len, n); i++) temp[i] = a[i];
                for (int i = Math.Min(len, n); i < len; i++) temp[i] = 0;
                long* inv = stackalloc long[len];
                for (int i = 0; i < len >> 1; i++) inv[i] = res[i];
                for (int i = len >> 1; i < len; i++) inv[i] = 0;
                long* buf = stackalloc long[len];
                PolynomialMul(len >> 1, res, len >> 1, res, buf, mod);
                for (int i = 0; i < len; i++) buf[i] = (mod - buf[i]) % mod;
                PolynomialMul(len, temp, len, buf, res, mod);
                for (int i = 0; i < len; i++) res[i] = buf[i];
                if (len > n) len = n;
            }
            return n;
        }

        private static void PolynomialMul(int n, long* a, int m, long* b, long* res, long mod)
        {
            for (int i = 0; i < n + m; i++) res[i] = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    res[i + j] = (res[i + j] + a[i] * b[j]) % mod;
        }

        private static long ModInverse(long a, long m)
        {
            long b = m, u = 1, v = 0;
            while (b != 0)
            {
                long t = a / b;
                a -= t * b; long tmp = a; a = b; b = tmp;
                u -= t * v; tmp = u; u = v; v = tmp;
            }
            return (u % m + m) % m;
        }
    }

    public static unsafe class FormalPowerSeriesLog
    {
        public static int Run(int n, long* a, long* res, long mod)
        {
            long* inv = stackalloc long[n];
            FormalPowerSeriesInverse.Run(n, a, inv, mod);
            long* der = stackalloc long[n];
            int derLen = Derivative(n, a, der, mod);
            long* conv = stackalloc long[n];
            PolynomialMul(derLen, der, n, inv, conv, mod);
            int integralLen = Integral(n - 1, conv, res, mod);
            return n;
        }

        private static int Derivative(int n, long* a, long* res, long mod)
        {
            for (int i = 1; i < n; i++) res[i - 1] = a[i] * i % mod;
            return n - 1;
        }

        private static int Integral(int n, long* a, long* res, long mod)
        {
            res[0] = 0;
            for (int i = 0; i < n; i++)
            {
                long inv = ModInverse(i + 1, mod);
                res[i + 1] = a[i] * inv % mod;
            }
            return n + 1;
        }

        private static void PolynomialMul(int n, long* a, int m, long* b, long* res, long mod)
        {
            for (int i = 0; i < n + m; i++) res[i] = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    res[i + j] = (res[i + j] + a[i] * b[j]) % mod;
        }

        private static long ModInverse(long a, long m)
        {
            long b = m, u = 1, v = 0;
            while (b != 0)
            {
                long t = a / b;
                a -= t * b; long tmp = a; a = b; b = tmp;
                u -= t * v; tmp = u; u = v; v = tmp;
            }
            return (u % m + m) % m;
        }
    }

    public static unsafe class FormalPowerSeriesExp
    {
        public static int Run(int n, long* a, long* res, long mod)
        {
            res[0] = 1;
            int len = 1;
            while (len < n)
            {
                len <<= 1;
                long* lnRes = stackalloc long[len];
                int lnLen = FormalPowerSeriesLog.Run(len, res, lnRes, mod);
                long* diff = stackalloc long[len];
                for (int i = 0; i < Math.Min(n, len); i++)
                    diff[i] = (a[i] - lnRes[i] + mod) % mod;
                diff[0] = (diff[0] + 1) % mod;
                for (int i = Math.Min(n, len); i < len; i++) diff[i] = 0;
                long* newRes = stackalloc long[len];
                PolynomialMul(len, res, len, diff, newRes, mod);
                for (int i = 0; i < len && i < n; i++) res[i] = newRes[i];
                if (len > n) len = n;
            }
            return n;
        }

        private static void PolynomialMul(int n, long* a, int m, long* b, long* res, long mod)
        {
            for (int i = 0; i < n + m; i++) res[i] = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    res[i + j] = (res[i + j] + a[i] * b[j]) % mod;
        }
    }

    public static unsafe class FormalPowerSeriesPow
    {
        public static int Run(int n, long* a, long k, long* res, long mod)
        {
            if (k == 0) { res[0] = 1; for (int i = 1; i < n; i++) res[i] = 0; return n; }
            int firstNonZero = 0;
            while (firstNonZero < n && a[firstNonZero] == 0) firstNonZero++;
            if (firstNonZero >= n) { for (int i = 0; i < n; i++) res[i] = 0; return n; }
            if (firstNonZero * k >= n) { for (int i = 0; i < n; i++) res[i] = 0; return n; }
            int newN = n - (int)(firstNonZero * k);
            long* shifted = stackalloc long[newN];
            for (int i = 0; i < newN; i++) shifted[i] = a[i + firstNonZero];
            long invFirst = ModInverse(shifted[0], mod);
            long powInvFirst = FastPow(invFirst, k, mod);
            long powFirst = FastPow(a[firstNonZero], k, mod);
            for (int i = 0; i < newN; i++) shifted[i] = shifted[i] * invFirst % mod;
            long* lnShifted = stackalloc long[newN];
            FormalPowerSeriesLog.Run(newN, shifted, lnShifted, mod);
            for (int i = 0; i < newN; i++) lnShifted[i] = lnShifted[i] * k % mod;
            long* expLn = stackalloc long[newN];
            FormalPowerSeriesExp.Run(newN, lnShifted, expLn, mod);
            for (int i = 0; i < newN; i++) expLn[i] = expLn[i] * powFirst % mod;
            for (int i = 0; i < (int)(firstNonZero * k); i++) res[i] = 0;
            for (int i = 0; i < newN; i++) res[(int)(firstNonZero * k) + i] = expLn[i];
            return n;
        }

        private static long ModInverse(long a, long m)
        {
            long b = m, u = 1, v = 0;
            while (b != 0)
            {
                long t = a / b;
                a -= t * b; long tmp = a; a = b; b = tmp;
                u -= t * v; tmp = u; u = v; v = tmp;
            }
            return (u % m + m) % m;
        }

        private static long FastPow(long a, long e, long mod)
        {
            long res = 1 % mod;
            long b = a % mod;
            while (e > 0)
            {
                if ((e & 1) == 1) res = res * b % mod;
                b = b * b % mod;
                e >>= 1;
            }
            return res;
        }
    }

    public static unsafe class FormalPowerSeriesSqrt
    {
        public static int Run(int n, long* a, long* res, long mod)
        {
            if ((a[0] & 1) == 0) return -1;
            res[0] = 1;
            int len = 1;
            while (len < n)
            {
                len <<= 1;
                long* inv = stackalloc long[len];
                long inv2 = (mod + 1) >> 1;
                for (int i = 0; i < len >> 1; i++) inv[i] = res[i];
                for (int i = len >> 1; i < len; i++) inv[i] = 0;
                long* temp = stackalloc long[len];
                for (int i = 0; i < Math.Min(len, n); i++) temp[i] = a[i];
                for (int i = Math.Min(len, n); i < len; i++) temp[i] = 0;
                long* half = stackalloc long[len];
                half[0] = 2;
                for (int i = 1; i < len; i++) half[i] = 0;
                long* prod = stackalloc long[len];
                PolynomialMul(len >> 1, res, len >> 1, res, prod, mod);
                for (int i = 0; i < len; i++) prod[i] = (temp[i] - prod[i] + mod) % mod;
                PolynomialMul(len, half, len, prod, temp, mod);
                for (int i = 0; i < len; i++) temp[i] = temp[i] * inv2 % mod;
                for (int i = 0; i < len && i < n; i++) res[i] = temp[i];
                if (len > n) len = n;
            }
            return n;
        }

        private static void PolynomialMul(int n, long* a, int m, long* b, long* res, long mod)
        {
            for (int i = 0; i < n + m; i++) res[i] = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    res[i + j] = (res[i + j] + a[i] * b[j]) % mod;
        }
    }
}
