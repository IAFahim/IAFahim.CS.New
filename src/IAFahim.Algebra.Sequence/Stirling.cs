namespace IAFahim.Algebra.Sequence
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Stirling
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FirstRow(int n, int MOD, long* result)
        {
            for (int i = 0; i <= n; i++) result[i] = 0L;
            result[0] = 1L;
            for (int i = 1; i <= n; i++)
            {
                UpdateFirstRow(i, MOD, result);
                result[0] = 0L;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateFirstRow(int i, int MOD, long* result)
        {
            long factor = ((long)MOD - ((long)(i - 1) % (long)MOD)) % (long)MOD;
            for (int j = i; j >= 1; j--)
                result[j] = (result[j - 1] + factor * result[j]) % (long)MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SecondRow(int n, int MOD, long* result)
        {
            for (int i = 0; i <= n; i++) result[i] = 0L;
            result[0] = 1L;
            for (int i = 1; i <= n; i++)
            {
                UpdateSecondRow(i, MOD, result);
                result[0] = 0L;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateSecondRow(int i, int MOD, long* result)
        {
            for (int j = i; j >= 1; j--)
                result[j] = (result[j - 1] + (long)j * result[j]) % (long)MOD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long First(int n, int k, int MOD)
        {
            long* row = stackalloc long[n + 1];
            FirstRow(n, MOD, row);
            return row[k];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Second(int n, int k, int MOD)
        {
            long* row = stackalloc long[n + 1];
            SecondRow(n, MOD, row);
            return row[k];
        }
    }
}