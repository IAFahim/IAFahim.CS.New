namespace IAFahim.DS.Grid.Tests
{
    using NUnit.Framework;

    public sealed unsafe class RotateTests
    {
        [Test]
        public void OneByOne_Clockwise_NoChange()
        {
            int* ptr = stackalloc int[] { 42 };
            int* temp = stackalloc int[1];
            IAFahim.DS.Grid.Rotate.Run(ptr, 1, 1, true, temp);
            Assert.AreEqual(42, ptr[0]);
        }

        [Test]
        public void TwoByTwo_Clockwise_Rotates()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4 };
            int* temp = stackalloc int[4];
            IAFahim.DS.Grid.Rotate.Run(ptr, 2, 2, true, temp);
            Assert.AreEqual(3, ptr[0]);
            Assert.AreEqual(1, ptr[1]);
            Assert.AreEqual(4, ptr[2]);
            Assert.AreEqual(2, ptr[3]);
        }

        [Test]
        public void TwoByTwo_CounterClockwise_Rotates()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4 };
            int* temp = stackalloc int[4];
            IAFahim.DS.Grid.Rotate.Run(ptr, 2, 2, false, temp);
            Assert.AreEqual(2, ptr[0]);
            Assert.AreEqual(4, ptr[1]);
            Assert.AreEqual(1, ptr[2]);
            Assert.AreEqual(3, ptr[3]);
        }

        [Test]
        public void ThreeByTwo_Clockwise_Rotates()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4, 5, 6 };
            int* temp = stackalloc int[6];
            IAFahim.DS.Grid.Rotate.Run(ptr, 3, 2, true, temp);
            Assert.AreEqual(4, ptr[0]);
            Assert.AreEqual(1, ptr[1]);
            Assert.AreEqual(5, ptr[2]);
            Assert.AreEqual(2, ptr[3]);
            Assert.AreEqual(6, ptr[4]);
            Assert.AreEqual(3, ptr[5]);
        }

        [Test]
        public void OneByThree_Clockwise_NoChange()
        {
            int* ptr = stackalloc int[] { 1, 2, 3 };
            int* temp = stackalloc int[3];
            IAFahim.DS.Grid.Rotate.Run(ptr, 3, 1, true, temp);
            Assert.AreEqual(1, ptr[0]);
            Assert.AreEqual(2, ptr[1]);
            Assert.AreEqual(3, ptr[2]);
        }
    }
}