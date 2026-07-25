namespace IAFahim.Graph.Cut.Tests
{
    using NUnit.Framework;

    public sealed unsafe class StoerWagnerTests
    {
        [Test]
        public void Cycle3_MinCut2()
        {
            // triangle unit weights
            long* w = stackalloc long[9] {
                0,1,1,
                1,0,1,
                1,1,0
            };
            Assert.AreEqual(2, StoerWagner.MinCutValue(w, 3));
        }
    }
}
