namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Combinatorial
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Eulerian(int n, int k, int MOD)
        {
            if (n <= 0) return 0L;
            if (k < 0 || k >= n) return 0L;
            if (k == 0) return 1L;

            long* dp = stackalloc long[k + 1];
            long* ndp = stackalloc long[k + 1];
            for (int i = 0; i <= k; i++) { dp[i] = 0L; ndp[i] = 0L; }
            dp[0] = 1L;

            for (int i = 1; i < n; i++)
            {
                int nextN = i + 1;
                for (int j = 0; j <= Math.Min(k, nextN); j++)
                {
                    long val = (j + 1) * dp[j];
                    if (j > 0) val = (val + (long)(nextN - j) * dp[j - 1]) % MOD;
                    ndp[j] = val % MOD;
                }
                for (int j = 0; j <= Math.Min(k, nextN); j++) dp[j] = ndp[j];
            }
            return dp[k] % MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Narayana(int n, int k, int MOD)
        {
            if (k <= 0 || k > n) return 0L;
            long c1 = Binom(n, k, MOD);
            long c2 = Binom(n, k - 1, MOD);
            long invN = ModPow(n, MOD - 2, MOD);
            return c1 * c2 % MOD * invN % MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Lah(int n, int k, int MOD)
        {
            if (k > n || k <= 0) return 0L;
            long nf = Factorial(n, MOD);
            long kf = Factorial(k, MOD);
            long bin = Binom(n - 1, k - 1, MOD);
            return (nf * ModPow(kf, MOD - 2, MOD)) % MOD * bin % MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long YoungTableaux(int* shape, int len, int MOD)
        {
            int n = 0;
            for (int i = 0; i < len; i++) n += shape[i];
            long result = Factorial(n, MOD);
            for (int i = 0; i < len; i++)
            for (int j = 0; j < shape[i]; j++)
            {
                int hook = shape[i] - j;
                for (int ii = i + 1; ii < len && shape[ii] > j; ii++) hook++;
                result = (result * ModPow(hook, MOD - 2, MOD)) % MOD;
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long HookLength(int* shape, int len, int MOD) => YoungTableaux(shape, len, MOD);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long QBinomial(int n, int k, long q, int MOD)
        {
            if (k < 0 || k > n) return 0L;
            if (k == 0 || k == n) return 1L;
            // q == 1 is a singularity (0/0 in the product); by L'Hopital [n choose k]_1 = C(n,k).
            if (q % MOD == 1) return Binom(n, k, MOD);
            long num = 1L, den = 1L;
            for (int i = 0; i < k; i++)
            {
                long qi = ModPow(q, n - i, MOD);
                num = (num * ((qi - 1 + MOD) % MOD)) % MOD;
                long qj = ModPow(q, i + 1, MOD);
                den = (den * ((qj - 1 + MOD) % MOD)) % MOD;
            }
            return (num * ModPow(den, MOD - 2, MOD)) % MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GaussianBinomial(int n, int k, long q, int MOD) => QBinomial(n, k, q, MOD);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Binom(int n, int k, int MOD)
        {
            if (k < 0 || k > n) return 0L;
            long num = Factorial(n, MOD);
            long den = (Factorial(k, MOD) * Factorial(n - k, MOD)) % MOD;
            return (num * ModPow(den, MOD - 2, MOD)) % MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Factorial(int n, int MOD)
        {
            long r = 1L;
            for (int i = 2; i <= n; i++) r = (r * i) % MOD;
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ModPow(long b, long e, long mod)
        {
            long r = 1L; b %= mod; if (b < 0L) b += mod;
            while (e > 0L) { if ((e & 1L) != 0L) r = (r * b) % mod; b = (b * b) % mod; e /= 2L; }
            return r;
        }
    }
}
