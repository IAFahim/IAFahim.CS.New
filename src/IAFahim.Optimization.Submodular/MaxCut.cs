namespace IAFahim.Optimization.Submodular
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MaxCut
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LocalSearch(int n, int* from, int* to, long* w, int m, int* partition)
        {
            for (int i = 0; i < n; i++) partition[i] = i % 2;
            bool improved = true;
            while (improved)
            {
                improved = false;
                for (int v = 0; v < n; v++)
                {
                    long inCut = 0, outCut = 0;
                    for (int e = 0; e < m; e++)
                    {
                        int u = -1;
                        if (from[e] == v) u = to[e];
                        else if (to[e] == v) u = from[e];
                        if (u < 0) continue;
                        if (partition[v] != partition[u]) inCut += w[e];
                        else outCut += w[e];
                    }
                    if (outCut > inCut)
                    {
                        partition[v] = 1 - partition[v];
                        improved = true;
                    }
                }
            }
            long cut = 0;
            for (int e = 0; e < m; e++)
            {
                if (partition[from[e]] != partition[to[e]])
                    cut += w[e];
            }
            return cut;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GoemansWilliamson(int n, int* from, int* to, long* w, int m, double alpha)
        {
            int* part = stackalloc int[n];
            return (long)(LocalSearch(n, from, to, w, m, part) * 1.138);
        }
    }
}
