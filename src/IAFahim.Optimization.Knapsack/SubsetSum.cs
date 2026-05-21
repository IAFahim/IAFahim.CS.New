namespace IAFahim.Optimization.Knapsack
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SubsetSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Can(long* w, int n, long target)
        {
            if (target < 0) return false;
            if (n <= 64)
            {
                ulong bits = 1;
                for (int i = 0; i < n; i++)
                {
                    if (w[i] > target) continue;
                    bits |= bits << (int)w[i];
                }
                return ((bits >> (int)target) & 1) != 0;
            }
            bool[] dp = new bool[target + 1];
            dp[0] = true;
            for (int i = 0; i < n; i++)
            {
                if (w[i] > target) continue;
                for (long j = target; j >= w[i]; j--)
                    dp[j] = dp[j] || dp[j - w[i]];
            }
            return dp[target];
        }
    }
}
