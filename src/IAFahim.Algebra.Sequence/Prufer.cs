namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Prufer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Rank(int* seq, int n, int MOD)
        {
            long r = 0;
            long pow = 1;
            for (int i = 0; i < n - 2; i++) pow = pow * n % MOD;
            for (int i = 0; i < n - 2; i++)
            {
                pow = (i == 0) ? Combinatorial.ModPow(n, n - 3, MOD) : pow / n;
                r = (r + seq[i] * pow) % MOD;
            }
            long result = 0;
            long pn = 1;
            for (int i = n - 3; i >= 0; i--)
            {
                result = (result + seq[i] * pn) % MOD;
                pn = pn * n % MOD;
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unrank(long rank, int n, int MOD, int* seq)
        {
            for (int i = n - 3; i >= 0; i--)
            {
                seq[i] = (int)(rank % n);
                rank /= n;
            }
        }
    }
}
