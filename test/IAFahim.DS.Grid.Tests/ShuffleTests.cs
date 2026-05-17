namespace IAFahim.DS.Grid.Tests
{
    using Xunit;

    public sealed unsafe class ShuffleTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            int* ptr = stackalloc int[0];
            Grid.Shuffle.Run(ptr, 0, 42);
        }

        [Fact]
        public void SingleElement_Unchanged()
        {
            int val = 42;
            Grid.Shuffle.Run(&val, 1, 123);
            Assert.Equal(42, val);
        }

        [Fact]
        public void SameSeed_SameResult()
        {
            const int N = 16;
            int* ptr1 = stackalloc int[N];
            int* ptr2 = stackalloc int[N];
            for (int i = 0; i < N; i++)
            {
                ptr1[i] = i;
                ptr2[i] = i;
            }
            Grid.Shuffle.Run(ptr1, N, 42);
            Grid.Shuffle.Run(ptr2, N, 42);
            for (int i = 0; i < N; i++)
                Assert.Equal(ptr1[i], ptr2[i]);
        }

        [Fact]
        public void SameInput_DifferentSeeds_DifferentResults()
        {
            const int N = 32;
            int* ptr1 = stackalloc int[N];
            int* ptr2 = stackalloc int[N];
            for (int i = 0; i < N; i++)
            {
                ptr1[i] = i;
                ptr2[i] = i;
            }
            Grid.Shuffle.Run(ptr1, N, 100);
            Grid.Shuffle.Run(ptr2, N, 200);
            bool allSame = true;
            for (int i = 0; i < N; i++)
            {
                if (ptr1[i] != ptr2[i])
                {
                    allSame = false;
                    break;
                }
            }
            Assert.False(allSame);
        }

        [Fact]
        public void PreservesAllElements()
        {
            const int N = 64;
            int* ptr = stackalloc int[N];
            for (int i = 0; i < N; i++)
                ptr[i] = i;
            Grid.Shuffle.Run(ptr, N, 42);
            int[] found = new int[N];
            for (int i = 0; i < N; i++)
                found[ptr[i]]++;
            for (int i = 0; i < N; i++)
                Assert.Equal(1, found[i]);
        }
    }
}