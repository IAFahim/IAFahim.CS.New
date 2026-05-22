namespace IAFahim.Algebra.GraphPoly
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Reliability
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Polynomial(int n, int edges, int* from, int* to, long p, int MOD)
        {
            long result = 0;
            int size = 1 << edges;
            int* parent = stackalloc int[n];
            for (int mask = 0; mask < size; mask++)
            {
                for (int i = 0; i < n; i++) parent[i] = i;
                int edgeCount = 0;
                for (int e = 0; e < edges; e++)
                {
                    if ((mask & (1 << e)) == 0) continue;
                    edgeCount++;
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
                bool connected = true;
                for (int i = 1; i < n; i++)
                    if (parent[i] != parent[0]) { connected = false; break; }
                if (connected)
                {
                    long pk = 1;
                    for (int i = 0; i < edgeCount; i++) pk = pk * p % MOD;
                    long qk = 1;
                    for (int i = 0; i < edges - edgeCount; i++) qk = qk * (1 - p + MOD) % MOD;
                    result = (result + pk * qk) % MOD;
                }
            }
            return result;
        }
    }
}