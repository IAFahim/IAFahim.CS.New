namespace IAFahim.Algebra.GraphPoly
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Rook
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Polynomial(int n, int m, bool* blocked, long x, int MOD)
        {
            long result = 0;
            int size = 1 << (n * m);
            bool* rowUsed = stackalloc bool[n];
            bool* colUsed = stackalloc bool[m];
            for (int mask = 0; mask < size; mask++)
            {
                int count = 0;
                bool valid = true;
                for (int i = 0; i < n; i++) rowUsed[i] = false;
                for (int j = 0; j < m; j++) colUsed[j] = false;
                for (int i = 0; i < n && valid; i++)
                for (int j = 0; j < m && valid; j++)
                {
                    int bit = i * m + j;
                    if ((mask & (1 << bit)) == 0) continue;
                    if (blocked[i * m + j]) { valid = false; break; }
                    if (rowUsed[i] || colUsed[j]) { valid = false; break; }
                    rowUsed[i] = true;
                    colUsed[j] = true;
                    count++;
                }
                if (valid)
                {
                    long xPow = 1;
                    for (int p = 0; p < count; p++) xPow = xPow * x % MOD;
                    result = (result + xPow) % MOD;
                }
            }
            return result;
        }
    }
}