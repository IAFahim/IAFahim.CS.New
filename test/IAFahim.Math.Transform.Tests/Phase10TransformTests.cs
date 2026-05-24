namespace IAFahim.Math.Transform.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.Math.Transform;
    using NUnit.Framework;

    public sealed unsafe class Phase10TransformTests
    {
        [Test]
        public void OrAndXorConvolution_Or_CorrectResult()
        {
            const int logN = 3;
            const int n = 1 << logN;
            const long mod = 998244353;
            long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* b = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* c = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* ta = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* tb = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            try
            {
                for (int i = 0; i < n; i++)
                {
                    a[i] = i + 1;
                    b[i] = i + 2;
                }
                OrAndXorConvolution.RunOr(a, b, c, logN, mod, ta, tb);
                long* expected = (long*)Marshal.AllocHGlobal(n * sizeof(long));
                try
                {
                    for (int i = 0; i < n; i++)
                    {
                        expected[i] = 0;
                    }
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            int idx = i | j;
                            expected[idx] = (expected[idx] + a[i] * b[j]) % mod;
                        }
                    }
                    for (int i = 0; i < n; i++)
                    {
                        Assert.AreEqual(expected[i], c[i]);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)expected);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
                Marshal.FreeHGlobal((nint)b);
                Marshal.FreeHGlobal((nint)c);
                Marshal.FreeHGlobal((nint)ta);
                Marshal.FreeHGlobal((nint)tb);
            }
        }

        [Test]
        public void OrAndXorConvolution_And_CorrectResult()
        {
            const int logN = 3;
            const int n = 1 << logN;
            const long mod = 998244353;
            long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* b = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* c = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* ta = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* tb = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            try
            {
                for (int i = 0; i < n; i++)
                {
                    a[i] = i + 1;
                    b[i] = i + 2;
                }
                OrAndXorConvolution.RunAnd(a, b, c, logN, mod, ta, tb);
                long* expected = (long*)Marshal.AllocHGlobal(n * sizeof(long));
                try
                {
                    for (int i = 0; i < n; i++)
                    {
                        expected[i] = 0;
                    }
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            int idx = i & j;
                            expected[idx] = (expected[idx] + a[i] * b[j]) % mod;
                        }
                    }
                    for (int i = 0; i < n; i++)
                    {
                        Assert.AreEqual(expected[i], c[i]);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)expected);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
                Marshal.FreeHGlobal((nint)b);
                Marshal.FreeHGlobal((nint)c);
                Marshal.FreeHGlobal((nint)ta);
                Marshal.FreeHGlobal((nint)tb);
            }
        }

        [Test]
        public void OrAndXorConvolution_Xor_CorrectResult()
        {
            const int logN = 3;
            const int n = 1 << logN;
            const long mod = 998244353;
            long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* b = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* c = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* ta = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* tb = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            try
            {
                for (int i = 0; i < n; i++)
                {
                    a[i] = i + 1;
                    b[i] = i + 2;
                }
                OrAndXorConvolution.RunXor(a, b, c, logN, mod, ta, tb);
                long* expected = (long*)Marshal.AllocHGlobal(n * sizeof(long));
                try
                {
                    for (int i = 0; i < n; i++)
                    {
                        expected[i] = 0;
                    }
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            int idx = i ^ j;
                            expected[idx] = (expected[idx] + a[i] * b[j]) % mod;
                        }
                    }
                    for (int i = 0; i < n; i++)
                    {
                        Assert.AreEqual(expected[i], c[i]);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)expected);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
                Marshal.FreeHGlobal((nint)b);
                Marshal.FreeHGlobal((nint)c);
                Marshal.FreeHGlobal((nint)ta);
                Marshal.FreeHGlobal((nint)tb);
            }
        }

        [Test]
        public void MinMaxConvolution_MinIndex_CorrectResult()
        {
            const int n = 5;
            const long mod = 998244353;
            long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* b = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* c = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* sa = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* sb = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* sc = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            try
            {
                for (int i = 0; i < n; i++)
                {
                    a[i] = i + 1;
                    b[i] = i + 2;
                }
                MinMaxConvolution.MinIndex(a, b, c, n, mod, sa, sb, sc);
                long* expected = (long*)Marshal.AllocHGlobal(n * sizeof(long));
                try
                {
                    for (int i = 0; i < n; i++)
                    {
                        expected[i] = 0;
                    }
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            int idx = Math.Min(i, j);
                            expected[idx] = (expected[idx] + a[i] * b[j]) % mod;
                        }
                    }
                    for (int i = 0; i < n; i++)
                    {
                        Assert.AreEqual(expected[i], c[i]);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)expected);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
                Marshal.FreeHGlobal((nint)b);
                Marshal.FreeHGlobal((nint)c);
                Marshal.FreeHGlobal((nint)sa);
                Marshal.FreeHGlobal((nint)sb);
                Marshal.FreeHGlobal((nint)sc);
            }
        }

        [Test]
        public void MinMaxConvolution_MaxIndex_CorrectResult()
        {
            const int n = 5;
            const long mod = 998244353;
            long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* b = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* c = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* pa = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* pb = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* pc = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            try
            {
                for (int i = 0; i < n; i++)
                {
                    a[i] = i + 1;
                    b[i] = i + 2;
                }
                MinMaxConvolution.MaxIndex(a, b, c, n, mod, pa, pb, pc);
                long* expected = (long*)Marshal.AllocHGlobal(n * sizeof(long));
                try
                {
                    for (int i = 0; i < n; i++)
                    {
                        expected[i] = 0;
                    }
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            int idx = Math.Max(i, j);
                            expected[idx] = (expected[idx] + a[i] * b[j]) % mod;
                        }
                    }
                    for (int i = 0; i < n; i++)
                    {
                        Assert.AreEqual(expected[i], c[i]);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)expected);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
                Marshal.FreeHGlobal((nint)b);
                Marshal.FreeHGlobal((nint)c);
                Marshal.FreeHGlobal((nint)pa);
                Marshal.FreeHGlobal((nint)pb);
                Marshal.FreeHGlobal((nint)pc);
            }
        }

        [Test]
        public void MinMaxConvolution_MinPlusConvexArbitrary_MatchesGeneral()
        {
            const int n = 5;
            const int m = 6;
            const int limit = n + m - 1;
            long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* b = (long*)Marshal.AllocHGlobal(m * sizeof(long));
            long* cGeneral = (long*)Marshal.AllocHGlobal(limit * sizeof(long));
            long* cConvex = (long*)Marshal.AllocHGlobal(limit * sizeof(long));
            try
            {
                // A is convex: differences are 1, 2, 3, 4
                a[0] = 0;
                a[1] = 1;
                a[2] = 3;
                a[3] = 6;
                a[4] = 10;
                for (int i = 0; i < m; i++)
                {
                    b[i] = (i - 3) * (i - 3);
                }
                MinMaxConvolution.MinPlusGeneral(a, n, b, m, cGeneral);
                MinMaxConvolution.MinPlusConvexArbitrary(a, n, b, m, cConvex);
                for (int i = 0; i < limit; i++)
                {
                    Assert.AreEqual(cGeneral[i], cConvex[i]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
                Marshal.FreeHGlobal((nint)b);
                Marshal.FreeHGlobal((nint)cGeneral);
                Marshal.FreeHGlobal((nint)cConvex);
            }
        }

        [Test]
        public void MinMaxConvolution_MinPlusConvexConvex_MatchesGeneral()
        {
            const int n = 5;
            const int m = 6;
            const int limit = n + m - 1;
            long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* b = (long*)Marshal.AllocHGlobal(m * sizeof(long));
            long* cGeneral = (long*)Marshal.AllocHGlobal(limit * sizeof(long));
            long* cConvex = (long*)Marshal.AllocHGlobal(limit * sizeof(long));
            try
            {
                a[0] = 0;
                a[1] = 1;
                a[2] = 3;
                a[3] = 6;
                a[4] = 10;
                b[0] = 0;
                b[1] = 2;
                b[2] = 5;
                b[3] = 9;
                b[4] = 14;
                b[5] = 20;
                MinMaxConvolution.MinPlusGeneral(a, n, b, m, cGeneral);
                MinMaxConvolution.MinPlusConvexConvex(a, n, b, m, cConvex);
                for (int i = 0; i < limit; i++)
                {
                    Assert.AreEqual(cGeneral[i], cConvex[i]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
                Marshal.FreeHGlobal((nint)b);
                Marshal.FreeHGlobal((nint)cGeneral);
                Marshal.FreeHGlobal((nint)cConvex);
            }
        }

        [Test]
        public void SubsetConvolutionRanked_Basic_CorrectResult()
        {
            const int logN = 3;
            const int n = 1 << logN;
            const long mod = 998244353;
            long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* b = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* c = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long totalSize = (long)(logN + 1) * n;
            long* f = (long*)Marshal.AllocHGlobal((nint)(totalSize * sizeof(long)));
            long* g = (long*)Marshal.AllocHGlobal((nint)(totalSize * sizeof(long)));
            long* h = (long*)Marshal.AllocHGlobal((nint)(totalSize * sizeof(long)));
            try
            {
                for (int i = 0; i < n; i++)
                {
                    a[i] = i + 1;
                    b[i] = i + 2;
                }
                SubsetConvolutionRanked.Run(a, b, c, logN, mod, f, g, h);
                long* expected = (long*)Marshal.AllocHGlobal(n * sizeof(long));
                try
                {
                    for (int i = 0; i < n; i++)
                    {
                        expected[i] = 0;
                    }
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if ((i & j) == 0)
                            {
                                int idx = i | j;
                                expected[idx] = (expected[idx] + a[i] * b[j]) % mod;
                            }
                        }
                    }
                    for (int i = 0; i < n; i++)
                    {
                        Assert.AreEqual(expected[i], c[i]);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)expected);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
                Marshal.FreeHGlobal((nint)b);
                Marshal.FreeHGlobal((nint)c);
                Marshal.FreeHGlobal((nint)f);
                Marshal.FreeHGlobal((nint)g);
                Marshal.FreeHGlobal((nint)h);
            }
        }

        [Test]
        public void PartitionConvolution_Convolve_CorrectResult()
        {
            const int n = 6;
            const long mod = 998244353;
            long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* c = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            long* backA = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            try
            {
                for (int i = 0; i < n; i++)
                {
                    a[i] = 0;
                }
                a[0] = 1;
                PartitionConvolution.ConvolveWithPartition(a, c, n, mod);
                // Partition function values should be: 1, 1, 2, 3, 5, 7
                Assert.AreEqual(1, c[0]);
                Assert.AreEqual(1, c[1]);
                Assert.AreEqual(2, c[2]);
                Assert.AreEqual(3, c[3]);
                Assert.AreEqual(5, c[4]);
                Assert.AreEqual(7, c[5]);

                PartitionConvolution.ConvolveWithPentagonal(c, backA, n, mod);
                for (int i = 0; i < n; i++)
                {
                    Assert.AreEqual(a[i], backA[i]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
                Marshal.FreeHGlobal((nint)c);
                Marshal.FreeHGlobal((nint)backA);
            }
        }

        [Test]
        public void PosetTransforms_Diamond_CorrectResult()
        {
            const int n = 4;
            const long mod = 998244353;
            int* topOrder = stackalloc int[n];
            bool* relation = stackalloc bool[n * n];
            long* f = stackalloc long[n];
            long* g = stackalloc long[n];
            long* backF = stackalloc long[n];

            // topological order: 0, 1, 2, 3
            for (int i = 0; i < n; i++)
            {
                topOrder[i] = i;
            }

            // relation matrix for diamond:
            // 0 <= 0, 1, 2, 3
            // 1 <= 1, 3
            // 2 <= 2, 3
            // 3 <= 3
            for (int i = 0; i < n * n; i++)
            {
                relation[i] = false;
            }
            for (int i = 0; i < n; i++)
            {
                relation[i * n + i] = true;
            }
            relation[0 * n + 1] = true;
            relation[0 * n + 2] = true;
            relation[0 * n + 3] = true;
            relation[1 * n + 3] = true;
            relation[2 * n + 3] = true;

            for (int i = 0; i < n; i++)
            {
                f[i] = i + 1; // f = [1, 2, 3, 4]
            }

            PosetTransforms.ZetaTransform(f, g, topOrder, relation, n, mod);
            // g[0] = f[0] = 1
            // g[1] = f[0] + f[1] = 3
            // g[2] = f[0] + f[2] = 4
            // g[3] = f[0] + f[1] + f[2] + f[3] = 10
            Assert.AreEqual(1, g[0]);
            Assert.AreEqual(3, g[1]);
            Assert.AreEqual(4, g[2]);
            Assert.AreEqual(10, g[3]);

            long* mu = stackalloc long[n * n];
            PosetTransforms.MobiusTransform(g, backF, topOrder, relation, n, mod, mu);
            for (int i = 0; i < n; i++)
            {
                Assert.AreEqual(f[i], backF[i]);
            }

            // Meet and Join check
            // Meet(1, 2) should be 0 (since 0 <= 1 and 0 <= 2)
            Assert.AreEqual(0, PosetTransforms.LatticeMeet(1, 2, relation, n));
            // Join(1, 2) should be 3 (since 1 <= 3 and 2 <= 3)
            Assert.AreEqual(3, PosetTransforms.LatticeJoin(1, 2, relation, n));

            // BooleanLatticeRank check
            Assert.AreEqual(0, PosetTransforms.BooleanLatticeRank(0));
            Assert.AreEqual(1, PosetTransforms.BooleanLatticeRank(1));
            Assert.AreEqual(1, PosetTransforms.BooleanLatticeRank(2));
            Assert.AreEqual(2, PosetTransforms.BooleanLatticeRank(3));
        }
    }
}
