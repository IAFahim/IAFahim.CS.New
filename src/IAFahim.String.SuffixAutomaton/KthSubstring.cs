namespace IAFahim.String.SuffixAutomaton
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class KthSubstring
    {
        private const int Sigma = 256;

        public static bool Find(SuffixAutomaton.State* stPtr, SuffixAutomaton.Edge* e, int stateCount, long k, int* outLen, int* outPtr, long* dp)
        {
            *outLen = 0;
            if (k < 1 || stateCount <= 0) return false;

            for (int i = 0; i < stateCount; i++) dp[i] = -1;
            if (CountDp(stPtr, e, 0, dp) <= 1) return false;

            int* chars = stackalloc int[Sigma];
            int* tos = stackalloc int[Sigma];

            int v = 0;
            int len = 0;
            long rem = k + 1;
            while (true)
            {
                if (rem == 1) break;
                rem -= 1;
                int deg = CollectEdgesDedup(stPtr, e, v, chars, tos);
                bool advanced = false;
                for (int i = 0; i < deg; i++)
                {
                    int w = tos[i];
                    long cw = dp[w];
                    if (cw < 0) continue;
                    if (rem <= cw)
                    {
                        outPtr[len++] = chars[i];
                        v = w;
                        advanced = true;
                        break;
                    }
                    rem -= cw;
                }
                if (!advanced) return false;
            }
            *outLen = len;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long CountDp(SuffixAutomaton.State* st, SuffixAutomaton.Edge* e, int v, long* dp)
        {
            if (dp[v] != -1) return dp[v];
            int* chars = stackalloc int[Sigma];
            int* tos = stackalloc int[Sigma];
            int deg = CollectEdgesDedup(st, e, v, chars, tos);
            long sum = 1;
            for (int i = 0; i < deg; i++)
                sum += CountDp(st, e, tos[i], dp);
            dp[v] = sum;
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CollectEdgesDedup(SuffixAutomaton.State* st, SuffixAutomaton.Edge* e, int v, int* chars, int* tos)
        {
            int n = 0;
            for (int edge = st[v].Head; edge != -1; edge = e[edge].Next)
            {
                int c = e[edge].Char;
                bool dup = false;
                for (int j = 0; j < n; j++)
                    if (chars[j] == c) { dup = true; break; }
                if (dup) continue;
                int p = n;
                while (p > 0 && chars[p - 1] > c) { chars[p] = chars[p - 1]; tos[p] = tos[p - 1]; p--; }
                chars[p] = c;
                tos[p] = e[edge].To;
                n++;
            }
            return n;
        }
    }
}
