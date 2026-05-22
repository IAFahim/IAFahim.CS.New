namespace IAFahim.Algebra.GraphPoly
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Matching
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Polynomial(int n, bool* adj, long x, int MOD)
        {
            long result = 0;
            int size = 1 << n;
            for (int mask = 0; mask < size; mask++)
            {
                bool valid = true;
                int edgeCount = 0;
                for (int i = 0; i < n && valid; i++)
                {
                    if ((mask & (1 << i)) == 0) continue;
                    for (int j = i + 1; j < n && valid; j++)
                    {
                        if ((mask & (1 << j)) == 0) continue;
                        if (adj[i * n + j]) valid = false;
                    }
                    for (int j = i + 1; j < n; j++)
                    {
                        if ((mask & (1 << j)) == 0) continue;
                        edgeCount++;
                    }
                }
                if (valid)
                {
                    long sign = (edgeCount % 2 == 0) ? 1 : -1;
                    long xPow = 1;
                    for (int p = 0; p < n; p++)
                    {
                        if ((mask & (1 << p)) != 0)
                            xPow = xPow * x % MOD;
                    }
                    result = (result + sign * xPow % MOD + MOD) % MOD;
                }
            }
            return result;
        }
    }
}
