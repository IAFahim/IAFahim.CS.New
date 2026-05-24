namespace IAFahim.Sort.Specialized.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class SortInt64sTests
    {
        [Test]
        public void EmptyInput_NoOp()
        {
            SortInt64s.Run(null, 0);
        }

        [Test]
        public void SingleElement_Unchanged()
        {
            long val = 42L;
            SortInt64s.Run(&val, 1);
            Assert.AreEqual(42L, val);
        }

        [Test]
        public void AlreadySorted_Unchanged()
        {
            long* ptr = stackalloc long[] { 1, 2, 3, 4 };
            SortInt64s.Run(ptr, 4);
            Assert.AreEqual(1, ptr[0]);
            Assert.AreEqual(2, ptr[1]);
            Assert.AreEqual(3, ptr[2]);
            Assert.AreEqual(4, ptr[3]);
        }

        [Test]
        public void Reversed_Sorts()
        {
            long* ptr = stackalloc long[] { 4, 3, 2, 1 };
            SortInt64s.Run(ptr, 4);
            Assert.AreEqual(1, ptr[0]);
            Assert.AreEqual(2, ptr[1]);
            Assert.AreEqual(3, ptr[2]);
            Assert.AreEqual(4, ptr[3]);
        }

        [Test]
        public void LargeN_CorrectOrder()
        {
            const int N = 512;
            long* ptr = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            try
            {
                for (int i = 0; i < N; i++)
                    ptr[i] = N - i;
                SortInt64s.Run(ptr, N);
                for (int i = 0; i < N; i++)
                    Assert.AreEqual(i + 1, ptr[i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }
    }
}