namespace IAFahim.Graph.Clique
{
    using System.Runtime.CompilerServices;

    public static unsafe class BronKerbosch
    {
        // Enumerates maximal cliques via classic Bron–Kerbosch with pivoting.
        // Graph: undirected adjacency matrix adj[i*n+j] nonzero => edge.
        // Writes each maximal clique as a bitset into outCliques[k] (bit i set if vertex i in clique).
        // Returns number of maximal cliques written (capped by outCap).
        public static int EnumerateMaximal(byte* adj, int n, ulong* outCliques, int outCap)
        {
            if (n <= 0 || outCap <= 0) return 0;
            if (n > 64) return 0;

            ulong full = n == 64 ? ~0UL : (1UL << n) - 1;
            int count = 0;
            Search(adj, n, 0UL, full, 0UL, outCliques, outCap, ref count);
            return count;
        }

        // Maximum clique size (Bron–Kerbosch with pruning by remaining candidates).
        public static int MaximumSize(byte* adj, int n)
        {
            if (n <= 0) return 0;
            if (n > 64) return 0;
            ulong full = n == 64 ? ~0UL : (1UL << n) - 1;
            int best = 0;
            MaxSearch(adj, n, 0UL, full, ref best);
            return best;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Neighbors(byte* adj, int n, int v)
        {
            ulong m = 0;
            for (int j = 0; j < n; j++)
                if (adj[v * n + j] != 0) m |= 1UL << j;
            return m;
        }

        private static void Search(byte* adj, int n, ulong r, ulong p, ulong x, ulong* outCliques, int outCap, ref int count)
        {
            if (p == 0 && x == 0)
            {
                if (count < outCap) outCliques[count++] = r;
                return;
            }
            ulong uMask = p | x;
            int u = -1;
            for (int i = 0; i < n; i++)
                if (((uMask >> i) & 1UL) != 0) { u = i; break; }
            ulong pivotN = u >= 0 ? Neighbors(adj, n, u) : 0UL;
            ulong candidates = p & ~pivotN;
            for (int v = 0; v < n; v++)
            {
                if (((candidates >> v) & 1UL) == 0) continue;
                ulong nv = Neighbors(adj, n, v);
                Search(adj, n, r | (1UL << v), p & nv, x & nv, outCliques, outCap, ref count);
                p &= ~(1UL << v);
                x |= 1UL << v;
            }
        }

        private static void MaxSearch(byte* adj, int n, ulong r, ulong p, ref int best)
        {
            int rSize = PopCount(r);
            if (rSize + PopCount(p) <= best) return;
            if (p == 0)
            {
                if (rSize > best) best = rSize;
                return;
            }
            for (int v = 0; v < n; v++)
            {
                if (((p >> v) & 1UL) == 0) continue;
                ulong nv = Neighbors(adj, n, v);
                MaxSearch(adj, n, r | (1UL << v), p & nv, ref best);
                p &= ~(1UL << v);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PopCount(ulong x)
        {
            int c = 0;
            while (x != 0) { c++; x &= x - 1; }
            return c;
        }
    }
}
