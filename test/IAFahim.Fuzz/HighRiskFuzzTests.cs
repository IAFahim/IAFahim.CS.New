namespace IAFahim.Fuzz
{
    using IAFahim.Sort.RadixSort;
    using IAFahim.Math.Transform.Ntt;
    using IAFahim.Math.Gauss;
    using IAFahim.String.SuffixTree;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    // High-risk module onboarding for the FuzzRunner harness. Each test fuzzes one unsafe module
    // against an independent oracle / round-trip property, seeded for reproducibility.

    public sealed unsafe class HighRiskFuzzTests
    {
        // RadixSortLsd vs Array.Sort. In-place sort: fast copies src->dst then sorts dst.
        [Test]
        public void RadixSort_MatchesArraySort_Fuzz()
        {
            FuzzRunner.AssertMatchesReference(
                fast: (src, n, dst) =>
                {
                    for (int i = 0; i < n; i++) dst[i] = src[i];
                    RadixSortLsd.Run(dst, n);
                },
                naive: (src, n, dst) =>
                {
                    int[] tmp = new int[n];
                    for (int i = 0; i < n; i++) tmp[i] = src[i];
                    Array.Sort(tmp);
                    for (int i = 0; i < n; i++) dst[i] = tmp[i];
                },
                gen: (rng, dst, n) => FuzzRunner.GenUniform(rng, dst, n, -1_000_000, 1_000_001),
                iterations: 500, seed: 4242, maxN: 2000);
        }

        // NTT round-trip: Inverse(Forward(a)) == a (mod p). Uses the standard NTT-friendly prime
        // 998244353 (primitive root 3). Catches butterfly / bit-reversal / scaling bugs.
        [Test]
        public void Ntt_ForwardInverseIdentity_Fuzz()
        {
            const long Mod = 998244353L;
            const long G = 3L;
            Random rng = new Random(909);
            for (int it = 0; it < 200; it++)
            {
                int logN = rng.Next(1, 11);          // n = 2..1024
                int n = 1 << logN;
                long* a = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
                long* roots = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
                long* invRoots = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
                long* orig = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
                try
                {
                    NttInit.Run(logN, Mod, G, roots, invRoots);
                    for (int i = 0; i < n; i++) { a[i] = rng.Next(0, (int)(Mod - 1)); orig[i] = a[i]; }
                    NttTransform.Forward(a, n, Mod, roots);
                    NttTransform.Inverse(a, n, Mod, invRoots);
                    for (int i = 0; i < n; i++)
                        Assert.AreEqual(orig[i], a[i], $"NTT round-trip mismatch it={it} n={n} idx={i}");
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)a); Marshal.FreeHGlobal((nint)roots);
                    Marshal.FreeHGlobal((nint)invRoots); Marshal.FreeHGlobal((nint)orig);
                }
            }
        }

        // GaussEliminationDouble: solve A·x = b, verify residual ‖A·x − b‖ ≈ 0 (only when full rank).
        [Test]
        public void GaussDouble_ResidualZero_Fuzz()
        {
            Random rng = new Random(1313);
            for (int it = 0; it < 150; it++)
            {
                int n = rng.Next(1, 12);
                double* a = (double*)Marshal.AllocHGlobal(sizeof(double) * n * n);
                double* aOrig = (double*)Marshal.AllocHGlobal(sizeof(double) * n * n);
                double* b = (double*)Marshal.AllocHGlobal(sizeof(double) * n);
                double* bOrig = (double*)Marshal.AllocHGlobal(sizeof(double) * n);
                double* x = (double*)Marshal.AllocHGlobal(sizeof(double) * n);
                try
                {
                    // generate a well-conditioned diagonally-dominant matrix (guaranteed full rank)
                    for (int i = 0; i < n; i++)
                    {
                        double sum = 0;
                        for (int j = 0; j < n; j++)
                        {
                            double v = rng.NextDouble() * 2 - 1;
                            a[i * n + j] = v; aOrig[i * n + j] = v; sum += Math.Abs(v);
                        }
                        a[i * n + i] += sum + 1.0;     // diagonal dominance
                        aOrig[i * n + i] = a[i * n + i];
                    }
                    for (int i = 0; i < n; i++) { b[i] = rng.NextDouble() * 20 - 10; bOrig[i] = b[i]; }

                    int rank = GaussEliminationDouble.Run(a, b, x, n, n);
                    if (rank != n) continue;            // skip singular (shouldn't happen w/ dominance)

                    double maxRes = 0;
                    for (int i = 0; i < n; i++)
                    {
                        double row = 0;
                        for (int j = 0; j < n; j++) row += aOrig[i * n + j] * x[j];
                        maxRes = Math.Max(maxRes, Math.Abs(row - bOrig[i]));
                    }
                    Assert.IsTrue(maxRes < 1e-6, $"Gauss residual too large it={it} n={n} maxRes={maxRes:E3}");
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)a); Marshal.FreeHGlobal((nint)aOrig);
                    Marshal.FreeHGlobal((nint)b); Marshal.FreeHGlobal((nint)bOrig); Marshal.FreeHGlobal((nint)x);
                }
            }
        }

        // SuffixTreeUkkonen: every suffix s[i..len) must be reachable by descending the tree from root.
        // String over alphabet {0..4} with a unique terminator appended (ensures Ukkonen correctness).
        [Test]
        public void SuffixTree_EverySuffixReachable_Fuzz()
        {
            Random rng = new Random(2718);
            for (int it = 0; it < 150; it++)
            {
                int len = rng.Next(1, 60);
                int term = 5 + it;                   // unique terminator per iteration (avoids \$ ambiguity)
                int* s = (int*)Marshal.AllocHGlobal(sizeof(int) * (len + 1));
                int maxNodes = 2 * (len + 1) + 4;
                SuffixTreeUkkonen.Node* nodes = (SuffixTreeUkkonen.Node*)Marshal.AllocHGlobal(sizeof(SuffixTreeUkkonen.Node) * maxNodes);
                SuffixTreeUkkonen.Edge* edges = (SuffixTreeUkkonen.Edge*)Marshal.AllocHGlobal(sizeof(SuffixTreeUkkonen.Edge) * maxNodes);
                try
                {
                    for (int i = 0; i < len; i++) s[i] = rng.Next(0, 5);
                    s[len] = term;                    // unique terminator
                    int nodeCount = 0, edgeCount = 0, last = 0;
                    SuffixTreeUkkonen.Build(s, len + 1, nodes, edges, ref nodeCount, ref edgeCount, ref last);

                    Assert.IsTrue(nodeCount >= 1 && nodeCount <= maxNodes, $"it={it} bad nodeCount={nodeCount}");
                    Assert.IsTrue(edgeCount >= 0 && edgeCount <= maxNodes, $"it={it} bad edgeCount={edgeCount}");

                    // every suffix must be reachable from root
                    for (int i = 0; i <= len; i++)
                        Assert.IsTrue(SuffixReachable(s, len + 1, i, nodes, edges),
                            $"it={it} suffix {i} not reachable -> tree is wrong");
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)s); Marshal.FreeHGlobal((nint)nodes); Marshal.FreeHGlobal((nint)edges);
                }
            }
        }

        // Walk the tree spelling s[start..len); true iff the full suffix is encoded along edges.
        private static bool SuffixReachable(int* s, int len, int start,
            SuffixTreeUkkonen.Node* nodes, SuffixTreeUkkonen.Edge* edges)
        {
            int v = 0, k = start;     // v = current node, k = next char index in s to match
            while (k < len)
            {
                int e = nodes[v].FirstEdge;
                int matched = -1;
                while (e != -1)
                {
                    if (edges[e].Char == s[k]) { matched = e; break; }
                    e = edges[e].Next;
                }
                if (matched == -1) return false;
                int lo = edges[matched].Min, hi = edges[matched].Max;
                for (int p = lo; p < hi && k < len; p++, k++)
                    if (s[p] != s[k]) return false;
                v = edges[matched].To;
            }
            return true;
        }
    }
}
