namespace IAFahim.DS.Grid.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class MakeGridTests
    {
        [Test]
        public void EmptyWidth_NoOp()
        {
            int* ptr = stackalloc int[0];
            Grid.MakeGrid.Run(ptr, 0, 0, 0);
        }

        [Test]
        public void SingleCell_FillsCorrectly()
        {
            int* ptr = stackalloc int[1];
            Grid.MakeGrid.Run(ptr, 1, 1, 1);
            Assert.AreEqual(0, ptr[0]);
        }

        [Test]
        public void TwoByTwo_FillsCorrectly()
        {
            int* ptr = stackalloc int[4];
            Grid.MakeGrid.Run(ptr, 4, 2, 2);
            Assert.AreEqual(0, ptr[0]);
            Assert.AreEqual(1, ptr[1]);
            Assert.AreEqual(2, ptr[2]);
            Assert.AreEqual(3, ptr[3]);
        }

        [Test]
        public void Rectangular_FillsCorrectly()
        {
            int* ptr = stackalloc int[6];
            Grid.MakeGrid.Run(ptr, 6, 3, 2);
            Assert.AreEqual(0, ptr[0]);
            Assert.AreEqual(1, ptr[1]);
            Assert.AreEqual(2, ptr[2]);
            Assert.AreEqual(3, ptr[3]);
            Assert.AreEqual(4, ptr[4]);
            Assert.AreEqual(5, ptr[5]);
        }
    }
}