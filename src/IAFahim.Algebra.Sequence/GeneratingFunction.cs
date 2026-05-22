namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class GeneratingFunction
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EgfMultiply(long* a, long* b, int n, int MOD, long* result)
        {
            for (int i = 0; i < n; i++) result[i] = 0;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n - i; j++)
            {
                long fact = Combinatorial.Factorial(i + j, MOD);
                long ifact = Combinatorial.ModPow(fact, MOD - 2, MOD);
                long bin = Combinatorial.Binom(i + j, i, MOD);
                result[i + j] = (result[i + j] + a[i] * b[j] % MOD * bin) % MOD;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OgfMultiply(long* a, long* b, int n, int MOD, long* result)
        {
            for (int i = 0; i < n; i++) result[i] = 0;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n - i; j++)
                result[i + j] = (result[i + j] + a[i] * b[j]) % MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LagrangeInversion(long* f, int n, int k, int MOD)
        {
            if (k <= 0) return 0;
            long* der = stackalloc long[n];
            for (int i = 1; i < n; i++)
                der[i - 1] = (long)i % MOD * f[i] % MOD;
            long result = der[k - 1] * k % MOD;
            long invK = Combinatorial.ModPow(k, MOD - 2, MOD);
            return result * invK % MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long TreeCount(int n, int MOD)
        {
            if (n <= 1) return 1;
            long result = Combinatorial.ModPow(n, n - 2, MOD);
            return result;
        }
    }
}
