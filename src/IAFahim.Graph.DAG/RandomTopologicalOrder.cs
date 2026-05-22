namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class RandomTopologicalOrder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* adjMask, int n, long* dp, int* order, ref uint state)
        {
            int maxMask = 1 << n;
            for (int i = 0; i < maxMask; i++) dp[i] = 0;
            dp[0] = 1;

            for (int mask = 0; mask < maxMask; mask++)
            {
                if (dp[mask] == 0) continue;
                for (int i = 0; i < n; i++)
                {
                    if ((mask & (1 << i)) == 0 && (adjMask[i] & mask) == adjMask[i])
                    {
                        dp[mask | (1 << i)] += dp[mask];
                    }
                }
            }

            int currentMask = (1 << n) - 1;
            for (int step = n - 1; step >= 0; step--)
            {
                long total = dp[currentMask];
                // simple LCG for random
                state = state * 1664525 + 1013904223;
                long r = (long)((state * (ulong)total) >> 32) + 1;
                
                for (int i = n - 1; i >= 0; i--)
                {
                    if ((currentMask & (1 << i)) != 0 && (adjMask[i] & (currentMask ^ (1 << i))) == adjMask[i])
                    {
                        long count = dp[currentMask ^ (1 << i)];
                        if (r <= count)
                        {
                            order[step] = i;
                            currentMask ^= (1 << i);
                            break;
                        }
                        r -= count;
                    }
                }
            }
        }
    }
}