namespace IAFahim.Search.Suffix.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class SuffixTests
    {
        [Test]
        public void SuffixSums_Empty_Returns0()
        {
            int* ptr = null;
            Assert.AreEqual(0, SuffixSums.Run(ptr, 0));
        }

        [Test]
        public void SuffixSums_Single_ReturnsElement()
        {
            int* ptr = stackalloc int[1];
            ptr[0] = 5;
            Assert.AreEqual(5, SuffixSums.Run(ptr, 1));
        }

        [Test]
        public void SuffixSums_Normal_ComputesCorrect()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 1; ptr[1] = 2; ptr[2] = 3; ptr[3] = 4; ptr[4] = 5;
            Assert.AreEqual(15, SuffixSums.Run(ptr, 5));
            Assert.AreEqual(15, ptr[0]);
            Assert.AreEqual(14, ptr[1]);
            Assert.AreEqual(12, ptr[2]);
            Assert.AreEqual(9, ptr[3]);
            Assert.AreEqual(5, ptr[4]);
        }

        [Test]
        public void SuffixSums_Long_ComputesCorrect()
        {
            int* ptr = stackalloc int[10];
            for (int i = 0; i < 10; i++) ptr[i] = 1;
            Assert.AreEqual(10, SuffixSums.Run(ptr, 10));
            Assert.AreEqual(10, ptr[0]);
            Assert.AreEqual(5, ptr[5]);
            Assert.AreEqual(1, ptr[9]);
        }

        [Test]
        public void SuffixMin_Normal_FindsMin()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int min = SuffixMin.Run(ptr, 5);
            Assert.AreEqual(1, min);
        }

        [Test]
        public void SuffixMin_MinIndex_ReturnsCorrect()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int idx = SuffixMin.MinIndex(ptr, 5);
            Assert.AreEqual(3, idx);
        }

        [Test]
        public void SuffixMax_Normal_FindsMax()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int max = SuffixMax.Run(ptr, 5);
            Assert.AreEqual(8, max);
        }

        [Test]
        public void SuffixMax_MaxIndex_ReturnsCorrect()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int idx = SuffixMax.MaxIndex(ptr, 5);
            Assert.AreEqual(2, idx);
        }

        [Test]
        public void SuffixSums_RangeSum_InvalidRange_Returns0()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 1; ptr[1] = 2; ptr[2] = 3; ptr[3] = 4; ptr[4] = 5;
            Assert.AreEqual(0, SuffixSums.RangeSum(ptr, 3, 1));
        }

        [Test]
        public void SuffixSums_RangeSum_Normal_ReturnsCorrect()
        {
            int* ptr = stackalloc int[6];
            ptr[0] = 1; ptr[1] = 2; ptr[2] = 3; ptr[3] = 4; ptr[4] = 5; ptr[5] = 6;
            Assert.AreEqual(4 + 5 + 6, SuffixSums.RangeSum(ptr, 3, 5));
        }

        [Test]
        public void SuffixSums_Sum_Empty_Returns0()
        {
            int* ptr = null;
            Assert.AreEqual(0, SuffixSums.Sum(ptr, 0));
        }

        [Test]
        public void SuffixSums_Sum_Normal_ReturnsFirst()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 10; ptr[1] = 20; ptr[2] = 30;
            Assert.AreEqual(10, SuffixSums.Sum(ptr, 3));
        }
    }
}