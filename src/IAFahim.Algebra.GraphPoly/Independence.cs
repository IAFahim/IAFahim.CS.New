namespace IAFahim.Algebra.GraphPoly
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Independence
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Polynomial(int n, bool* adj, long x, int MOD)
        {
            long result = 0;
            int size = 1 << n;
            for (int mask = 0; mask < size; mask++)
            {
                bool valid = true;
                for (int i = 0; i < n && valid; i++)
                {
                    if ((mask & (1 << i)) == 0) continue;
                    for (int j = i + 1; j < n && valid; j++)
                    {
                        if ((mask & (1 << j)) == 0) continue;
                        if (adj[i * n + j]) valid = false;
                    }
                }
                if (valid)
                {
                    int bits = 0;
                    for (int v = 0; v < n; v++)
                        if ((mask & (1 << v)) != 0) bits++;
                    long xPow = 1;
                    for (int p = 0; p < bits; p++) xPow = xPow * x % MOD;
                    result = (result + xPow) % MOD;
                }
            }
            return result;
        }
    }
}
