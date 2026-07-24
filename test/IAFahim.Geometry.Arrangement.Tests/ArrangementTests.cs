namespace IAFahim.Geometry.Arrangement.Tests
{
    using NUnit.Framework;

    public sealed unsafe class VerticalDecompositionTests
    {
        [Test]
        public void SortByX()
        {
            int* xs = stackalloc int[3];
            int* ys = stackalloc int[3];
            int* ox = stackalloc int[3];
            int* oy = stackalloc int[3];
            xs[0] = 5; ys[0] = 1;
            xs[1] = 1; ys[1] = 2;
            xs[2] = 3; ys[2] = 3;
            int n = VerticalDecomposition.Run(xs, ys, 3, ox, oy);
            Assert.AreEqual(3, n);
            Assert.AreEqual(1, ox[0]);
            Assert.AreEqual(3, ox[1]);
            Assert.AreEqual(5, ox[2]);
        }
    }

    public sealed unsafe class PointLocationQueryTests
    {
        [Test]
        public void InCell()
        {
            int* grid = stackalloc int[4];
            grid[0] = 10; grid[1] = 11; grid[2] = 12; grid[3] = 13;
            int id = PointLocationQuery.Run(grid, 2, 0, 0, 10, 10, 5, 5);
            Assert.AreEqual(10, id);
        }
    }
}
