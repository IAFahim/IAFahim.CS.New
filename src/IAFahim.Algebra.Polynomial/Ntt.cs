namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Ntt
    {
        private const long MOD1 = 998244353L;
        private const long MOD2 = 985661441L;
        private const long MOD3 = 754974721L;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Convolve(long* a, long* b, long* result, int n, int MOD, long primRoot)
        {
            ToomCook.Multiply(a, n, b, n, result, MOD);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThreePrime(long* a, long* b, long* result, int n)
        {
            ToomCook.Multiply(a, n, b, n, result, (int)MOD1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Crt(long* r1, long* r2, long* r3, long* result, int n, int MOD)
        {
            long m1 = MOD1, m2 = MOD2, m3 = MOD3;
            long m12 = m1 * m2;
            long inv1 = ModPow(m1 % m2, m2 - 2L, m2);
            long inv12 = ModPow(m12 % m3, m3 - 2L, m3);
            for (int i = 0; i < n; i++)
            {
                long a1 = r1[i], a2 = r2[i], a3 = r3[i];
                long k2 = ((a2 - a1 % m2 + m2) % m2 * inv1) % m2;
                long a12 = a1 + m1 * k2;
                long k3 = ((a3 - a12 % m3 + m3) % m3 * inv12) % m3;
                result[i] = (a12 + (m12 % (long)MOD) * k3) % (long)MOD;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModPow(long b, long e, long mod)
        {
            long r = 1L; b %= mod; if (b < 0L) b += mod;
            while (e > 0L) { if ((e & 1L) != 0L) r = (r * b) % mod; b = (b * b) % mod; e >>= 1; }
            return r;
        }
    }
}