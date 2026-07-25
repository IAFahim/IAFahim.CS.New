namespace IAFahim.Geometry.SweepPrune.Tests
{
    using NUnit.Framework;

    public sealed unsafe class SweepAndPruneTests
    {
        [Test]
        public void TwoOverlapping_OnePair()
        {
            double* minX = stackalloc double[2] { 0, 0.5 };
            double* maxX = stackalloc double[2] { 1, 1.5 };
            double* minY = stackalloc double[2] { 0, 0.5 };
            double* maxY = stackalloc double[2] { 1, 1.5 };
            int* a = stackalloc int[4];
            int* b = stackalloc int[4];
            int c = SweepAndPrune.FindOverlaps(minX, maxX, minY, maxY, 2, a, b, 4);
            Assert.AreEqual(1, c);
        }
    }
}
