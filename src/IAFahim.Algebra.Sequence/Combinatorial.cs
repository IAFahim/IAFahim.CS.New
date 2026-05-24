namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Combinatorial
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Eulerian(int n, int k, int MOD)
        {
            if (n == 0) return 0;
            if (k >= n || k < 0) return 0;
            if (k == 0) return 1;
            long* dp = stackalloc long[k + 2];
            long* ndp = stackalloc long[k + 2];
            for (int i = 0; i <= k + 1; i++) { dp[i] = 0; ndp[i] = 0; }
            dp[0] = 1;
            for (int i = 2; i <= n; i++)
            {
                UpdateEulerianRow(i, k, dp, ndp, MOD);
                SwapPointers(&dp, &ndp);
            }
            return dp[k];
        }

        private static void UpdateEulerianRow(int i, int k, long* dp, long* ndp, int MOD)
        {
            for (int j = 0; j <= Math.Min(k, i - 1); j++)
                ndp[j] = ((long)(j + 1) % MOD * dp[j] % MOD + (j > 0 ? (long)(i - j) % MOD * dp[j - 1] % MOD : 0)) % MOD;
        }

        private static void SwapPointers(long** a, long** b)
        {
            long* t = *a; *a = *b; *b = t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Narayana(int n, int k, int MOD)
        {
            if (k <= 0 || k > n) return 0;
            long c1 = Binom(n, k, MOD);
            long c2 = Binom(n, k - 1, MOD);
            return (c1 - c2 + MOD) % MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Lah(int n, int k, int MOD)
        {
            if (k > n || k <= 0) return 0;
            long nf = Factorial(n, MOD);
            long kf = Factorial(k, MOD);
            long bin = Binom(n - 1, k - 1, MOD);
            return nf * ModPow(kf, MOD - 2, MOD) % MOD * bin % MOD;
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
                int hook = CalculateHook(shape, len, i, j);
                result = result * ModPow(hook, MOD - 2, MOD) % MOD;
            }
            return result;
        }

        private static int CalculateHook(int* shape, int len, int i, int j)
        {
            int hook = shape[i] - j;
            for (int ii = i + 1; ii < len; ii++)
            {
                if (shape[ii] > j) hook++;
                else break;
            }
            return hook;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long HookLength(int* shape, int len, int MOD)
        {
            return YoungTableaux(shape, len, MOD);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long QBinomial(int n, int k, long q, int MOD)
        {
            if (k < 0 || k > n) return 0;
            if (k == 0 || k == n) return 1;
            long num = 1, den = 1;
            for (int i = 0; i < k; i++)
            {
                long qi = ModPow(q, n - 1 - i, MOD);
                num = num * ((qi - 1 + MOD) % MOD) % MOD;
                long qj = ModPow(q, i + 1, MOD);
                den = den * ((qj - 1 + MOD) % MOD) % MOD;
            }
            return num * ModPow(den, MOD - 2, MOD) % MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GaussianBinomial(int n, int k, long q, int MOD)
        {
            return QBinomial(n, k, q, MOD);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Binom(int n, int k, int MOD)
        {
            if (k < 0 || k > n) return 0;
            long num = Factorial(n, MOD);
            long den = Factorial(k, MOD) * Factorial(n - k, MOD) % MOD;
            return num * ModPow(den, MOD - 2, MOD) % MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Factorial(int n, int MOD)
        {
            long r = 1;
            for (int i = 2; i <= n; i++) r = r * i % MOD;
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ModPow(long b, long e, long mod)
        {
            long r = 1; b %= mod; if (b < 0) b += mod;
            while (e > 0) { if ((e & 1) != 0) r = r * b % mod; b = b * b % mod; e >>= 1; }
            return r;
        }
    }
}
