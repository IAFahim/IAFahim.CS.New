namespace AlgoArena.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;
    using IAFahim.Sort.Insertion;
    using IAFahim.Sort.Merge;
    using IAFahim.Sort.Partition;
    using IAFahim.Search.Specialized;
    using IAFahim.Search.Bit;

    public sealed unsafe class CardArenaTests
    {
        [Fact]
        public void InsertionSort_Reversed_SortsCorrectly()
        {
            const int N = 64;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = N - i;

                Insertion.Run(ptr, N);

                for (int i = 0; i < N; i++)
                    Assert.Equal(i + 1, ptr[i]);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Fact]
        public void InsertionSort_EmptyInput_NoOp()
        {
            Insertion.Run<int>(null, 0);
        }

        [Fact]
        public void InsertionSort_SingleElement_NoOp()
        {
            int* ptr = (int*)Marshal.AllocHGlobal(sizeof(int));
            try
            {
                ptr[0] = 42;
                Insertion.Run(ptr, 1);
                Assert.Equal(42, ptr[0]);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Fact]
        public void InsertionSort_AlreadySorted_NoChange()
        {
            const int N = 16;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = i;
                Insertion.Run(ptr, N);
                for (int i = 0; i < N; i++)
                    Assert.Equal(i, ptr[i]);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Fact]
        public void InsertionSort_Duplicates_StableSort()
        {
            const int N = 32;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = (i % 4 == 0) ? 1 : (i % 4 == 1) ? 2 : (i % 4 == 2) ? 3 : 4;
                Insertion.Run(ptr, N);
                for (int i = 1; i < N; i++)
                    Assert.True(ptr[i] >= ptr[i - 1]);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Fact]
        public void MergeSort_Reversed_SortsCorrectly()
        {
            const int N = 64;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = N - i;

                MergeSorted.RunInPlace(ptr, N);

                for (int i = 0; i < N; i++)
                    Assert.Equal(i + 1, ptr[i]);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Fact]
        public void MergeSort_PreservesElements()
        {
            const int N = 64;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                long sumBefore = 0;
                for (int i = 0; i < N; i++) { ptr[i] = N - i; sumBefore += ptr[i]; }

                MergeSorted.RunInPlace(ptr, N);

                long sumAfter = 0;
                for (int i = 0; i < N; i++) sumAfter += ptr[i];
                Assert.Equal(sumBefore, sumAfter);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Fact]
        public void MergeSort_AllSizes_OneToThirtyThree()
        {
            for (int sz = 1; sz <= 33; sz++)
            {
                int* q = (int*)Marshal.AllocHGlobal(sz * sizeof(int));
                for (int i = 0; i < sz; i++) q[i] = sz - i;
                MergeSorted.RunInPlace(q, sz);
                for (int i = 1; i < sz; i++)
                    Assert.True(q[i] >= q[i - 1], $"size={sz} not sorted at {i}");
                Marshal.FreeHGlobal((nint)q);
            }
        }

        [Fact]
        public void MergeSort_EmptyInput_NoOp()
        {
            MergeSorted.RunInPlace<int>(null, 0);
        }

        [Fact]
        public void MergeSort_SingleElement_NoOp()
        {
            int* ptr = (int*)Marshal.AllocHGlobal(sizeof(int));
            try
            {
                ptr[0] = 99;
                MergeSorted.RunInPlace(ptr, 1);
                Assert.Equal(99, ptr[0]);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Fact]
        public void BinarySearch_Found_ReturnsTrue()
        {
            const int N = 32;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = i * 2;

                bool found = BinarySearch.TryFind(ptr, N, 20, out int index);

                Assert.True(found);
                Assert.Equal(10, index);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Fact]
        public void BinarySearch_NotFound_ReturnsFalse()
        {
            const int N = 32;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = i * 2;

                bool found = BinarySearch.TryFind(ptr, N, 21, out int index);

                Assert.False(found);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Fact]
        public void BinarySearch_EmptyArray_ReturnsFalse()
        {
            bool found = BinarySearch.TryFind(null, 0, 5, out int index);
            Assert.False(found);
        }

        [Fact]
        public void BinarySearch_FirstElement()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7, 9 };
            bool found = BinarySearch.TryFind(ptr, 5, 1, out int index);
            Assert.True(found);
            Assert.Equal(0, index);
        }

        [Fact]
        public void BinarySearch_LastElement()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7, 9 };
            bool found = BinarySearch.TryFind(ptr, 5, 9, out int index);
            Assert.True(found);
            Assert.Equal(4, index);
        }

        [Fact]
        public void Partition_Lomuto_CorrectPlacement()
        {
            const int N = 9;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                int[] input = { 5, 3, 8, 1, 9, 2, 7, 4, 6 };
                for (int i = 0; i < N; i++) ptr[i] = input[i];

                int pivotIdx = 4;
                int pi = Partition.Run(ptr, N, pivotIdx);

                for (int i = 0; i < pi; i++)
                    Assert.True(ptr[i] <= ptr[pi]);
                for (int i = pi + 1; i < N; i++)
                    Assert.True(ptr[i] >= ptr[pi]);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Fact]
        public void Partition_NullPtr()
        {
            int pi = Partition.Run<int>(null, 0, 0);
            Assert.True(pi < 0);
        }

        [Fact]
        public void LowerBound_FindsFirstGreaterOrEqual()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7, 9 };
            Assert.Equal(2, BitSearch.LowerBound(ptr, 5, 5));
            Assert.Equal(0, BitSearch.LowerBound(ptr, 5, 0));
            Assert.Equal(5, BitSearch.LowerBound(ptr, 5, 10));
        }

        [Fact]
        public void UpperBound_FindsFirstGreater()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7, 9 };
            Assert.Equal(3, BitSearch.UpperBound(ptr, 5, 5));
            Assert.Equal(0, BitSearch.UpperBound(ptr, 5, 0));
            Assert.Equal(5, BitSearch.UpperBound(ptr, 5, 9));
        }
    }
}