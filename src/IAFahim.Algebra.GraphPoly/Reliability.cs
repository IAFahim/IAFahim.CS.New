namespace IAFahim.Algebra.GraphPoly
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Reliability
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsConnected(int n, int edges, int* from, int* to, int mask, int* parent)
        {
            for (int i = 0; i < n; i++) parent[i] = i;
            for (int e = 0; e < edges; e++)
            {
                if ((mask & (1 << e)) == 0) continue;
                int u = from[e], v = to[e];
                while (parent[u] != u) u = parent[u];
                while (parent[v] != v) v = parent[v];
                if (u != v) parent[u] = v;
            }
            for (int i = 0; i < n; i++)
            {
                int r = i;
                while (parent[r] != r) r = parent[r];
                parent[i] = r;
            }
            for (int i = 1; i < n; i++)
                if (parent[i] != parent[0]) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, int edges, int* from, int* to, long p, int MOD)
        {
            long result = 0L;
            int size = 1 << edges;
            int* parent = stackalloc int[n];
            for (int mask = 0; mask < size; mask++)
            {
                if (IsConnected(n, edges, from, to, mask, parent))
                {
                    int edgeCount = 0;
                    int m = mask;
                    while (m > 0) { if ((m & 1) != 0) edgeCount++; m >>= 1; }
                    long prob = CalculateProbability(edges, edgeCount, p, MOD);
                    result = (result + prob) % (long)MOD;
                }
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long CalculateProbability(int totalEdges, int edgeCount, long p, int MOD)
        {
            long pk = 1L;
            for (int i = 0; i < edgeCount; i++) pk = (pk * p) % (long)MOD;
            long qk = 1L;
            long q = (1L - p + (long)MOD) % (long)MOD;
            for (int i = 0; i < totalEdges - edgeCount; i++) qk = (qk * q) % (long)MOD;
            return (pk * qk) % (long)MOD;
        }
    }
}