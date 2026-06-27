namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class PermutationSqrt
    {
        // Find res (a permutation of 0..n-1) with res^2 == p, i.e. res[res[i]] == p[i] for all i.
        // Return true (writing res) if such a square root exists, else false.
        //
        // Squaring a single cycle C of length L:
        //   * L odd  -> C^2 is again ONE cycle of length L (the odd cycles of p each come from a
        //               same-length cycle of res, recoverable on their own).
        //   * L even -> C^2 SPLITS into two cycles of length L/2. So an even-length cycle of p can
        //               only appear as half of such a split, meaning even-length cycles of p must
        //               occur in equal-length PAIRS. A lone even-length cycle => no square root.
        //
        // Reconstruction:
        //   * odd cycle (a0 a1 ... a_{L-1}) of p: the root cycle has the same elements; advancing
        //     2 steps in the root equals 1 step in p, and since L is odd, stepping by 2 visits all
        //     L nodes. The root successor of a_j is a_{(j + (L+1)/2) mod L} (because 2*(L+1)/2 = L+1 ≡ 1).
        //   * even cycle pair A=(a0..a_{L-1}), B=(b0..b_{L-1}) of p: interleave into a length-2L root
        //     cycle a0 b0 a1 b1 ... so that two steps land on the next element of the same original cycle.
        //     res: a_j -> b_j, b_j -> a_{(j+1) mod L}.
        //
        // Scratch: pendingByLen[L] = a not-yet-matched even cycle's start awaiting a partner, indexed
        //          by length (size n+1). We also reuse res itself as the "visited" marker (seed -1).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int* p, int n, int* res)
        {
            if (n <= 0) return true;

            // res doubles as the visited marker during scanning; final values overwrite it.
            for (int i = 0; i < n; i++) res[i] = -1;

            // pendingByLen[L] = start of an even cycle of length L still waiting for a partner, or -1.
            int* pendingByLen = (int*)Marshal.AllocHGlobal(sizeof(int) * (n + 1));
            for (int i = 0; i <= n; i++) pendingByLen[i] = -1;

            // Pass 1: measure every cycle; build odd roots immediately; pair up even cycles.
            for (int start = 0; start < n; start++)
            {
                if (res[start] != -1) continue; // already consumed by an earlier cycle

                // Measure the cycle length, marking members as visited (res != -1) as we go.
                int len = 0;
                int node = start;
                do
                {
                    res[node] = -2;        // temporary "seen, value pending" marker
                    node = p[node];
                    len++;
                } while (node != start);

                if ((len & 1) != 0)
                {
                    // Odd cycle: build its root in place.
                    BuildOddRoot(p, start, len, res);
                }
                else
                {
                    // Even cycle: try to match with a previously pending one of the same length.
                    int partner = pendingByLen[len];
                    if (partner == -1)
                    {
                        pendingByLen[len] = start; // wait for a future partner
                    }
                    else
                    {
                        pendingByLen[len] = -1;
                        BuildEvenRoot(p, partner, start, len, res);
                    }
                }
            }

            // Any even-length cycle left unpaired => no square root exists.
            bool ok = true;
            for (int L = 1; L <= n; L++)
            {
                if (pendingByLen[L] != -1) { ok = false; break; }
            }
            Marshal.FreeHGlobal(new System.IntPtr((void*)pendingByLen));
            return ok;
        }

        // Odd cycle (length L) starting at 'start': res successor advances (L+1)/2 steps along p,
        // because 2*((L+1)/2) = L+1 ≡ 1 (mod L), so res^2 advances exactly one p-step.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildOddRoot(int* p, int start, int len, int* res)
        {
            int half = (len + 1) >> 1; // (L+1)/2 steps in p == 1 step in res
            int cur = start;
            for (int i = 0; i < len; i++)
            {
                // Successor of cur = node 'half' p-steps ahead of cur.
                int t = cur;
                for (int s = 0; s < half; s++) t = p[t];
                res[cur] = t;
                cur = p[cur];
            }
        }

        // Two equal-length (L) cycles A=start a, B=start b of p, interleaved into one 2L root cycle:
        //   res: a_j -> b_j, b_j -> a_{(j+1) mod L}.  Then res^2 advances each original cycle by one.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildEvenRoot(int* p, int a, int b, int len, int* res)
        {
            int ca = a;
            int cb = b;
            for (int j = 0; j < len; j++)
            {
                int nextA = p[ca]; // a_{j+1}
                res[ca] = cb;       // a_j -> b_j
                res[cb] = nextA;    // b_j -> a_{j+1}  (wraps to a_0 on the last step)
                ca = nextA;
                cb = p[cb];         // b_{j+1}
            }
        }
    }
}
