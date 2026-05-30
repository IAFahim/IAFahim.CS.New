namespace IAFahim.Algebra.GraphPoly.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class GraphPolyTests
    {
        private const int MOD = 1000000007;

        [Test]
        public void Chromatic_Triangle_ReturnsThree()
        {
            int n = 3;
            bool* adj = (bool*)Marshal.AllocHGlobal(n * n * sizeof(bool));
            try
            {
                for (int i = 0; i < n * n; i++) adj[i] = false;
                adj[0 * 3 + 1] = adj[1 * 3 + 0] = true;
                adj[1 * 3 + 2] = adj[2 * 3 + 1] = true;
                adj[2 * 3 + 0] = adj[0 * 3 + 2] = true;

                int ans = Chromatic.NumberDp(n, adj, MOD);
                Assert.AreEqual(3, ans);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)adj);
            }
        }

        [Test]
        public void Independence_Triangle_CorrectEvaluation()
        {
            int n = 3;
            bool* adj = (bool*)Marshal.AllocHGlobal(n * n * sizeof(bool));
            try
            {
                for (int i = 0; i < n * n; i++) adj[i] = false;
                adj[0 * 3 + 1] = adj[1 * 3 + 0] = true;
                adj[1 * 3 + 2] = adj[2 * 3 + 1] = true;
                adj[2 * 3 + 0] = adj[0 * 3 + 2] = true;

                // Independent sets:
                // Size 0: 1 (empty)
                // Size 1: 3 (singletons)
                // Polynomial: 1 + 3x
                // At x = 2: 1 + 6 = 7
                long ans = Independence.Polynomial(n, adj, 2, MOD);
                Assert.AreEqual(7, ans);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)adj);
            }
        }

        [Test]
        public void Matching_Triangle_CorrectEvaluation()
        {
            int n = 3;
            bool* adj = (bool*)Marshal.AllocHGlobal(n * n * sizeof(bool));
            try
            {
                for (int i = 0; i < n * n; i++) adj[i] = false;
                adj[0 * 3 + 1] = adj[1 * 3 + 0] = true;
                adj[1 * 3 + 2] = adj[2 * 3 + 1] = true;
                adj[2 * 3 + 0] = adj[0 * 3 + 2] = true;

                // Matchings:
                // Size 0: 1
                // Size 1: 3
                // Polynomial: 1 + 3x
                // At x = 2: 7
                long ans = Matching.Polynomial(n, adj, 2, MOD);
                Assert.AreEqual(7, ans);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)adj);
            }
        }

        [Test]
        public void Reliability_Triangle_CorrectProbability()
        {
            int n = 3;
            int edges = 3;
            int* from = (int*)Marshal.AllocHGlobal(edges * sizeof(int));
            int* to = (int*)Marshal.AllocHGlobal(edges * sizeof(int));
            try
            {
                from[0] = 0; to[0] = 1;
                from[1] = 1; to[1] = 2;
                from[2] = 2; to[2] = 0;

                // Failure probability p = 1/2 mod 10^9+7
                long p = 500000004;

                // Connecteds: 3 edges (prob 1/8), 2 edges (3 configs, each 1/8 = 3/8)
                // Total prob = 4/8 = 1/2
                long expected = 500000004;
                long ans = Reliability.Run(n, edges, from, to, p, MOD);
                Assert.AreEqual(expected, ans);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)from);
                Marshal.FreeHGlobal((nint)to);
            }
        }

        [Test]
        public void Rook_2x2_CorrectEvaluation()
        {
            int n = 2;
            int m = 2;
            bool* blocked = (bool*)Marshal.AllocHGlobal(n * m * sizeof(bool));
            try
            {
                for (int i = 0; i < n * m; i++) blocked[i] = false;

                // Rooks on 2x2:
                // 0 rooks: 1
                // 1 rook: 4
                // 2 rooks: 2
                // Poly: 1 + 4x + 2x^2
                // At x = 2: 1 + 8 + 8 = 17
                long ans = Rook.Run(n, m, blocked, 2, MOD);
                Assert.AreEqual(17, ans);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)blocked);
            }
        }

        [Test]
        public void Tutte_Triangle_CorrectEvaluation()
        {
            int n = 3;
            int edges = 3;
            int* from = (int*)Marshal.AllocHGlobal(edges * sizeof(int));
            int* to = (int*)Marshal.AllocHGlobal(edges * sizeof(int));
            try
            {
                from[0] = 0; to[0] = 1;
                from[1] = 1; to[1] = 2;
                from[2] = 2; to[2] = 0;

                // Sum formulation evaluated at (2, 3) gives 16 for K_3
                long ans = Tutte.Subset(n, edges, from, to, 2, 3, MOD);
                Assert.AreEqual(16, ans);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)from);
                Marshal.FreeHGlobal((nint)to);
            }
        }
    }
}
