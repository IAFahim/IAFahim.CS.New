namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class PermutationNthRoot
    {
        // Find res (a permutation of 0..n-1) with res^k == p, i.e. applying res k times equals p.
        // Returns true (writing res) if a k-th root exists, else false. Generalizes PermutationSqrt (k==2).
        //
        // Theory: raising a single res-cycle of length m to the power k splits it into exactly
        //   g = gcd(m, k) cycles, each of length m/g. So to BUILD a length-L cycle of p we must merge
        //   some number t of p's length-L cycles into one root cycle of length m = t*L, and that root
        //   cycle's k-th power must yield back exactly t cycles of length L. That requires
        //       g := gcd(m, k) == t   and   m / g == L   <=>   gcd(t*L, k) == t.
        //   The canonical merge count t for a length L is the least fixed point of t -> gcd(L*t, k)
        //   started at t = 1 (the iteration is non-decreasing and divides k, so it stabilizes). Every
        //   length-L cycle of p must join such a group, so feasibility requires (count of length-L
        //   cycles) % t == 0. If that holds for all lengths, a root exists.
        //
        // Merge construction: to fuse t cycles A_0..A_{t-1} (each length L) into a root cycle 'cyc' of
        //   length m = t*L, place A_x[y] at index (x + y*k) mod m, then res steps cyc[i] -> cyc[i+1].
        //   res^k advances the index by k, so A_x[y] -> A_x[y+1] (wraps because t | k), reproducing
        //   each original p-cycle; gcd(t*L,k)==t makes (x,y) -> (x + y*k) mod m a bijection.
        //
        // Scratch (all Allocator.Temp, freed): res seeded to -1 doubles as the visited marker;
        //   cycleStart[]/cycleLen[] record every p-cycle; group[] buffers the t starts of one merge.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int* p, int n, int k, int* res)
        {
            if (n <= 0) return true;
            if (k <= 0) return false;          // k-th root undefined for non-positive k
            if (k == 1)                         // res^1 == p  =>  res == p
            {
                for (int i = 0; i < n; i++) res[i] = p[i];
                return true;
            }

            for (int i = 0; i < n; i++) res[i] = -1; // -1 == not yet visited / assigned

            int* cycleStart = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            int* cycleLen = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            int* group = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            try
            {
                // Pass 1: enumerate every cycle of p, marking members visited (res != -1).
                int cycleCount = 0;
                for (int start = 0; start < n; start++)
                {
                    if (res[start] != -1) continue; // already inside an earlier cycle

                    int len = 0;
                    int node = start;
                    do
                    {
                        res[node] = -2;     // "seen, value pending"
                        node = p[node];
                        len++;
                    } while (node != start);

                    cycleStart[cycleCount] = start;
                    cycleLen[cycleCount] = len;
                    cycleCount++;
                }

                // For each distinct length L, gather its cycles and merge them in groups of t.
                // A length is "done" once we've consumed all its cycles; we mark consumed entries len=0.
                for (int ci = 0; ci < cycleCount; ci++)
                {
                    int L = cycleLen[ci];
                    if (L == 0) continue; // already consumed as part of an earlier length-group sweep

                    int t = MergeCount(L, k); // canonical number of length-L cycles per root cycle

                    // Collect the t starts of one group, sweeping forward over remaining length-L cycles.
                    int filled = 0;
                    for (int cj = ci; cj < cycleCount; cj++)
                    {
                        if (cycleLen[cj] != L) continue;
                        cycleLen[cj] = 0;          // consume
                        group[filled++] = cycleStart[cj];
                        if (filled == t)
                        {
                            BuildMergedRoot(p, group, t, L, k, res);
                            filled = 0;
                        }
                    }

                    // Leftover cycles that couldn't form a full group => no k-th root exists.
                    if (filled != 0) return false;
                }

                return true;
            }
            finally
            {
                Marshal.FreeHGlobal(new System.IntPtr((void*)group));
                Marshal.FreeHGlobal(new System.IntPtr((void*)cycleLen));
                Marshal.FreeHGlobal(new System.IntPtr((void*)cycleStart));
            }
        }

        // Least fixed point of t -> gcd(L*t, k) starting at t = 1: the number of length-L p-cycles
        // that merge into one root cycle of length t*L. Guaranteed to stabilize (non-decreasing, t | k).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int MergeCount(int L, int k)
        {
            int t = 1;
            while (true)
            {
                int g = Gcd((int)(((long)L * t) % k), k); // gcd(L*t, k); reduce L*t mod k first (gcd-safe)
                if (g == t) return t;
                t = g;
            }
        }

        // Merge t cycles (starts in group[0..t-1], each length L) of p into one root cycle of length
        // m = t*L by placing A_x[y] at index (x + y*k) mod m, then linking cyc[i] -> cyc[(i+1) mod m].
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildMergedRoot(int* p, int* group, int t, int L, int k, int* res)
        {
            long m = (long)t * L;
            int* cyc = (int*)Marshal.AllocHGlobal(sizeof(int) * (int)m);
            try
            {
                long kr = k % m; // step is taken mod m; kr in [0, m)
                for (int x = 0; x < t; x++)
                {
                    int node = group[x];
                    long pos = x;          // y = 0 -> index x
                    for (int y = 0; y < L; y++)
                    {
                        cyc[pos] = node;
                        node = p[node];
                        pos += kr;
                        if (pos >= m) pos -= m;
                    }
                }

                for (long i = 0; i < m; i++)
                {
                    long ni = i + 1;
                    if (ni == m) ni = 0;
                    res[cyc[i]] = cyc[ni];
                }
            }
            finally
            {
                Marshal.FreeHGlobal(new System.IntPtr((void*)cyc));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Gcd(int a, int b)
        {
            while (b != 0)
            {
                int r = a % b;
                a = b;
                b = r;
            }
            return a;
        }
    }
}
