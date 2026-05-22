namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Stirling
    {
        public static void FirstRow(int n, int MOD, long* result)
        {
            for (int i = 0; i <= n; i++) result[i] = 0;
            result[0] = 1;
            for (int i = 1; i <= n; i++)
            {
                for (int j = i; j >= 1; j--)
                    result[j] = (result[j - 1] + (MOD - (long)(i - 1) % MOD) % MOD * result[j]) % MOD;
                result[0] = 0;
            }
        }

        public static void SecondRow(int n, int MOD, long* result)
        {
            for (int i = 0; i <= n; i++) result[i] = 0;
            result[0] = 1;
            for (int i = 1; i <= n; i++)
            {
                for (int j = i; j >= 1; j--)
                    result[j] = (result[j - 1] + (long)j * result[j]) % MOD;
                result[0] = 0;
            }
        }

        public static long First(int n, int k, int MOD)
        {
            long* row = stackalloc long[n + 1];
            FirstRow(n, MOD, row);
            return row[k];
        }

        public static long Second(int n, int k, int MOD)
        {
            long* row = stackalloc long[n + 1];
            SecondRow(n, MOD, row);
            return row[k];
        }
    }
}
