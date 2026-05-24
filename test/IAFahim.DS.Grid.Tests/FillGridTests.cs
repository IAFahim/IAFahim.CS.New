namespace IAFahim.DS.Grid.Tests
{
    using NUnit.Framework;

    public sealed unsafe class FillGridTests
    {
        [Test]
        public void EmptyGrid_NoOp()
        {
            int* ptr = stackalloc int[0];
            IAFahim.DS.Grid.FillGrid.Run(ptr, 0, 0, 42);
        }

        [Test]
        public void SingleCell_FillsCorrectly()
        {
            int* ptr = stackalloc int[1];
            IAFahim.DS.Grid.FillGrid.Run(ptr, 1, 1, 99);
            Assert.AreEqual(99, ptr[0]);
        }

        [Test]
        public void TwoByTwo_FillsAll()
        {
            int* ptr = stackalloc int[4];
            IAFahim.DS.Grid.FillGrid.Run(ptr, 2, 2, 7);
            for (int i = 0; i < 4; i++)
                Assert.AreEqual(7, ptr[i]);
        }

        [Test]
        public void Rectangular_FillsAll()
        {
            int* ptr = stackalloc int[6];
            IAFahim.DS.Grid.FillGrid.Run(ptr, 3, 2, 5);
            for (int i = 0; i < 6; i++)
                Assert.AreEqual(5, ptr[i]);
        }
    }

    public sealed unsafe class FillGridXYTests
    {
        [Test]
        public void SingleCell_GetsValue()
        {
            int* ptr = stackalloc int[1];
            IAFahim.DS.Grid.FillGrid.RunXY(ptr, 1, 1, 99, new FillXYConst(99));
            Assert.AreEqual(99, ptr[0]);
        }

        [Test]
        public void TwoByTwo_GetsCorrectValues()
        {
            int* ptr = stackalloc int[4];
            IAFahim.DS.Grid.FillGrid.RunXY(ptr, 2, 2, 0, new FillXYLinear(2));
            Assert.AreEqual(0, ptr[0]);
            Assert.AreEqual(1, ptr[1]);
            Assert.AreEqual(2, ptr[2]);
            Assert.AreEqual(3, ptr[3]);
        }

        private struct FillXYConst : IAFahim.DS.Grid.IFillGridXY<int>
        {
            private readonly int _value;

            public FillXYConst(int value) => _value = value;
            public int Get(int x, int y) => _value;
        }

        private struct FillXYLinear : IAFahim.DS.Grid.IFillGridXY<int>
        {
            private readonly int _width;

            public FillXYLinear(int width) => _width = width;
            public int Get(int x, int y) => y * _width + x;
        }
    }
}