namespace IAFahim.String.Automata
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SubsequenceAutomaton
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Build(byte* text, int len, int* next, int sigma)
        {
            int* last = stackalloc int[sigma];
            for (int c = 0; c < sigma; c++) last[c] = len;
            for (int i = len; i >= 0; i--)
            {
                for (int c = 0; c < sigma; c++)
                    next[i * sigma + c] = last[c];
                if (i > 0)
                    last[text[i - 1]] = i - 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(int* next, byte* pattern, int patLen, int sigma)
        {
            int state = 0;
            for (int i = 0; i < patLen; i++)
            {
                state = next[state * sigma + pattern[i]];
                if (state == -1) return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long CountDistinct(int* next, int len, int sigma, int MOD)
        {
            long* dp = stackalloc long[len + 1];
            for (int i = 0; i <= len; i++) dp[i] = 0;
            dp[0] = 1;
            for (int i = 0; i <= len; i++)
            {
                for (int c = 0; c < sigma; c++)
                {
                    int nxt = next[i * sigma + c];
                    if (nxt != -1)
                        dp[nxt + 1] = (dp[nxt + 1] + dp[i]) % MOD;
                }
            }
            long total = 0;
            for (int i = 0; i <= len; i++)
                total = (total + dp[i]) % MOD;
            return total;
        }
    }
}
