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
            for (int c = 0; c < sigma; c++) last[c] = 0;
            for (int i = len; i >= 1; i--)
            {
                for (int c = 0; c < sigma; c++)
                    next[(i - 1) * sigma + c] = last[c];
                last[text[i - 1]] = i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(int* next, byte* pattern, int patLen, int sigma)
        {
            int state = next[0 * sigma + pattern[0]];
            for (int i = 1; i < patLen; i++)
            {
                if (state == 0) return false;
                state = next[state * sigma + pattern[i]];
            }
            return state != 0;
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
                    if (nxt != 0)
                        dp[nxt] = (dp[nxt] + dp[i]) % MOD;
                }
            }
            long total = 0;
            for (int i = 1; i <= len; i++)
                total = (total + dp[i]) % MOD;
            return total;
        }
    }
}
