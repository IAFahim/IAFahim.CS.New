namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Ntt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Convolve(long* a, long* b, long* result, int n, int MOD, long primRoot)
        {
            ToomCook.Multiply(a, b, result, n, MOD);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThreePrime(long* a, long* b, long* result, int n)
        {
            ToomCook.Multiply(a, b, result, n, 998244353);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Crt(long* r1, long* r2, long* r3, long* result, int n, int MOD)
        {
            for (int i = 0; i < n; i++)
            {
                long m1 = 998244353, m2 = 985661441, m3 = 754974721;
                long m12 = m1 * m2;
                long a1 = r1[i], a2 = r2[i], a3 = r3[i];
                long inv1 = ModPow(m1 % m2, m2 - 2, m2);
                long k2 = (a2 - a1 % m2 + m2) % m2 * inv1 % m2;
                long a12 = a1 + m1 * k2;
                long inv12 = ModPow(m12 % m3, m3 - 2, m3);
                long k3 = (a3 - a12 % m3 + m3) % m3 * inv12 % m3;
                result[i] = (a12 + m12 % MOD * k3) % MOD;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModPow(long b, long e, long mod)
        {
            long r = 1; b %= mod; if (b < 0) b += mod;
            while (e > 0) { if ((e & 1) != 0) r = r * b % mod; b = b * b % mod; e >>= 1; }
            return r;
        }
    }
}
