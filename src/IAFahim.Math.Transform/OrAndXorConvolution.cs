namespace IAFahim.Math.Transform
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class OrAndXorConvolution
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FwtOr(long* f, int n, long mod, bool inverse)
        {
            for (int len = 1; len < n; len <<= 1)
            {
                for (int i = 0; i < n; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        long u = f[i + j];
                        long v = f[i + j + len];
                        if (!inverse)
                        {
                            f[i + j + len] = (v + u) % mod;
                        }
                        else
                        {
                            f[i + j + len] = (v - u + mod) % mod;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FwtAnd(long* f, int n, long mod, bool inverse)
        {
            for (int len = 1; len < n; len <<= 1)
            {
                for (int i = 0; i < n; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        long u = f[i + j];
                        long v = f[i + j + len];
                        if (!inverse)
                        {
                            f[i + j] = (u + v) % mod;
                        }
                        else
                        {
                            f[i + j] = (u - v + mod) % mod;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FwtXor(long* f, int n, long mod)
        {
            for (int len = 1; len < n; len <<= 1)
            {
                for (int i = 0; i < n; i += len << 1)
                {
                    for (int j = 0; j < len; j++)
                    {
                        long u = f[i + j];
                        long v = f[i + j + len];
                        f[i + j] = (u + v) % mod;
                        f[i + j + len] = (u - v + mod) % mod;
                    }
                }
            }
        }

        private static long ModInverse(long a, long mod)
        {
            long x;
            long y;
            long g = ExtGcd(a, mod, &x, &y);
            if (g != 1)
            {
                return 1;
            }
            return (x % mod + mod) % mod;
        }

        private static long ExtGcd(long a, long b, long* x, long* y)
        {
            if (b == 0)
            {
                *x = 1;
                *y = 0;
                return a;
            }
            long x1;
            long y1;
            long g = ExtGcd(b, a % b, &x1, &y1);
            *x = y1;
            *y = x1 - (a / b) * y1;
            return g;
        }

        public static void RunOr(long* a, long* b, long* c, int logN, long mod, long* ta, long* tb)
        {
            int n = 1 << logN;
            for (int i = 0; i < n; i++)
            {
                ta[i] = a[i] % mod;
                tb[i] = b[i] % mod;
            }
            FwtOr(ta, n, mod, false);
            FwtOr(tb, n, mod, false);
            for (int i = 0; i < n; i++)
            {
                c[i] = ta[i] * tb[i] % mod;
            }
            FwtOr(c, n, mod, true);
        }

        public static void RunAnd(long* a, long* b, long* c, int logN, long mod, long* ta, long* tb)
        {
            int n = 1 << logN;
            for (int i = 0; i < n; i++)
            {
                ta[i] = a[i] % mod;
                tb[i] = b[i] % mod;
            }
            FwtAnd(ta, n, mod, false);
            FwtAnd(tb, n, mod, false);
            for (int i = 0; i < n; i++)
            {
                c[i] = ta[i] * tb[i] % mod;
            }
            FwtAnd(c, n, mod, true);
        }

        public static void RunXor(long* a, long* b, long* c, int logN, long mod, long* ta, long* tb)
        {
            int n = 1 << logN;
            for (int i = 0; i < n; i++)
            {
                ta[i] = a[i] % mod;
                tb[i] = b[i] % mod;
            }
            FwtXor(ta, n, mod);
            FwtXor(tb, n, mod);
            for (int i = 0; i < n; i++)
            {
                c[i] = ta[i] * tb[i] % mod;
            }
            FwtXor(c, n, mod);
            long invN = ModInverse(n, mod);
            for (int i = 0; i < n; i++)
            {
                c[i] = c[i] * invN % mod;
            }
        }
    }
}
