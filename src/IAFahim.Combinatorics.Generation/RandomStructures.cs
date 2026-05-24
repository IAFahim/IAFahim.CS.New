namespace IAFahim.Combinatorics.Generation;

using System;
using System.Runtime.CompilerServices;

    public static unsafe class RandomStructures
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RandomPermutation(int n, int* a, ref uint seed)
        {
            for (int i = 0; i < n; i++) a[i] = i;
            for (int i = n - 1; i > 0; i--) Swap(a, i, (int)(XorShift(ref seed) % (uint)(i + 1)));
        }

        private static uint XorShift(ref uint state) { state ^= state << 13; state ^= state >> 17; state ^= state << 5; return state; }
        private static void Swap(int* a, int i, int j) { int t = a[i]; a[i] = a[j]; a[j] = t; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RandomTreePrufer(int n, int* prufer, ref uint seed)
        {
            if (n <= 2) return;
            for (int i = 0; i < n - 2; i++) prufer[i] = (int)(XorShift(ref seed) % (uint)n);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RandomConnectedGraph(int n, int m, int* outFrom, int* outTo, ref uint seed)
        {
            m = Math.Min(m, (int)Math.Min((long)n * (n - 1) / 2, int.MaxValue));
            if (m < n - 1) m = n - 1;

            int edgeCount = BuildInitialTree(n, outFrom, outTo, ref seed);
            while (edgeCount < m) TryAddRandomEdge(n, outFrom, outTo, ref edgeCount, ref seed);
        }

        private static int BuildInitialTree(int n, int* outFrom, int* outTo, ref uint seed)
        {
            int* perm = stackalloc int[n]; RandomPermutation(n, perm, ref seed);
            int ec = 0;
            for (int i = 1; i < n; i++)
            {
                int v = (int)(XorShift(ref seed) % (uint)i);
                outFrom[ec] = perm[i]; outTo[ec++] = perm[v];
            }
            return ec;
        }

        private static void TryAddRandomEdge(int n, int* outFrom, int* outTo, ref int ec, ref uint seed)
        {
            int u = (int)(XorShift(ref seed) % (uint)n), v = (int)(XorShift(ref seed) % (uint)n);
            if (u != v && !EdgeExists(u, v, outFrom, outTo, ec)) { outFrom[ec] = u; outTo[ec++] = v; }
        }

        private static bool EdgeExists(int u, int v, int* outFrom, int* outTo, int count)
        {
            for (int k = 0; k < count; k++)
                if ((outFrom[k] == u && outTo[k] == v) || (outFrom[k] == v && outTo[k] == u)) return true;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RandomDAG(int n, int m, int* outFrom, int* outTo, ref uint seed)
        {
            m = Math.Min(m, (int)Math.Min((long)n * (n - 1) / 2, int.MaxValue));
            int ec = 0;
            while (ec < m)
            {
                int u = (int)(XorShift(ref seed) % (uint)n), v = (int)(XorShift(ref seed) % (uint)n);
                if (u < v && !EdgeExists(u, v, outFrom, outTo, ec)) { outFrom[ec] = u; outTo[ec++] = v; }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RandomErdosRenyi(int n, double p, int* outFrom, int* outTo, ref uint seed, int* edgeCount)
        {
            *edgeCount = 0; uint threshold = (uint)(p * uint.MaxValue);
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    if (XorShift(ref seed) < threshold) { outFrom[*edgeCount] = i; outTo[(*edgeCount)++] = j; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RandomBipartiteGraph(int n1, int n2, int m, int* outFrom, int* outTo, ref uint seed)
        {
            int ec = 0;
            while (ec < m)
            {
                int u = (int)(XorShift(ref seed) % (uint)n1), v = (int)(XorShift(ref seed) % (uint)n2);
                if (!EdgeExists(u, v + n1, outFrom, outTo, ec)) { outFrom[ec] = u; outTo[ec++] = v + n1; }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RandomRegular(int n, int d, int* outFrom, int* outTo, ref uint seed, int* edgeCount)
        {
            *edgeCount = 0; if (n * d % 2 != 0) return;
            int total = n * d; int* pts = stackalloc int[total];
            for (int i = 0; i < total; i++) pts[i] = i / d;
            ShufflePoints(pts, total, ref seed);
            for (int i = 0; i < total; i += 2)
            {
                int u = pts[i], v = pts[i + 1];
                if (u != v && !EdgeExists(u, v, outFrom, outTo, *edgeCount)) { outFrom[*edgeCount] = u; outTo[(*edgeCount)++] = v; }
            }
        }

        private static void ShufflePoints(int* points, int n, ref uint seed)
        {
            for (int i = n - 1; i > 0; i--) Swap(points, i, (int)(XorShift(ref seed) % (uint)(i + 1)));
        }
    }
