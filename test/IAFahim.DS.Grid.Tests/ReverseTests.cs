namespace IAFahim.DS.Grid.Tests
{
    using Xunit;

    public sealed unsafe class ReverseTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            int* ptr = stackalloc int[0];
            Grid.Reverse.Run(ptr, 0);
        }

        [Fact]
        public void SingleElement_Unchanged()
        {
            int val = 42;
            Grid.Reverse.Run(&val, 1);
            Assert.Equal(42, val);
        }

        [Fact]
        public void TwoElements_Swaps()
        {
            int* ptr = stackalloc int[] { 1, 2 };
            Grid.Reverse.Run(ptr, 2);
            Assert.Equal(2, ptr[0]);
            Assert.Equal(1, ptr[1]);
        }

        [Fact]
        public void OddLength_ReversesCorrectly()
        {
            int* ptr = stackalloc int[] { 1, 2, 3 };
            Grid.Reverse.Run(ptr, 3);
            Assert.Equal(3, ptr[0]);
            Assert.Equal(2, ptr[1]);
            Assert.Equal(1, ptr[2]);
        }

        [Fact]
        public void EvenLength_ReversesCorrectly()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4 };
            Grid.Reverse.Run(ptr, 4);
            Assert.Equal(4, ptr[0]);
            Assert.Equal(3, ptr[1]);
            Assert.Equal(2, ptr[2]);
            Assert.Equal(1, ptr[3]);
        }

        [Fact]
        public void LargeN_ReversesCorrectly()
        {
            const int N = 64;
            int* ptr = stackalloc int[N];
            for (int i = 0; i < N; i++)
                ptr[i] = i;
            Grid.Reverse.Run(ptr, N);
            for (int i = 0; i < N; i++)
                Assert.Equal(N - 1 - i, ptr[i]);
        }
    }
}