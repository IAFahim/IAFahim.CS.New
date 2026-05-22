namespace IAFahim.Algebra.GraphPoly
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Chromatic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Subset(int n, bool* adj, int MOD, long* coeffs)
        {
            int size = 1 << n;
            long* indep = stackalloc long[n + 1];
            for (int i = 0; i <= n; i++) indep[i] = 0;
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
                    indep[bits]++;
                }
            }
            for (int k = 0; k <= n; k++)
            {
                long c = 0;
                for (int i = k; i <= n; i++)
                {
                    long stir = Stirling1(i, k, MOD);
                    long sign = ((i - k) % 2 == 0) ? 1 : MOD - 1;
                    c = (c + indep[i] * stir % MOD * sign) % MOD;
                }
                coeffs[k] = c;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NumberDp(int n, bool* adj, int MOD)
        {
            long* coeffs = stackalloc long[n + 1];
            Subset(n, adj, MOD, coeffs);
            for (int k = 1; k <= n; k++)
            {
                long val = 0;
                long kPow = 1;
                for (int i = 0; i <= n; i++)
                {
                    val = (val + coeffs[i] * kPow) % MOD;
                    kPow = kPow * k % MOD;
                }
                if (val > 0) return k;
            }
            return n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DeletionContraction(int n, bool* adj, int edges, int* from, int* to, int MOD, long* coeffs)
        {
            Subset(n, adj, MOD, coeffs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Stirling1(int n, int k, int MOD)
        {
            if (n == 0 && k == 0) return 1;
            if (n == 0 || k == 0) return 0;
            long* row = stackalloc long[n + 1];
            for (int i = 0; i <= n; i++) row[i] = 0;
            row[0] = 1;
            for (int i = 1; i <= n; i++)
                for (int j = i; j >= 1; j--)
                    row[j] = (row[j - 1] + (MOD - (long)(i - 1) % MOD) % MOD * row[j]) % MOD;
            return row[k];
        }
    }
}
