namespace IAFahim.Combinatorics.Generation;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

    public static unsafe class CatalanStructures
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGenerateDyckWord(int n, byte* a, ref bool first)
        {
            if (n == 0) return false;
            if (first)
            {
                InitializeDyckWord(n, a);
                first = false;
                return true;
            }

            int ones = n, zeros = n;
            for (int i = 2 * n - 1; i >= 0; i--)
            {
                if (a[i] == 1) ones--; else zeros--;

                if (a[i] == 1 && ones >= zeros + 1)
                {
                    a[i] = 0;
                    UpdateDyckSuffix(a, i + 1, 2 * n, n - ones);
                    return true;
                }
            }
            return false;
        }

        private static void InitializeDyckWord(int n, byte* a)
        {
            for (int i = 0; i < n; i++)
            {
                a[i] = 1;
                a[n + i] = 0;
            }
        }

        private static void UpdateDyckSuffix(byte* a, int start, int end, int remainingOnes)
        {
            for (int j = start; j < end; j++)
            {
                if (remainingOnes > 0)
                {
                    a[j] = 1;
                    remainingOnes--;
                }
                else a[j] = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnrankCatalanObject(long rank, int n, byte* outObj)
        {
            if (n == 0) return;
            long* dp = (long*)Marshal.AllocHGlobal((nint)((long)(n + 2) * (n + 2) * sizeof(long)));
            PrecomputeCatalanDp(n, dp);

            int x = 0, y = 0;
            for (int i = 0; i < 2 * n; i++)
            {
                long ways = 0;
                if (x + 1 <= n) ways = dp[(x + 1) * (n + 2) + y];
                if (rank < ways) { outObj[i] = 1; x++; }
                else { outObj[i] = 0; rank -= ways; y++; }
            }
            Marshal.FreeHGlobal((nint)dp);
        }

        public static long RankCatalanObject(byte* obj, int n)
        {
            if (n == 0) return 0;
            long* dp = (long*)Marshal.AllocHGlobal((nint)((long)(n + 2) * (n + 2) * sizeof(long)));
            PrecomputeCatalanDp(n, dp);

            int x = 0, y = 0;
            long rank = 0;
            for (int i = 0; i < 2 * n; i++)
            {
                if (obj[i] == 0)
                {
                    if (x + 1 <= n) rank += dp[(x + 1) * (n + 2) + y];
                    y++;
                }
                else x++;
            }
            Marshal.FreeHGlobal((nint)dp);
            return rank;
        }

        private static void PrecomputeCatalanDp(int n, long* dp)
        {
            int size = n + 2;
            for (int i = 0; i < size * size; i++) dp[i] = 0;
            dp[n * size + n] = 1;
            for (int i = n; i >= 0; i--)
            {
                for (int j = n; j >= 0; j--)
                {
                    if (i < j || (i == n && j == n)) continue;
                    long val = 0;
                    if (i + 1 <= n) val += dp[(i + 1) * size + j];
                    if (j + 1 <= i) val += dp[i * size + (j + 1)];
                    dp[i * size + j] = val;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnrankDyckWord(long rank, int n, byte* outObj) => UnrankCatalanObject(rank, n, outObj);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RankDyckWord(byte* word, int n) => RankCatalanObject(word, n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RankBalancedParentheses(byte* s, int n) => RankDyckWord(s, n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnrankBalancedParentheses(long rank, int n, byte* outObj) => UnrankDyckWord(rank, n, outObj);
    }