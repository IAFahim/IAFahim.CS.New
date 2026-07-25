namespace IAFahim.Graph.Cut
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class StoerWagner
    {
        // Global min-cut value on undirected weighted graph given as dense n x n matrix w (symmetric).
        // Weights must be non-negative. Returns cut value.
        public static long MinCutValue(long* w, int n)
        {
            if (n <= 1) return 0;
            long* mat = (long*)Marshal.AllocHGlobal((nint)((long)n * n * sizeof(long)));
            for (int i = 0; i < n * n; i++) mat[i] = w[i];

            int* merged = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            for (int i = 0; i < n; i++) merged[i] = i;

            long best = long.MaxValue;
            int nn = n;
            long* weight = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            byte* added = (byte*)Marshal.AllocHGlobal(n);

            while (nn > 1)
            {
                for (int i = 0; i < nn; i++) { weight[i] = 0; added[i] = 0; }
                int prev = -1;
                for (int phase = 0; phase < nn; phase++)
                {
                    int sel = -1;
                    long bestW = -1;
                    for (int i = 0; i < nn; i++)
                    {
                        if (added[i] == 0 && weight[i] > bestW) { bestW = weight[i]; sel = i; }
                    }
                    if (sel < 0) break;
                    added[sel] = 1;
                    if (phase == nn - 1)
                    {
                        if (bestW < best) best = bestW;
                        // Merge sel into prev
                        for (int i = 0; i < nn; i++)
                        {
                            if (i == prev || i == sel) continue;
                            mat[prev * n + i] += mat[sel * n + i];
                            mat[i * n + prev] = mat[prev * n + i];
                        }
                        // Remove sel by swapping with last
                        int last = nn - 1;
                        if (sel != last)
                        {
                            for (int i = 0; i < nn; i++)
                            {
                                long t = mat[sel * n + i]; mat[sel * n + i] = mat[last * n + i]; mat[last * n + i] = t;
                            }
                            for (int i = 0; i < n; i++)
                            {
                                long t = mat[i * n + sel]; mat[i * n + sel] = mat[i * n + last]; mat[i * n + last] = t;
                            }
                            if (prev == last) prev = sel;
                        }
                        nn--;
                        break;
                    }
                    prev = sel;
                    for (int i = 0; i < nn; i++)
                        if (added[i] == 0) weight[i] += mat[sel * n + i];
                }
            }

            Marshal.FreeHGlobal((nint)added);
            Marshal.FreeHGlobal((nint)weight);
            Marshal.FreeHGlobal((nint)merged);
            Marshal.FreeHGlobal((nint)mat);
            return best == long.MaxValue ? 0 : best;
        }
    }
}
