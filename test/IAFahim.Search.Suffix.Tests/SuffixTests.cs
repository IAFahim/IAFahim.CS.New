namespace IAFahim.Search.Suffix.Tests
{
    using System;
    using Xunit;

    public sealed unsafe class SuffixTests
    {
        [Fact]
        public void SuffixSums_Empty_Returns0()
        {
            int* ptr = null;
            Assert.Equal(0, SuffixSums.Run(ptr, 0));
        }

        [Fact]
        public void SuffixSums_Single_ReturnsElement()
        {
            int* ptr = stackalloc int[1];
            ptr[0] = 5;
            Assert.Equal(5, SuffixSums.Run(ptr, 1));
        }

        [Fact]
        public void SuffixSums_Normal_ComputesCorrect()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 1; ptr[1] = 2; ptr[2] = 3; ptr[3] = 4; ptr[4] = 5;
            Assert.Equal(15, SuffixSums.Run(ptr, 5));
            Assert.Equal(15, ptr[0]);
            Assert.Equal(14, ptr[1]);
            Assert.Equal(12, ptr[2]);
            Assert.Equal(9, ptr[3]);
            Assert.Equal(5, ptr[4]);
        }

        [Fact]
        public void SuffixSums_Long_ComputesCorrect()
        {
            int* ptr = stackalloc int[10];
            for (int i = 0; i < 10; i++) ptr[i] = 1;
            Assert.Equal(10, SuffixSums.Run(ptr, 10));
            Assert.Equal(10, ptr[0]);
            Assert.Equal(5, ptr[5]);
            Assert.Equal(1, ptr[9]);
        }

        [Fact]
        public void SuffixMin_Normal_FindsMin()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int min = SuffixMin.Run(ptr, 5);
            Assert.Equal(1, min);
        }

        [Fact]
        public void SuffixMin_MinIndex_ReturnsCorrect()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int idx = SuffixMin.MinIndex(ptr, 5);
            Assert.Equal(3, idx);
        }

        [Fact]
        public void SuffixMax_Normal_FindsMax()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int max = SuffixMax.Run(ptr, 5);
            Assert.Equal(8, max);
        }

        [Fact]
        public void SuffixMax_MaxIndex_ReturnsCorrect()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int idx = SuffixMax.MaxIndex(ptr, 5);
            Assert.Equal(2, idx);
        }

        [Fact]
        public void SuffixSums_RangeSum_InvalidRange_Returns0()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 1; ptr[1] = 2; ptr[2] = 3; ptr[3] = 4; ptr[4] = 5;
            Assert.Equal(0, SuffixSums.RangeSum(ptr, 3, 1));
        }

        [Fact]
        public void SuffixSums_RangeSum_Normal_ReturnsCorrect()
        {
            int* ptr = stackalloc int[6];
            ptr[0] = 1; ptr[1] = 2; ptr[2] = 3; ptr[3] = 4; ptr[4] = 5; ptr[5] = 6;
            Assert.Equal(4 + 5 + 6, SuffixSums.RangeSum(ptr, 3, 5));
        }

        [Fact]
        public void SuffixSums_Sum_Empty_Returns0()
        {
            int* ptr = null;
            Assert.Equal(0, SuffixSums.Sum(ptr, 0));
        }

        [Fact]
        public void SuffixSums_Sum_Normal_ReturnsFirst()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 10; ptr[1] = 20; ptr[2] = 30;
            Assert.Equal(10, SuffixSums.Sum(ptr, 3));
        }
    }
}