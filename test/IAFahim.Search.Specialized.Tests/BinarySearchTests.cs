namespace IAFahim.Search.Specialized.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class BinarySearchTests
    {
        [Fact]
        public void EmptyInput_NotFound()
        {
            int index;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(null, 0, 5, out index);
            Assert.False(found);
            Assert.Equal(0, index);
        }

        [Fact]
        public void SingleElement_Found()
        {
            int* ptr = stackalloc int[] { 5 };
            int index;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, 1, 5, out index);
            Assert.True(found);
            Assert.Equal(0, index);
        }

        [Fact]
        public void SingleElement_NotFound()
        {
            int* ptr = stackalloc int[] { 3 };
            int index;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, 1, 5, out index);
            Assert.False(found);
        }

        [Fact]
        public void MultipleElements_Found()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7, 9 };
            int index;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, 5, 7, out index);
            Assert.True(found);
            Assert.Equal(3, index);
        }

        [Fact]
        public void MultipleElements_NotFound()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7, 9 };
            int index;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, 5, 4, out index);
            Assert.False(found);
        }

        [Fact]
        public void LargeN_FoundCorrectly()
        {
            const int N = 1024;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            for (int i = 0; i < N; i++)
                ptr[i] = i * 2;
            int index;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, N, 500, out index);
            Assert.True(found);
            Assert.Equal(250, index);
            Marshal.FreeHGlobal((IntPtr)ptr);
        }

        [Fact]
        public void LargeN_NotFound()
        {
            const int N = 1024;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            for (int i = 0; i < N; i++)
                ptr[i] = i * 2;
            int index;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, N, 501, out index);
            Assert.False(found);
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}