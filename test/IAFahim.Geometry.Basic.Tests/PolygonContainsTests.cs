namespace IAFahim.Geometry.Basic.Tests
{
    using NUnit.Framework;

    public sealed unsafe class PolygonContainsTests
    {
        [Test]
        public void Square_InsideAndOutside()
        {
            long* x = stackalloc long[] { 0, 4, 4, 0 };
            long* y = stackalloc long[] { 0, 0, 4, 4 };
            Assert.AreEqual(1, PolygonContains.Run(4, x, y, 2, 2));
            Assert.AreEqual(0, PolygonContains.Run(4, x, y, 5, 5));
            Assert.AreEqual(0, PolygonContains.Run(4, x, y, -1, 2));
        }

        [Test]
        public void Triangle_DownwardEdgeCrossing()
        {
            long* x = stackalloc long[] { 0, 6, 3 };
            long* y = stackalloc long[] { 0, 0, 6 };
            Assert.AreEqual(1, PolygonContains.Run(3, x, y, 3, 2));
            Assert.AreEqual(0, PolygonContains.Run(3, x, y, 5, 5));
        }
    }
}
