namespace IAFahim.Algebra.Tests
{
    using System;
    using IAFahim.Algebra.Polynomial;
    using IAFahim.Algebra.Sequence;
    using IAFahim.Algebra.GraphPoly;
    using NUnit.Framework;

    public sealed unsafe class AlgebraTests
    {
        const int MOD = 998244353;

        [Test]
        public void ToomCook_Multiply()
        {
            long* a = stackalloc long[4] { 1, 2, 3, 0 };
            long* b = stackalloc long[2] { 4, 5 };
            long* r = stackalloc long[8];
            ToomCook.Multiply(a, 4, b, 2, r, MOD);
            Assert.AreEqual(4, r[0]);
            Assert.AreEqual(13, r[1]);
            Assert.AreEqual(22, r[2]);
            Assert.AreEqual(15, r[3]);
        }

        [Test]
        public void RootFind_Quadratic()
        {
            long* poly = stackalloc long[3] { 6, 0, 1 };
            long* roots = stackalloc long[3];
            int c = RootFind.Find(poly, 3, 7, roots);
            Assert.IsTrue(c >= 1);
        }

        [Test]
        public void OfflineQuery_Eval()
        {
            long* poly = stackalloc long[3] { 1, 2, 3 };
            long v = OfflineQuery.MultiEval(poly, 3, 2, MOD);
            Assert.AreEqual(17, v);
        }

        [Test]
        public void Ntt_ThreePrime()
        {
            long* a = stackalloc long[4] { 1, 1, 0, 0 };
            long* b = stackalloc long[4] { 1, 1, 0, 0 };
            long* r = stackalloc long[8];
            Ntt.ThreePrime(a, b, r, 4);
            Assert.AreEqual(1, r[0]);
            Assert.AreEqual(2, r[1]);
            Assert.AreEqual(1, r[2]);
        }

        [Test]
        public void Stirling_SecondRow4()
        {
            long* row = stackalloc long[5];
            Stirling.SecondRow(4, MOD, row);
            Assert.AreEqual((long)0, row[0]);
            Assert.AreEqual((long)1, row[1]);
            Assert.AreEqual((long)7, row[2]);
            Assert.AreEqual((long)6, row[3]);
            Assert.AreEqual((long)1, row[4]);
        }

        [Test]
        public void Stirling_FirstRow3()
        {
            long* row = stackalloc long[4];
            Stirling.FirstRow(3, MOD, row);
            Assert.IsTrue(row[1] >= 0);
            Assert.IsTrue(row[2] >= 0);
            Assert.IsTrue(row[3] >= 0);
        }

        [Test]
        public void Stirling_S2_53()
        {
            long v = Stirling.Second(5, 3, MOD);
            Assert.AreEqual((long)25, v);
        }

        [Test]
        public void Combinatorial_Binom()
        {
            long v = Combinatorial.Binom(10, 3, MOD);
            Assert.AreEqual((long)120, v);
        }

        [Test]
        public void Combinatorial_Eulerian()
        {
            long v = Combinatorial.Eulerian(4, 1, MOD);
            Assert.AreEqual((long)11, v);
        }

        [Test]
        public void Combinatorial_Narayana()
        {
            long v = Combinatorial.Narayana(5, 2, MOD);
            Assert.IsTrue(v > 0);
        }

        [Test]
        public void Combinatorial_Lah()
        {
            long v = Combinatorial.Lah(5, 3, MOD);
            Assert.IsTrue(v > 0);
        }

        [Test]
        public void Combinatorial_YoungTableaux()
        {
            int* shape = stackalloc int[2] { 3, 2 };
            long v = Combinatorial.YoungTableaux(shape, 2, MOD);
            Assert.IsTrue(v > 0);
        }

        [Test]
        public void Combinatorial_QBinomial()
        {
            long v = Combinatorial.QBinomial(3, 1, 2, MOD);
            Assert.IsTrue(v > 0);
        }

        [Test]
        public void Transform_Binomial()
        {
            long* a = stackalloc long[5] { 1, 0, 0, 0, 0 };
            long* b = stackalloc long[5];
            Transform.Binomial(a, 5, MOD, b);
            Assert.AreEqual((long)1, b[0]);
            Assert.AreEqual((long)1, b[1]);
        }

        [Test]
        public void Transform_SetPartition()
        {
            long v = Transform.SetPartition(4, MOD);
            Assert.AreEqual((long)15, v);
        }

        [Test]
        public void Transform_Cayley()
        {
            long v = Transform.CayleyCount(4, MOD);
            Assert.AreEqual((long)16, v);
        }

        [Test]
        public void Prufer_Unrank()
        {
            int* seq = stackalloc int[3];
            Prufer.Unrank(0, 5, MOD, seq);
            Assert.AreEqual(0, seq[0]);
            Assert.AreEqual(0, seq[1]);
            Assert.AreEqual(0, seq[2]);
        }

        [Test]
        public void GeneratingFunction_TreeCount()
        {
            long v = GeneratingFunction.TreeCount(4, MOD);
            Assert.AreEqual((long)16, v);
        }

        [Test]
        public void Chromatic_PathNeeds2()
        {
            int n = 3;
            bool* adj = stackalloc bool[9];
            for (int i = 0; i < 9; i++) adj[i] = false;
            adj[0 * 3 + 1] = adj[1 * 3 + 0] = true;
            adj[1 * 3 + 2] = adj[2 * 3 + 1] = true;
            int chi = Chromatic.NumberDp(n, adj, MOD);
            Assert.IsTrue(chi >= 1);
        }

        [Test]
        public void Chromatic_TriangleNeeds3()
        {
            int n = 3;
            bool* adj = stackalloc bool[9];
            for (int i = 0; i < 9; i++) adj[i] = false;
            adj[0 * 3 + 1] = adj[1 * 3 + 0] = true;
            adj[1 * 3 + 2] = adj[2 * 3 + 1] = true;
            adj[0 * 3 + 2] = adj[2 * 3 + 0] = true;
            int chi = Chromatic.NumberDp(n, adj, MOD);
            Assert.IsTrue(chi >= 1);
        }

        [Test]
        public void Independence_Polynomial()
        {
            int n = 3;
            bool* adj = stackalloc bool[9];
            for (int i = 0; i < 9; i++) adj[i] = false;
            adj[0 * 3 + 1] = adj[1 * 3 + 0] = true;
            long v = Independence.Polynomial(n, adj, 1, MOD);
            Assert.IsTrue(v > 0);
        }

        [Test]
        public void Matching_Polynomial()
        {
            int n = 3;
            bool* adj = stackalloc bool[9];
            for (int i = 0; i < 9; i++) adj[i] = false;
            adj[0 * 3 + 1] = adj[1 * 3 + 0] = true;
            long v = Matching.Polynomial(n, adj, 1, MOD);
            Assert.IsTrue(v > 0);
        }

        [Test]
        public void Tutte_SingleEdge()
        {
            int* from = stackalloc int[1] { 0 };
            int* to = stackalloc int[1] { 1 };
            long v = Tutte.Subset(2, 1, from, to, 1, 1, MOD);
            Assert.IsTrue(v > 0);
        }
    }
}
