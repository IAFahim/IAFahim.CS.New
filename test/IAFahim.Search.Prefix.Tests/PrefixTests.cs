namespace IAFahim.Search.Prefix.Tests
{
    using System;
    using Xunit;

    public sealed unsafe class PrefixTests
    {
        [Fact]
        public void PrefixSums_Empty_Returns0()
        {
            int* ptr = null;
            Assert.Equal(0, PrefixSums.Run(ptr, 0));
        }

        [Fact]
        public void PrefixSums_Single_ReturnsElement()
        {
            int* ptr = stackalloc int[1];
            ptr[0] = 5;
            Assert.Equal(5, PrefixSums.Run(ptr, 1));
            Assert.Equal(5, ptr[0]);
        }

        [Fact]
        public void PrefixSums_Normal_ComputesCorrect()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 1; ptr[1] = 2; ptr[2] = 3; ptr[3] = 4; ptr[4] = 5;
            Assert.Equal(15, PrefixSums.Run(ptr, 5));
            Assert.Equal(1, ptr[0]);
            Assert.Equal(3, ptr[1]);
            Assert.Equal(6, ptr[2]);
            Assert.Equal(10, ptr[3]);
            Assert.Equal(15, ptr[4]);
        }

        [Fact]
        public void PrefixSums_Long_ComputesCorrect()
        {
            int* ptr = stackalloc int[10];
            for (int i = 0; i < 10; i++) ptr[i] = 1;
            Assert.Equal(10, PrefixSums.Run(ptr, 10));
            Assert.Equal(5, ptr[4]);
            Assert.Equal(10, ptr[9]);
        }

        [Fact]
        public void PrefixXor_Empty_Returns0()
        {
            int* ptr = null;
            Assert.Equal(0, PrefixXor.Run(ptr, 0));
        }

        [Fact]
        public void PrefixXor_Single_ReturnsElement()
        {
            int* ptr = stackalloc int[1];
            ptr[0] = 7;
            Assert.Equal(7, PrefixXor.Run(ptr, 1));
        }

        [Fact]
        public void PrefixXor_Normal_ComputesCorrect()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 1; ptr[1] = 3; ptr[2] = 5; ptr[3] = 7; ptr[4] = 9;
            Assert.Equal(1 ^ 3 ^ 5 ^ 7 ^ 9, PrefixXor.Run(ptr, 5));
            Assert.Equal(1, ptr[0]);
            Assert.Equal(2, ptr[1]);
            Assert.Equal(7, ptr[2]);
            Assert.Equal(0, ptr[3]);
            Assert.Equal(9, ptr[4]);
        }

        [Fact]
        public void PrefixMin_Normal_FindsMin()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int min = PrefixMin.Run(ptr, 5);
            Assert.Equal(1, min);
        }

        [Fact]
        public void PrefixMin_MinIndex_ReturnsCorrect()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int idx = PrefixMin.MinIndex(ptr, 5);
            Assert.Equal(3, idx);
        }

        [Fact]
        public void PrefixMax_Normal_FindsMax()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int max = PrefixMax.Run(ptr, 5);
            Assert.Equal(8, max);
        }

        [Fact]
        public void PrefixMax_MaxIndex_ReturnsCorrect()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int idx = PrefixMax.MaxIndex(ptr, 5);
            Assert.Equal(2, idx);
        }

        [Fact]
        public void PrefixXor_RangeXor_Normal_ReturnsCorrect()
        {
            int* ptr = stackalloc int[6];
            ptr[0] = 1; ptr[1] = 3; ptr[2] = 5; ptr[3] = 7; ptr[4] = 9; ptr[5] = 11;
            PrefixXor.Run(ptr, 6);
            Assert.Equal(1 ^ 3 ^ 5 ^ 7 ^ 9 ^ 11, PrefixXor.RangeXor(ptr, 0, 5));
            Assert.Equal(3 ^ 5 ^ 7, PrefixXor.RangeXor(ptr, 1, 3));
        }
    }
}