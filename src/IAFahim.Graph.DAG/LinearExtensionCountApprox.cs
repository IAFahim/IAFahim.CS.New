namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class LinearExtensionCountApprox
    {
        // Number of linear extensions of a DAG = number of topological orderings.
        // adjMask[i] is a bitmask: bit j set => directed edge i -> j (i must precede j).
        //
        // Exact subset DP (same recurrence as the sibling CountTopologicalOrders, but
        // returned as a double so large counts don't silently overflow a long):
        //   dp[mask] = number of ways to linearly order exactly the vertices in 'mask'
        //   as a valid topological prefix.
        // A vertex i may be appended to a placed set 'mask' iff i is not yet placed and
        // every predecessor of i is already in 'mask'. Since adjMask encodes successors
        // (i -> j means i precedes j), the condition "(adjMask[i] & mask) == adjMask[i]"
        // is wrong for that direction; instead we require all predecessors of i to be in
        // 'mask', using the 'pred' masks built once from adjMask.
        //
        // The caller supplies the dp scratch buffer of length 2^n (this matches the
        // module's allocation-free convention — see CountTopologicalOrders.Run — and
        // keeps the method Burst-friendly with zero internal allocation). It is overwritten.
        // The double result is exact for counts up to 2^53. n must satisfy 1 <= n <= 31
        // so that 1 << n is a valid positive int; in practice n <= ~20 is the usable
        // range before the 2^n table dominates memory.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Run(int* adjMask, int n, double* dp)
        {
            if (n <= 1) return 1.0; // empty / single-vertex DAG: exactly one linear extension

            // pred[i] = bitmask of vertices that must precede i (its direct predecessors),
            // derived from the successor-encoded adjMask: k precedes i when adjMask[k] has bit i.
            // n <= 31 here, so a fixed 32-slot stack buffer covers every valid input.
            int* pred = stackalloc int[32];
            for (int i = 0; i < n; i++) pred[i] = 0;
            for (int k = 0; k < n; k++)
            {
                int succ = adjMask[k];
                for (int i = 0; i < n; i++)
                    if ((succ & (1 << i)) != 0)
                        pred[i] |= 1 << k;
            }

            int maxMask = 1 << n;
            int full = maxMask - 1;

            for (int m = 0; m < maxMask; m++) dp[m] = 0.0;
            dp[0] = 1.0;

            for (int mask = 0; mask < maxMask; mask++)
            {
                double ways = dp[mask];
                if (ways == 0.0) continue;

                for (int i = 0; i < n; i++)
                {
                    if ((mask & (1 << i)) != 0) continue;       // already placed
                    if ((pred[i] & mask) != pred[i]) continue;  // a predecessor still missing
                    dp[mask | (1 << i)] += ways;
                }
            }

            return dp[full];
        }
    }
}
