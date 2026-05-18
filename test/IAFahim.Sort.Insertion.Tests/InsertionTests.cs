namespace IAFahim.Sort.Tests
{
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class InsertionTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            Insertion.Insertion.Run<int>(null, 0);
        }

        [Fact]
        public void SingleElement_Unchanged()
        {
            int value = 42;
            Insertion.Insertion.Run(&value, 1);
            Assert.Equal(42, value);
        }

        [Fact]
        public void AlreadySorted_Unchanged()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4 };
            Insertion.Insertion.Run(ptr, 4);

            for (int i = 0; i < 4; i++)
                Assert.Equal(i + 1, ptr[i]);
        }

        [Fact]
        public void Reversed_Sorts()
        {
            int* ptr = stackalloc int[] { 4, 3, 2, 1 };
            Insertion.Insertion.Run(ptr, 4);

            for (int i = 0; i < 4; i++)
                Assert.Equal(i + 1, ptr[i]);
        }

        [Fact]
        public void AllDuplicates_Unchanged()
        {
            int* ptr = stackalloc int[] { 7, 7, 7, 7 };
            Insertion.Insertion.Run(ptr, 4);

            for (int i = 0; i < 4; i++)
                Assert.Equal(7, ptr[i]);
        }

        [Fact]
        public void LargeN_CorrectOrder()
        {
            const int N = 1024;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++)
                    ptr[i] = N - i;

                Insertion.Insertion.Run(ptr, N);

                for (int i = 0; i < N; i++)
                    Assert.Equal(i + 1, ptr[i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }

        [Fact]
        public void Descending_CorrectOrder()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4 };
            Insertion.Insertion.RunDescending(ptr, 4);

            Assert.Equal(4, ptr[0]);
            Assert.Equal(3, ptr[1]);
            Assert.Equal(2, ptr[2]);
            Assert.Equal(1, ptr[3]);
        }

        [Fact]
        public void Float_Sorts()
        {
            float* ptr = stackalloc float[] { 3.14f, 1.41f, 2.71f };
            Insertion.Insertion.Run(ptr, 3);

            Assert.Equal(1.41f, ptr[0]);
            Assert.Equal(2.71f, ptr[1]);
            Assert.Equal(3.14f, ptr[2]);
        }
    }
}
