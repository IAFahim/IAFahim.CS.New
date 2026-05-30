namespace IAFahim.Algebra.Tests
{
    using System;
    using System.Diagnostics;
    using IAFahim.Algebra.Polynomial;
    using IAFahim.Algebra.Sequence;
    using IAFahim.Algebra.GraphPoly;
    using NUnit.Framework;

    public sealed unsafe class GraphPolyHardTests
    {
        const int MOD = 998244353;

        [Test]
        public void Chromatic_Path3_Needs3()
        {
            int n = 3;
            bool* adj = stackalloc bool[n * n];
            for (int i = 0; i < n * n; i++) adj[i] = false;
            adj[0 * n + 1] = adj[1 * n + 0] = true;
            adj[1 * n + 2] = adj[2 * n + 1] = true;

            int chi = Chromatic.NumberDp(n, adj, MOD);
            Assert.AreEqual(3, chi, "P3 path needs 3 colors");
        }

        [Test]
        public void Chromatic_K4()
        {
            int n = 4;
            bool* adj = stackalloc bool[n * n];
            for (int i = 0; i < n * n; i++) adj[i] = false;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    adj[i * n + j] = adj[j * n + i] = true;

            int chi = Chromatic.NumberDp(n, adj, MOD);
            Assert.AreEqual(4, chi, "K4 needs 4 colors");
        }

        [Test]
        public void Chromatic_C5()
        {
            int n = 5;
            bool* adj = stackalloc bool[n * n];
            for (int i = 0; i < n * n; i++) adj[i] = false;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                adj[i * n + j] = adj[j * n + i] = true;
            }

            int chi = Chromatic.NumberDp(n, adj, MOD);
            Assert.AreEqual(3, chi, "C5 needs 3 colors");
        }

        [Test]
        public void Chromatic_Star5()
        {
            int n = 5;
            bool* adj = stackalloc bool[n * n];
            for (int i = 0; i < n * n; i++) adj[i] = false;
            for (int i = 1; i < n; i++)
                adj[0 * n + i] = adj[i * n + 0] = true;

            int chi = Chromatic.NumberDp(n, adj, MOD);
            Assert.AreEqual(2, chi, "Star K1,4 needs 2 colors");
        }

        [Test]
        public void Chromatic_Subset_Coefficients_NonZero()
        {
            int n = 3;
            bool* adj = stackalloc bool[n * n];
            for (int i = 0; i < n * n; i++) adj[i] = false;
            adj[0 * n + 1] = adj[1 * n + 0] = true;
            adj[1 * n + 2] = adj[2 * n + 1] = true;

            long* coeffs = stackalloc long[n + 1];
            Chromatic.Subset(n, adj, MOD, coeffs);

            long sum = 0L;
            for (int k = 0; k <= n; k++) sum = (sum + coeffs[k]) % MOD;
            Assert.AreNotEqual(0, sum, "Coefficients should not all be zero");
        }

        [Test]
        public void Matching_Triangle_K3()
        {
            int n = 3;
            bool* adj = stackalloc bool[n * n];
            for (int i = 0; i < n * n; i++) adj[i] = false;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    adj[i * n + j] = adj[j * n + i] = true;

            long v1 = Matching.Polynomial(n, adj, 1, MOD);
            Assert.AreEqual(4L, v1, "K3: empty + 3 single edges = 4");
        }

        [Test]
        public void Matching_Path3()
        {
            int n = 3;
            bool* adj = stackalloc bool[n * n];
            for (int i = 0; i < n * n; i++) adj[i] = false;
            adj[0 * n + 1] = adj[1 * n + 0] = true;
            adj[1 * n + 2] = adj[2 * n + 1] = true;

            long v = Matching.Polynomial(n, adj, 1, MOD);
            Assert.IsTrue(v > 0, "Path 3 matching poly positive at x=1");
        }

        [Test]
        public void Tutte_SingleEdge()
        {
            int n = 2;
            int edges = 1;
            int* from = stackalloc int[edges];
            int* to = stackalloc int[edges];
            from[0] = 0; to[0] = 1;

            long v = Tutte.Subset(n, edges, from, to, 1, 1, MOD);
            Assert.IsTrue(v > 0, "Single edge Tutte positive");
        }

        [Test]
        public void Combinatorial_Eulerian_Known()
        {
            Assert.AreEqual(11L, Combinatorial.Eulerian(4, 1, MOD));
            Assert.AreEqual(11L, Combinatorial.Eulerian(4, 3, MOD));
            Assert.AreEqual(1L, Combinatorial.Eulerian(4, 0, MOD));
            Assert.AreEqual(1L, Combinatorial.Eulerian(4, 4, MOD));
        }

        [Test]
        public void Stirling_SecondRow_Known()
        {
            int n = 5;
            long* row = stackalloc long[n + 1];
            Stirling.SecondRow(n, MOD, row);

            Assert.AreEqual(0L, row[0]);
            Assert.AreEqual(1L, row[1]);
            Assert.AreEqual(15L, row[2]);
            Assert.AreEqual(25L, row[3]);
            Assert.AreEqual(10L, row[4]);
            Assert.AreEqual(1L, row[5]);
        }

        [Test]
        public void Combinatorial_Binom_Known()
        {
            Assert.AreEqual(120L, Combinatorial.Binom(10, 3, MOD));
            Assert.AreEqual(1L, Combinatorial.Binom(5, 0, MOD));
            Assert.AreEqual(1L, Combinatorial.Binom(5, 5, MOD));
            Assert.AreEqual(5L, Combinatorial.Binom(5, 1, MOD));
            Assert.AreEqual(10L, Combinatorial.Binom(5, 2, MOD));
        }

        [Test]
        public void Independence_K4()
        {
            int n = 4;
            bool* adj = stackalloc bool[n * n];
            for (int i = 0; i < n * n; i++) adj[i] = false;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    adj[i * n + j] = adj[j * n + i] = true;

            long v1 = Independence.Polynomial(n, adj, 1, MOD);
            Assert.AreEqual(5L, v1, "K4 at x=1: empty + 4 singletons = 5");
        }

        [Test]
        public void Independence_Path3()
        {
            int n = 3;
            bool* adj = stackalloc bool[n * n];
            for (int i = 0; i < n * n; i++) adj[i] = false;
            adj[0 * n + 1] = adj[1 * n + 0] = true;
            adj[1 * n + 2] = adj[2 * n + 1] = true;

            long v1 = Independence.Polynomial(n, adj, 1, MOD);
            Assert.IsTrue(v1 > 0, "Path 3 independence positive at x=1");
        }

        [Test]
        public void Chromatic_SpeedTest_N10()
        {
            int n = 10;
            bool* adj = stackalloc bool[n * n];
            for (int i = 0; i < n * n; i++) adj[i] = false;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    if ((i + j) % 3 == 0)
                        adj[i * n + j] = adj[j * n + i] = true;

            var sw = Stopwatch.StartNew();
            int chi = Chromatic.NumberDp(n, adj, MOD);
            sw.Stop();

            Console.WriteLine("Chromatic n=10: " + chi + " colors, " + sw.ElapsedTicks + " ticks");
            Assert.IsTrue(sw.ElapsedMilliseconds < 5000, "Should complete within 5 seconds");
        }

        [Test]
        public void Matching_SpeedTest_N10()
        {
            int n = 10;
            bool* adj = stackalloc bool[n * n];
            for (int i = 0; i < n * n; i++) adj[i] = false;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    if ((i * j) % 5 == 0)
                        adj[i * n + j] = adj[j * n + i] = true;

            var sw = Stopwatch.StartNew();
            long v = Matching.Polynomial(n, adj, 1, MOD);
            sw.Stop();

            Console.WriteLine("Matching n=10: result=" + v + ", " + sw.ElapsedTicks + " ticks");
            Assert.IsTrue(sw.ElapsedMilliseconds < 5000, "Should complete within 5 seconds");
        }

        [Test]
        public void Tutte_SpeedTest_E8()
        {
            int n = 5;
            int edges = 8;
            int* from = stackalloc int[edges];
            int* to = stackalloc int[edges];

            from[0] = 0; to[0] = 1;
            from[1] = 1; to[1] = 2;
            from[2] = 2; to[2] = 0;
            from[3] = 0; to[3] = 3;
            from[4] = 1; to[4] = 3;
            from[5] = 2; to[5] = 3;
            from[6] = 3; to[6] = 4;
            from[7] = 4; to[7] = 0;

            var sw = Stopwatch.StartNew();
            long v = Tutte.Subset(n, edges, from, to, 1, 1, MOD);
            sw.Stop();

            Console.WriteLine("Tutte e=8: result=" + v + ", " + sw.ElapsedTicks + " ticks");
            Assert.IsTrue(sw.ElapsedMilliseconds < 5000, "Should complete within 5 seconds");
        }
    }
}
