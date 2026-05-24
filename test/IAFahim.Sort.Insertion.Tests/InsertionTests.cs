namespace IAFahim.Sort.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class InsertionTests
    {
        [Test]
        public void EmptyInput_NoOp()
        {
            Insertion.Insertion.Run<int>(null, 0);
        }

        [Test]
        public void SingleElement_Unchanged()
        {
            int value = 42;
            Insertion.Insertion.Run(&value, 1);
            Assert.AreEqual(42, value);
        }

        [Test]
        public void AlreadySorted_Unchanged()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4 };
            Insertion.Insertion.Run(ptr, 4);

            for (int i = 0; i < 4; i++)
                Assert.AreEqual(i + 1, ptr[i]);
        }

        [Test]
        public void Reversed_Sorts()
        {
            int* ptr = stackalloc int[] { 4, 3, 2, 1 };
            Insertion.Insertion.Run(ptr, 4);

            for (int i = 0; i < 4; i++)
                Assert.AreEqual(i + 1, ptr[i]);
        }

        [Test]
        public void AllDuplicates_Unchanged()
        {
            int* ptr = stackalloc int[] { 7, 7, 7, 7 };
            Insertion.Insertion.Run(ptr, 4);

            for (int i = 0; i < 4; i++)
                Assert.AreEqual(7, ptr[i]);
        }

        [Test]
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
                    Assert.AreEqual(i + 1, ptr[i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }

        [Test]
        public void Descending_CorrectOrder()
        {
            int* ptr = stackalloc int[] { 1, 2, 3, 4 };
            Insertion.Insertion.RunDescending(ptr, 4);

            Assert.AreEqual(4, ptr[0]);
            Assert.AreEqual(3, ptr[1]);
            Assert.AreEqual(2, ptr[2]);
            Assert.AreEqual(1, ptr[3]);
        }

        [Test]
        public void Float_Sorts()
        {
            float* ptr = stackalloc float[] { 3.14f, 1.41f, 2.71f };
            Insertion.Insertion.Run(ptr, 3);

            Assert.AreEqual(1.41f, ptr[0]);
            Assert.AreEqual(2.71f, ptr[1]);
            Assert.AreEqual(3.14f, ptr[2]);
        }
    }
}
