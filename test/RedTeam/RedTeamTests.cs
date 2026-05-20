namespace IAFahim.RedTeam
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class SortRedTeam
    {
        [Fact]
        public void InsertionSort_NegativeNumbers()
        {
            int len = 10;
            int* ptr = stackalloc int[len];
            ptr[0] = -5; ptr[1] = 3; ptr[2] = -1; ptr[3] = 0; ptr[4] = -10;
            ptr[5] = 7; ptr[6] = -3; ptr[7] = 2; ptr[8] = -8; ptr[9] = 4;

            IAFahim.Sort.Insertion.Insertion.Run(ptr, len);

            for (int i = 0; i < len - 1; i++)
                Assert.True(ptr[i] <= ptr[i + 1], $"Not sorted at {i}: {ptr[i]} > {ptr[i + 1]}");
        }

        [Fact]
        public void InsertionSort_IntMinIntMax()
        {
            int len = 4;
            int* ptr = stackalloc int[len];
            ptr[0] = int.MaxValue;
            ptr[1] = int.MinValue;
            ptr[2] = 0;
            ptr[3] = 42;

            IAFahim.Sort.Insertion.Insertion.Run(ptr, len);

            Assert.Equal(int.MinValue, ptr[0]);
            Assert.Equal(0, ptr[1]);
            Assert.Equal(42, ptr[2]);
            Assert.Equal(int.MaxValue, ptr[3]);
        }

        [Fact]
        public void MergeSorted_OneEmptyArray()
        {
            int* a = stackalloc int[3] { 1, 2, 3 };
            int* b = stackalloc int[0];
            int* dst = stackalloc int[3];

            IAFahim.Sort.Merge.MergeSorted.Run(a, 3, b, 0, dst);

            Assert.Equal(1, dst[0]);
            Assert.Equal(2, dst[1]);
            Assert.Equal(3, dst[2]);
        }

        [Fact]
        public void MergeSorted_LargeValues()
        {
            int* a = stackalloc int[2] { int.MaxValue - 2, int.MaxValue };
            int* b = stackalloc int[2] { int.MinValue, int.MinValue + 2 };
            int* dst = stackalloc int[4];

            IAFahim.Sort.Merge.MergeSorted.Run(a, 2, b, 2, dst);

            Assert.Equal(int.MinValue, dst[0]);
            Assert.Equal(int.MinValue + 2, dst[1]);
            Assert.Equal(int.MaxValue - 2, dst[2]);
            Assert.Equal(int.MaxValue, dst[3]);
        }

        [Fact]
        public void Partition_InvalidPivotIndex()
        {
            int len = 5;
            int* ptr = stackalloc int[5] { 3, 1, 4, 1, 5 };

            int result = IAFahim.Sort.Partition.Partition.Run(ptr, len, -1);
            Assert.Equal(-1, result);

            result = IAFahim.Sort.Partition.Partition.Run(ptr, len, len);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void SortInts_WithDuplicates()
        {
            int len = 10;
            int* ptr = stackalloc int[len];
            ptr[0] = 5; ptr[1] = 3; ptr[2] = 5; ptr[3] = 3; ptr[4] = 1;
            ptr[5] = 5; ptr[6] = 3; ptr[7] = 1; ptr[8] = 5; ptr[9] = 3;

            IAFahim.Sort.Specialized.SortInts.Run(ptr, len);

            for (int i = 0; i < len - 1; i++)
                Assert.True(ptr[i] <= ptr[i + 1], $"Not sorted at {i}: {ptr[i]} > {ptr[i + 1]}");
        }
    }

    public sealed unsafe class SearchRedTeam
    {
        [Fact]
        public void BinarySearch_EdgeCases()
        {
            int len = 5;
            int* ptr = stackalloc int[5] { 1, 2, 3, 4, 5 };

            int idx;
            bool found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, len, 1, out idx);
            Assert.True(found);
            Assert.Equal(0, idx);

            found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, len, 5, out idx);
            Assert.True(found);
            Assert.Equal(4, idx);

            found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, len, 0, out idx);
            Assert.False(found);

            found = IAFahim.Search.Specialized.BinarySearch.TryFind(ptr, len, 6, out idx);
            Assert.False(found);
        }

        [Fact]
        public void LowerBound_AllElementsSame()
        {
            int len = 100;
            int* ptr = stackalloc int[len];
            for (int i = 0; i < len; i++) ptr[i] = 42;

            int pos = IAFahim.Search.Specialized.LowerBound.Run(ptr, len, 42);
            Assert.Equal(0, pos);

            pos = IAFahim.Search.Specialized.LowerBound.Run(ptr, len, 0);
            Assert.Equal(0, pos);

            pos = IAFahim.Search.Specialized.LowerBound.Run(ptr, len, 100);
            Assert.Equal(len, pos);
        }

        [Fact]
        public void UpperBound_AllElementsSame()
        {
            int len = 100;
            int* ptr = stackalloc int[len];
            for (int i = 0; i < len; i++) ptr[i] = 42;

            int pos = IAFahim.Search.Specialized.UpperBound.Run(ptr, len, 42);
            Assert.Equal(len, pos);

            pos = IAFahim.Search.Specialized.UpperBound.Run(ptr, len, 0);
            Assert.Equal(0, pos);

            pos = IAFahim.Search.Specialized.UpperBound.Run(ptr, len, 100);
            Assert.Equal(len, pos);
        }

        [Fact]
        public void TernarySearch_SingleElement()
        {
            int len = 1;
            int* ptr = stackalloc int[1] { 42 };

            int result = IAFahim.Search.Specialized.TernarySearch.Run(ptr, len, 42);
            Assert.Equal(0, result);

            result = IAFahim.Search.Specialized.TernarySearch.Run(ptr, len, 0);
            Assert.True(result < 0);
        }
    }

    public sealed unsafe class RangeRedTeam
    {
        [Fact]
        public void RangeMex_AllConsecutiveFromZero()
        {
            int len = 5;
            int* a = stackalloc int[5] { 0, 1, 2, 3, 4 };
            int result = IAFahim.Search.Range.RangeMex.Run(len, a, 0, len - 1);
            Assert.Equal(5, result);
        }

        [Fact]
        public void RangeMex_NegativeNumbers()
        {
            int len = 5;
            int* a = stackalloc int[5] { -5, -1, 0, 1, 2 };
            int result = IAFahim.Search.Range.RangeMex.Run(len, a, 0, len - 1);
            Assert.Equal(3, result);
        }

        [Fact]
        public void RangeMex_EmptyRange()
        {
            int len = 5;
            int* a = stackalloc int[5] { 0, 1, 2, 3, 4 };
            int result = IAFahim.Search.Range.RangeMex.Run(len, a, 3, 2);
            Assert.Equal(0, result);
        }
    }
}
