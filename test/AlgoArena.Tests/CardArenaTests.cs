namespace AlgoArena.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using IAFahim.Sort.Insertion;
    using IAFahim.Sort.Merge;
    using IAFahim.Sort.Partition;
    using IAFahim.Search.Specialized;
    using IAFahim.Search.Bit;

    public sealed unsafe class CardArenaTests
    {
        [Test]
        public void InsertionSort_Reversed_SortsCorrectly()
        {
            const int N = 64;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = N - i;

                Insertion.Run(ptr, N);

                for (int i = 0; i < N; i++)
                    Assert.AreEqual(i + 1, ptr[i]);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Test]
        public void InsertionSort_EmptyInput_NoOp()
        {
            Insertion.Run<int>(null, 0);
        }

        [Test]
        public void InsertionSort_SingleElement_NoOp()
        {
            int* ptr = (int*)Marshal.AllocHGlobal(sizeof(int));
            try
            {
                ptr[0] = 42;
                Insertion.Run(ptr, 1);
                Assert.AreEqual(42, ptr[0]);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Test]
        public void InsertionSort_AlreadySorted_NoChange()
        {
            const int N = 16;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = i;
                Insertion.Run(ptr, N);
                for (int i = 0; i < N; i++)
                    Assert.AreEqual(i, ptr[i]);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Test]
        public void InsertionSort_Duplicates_StableSort()
        {
            const int N = 32;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = (i % 4 == 0) ? 1 : (i % 4 == 1) ? 2 : (i % 4 == 2) ? 3 : 4;
                Insertion.Run(ptr, N);
                for (int i = 1; i < N; i++)
                    Assert.IsTrue(ptr[i] >= ptr[i - 1]);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Test]
        public void MergeSort_Reversed_SortsCorrectly()
        {
            const int N = 64;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = N - i;

                MergeSorted.RunInPlace(ptr, N);

                for (int i = 0; i < N; i++)
                    Assert.AreEqual(i + 1, ptr[i]);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Test]
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
                Assert.AreEqual(sumBefore, sumAfter);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Test]
        public void MergeSort_AllSizes_OneToThirtyThree()
        {
            for (int sz = 1; sz <= 33; sz++)
            {
                int* q = (int*)Marshal.AllocHGlobal(sz * sizeof(int));
                for (int i = 0; i < sz; i++) q[i] = sz - i;
                MergeSorted.RunInPlace(q, sz);
                for (int i = 1; i < sz; i++)
                    Assert.IsTrue(q[i] >= q[i - 1], $"size={sz} not sorted at {i}");
                Marshal.FreeHGlobal((nint)q);
            }
        }

        [Test]
        public void MergeSort_EmptyInput_NoOp()
        {
            MergeSorted.RunInPlace<int>(null, 0);
        }

        [Test]
        public void MergeSort_SingleElement_NoOp()
        {
            int* ptr = (int*)Marshal.AllocHGlobal(sizeof(int));
            try
            {
                ptr[0] = 99;
                MergeSorted.RunInPlace(ptr, 1);
                Assert.AreEqual(99, ptr[0]);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Test]
        public void BinarySearch_Found_ReturnsTrue()
        {
            const int N = 32;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = i * 2;

                bool found = BinarySearch.TryFind(ptr, N, 20, out int index);

                Assert.IsTrue(found);
                Assert.AreEqual(10, index);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Test]
        public void BinarySearch_NotFound_ReturnsFalse()
        {
            const int N = 32;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) ptr[i] = i * 2;

                bool found = BinarySearch.TryFind(ptr, N, 21, out int index);

                Assert.IsFalse(found);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Test]
        public void BinarySearch_EmptyArray_ReturnsFalse()
        {
            bool found = BinarySearch.TryFind(null, 0, 5, out int index);
            Assert.IsFalse(found);
        }

        [Test]
        public void BinarySearch_FirstElement()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7, 9 };
            bool found = BinarySearch.TryFind(ptr, 5, 1, out int index);
            Assert.IsTrue(found);
            Assert.AreEqual(0, index);
        }

        [Test]
        public void BinarySearch_LastElement()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7, 9 };
            bool found = BinarySearch.TryFind(ptr, 5, 9, out int index);
            Assert.IsTrue(found);
            Assert.AreEqual(4, index);
        }

        [Test]
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
                    Assert.IsTrue(ptr[i] <= ptr[pi]);
                for (int i = pi + 1; i < N; i++)
                    Assert.IsTrue(ptr[i] >= ptr[pi]);
            }
            finally { Marshal.FreeHGlobal((nint)ptr); }
        }

        [Test]
        public void Partition_NullPtr()
        {
            int pi = Partition.Run<int>(null, 0, 0);
            Assert.IsTrue(pi < 0);
        }

        [Test]
        public void LowerBound_FindsFirstGreaterOrEqual()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7, 9 };
            Assert.AreEqual(2, LowerBound.Run(ptr, 5, 5));
            Assert.AreEqual(0, LowerBound.Run(ptr, 5, 0));
            Assert.AreEqual(5, LowerBound.Run(ptr, 5, 10));
        }

        [Test]
        public void UpperBound_FindsFirstGreater()
        {
            int* ptr = stackalloc int[] { 1, 3, 5, 7, 9 };
            Assert.AreEqual(3, UpperBound.Run(ptr, 5, 5));
            Assert.AreEqual(0, UpperBound.Run(ptr, 5, 0));
            Assert.AreEqual(5, UpperBound.Run(ptr, 5, 9));
        }
    }
}