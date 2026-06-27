namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class PermutationCyclePower
    {
        // res = p^k : applies the permutation p (of 0..n-1) k times.
        // Per cycle of length L the effective shift is k mod L, so huge k stays O(n).
        // Zero scratch: res itself is the visited marker (seed -1, every final value is in 0..n-1).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* p, int n, long k, int* res)
        {
            for (int i = 0; i < n; i++) res[i] = -1;

            for (int start = 0; start < n; start++)
            {
                if (res[start] != -1) continue; // already assigned via its cycle

                // 1) Measure cycle length L.
                int len = 0;
                int node = start;
                do
                {
                    node = p[node];
                    len++;
                } while (node != start);

                // 2) Effective non-negative shift along the cycle.
                int shift = (int)(k % len);
                if (shift < 0) shift += len;

                // 3) target = node 'shift' steps ahead of 'start'.
                int target = start;
                for (int s = 0; s < shift; s++) target = p[target];

                // 4) Walk the whole cycle, writing res[cur] = (cur advanced shift steps).
                int cur = start;
                for (int i = 0; i < len; i++)
                {
                    int next = p[cur];     // capture before res[cur] overwrites nothing (res != p, but be explicit)
                    res[cur] = target;
                    cur = next;
                    target = p[target];
                }
            }
        }
    }
}
