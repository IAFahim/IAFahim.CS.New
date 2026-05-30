namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Prufer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Rank(int* seq, int n, int MOD)
        {
            long r = 0L;
            long pow = 1L;
            for (int i = 0; i < n - 2; i++) pow = (pow * (long)n) % (long)MOD;
            for (int i = 0; i < n - 2; i++)
            {
                pow = (i == 0) ? Combinatorial.ModPow((long)n, (long)(n - 3), (long)MOD) : pow / (long)n;
                r = (r + (long)seq[i] * pow) % (long)MOD;
            }
            long result = 0L;
            long pn = 1L;
            for (int i = n - 3; i >= 0; i--)
            {
                result = (result + (long)seq[i] * pn) % (long)MOD;
                pn = (pn * (long)n) % (long)MOD;
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unrank(long rank, int n, int MOD, int* seq)
        {
            for (int i = n - 3; i >= 0; i--)
            {
                seq[i] = (int)(rank % (long)n);
                rank /= (long)n;
            }
        }
    }
}