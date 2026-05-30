namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class GeneratingFunction
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EgfMultiply(long* a, long* b, int n, int MOD, long* result)
        {
            for (int i = 0; i < n; i++) result[i] = 0L;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n - i; j++)
            {
                long fact = Combinatorial.Factorial(i + j, MOD);
                long ifact = Combinatorial.ModPow(fact, (long)MOD - 2L, (long)MOD);
                long bin = Combinatorial.Binom(i + j, i, MOD);
                result[i + j] = (result[i + j] + ((a[i] * b[j]) % (long)MOD) * bin) % (long)MOD;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OgfMultiply(long* a, long* b, int n, int MOD, long* result)
        {
            for (int i = 0; i < n; i++) result[i] = 0L;
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n - i; j++)
                result[i + j] = (result[i + j] + a[i] * b[j]) % (long)MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LagrangeInversion(long* f, int n, int k, int MOD)
        {
            if (k <= 0) return 0L;
            long* der = stackalloc long[n];
            for (int i = 1; i < n; i++)
                der[i - 1] = ((long)i % (long)MOD * f[i]) % (long)MOD;
            long result = (der[k - 1] * (long)k) % (long)MOD;
            long invK = Combinatorial.ModPow((long)k, (long)MOD - 2L, (long)MOD);
            return (result * invK) % (long)MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long TreeCount(int n, int MOD)
        {
            if (n <= 1) return 1L;
            long result = Combinatorial.ModPow((long)n, (long)(n - 2), (long)MOD);
            return result;
        }
    }
}