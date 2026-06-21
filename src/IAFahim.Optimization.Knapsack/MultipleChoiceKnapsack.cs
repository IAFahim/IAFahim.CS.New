namespace IAFahim.Optimization.Knapsack
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MultipleChoiceKnapsack
    {
        private const long NegInf = long.MinValue / 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int* groupStart, int* itemW, long* itemV, int n, int cap, long* dp)
        {
            dp[0] = 0;
            for (int c = 1; c <= cap; c++) dp[c] = NegInf;
            for (int g = 0; g < n; g++)
            {
                for (int c = cap; c >= 0; c--)
                {
                    long best = NegInf;
                    for (int idx = groupStart[g]; idx < groupStart[g + 1]; idx++)
                    {
                        if (itemW[idx] <= c && dp[c - itemW[idx]] > NegInf)
                        {
                            long cand = dp[c - itemW[idx]] + itemV[idx];
                            if (cand > best) best = cand;
                        }
                    }
                    dp[c] = best;
                }
            }
            long answer = NegInf;
            for (int c = 0; c <= cap; c++)
                if (dp[c] > answer) answer = dp[c];
            return answer;
        }
    }
}