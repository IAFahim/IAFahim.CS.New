namespace IAFahim.Optimization.Treewidth
{
    using System.Runtime.CompilerServices;

    public static unsafe class CutAndCount
    {
        public static int Run(int n, bool* adj, int* bag, int bagSize, long* dp)
        {
            if (bagSize <= 0) return 0;
            if (bagSize > 20) return 0;

            int full = 1 << bagSize;
            for (int mask = 0; mask < full; mask++) dp[mask] = 0;

            int count = 0;
            for (int mask = 1; mask < full; mask++)
            {
                if (!InducedConnected(n, adj, bag, bagSize, mask)) continue;
                dp[mask] = 1;
                count++;
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InducedConnected(int n, bool* adj, int* bag, int bagSize, int mask)
        {
            int first = -1;
            int want = 0;
            for (int i = 0; i < bagSize; i++)
            {
                if (((mask >> i) & 1) == 0) continue;
                want++;
                if (first < 0) first = i;
            }
            if (want <= 1) return true;

            int* stack = stackalloc int[bagSize];
            byte* seen = stackalloc byte[bagSize];
            for (int i = 0; i < bagSize; i++) seen[i] = 0;
            int top = 0;
            stack[top++] = first;
            seen[first] = 1;
            int reached = 1;
            while (top > 0)
            {
                int u = stack[--top];
                int uu = bag[u];
                if ((uint)uu >= (uint)n) return false;
                for (int v = 0; v < bagSize; v++)
                {
                    if (seen[v] != 0) continue;
                    if (((mask >> v) & 1) == 0) continue;
                    int vv = bag[v];
                    if ((uint)vv >= (uint)n) return false;
                    if (!adj[uu * n + vv]) continue;
                    seen[v] = 1;
                    stack[top++] = v;
                    reached++;
                }
            }
            return reached == want;
        }
    }
}
