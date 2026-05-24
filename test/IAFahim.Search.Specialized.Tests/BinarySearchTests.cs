namespace IAFahim.Search.Specialized.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class BinarySearchTests
    {
        [Test]
        public void EmptyInput_NotFound()
        {
            int index;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(null, 0, 5, out index);
            Assert.IsFalse(found);
            Assert.AreEqual(0, index);
        }

        [Test]
        public void SingleElement_Found()
        {
            int* ptr = stackalloc int[] { 5 };
            int index;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, 1, 5, out index);
            Assert.IsTrue(found);
            Assert.AreEqual(0, index);
        }

        [Test]
        public void SingleElement_NotFound()
        {
            int* ptr = stackalloc int[] { 3 };
            int index;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, 1, 5, out index);
            Assert.IsFalse(found);
        }

        [Test]
        public void MultipleElements_Found()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7, 9 };
            int index;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, 5, 7, out index);
            Assert.IsTrue(found);
            Assert.AreEqual(3, index);
        }

        [Test]
        public void MultipleElements_NotFound()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7, 9 };
            int index;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, 5, 4, out index);
            Assert.IsFalse(found);
        }

        [Test]
        public void LargeN_FoundCorrectly()
        {
            const int N = 1024;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            for (int i = 0; i < N; i++)
                ptr[i] = i * 2;
            int index;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, N, 500, out index);
            Assert.IsTrue(found);
            Assert.AreEqual(250, index);
            Marshal.FreeHGlobal((nint)ptr);
        }

        [Test]
        public void LargeN_NotFound()
        {
            const int N = 1024;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            for (int i = 0; i < N; i++)
                ptr[i] = i * 2;
            int index;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, N, 501, out index);
            Assert.IsFalse(found);
            Marshal.FreeHGlobal((nint)ptr);
        }
    }
}