namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Prufer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Rank(int* seq, int n, int MOD)
        {
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