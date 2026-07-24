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
    }
}
