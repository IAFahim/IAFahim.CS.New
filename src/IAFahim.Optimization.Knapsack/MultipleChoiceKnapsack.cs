namespace IAFahim.Optimization.Knapsack
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MultipleChoiceKnapsack
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int* groupStart, int* itemW, long* itemV, int n, int cap)
        {
            long* dp = stackalloc long[cap + 1];
            for (int i = 0; i <= cap; i++) dp[i] = 0;
            for (int g = 0; g < n; g++)
            {
                for (int c = cap; c >= 0; c--)
                {
                    long best = 0;
                    for (int idx = groupStart[g]; idx < groupStart[g + 1]; idx++)
                    {
                        if (itemW[idx] <= c)
                        {
                            long cand = dp[c - itemW[idx]] + itemV[idx];
                            if (cand > best) best = cand;
                        }
                    }
                    dp[c] = best;
                }
            }
            return dp[cap];
        }
    }
}
