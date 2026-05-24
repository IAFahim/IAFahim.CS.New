namespace IAFahim.DS.Grid.Tests
{
    using Xunit;

    public sealed unsafe class RotateTests
    {
        [Fact]
        public void OneByOne_Clockwise_NoChange()
        {
            int* ptr = stackalloc int[] { 42 };
            int* temp = stackalloc int[1];
            IAFahim.DS.Grid.Rotate.Run(ptr, 1, 1, true, temp);
            Assert.Equal(42, ptr[0]);
        }

        [Fact]
        public void TwoByTwo_Clockwise_Rotates()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4 };
            int* temp = stackalloc int[4];
            IAFahim.DS.Grid.Rotate.Run(ptr, 2, 2, true, temp);
            Assert.Equal(3, ptr[0]);
            Assert.Equal(1, ptr[1]);
            Assert.Equal(4, ptr[2]);
            Assert.Equal(2, ptr[3]);
        }

        [Fact]
        public void TwoByTwo_CounterClockwise_Rotates()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4 };
            int* temp = stackalloc int[4];
            IAFahim.DS.Grid.Rotate.Run(ptr, 2, 2, false, temp);
            Assert.Equal(2, ptr[0]);
            Assert.Equal(4, ptr[1]);
            Assert.Equal(1, ptr[2]);
            Assert.Equal(3, ptr[3]);
        }

        [Fact]
        public void ThreeByTwo_Clockwise_Rotates()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4, 5, 6 };
            int* temp = stackalloc int[6];
            IAFahim.DS.Grid.Rotate.Run(ptr, 3, 2, true, temp);
            Assert.Equal(4, ptr[0]);
            Assert.Equal(1, ptr[1]);
            Assert.Equal(5, ptr[2]);
            Assert.Equal(2, ptr[3]);
            Assert.Equal(6, ptr[4]);
            Assert.Equal(3, ptr[5]);
        }

        [Fact]
        public void OneByThree_Clockwise_NoChange()
        {
            int* ptr = stackalloc int[] { 1, 2, 3 };
            int* temp = stackalloc int[3];
            IAFahim.DS.Grid.Rotate.Run(ptr, 3, 1, true, temp);
            Assert.Equal(1, ptr[0]);
            Assert.Equal(2, ptr[1]);
            Assert.Equal(3, ptr[2]);
        }
    }
}