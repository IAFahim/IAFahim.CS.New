namespace IAFahim.Optimization.Matroid.Tests
{
    using NUnit.Framework;

    public sealed unsafe class MatroidGreedyTests
    {
        private static bool AlwaysIndependent(int* set, int setSize, int elem) => true;

        [Test]
        public void AllIndependent_SumsPositiveWeights()
        {
            const int N = 3;
            int* set = stackalloc int[N];
            long* w = stackalloc long[N];
            w[0] = 5; w[1] = 3; w[2] = 4;
            long ans = MatroidGreedy.Run(N, set, 0, w, &AlwaysIndependent);
            Assert.AreEqual(12, ans);
        }

        [Test]
        public void NegativeWeights_Skipped()
        {
            const int N = 2;
            int* set = stackalloc int[N];
            long* w = stackalloc long[N];
            w[0] = -1; w[1] = -2;
            long ans = MatroidGreedy.Run(N, set, 0, w, &AlwaysIndependent);
            Assert.AreEqual(0, ans);
        }

        [Test]
        public void MixedWeights_OnlyPositiveSummed()
        {
            const int N = 4;
            int* set = stackalloc int[N];
            long* w = stackalloc long[N];
            w[0] = 10; w[1] = -5; w[2] = 3; w[3] = 0;
            long ans = MatroidGreedy.Run(N, set, 0, w, &AlwaysIndependent);
            Assert.AreEqual(13, ans);
        }
    }

    public sealed unsafe class LinearMatroidTests
    {
        [Test]
        public void Rank_IdentityRows()
        {
            int* a = stackalloc int[] { 1, 0, 0, 1 };
            int* basis = stackalloc int[2];
            int rank = LinearMatroid.Rank(2, 2, a, basis);
            Assert.AreEqual(2, rank);
        }

        [Test]
        public void Rank_DuplicateRows()
        {
            int* a = stackalloc int[] { 1, 1, 1, 1 };
            int* basis = stackalloc int[2];
            int rank = LinearMatroid.Rank(2, 2, a, basis);
            Assert.AreEqual(1, rank);
        }
    }
}
