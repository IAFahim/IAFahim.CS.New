namespace IAFahim.Algebra.GraphPoly
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Rook
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidPlacement(int n, int m, bool* blocked, long mask, bool* rowUsed, bool* colUsed, out int count)
        {
            count = 0;
            for (int i = 0; i < n; i++) rowUsed[i] = false;
            for (int j = 0; j < m; j++) colUsed[j] = false;

            for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
            {
                int bit = i * m + j;
                if ((mask & (1L << bit)) == 0) continue;
                if (blocked[bit] || rowUsed[i] || colUsed[j]) return false;
                rowUsed[i] = true;
                colUsed[j] = true;
                count++;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, int m, bool* blocked, long x, int MOD)
        {
            long result = 0L;
            long size = 1L << (n * m);
            bool* rowUsed = stackalloc bool[n];
            bool* colUsed = stackalloc bool[m];
            for (long mask = 0; mask < size; mask++)
            {
                if (IsValidPlacement(n, m, blocked, mask, rowUsed, colUsed, out int count))
                {
                    long xPow = 1L;
                    for (int p = 0; p < count; p++) xPow = (xPow * x) % (long)MOD;
                    result = (result + xPow) % (long)MOD;
                }
            }
            return result;
        }
    }
}