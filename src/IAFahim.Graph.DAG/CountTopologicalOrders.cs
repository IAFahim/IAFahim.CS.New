namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class CountTopologicalOrders
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int* adjMask, int n, long* dp)
        {
            int maxMask = 1 << n;
            for (int i = 0; i < maxMask; i++) dp[i] = 0;
            dp[0] = 1;

            for (int mask = 0; mask < maxMask; mask++)
            {
                if (dp[mask] == 0) continue;
                for (int i = 0; i < n; i++)
                {
                    if ((mask & (1 << i)) == 0)
                    {
                        if ((adjMask[i] & mask) == adjMask[i]) // All dependencies met
                        {
                            dp[mask | (1 << i)] += dp[mask];
                        }
                    }
                }
            }
            return dp[maxMask - 1];
        }
    }
}