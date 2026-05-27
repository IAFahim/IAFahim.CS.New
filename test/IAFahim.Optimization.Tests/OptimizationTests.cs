namespace IAFahim.Optimization.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.Optimization.Exact;
    using IAFahim.Optimization.Treewidth;
    using IAFahim.Optimization.DivideConquer;
    using IAFahim.Optimization.Knapsack;
    using IAFahim.Optimization.Approximation;
    using IAFahim.Optimization.Matroid;
    using IAFahim.Optimization.Submodular;
    using IAFahim.Optimization.Geometric;
    using IAFahim.Optimization.Games;
    using NUnit.Framework;

    public sealed unsafe class OptimizationTests
    {
        [Test]
        public void HamiltonianPath_ThreeNodes_Finds()
        {
            int n = 3;
            long inf = 1000000;
            long* w = stackalloc long[9];
            for (int i = 0; i < 9; i++) w[i] = inf;
            w[0 * 3 + 1] = 1; w[1 * 3 + 2] = 2; w[2 * 3 + 0] = 3;
            long* dp = stackalloc long[8 * 3];
            int* perm = stackalloc int[3];
            long result = IAFahim.Optimization.Exact.HamiltonianPath.Run(n, w, inf, dp, perm);
            Assert.AreEqual(3, result);
        }

        [Test]
        public void HamiltonianCycle_ThreeNodes_Finds()
        {
            int n = 3;
            long inf = 1000000;
            long* w = stackalloc long[9];
            for (int i = 0; i < 9; i++) w[i] = inf;
            w[0 * 3 + 1] = 1; w[1 * 3 + 2] = 2; w[2 * 3 + 0] = 3;
            long* dp = stackalloc long[8 * 3];
            long result = IAFahim.Optimization.Exact.HamiltonianCycle.Run(n, w, inf, dp);
            Assert.AreEqual(6, result);
        }

        [Test]
        public void MaxIndependentSet_Triangle()
        {
            int n = 3;
            bool* adj = stackalloc bool[9];
            for (int i = 0; i < 9; i++) adj[i] = false;
            adj[0 * 3 + 1] = adj[1 * 3 + 0] = true;
            adj[1 * 3 + 2] = adj[2 * 3 + 1] = true;
            adj[0 * 3 + 2] = adj[2 * 3 + 0] = true;
            int* used = stackalloc int[3];
            int* best = stackalloc int[1];
            int* tmp = stackalloc int[3];
            int result = IAFahim.Optimization.Exact.MaxIndependentSet.Run(n, adj, used, best, tmp);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void MaxIndependentSet_Path()
        {
            int n = 3;
            bool* adj = stackalloc bool[9];
            for (int i = 0; i < 9; i++) adj[i] = false;
            adj[0 * 3 + 1] = adj[1 * 3 + 0] = true;
            adj[1 * 3 + 2] = adj[2 * 3 + 1] = true;
            int* used = stackalloc int[3];
            int* best = stackalloc int[1];
            int* tmp = stackalloc int[3];
            int result = IAFahim.Optimization.Exact.MaxIndependentSet.Run(n, adj, used, best, tmp);
            Assert.AreEqual(2, result);
        }

        [Test]
        public void SubsetSum_CanFind()
        {
            long* w = stackalloc long[4];
            w[0] = 1; w[1] = 3; w[2] = 5; w[3] = 7;
            Assert.IsTrue(IAFahim.Optimization.Knapsack.SubsetSum.Can(w, 4, 8));
        }

        [Test]
        public void SubsetSum_CannotFind()
        {
            long* w = stackalloc long[4];
            w[0] = 1; w[1] = 3; w[2] = 5; w[3] = 7;
            Assert.IsFalse(IAFahim.Optimization.Knapsack.SubsetSum.Can(w, 4, 2));
        }

        [Test]
        public void MeetInMiddle_SmallKnapsack()
        {
            long* w = stackalloc long[4];
            w[0] = 1; w[1] = 2; w[2] = 3; w[3] = 4;
            long* v = stackalloc long[4];
            v[0] = 10; v[1] = 20; v[2] = 30; v[3] = 40;
            long* left = stackalloc long[1 << (4 / 2) * 2];
            long result = IAFahim.Optimization.Knapsack.MeetInMiddle.Run(w, v, 4, 5, left);
            Assert.AreEqual(50, result);
        }

        [Test]
        public void BoundedKnapsack_BinarySplit()
        {
            long* w = stackalloc long[2];
            w[0] = 2; w[1] = 3;
            long* v = stackalloc long[2];
            v[0] = 3; v[1] = 5;
            int* cnt = stackalloc int[2];
            cnt[0] = 3; cnt[1] = 2;
            long* dp = stackalloc long[11];
            long result = IAFahim.Optimization.Knapsack.BoundedKnapsack.BinarySplit(w, v, cnt, 2, 10, dp);
            Assert.IsTrue(result > 0);
        }

        [Test]
        public void KSum_CountTwoSum()
        {
            int* a = stackalloc int[4];
            a[0] = 1; a[1] = 2; a[2] = 3; a[3] = 4;
            int count = IAFahim.Optimization.Knapsack.KSum.Count(a, 4, 2, 5);
            Assert.AreEqual(2, count);
        }

        [Test]
        public void MinEnclosingBall_ThreePoints()
        {
            double* xs = stackalloc double[3];
            double* ys = stackalloc double[3];
            xs[0] = 0; ys[0] = 0;
            xs[1] = 1; ys[1] = 0;
            xs[2] = 0.5; ys[2] = 1;
            int* p = stackalloc int[3];
            MinEnclosingBall.Circle c = IAFahim.Optimization.Geometric.MinEnclosingBall.Welzl(xs, ys, 3, p);
            Assert.IsTrue(c.R > 0);
            Assert.IsTrue(c.R < 2);
        }

        [Test]
        public void Grundy_Mex()
        {
            int* vals = stackalloc int[3];
            vals[0] = 0; vals[1] = 1; vals[2] = 3;
            int m = IAFahim.Optimization.Games.Grundy.Mex(vals, 3);
            Assert.AreEqual(2, m);
        }

        [Test]
        public void Grundy_MexZero()
        {
            int* vals = stackalloc int[3];
            vals[0] = 1; vals[1] = 2; vals[2] = 3;
            int m = IAFahim.Optimization.Games.Grundy.Mex(vals, 3);
            Assert.AreEqual(0, m);
        }

        [Test]
        public void ConvexHull_CheckMonge()
        {
            int m = 2, n = 2;
            long* a = stackalloc long[4];
            a[0] = 1; a[1] = 2; a[2] = 3; a[3] = 5;
            Assert.IsFalse(IAFahim.Optimization.Treewidth.ConvexHull.CheckMonge(a, m, n));
        }

        [Test]
        public void ConvexHull_CheckIsMonge()
        {
            int m = 2, n = 2;
            long* a = stackalloc long[4];
            a[0] = 1; a[1] = 3; a[2] = 2; a[3] = 4;
            Assert.IsTrue(IAFahim.Optimization.Treewidth.ConvexHull.CheckMonge(a, m, n));
        }

        [Test]
        public void MatrixSearch_Found()
        {
            int* a = stackalloc int[4];
            a[0] = 1; a[1] = 2; a[2] = 3; a[3] = 4;
            int idx = IAFahim.Optimization.DivideConquer.MatrixSearch.Run(2, 2, a, 3);
            Assert.AreEqual(2, idx);
        }

        [Test]
        public void MatrixSearch_NotFound()
        {
            int* a = stackalloc int[4];
            a[0] = 1; a[1] = 2; a[2] = 3; a[3] = 4;
            int idx = IAFahim.Optimization.DivideConquer.MatrixSearch.Run(2, 2, a, 99);
            Assert.AreEqual(-1, idx);
        }

        [Test]
        public void MaxCut_LocalSearch()
        {
            int n = 4, m = 4;
            int* from = stackalloc int[4];
            int* to = stackalloc int[4];
            long* w = stackalloc long[4];
            from[0] = 0; to[0] = 1; w[0] = 1;
            from[1] = 1; to[1] = 2; w[1] = 1;
            from[2] = 2; to[2] = 3; w[2] = 1;
            from[3] = 3; to[3] = 0; w[3] = 1;
            int* part = stackalloc int[4];
            long cut = IAFahim.Optimization.Submodular.MaxCut.LocalSearch(n, from, to, w, m, part);
            Assert.IsTrue(cut >= 1);
        }

        [Test]
        public void Mdp_ValueIteration()
        {
            int n = 2, m = 2;
            double* trans = stackalloc double[8];
            trans[0] = 0.5; trans[1] = 0.5; trans[2] = 0.5; trans[3] = 0.5;
            trans[4] = 0.5; trans[5] = 0.5; trans[6] = 0.5; trans[7] = 0.5;
            double* reward = stackalloc double[4];
            reward[0] = 1; reward[1] = 0; reward[2] = 0; reward[3] = 1;
            double* v = stackalloc double[2];
            v[0] = 0; v[1] = 0;
            double* newV = stackalloc double[2];
            IAFahim.Optimization.Games.Mdp.ValueIteration(n, m, trans, reward, 0.9, v, newV, 100);
            Assert.IsTrue(v[0] > 0);
            Assert.IsTrue(v[1] > 0);
        }

        [Test]
        public void SlopeTrick_AddAbs()
        {
            IAFahim.Optimization.DivideConquer.SlopeTrick.State s;
            IAFahim.Optimization.DivideConquer.SlopeTrick.Init(&s);
            IAFahim.Optimization.DivideConquer.SlopeTrick.AddAbs(&s, 5);
            long val = IAFahim.Optimization.DivideConquer.SlopeTrick.Query(&s);
            Assert.IsTrue(val >= 0);
        }

        [Test]
        public void LagrangianRelaxation_BasicSearch()
        {
            long* w = stackalloc long[5];
            w[0] = 10; w[1] = 20; w[2] = 30; w[3] = 40; w[4] = 50;
            long result = IAFahim.Optimization.DivideConquer.LagrangianRelaxation.Search(w, 5, 2, 0, 100);
            Assert.IsTrue(result >= 0);
        }

        [Test]
        public void LinearMatroid_Rank()
        {
            int n = 3, m = 3;
            int* a = stackalloc int[9];
            a[0] = 1; a[1] = 0; a[2] = 0;
            a[3] = 0; a[4] = 1; a[5] = 0;
            a[6] = 0; a[7] = 0; a[8] = 1;
            int* basis = stackalloc int[3];
            int r = IAFahim.Optimization.Matroid.LinearMatroid.Rank(n, m, a, basis);
            Assert.AreEqual(3, r);
        }
    }
}
