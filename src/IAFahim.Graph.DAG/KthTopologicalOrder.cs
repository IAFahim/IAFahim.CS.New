namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class KthTopologicalOrder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int* adjMask, int n, long* dp, long k, int* order)
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

            if (k > dp[maxMask - 1] || k <= 0) return false;

            int currentMask = (1 << n) - 1;
            for (int step = n - 1; step >= 0; step--)
            {
                for (int i = n - 1; i >= 0; i--) // Iterating backwards to find the node correctly based on k
                {
                    if ((currentMask & (1 << i)) != 0 && (adjMask[i] & (currentMask ^ (1 << i))) == adjMask[i])
                    {
                        long count = dp[currentMask ^ (1 << i)];
                        if (k <= count)
                        {
                            order[step] = i;
                            currentMask ^= (1 << i);
                            break;
                        }
                        k -= count;
                    }
                }
            }
            return true;
        }
    }
}