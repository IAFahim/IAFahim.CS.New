namespace IAFahim.Optimization.Submodular.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class MaxCutTests
    {
        [Test]
        public void SingleEdge_LocalSearch()
        {
            int* from = stackalloc int[1];
            int* to = stackalloc int[1];
            long* w = stackalloc long[1];
            int* part = stackalloc int[2];
            from[0] = 0; to[0] = 1; w[0] = 5;
            part[0] = 0; part[1] = 0;
            long cut = MaxCut.LocalSearch(2, from, to, w, 1, part);
            Assert.IsTrue(cut >= 0 && cut <= 5);
            Assert.IsTrue(part[0] != part[1] || cut == 0);
        }

        [Test]
        public void GoemansWilliamson_SingleEdge()
        {
            int* from = stackalloc int[] { 0 };
            int* to = stackalloc int[] { 1 };
            long* w = stackalloc long[] { 4 };
            long cut = MaxCut.GoemansWilliamson(2, from, to, w, 1, 0.878);
            Assert.IsTrue(cut >= 0);
        }
    }

    public sealed unsafe class SubmodularGreedyTests
    {
        [Test]
        public void Run_PicksTopKGains()
        {
            const int n = 3;
            const int k = 2;
            long* gain = stackalloc long[k * n];
            gain[0] = 1; gain[1] = 5; gain[2] = 3;
            gain[3] = 4; gain[4] = 0; gain[5] = 2;
            int* selected = stackalloc int[k];
            long total = SubmodularGreedy.Run(n, gain, k, selected);
            Assert.AreEqual(9, total);
            Assert.AreEqual(1, selected[0]);
            Assert.AreEqual(0, selected[1]);
        }

        [Test]
        public void Run_LargeGains_ReturnsLongNotTruncated()
        {
            const int n = 1;
            const int k = 1;
            long* gain = stackalloc long[1];
            gain[0] = 3_000_000_000L;
            int* selected = stackalloc int[1];
            long total = SubmodularGreedy.Run(n, gain, k, selected);
            Assert.AreEqual(3_000_000_000L, total);
        }

        [Test]
        public void GreedySetCover_CoversAll()
        {
            int* counts = stackalloc int[] { 2, 2 };
            int* s0 = stackalloc int[] { 0, 1 };
            int* s1 = stackalloc int[] { 1, 2 };
            int** sets = stackalloc int*[2];
            sets[0] = s0;
            sets[1] = s1;
            int* cover = stackalloc int[2];
            long used = SubmodularGreedy.GreedySetCover(3, counts, sets, 2, cover);
            Assert.AreEqual(2, used);
        }
    }

    public sealed unsafe class RoundingTests
    {
        [Test]
        public void Pipage_RoundsNearest()
        {
            double* frac = stackalloc double[] { 0.2, 0.8 };
            int* result = stackalloc int[2];
            Rounding.Pipage(2, frac, result);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(1, result[1]);
        }

        [Test]
        public void Random_BinaryOutputs()
        {
            double* frac = stackalloc double[] { 0.0, 1.0 };
            int* result = stackalloc int[2];
            Rounding.Random(2, frac, result, new Random(1));
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(1, result[1]);
        }

        [Test]
        public void Dependent_SumRounded()
        {
            double* frac = stackalloc double[] { 0.5, 0.5, 0.5 };
            int* result = stackalloc int[3];
            Rounding.Dependent(3, frac, result);
            int sum = result[0] + result[1] + result[2];
            Assert.AreEqual(2, sum);
        }
    }
}
