namespace IAFahim.DS.Grid.Tests
{
    using NUnit.Framework;

    public sealed unsafe class ReverseTests
    {
        [Test]
        public void EmptyInput_NoOp()
        {
            int* ptr = stackalloc int[0];
            Grid.Reverse.Run(ptr, 0);
        }

        [Test]
        public void SingleElement_Unchanged()
        {
            int val = 42;
            Grid.Reverse.Run(&val, 1);
            Assert.AreEqual(42, val);
        }

        [Test]
        public void TwoElements_Swaps()
        {
            int* ptr = stackalloc int[] { 1, 2 };
            Grid.Reverse.Run(ptr, 2);
            Assert.AreEqual(2, ptr[0]);
            Assert.AreEqual(1, ptr[1]);
        }

        [Test]
        public void OddLength_ReversesCorrectly()
        {
            int* ptr = stackalloc int[] { 1, 2, 3 };
            Grid.Reverse.Run(ptr, 3);
            Assert.AreEqual(3, ptr[0]);
            Assert.AreEqual(2, ptr[1]);
            Assert.AreEqual(1, ptr[2]);
        }

        [Test]
        public void EvenLength_ReversesCorrectly()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4 };
            Grid.Reverse.Run(ptr, 4);
            Assert.AreEqual(4, ptr[0]);
            Assert.AreEqual(3, ptr[1]);
            Assert.AreEqual(2, ptr[2]);
            Assert.AreEqual(1, ptr[3]);
        }

        [Test]
        public void LargeN_ReversesCorrectly()
        {
            const int N = 64;
            int* ptr = stackalloc int[N];
            for (int i = 0; i < N; i++)
                ptr[i] = i;
            Grid.Reverse.Run(ptr, N);
            for (int i = 0; i < N; i++)
                Assert.AreEqual(N - 1 - i, ptr[i]);
        }
    }
}