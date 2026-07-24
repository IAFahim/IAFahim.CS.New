namespace IAFahim.Optimization.Submodular.Tests
{
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
    }
}
