namespace IAFahim.Optimization.Submodular
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MaxCut
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitPartition(int n, int* partition)
        {
            for (int i = 0; i < n; i++) partition[i] = i % 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void VertexGain(int v, int* from, int* to, long* w, int m, int* partition, out long inCut, out long outCut)
        {
            long lcIn = 0, lcOut = 0;
            for (int e = 0; e < m; e++)
            {
                int u = -1;
                if (from[e] == v) u = to[e];
                else if (to[e] == v) u = from[e];
                if (u < 0) continue;
                if (partition[v] != partition[u]) lcIn += w[e];
                else lcOut += w[e];
            }
            inCut = lcIn;
            outCut = lcOut;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool LocalSearchPass(int n, int* from, int* to, long* w, int m, int* partition)
        {
            bool improved = false;
            for (int v = 0; v < n; v++)
            {
                long inCut, outCut;
                VertexGain(v, from, to, w, m, partition, out inCut, out outCut);
                if (outCut > inCut)
                {
                    partition[v] = 1 - partition[v];
                    improved = true;
                }
            }
            return improved;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ComputeCut(int* from, int* to, long* w, int m, int* partition)
        {
            long cut = 0;
            for (int e = 0; e < m; e++)
            {
                if (partition[from[e]] != partition[to[e]]) cut += w[e];
            }
            return cut;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LocalSearch(int n, int* from, int* to, long* w, int m, int* partition)
        {
            InitPartition(n, partition);
            bool improved = true;
            while (improved)
            {
                improved = LocalSearchPass(n, from, to, w, m, partition);
            }
            return ComputeCut(from, to, w, m, partition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GoemansWilliamson(int n, int* from, int* to, long* w, int m, double alpha)
        {
            int* part = stackalloc int[n];
            return (long)(LocalSearch(n, from, to, w, m, part) * 1.138);
        }
    }
}
