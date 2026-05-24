namespace IAFahim.DS.Grid.Tests
{
    using NUnit.Framework;

    public sealed unsafe class FillSliceTests
    {
        [Test]
        public void EmptyInput_NoOp()
        {
            int* ptr = stackalloc int[0];
            IAFahim.DS.Grid.FillSlice.Run(ptr, 0, 0, 0, 42);
        }

        [Test]
        public void SingleElement_FillsCorrectly()
        {
            int* ptr = stackalloc int[] { 0, 0, 0 };
            IAFahim.DS.Grid.FillSlice.Run(ptr, 3, 1, 2, 99);
            Assert.AreEqual(0, ptr[0]);
            Assert.AreEqual(99, ptr[1]);
            Assert.AreEqual(0, ptr[2]);
        }

        [Test]
        public void FullRange_FillsAll()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4 };
            IAFahim.DS.Grid.FillSlice.Run(ptr, 4, 0, 4, 7);
            for (int i = 0; i < 4; i++)
                Assert.AreEqual(7, ptr[i]);
        }

        [Test]
        public void PartialRange_FillsCorrectRange()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4, 5 };
            IAFahim.DS.Grid.FillSlice.Run(ptr, 5, 1, 4, 8);
            Assert.AreEqual(1, ptr[0]);
            Assert.AreEqual(8, ptr[1]);
            Assert.AreEqual(8, ptr[2]);
            Assert.AreEqual(8, ptr[3]);
            Assert.AreEqual(5, ptr[4]);
        }

        [Test]
        public void StartBeyondLength_NoOp()
        {
            int* ptr = stackalloc int[] { 1, 2, 3 };
            IAFahim.DS.Grid.FillSlice.Run(ptr, 3, 5, 8, 42);
            for (int i = 0; i < 3; i++)
                Assert.AreEqual(i + 1, ptr[i]);
        }

        [Test]
        public void StartEqualsEnd_NoOp()
        {
            int* ptr = stackalloc int[] { 1, 2, 3 };
            IAFahim.DS.Grid.FillSlice.Run(ptr, 3, 2, 2, 42);
            for (int i = 0; i < 3; i++)
                Assert.AreEqual(i + 1, ptr[i]);
        }
    }
}