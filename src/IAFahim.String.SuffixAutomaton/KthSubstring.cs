namespace IAFahim.String.SuffixAutomaton
{
using System.Runtime.InteropServices;
    using System;

    public static unsafe class KthSubstring
    {
        public static bool Find(int* stPtr, int stateCount, long k, int* outLen, int* outPtr)
        {
            var state = (SuffixAutomaton.State*)stPtr;
            long sum = 0;
            int v = 0;
            while (true)
            {
                if (state[v].Len > 0)
                {
                    for (int c = 0; c < 256; c++)
                    {
                        int next = GetNext(v, c, state);
                        if (next != -1)
                        {
                            long cnt = CountSubstrings(next, state, stateCount);
                            if (sum + cnt >= k)
                            {
                                outPtr[0] = c;
                                return true;
                            }
                            sum += cnt;
                        }
                    }
                }
                if (state[v].Link == -1) break;
                v = state[v].Link;
            }
            return false;
        }

        private static int GetNext(int v, int c, SuffixAutomaton.State* state)
        {
            var ptr = ((IntPtr)state + v * sizeof(SuffixAutomaton.State) + sizeof(int));
            return ((int*)ptr)[c];
        }

        private static long CountSubstrings(int v, SuffixAutomaton.State* state, int stateCount)
        {
            long* dp = (long*)Marshal.AllocHGlobal(sizeof(long) * stateCount);
            for (int i = 0; i < stateCount; i++)
                dp[i] = -1;
            long result = Dfs(v, state, dp, stateCount);
            Marshal.FreeHGlobal((nint)dp);
            return result;
        }

        private static long Dfs(int v, SuffixAutomaton.State* state, long* dp, int stateCount)
        {
            if (dp[v] != -1) return dp[v];
            long sum = 1;
            for (int c = 0; c < 256; c++)
            {
                int next = GetNext(v, c, state);
                if (next != -1)
                    sum += Dfs(next, state, dp, stateCount);
            }
            dp[v] = sum;
            return sum;
        }
    }
}
