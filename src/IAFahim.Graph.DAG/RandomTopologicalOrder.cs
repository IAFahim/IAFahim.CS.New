namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class RandomTopologicalOrder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CountTopologicalOrders(int* adjMask, int n, long* dp, int maxMask)
        {
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long AdvanceRng(ref uint state, long total)
        {
            state = state * 1664525 + 1013904223;
            ulong t = (ulong)total;
            ulong r0 = (ulong)state * (t >> 32);
            ulong r1 = ((ulong)state * (t & 0xFFFFFFFFUL)) >> 32;
            return (long)(r0 + r1) + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* adjMask, int n, long* dp, int* order, ref uint state)
        {
            int maxMask = 1 << n;
            CountTopologicalOrders(adjMask, n, dp, maxMask);

            int currentMask = (1 << n) - 1;
            for (int step = n - 1; step >= 0; step--)
            {
                long total = dp[currentMask];
                // Lemire-style r in [1, total]. Overflow-safe 32x64 multiply:
                // floor(state * total / 2^32) via splitting total into hi/lo 32 bits.
                long r = AdvanceRng(ref state, total);

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