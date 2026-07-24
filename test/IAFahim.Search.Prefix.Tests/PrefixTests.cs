namespace IAFahim.Search.Prefix.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class PrefixTests
    {
        [Test]
        public void PrefixSums_Empty_Returns0()
        {
            int* ptr = null;
            Assert.AreEqual(0, PrefixSums.Run(ptr, 0));
        }

        [Test]
        public void PrefixSums_Single_ReturnsElement()
        {
            int* ptr = stackalloc int[1];
            ptr[0] = 5;
            Assert.AreEqual(5, PrefixSums.Run(ptr, 1));
            Assert.AreEqual(5, ptr[0]);
        }

        [Test]
        public void PrefixSums_Normal_ComputesCorrect()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 1; ptr[1] = 2; ptr[2] = 3; ptr[3] = 4; ptr[4] = 5;
            Assert.AreEqual(15, PrefixSums.Run(ptr, 5));
            Assert.AreEqual(1, ptr[0]);
            Assert.AreEqual(3, ptr[1]);
            Assert.AreEqual(6, ptr[2]);
            Assert.AreEqual(10, ptr[3]);
            Assert.AreEqual(15, ptr[4]);
        }

        [Test]
        public void PrefixSums_Long_ComputesCorrect()
        {
            int* ptr = stackalloc int[10];
            for (int i = 0; i < 10; i++) ptr[i] = 1;
            Assert.AreEqual(10, PrefixSums.Run(ptr, 10));
            Assert.AreEqual(5, ptr[4]);
            Assert.AreEqual(10, ptr[9]);
        }

        [Test]
        public void PrefixXor_Empty_Returns0()
        {
            int* ptr = null;
            Assert.AreEqual(0, PrefixXor.Run(ptr, 0));
        }

        [Test]
        public void PrefixXor_Single_ReturnsElement()
        {
            int* ptr = stackalloc int[1];
            ptr[0] = 7;
            Assert.AreEqual(7, PrefixXor.Run(ptr, 1));
        }

        [Test]
        public void PrefixXor_Normal_ComputesCorrect()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 1; ptr[1] = 3; ptr[2] = 5; ptr[3] = 7; ptr[4] = 9;
            Assert.AreEqual(1 ^ 3 ^ 5 ^ 7 ^ 9, PrefixXor.Run(ptr, 5));
            Assert.AreEqual(1, ptr[0]);
            Assert.AreEqual(2, ptr[1]);
            Assert.AreEqual(7, ptr[2]);
            Assert.AreEqual(0, ptr[3]);
            Assert.AreEqual(9, ptr[4]);
        }

        [Test]
        public void RangeXor_BytePrefix()
        {
            byte* ptr = stackalloc byte[] { 1, 3, 0, 0 };
            ptr[1] = (byte)(ptr[1] ^ ptr[0]);
            ptr[2] = (byte)(5 ^ ptr[1]);
            ptr[3] = (byte)(7 ^ ptr[2]);
            byte full = PrefixXor.RangeXor(ptr, 0, 3);
            Assert.AreEqual((byte)(1 ^ 3 ^ 5 ^ 7), full);
            byte mid = PrefixXor.RangeXor(ptr, 1, 2);
            Assert.AreEqual((byte)(3 ^ 5), mid);
        }

        [Test]
        public void PrefixSearch_MatchFindCount()
        {
            byte* text = stackalloc byte[] { (byte)'a', (byte)'b', (byte)'a', (byte)'b', (byte)'c' };
            byte* pat = stackalloc byte[] { (byte)'a', (byte)'b' };
            Assert.IsTrue(PrefixSearch.Match(text, 5, pat, 2));
            Assert.AreEqual(0, PrefixSearch.FindFirst(text, 5, pat, 2));
            Assert.AreEqual(2, PrefixSearch.CountOccurrences(text, 5, pat, 2));
            Assert.AreEqual(2, PrefixSearch.LongestCommonPrefix(text, pat, 2));
        }

        [Test]
        public void PrefixMin_Normal_FindsMin()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int min = PrefixMin.Run(ptr, 5);
            Assert.AreEqual(1, min);
        }

        [Test]
        public void PrefixMin_MinIndex_ReturnsCorrect()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int idx = PrefixMin.MinIndex(ptr, 5);
            Assert.AreEqual(3, idx);
        }

        [Test]
        public void PrefixMax_Normal_FindsMax()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int max = PrefixMax.Run(ptr, 5);
            Assert.AreEqual(8, max);
        }

        [Test]
        public void PrefixMax_MaxIndex_ReturnsCorrect()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 2; ptr[2] = 8; ptr[3] = 1; ptr[4] = 6;
            int idx = PrefixMax.MaxIndex(ptr, 5);
            Assert.AreEqual(2, idx);
        }

        [Test]
        public void PrefixXor_RangeXor_Normal_ReturnsCorrect()
        {
            int* ptr = stackalloc int[6];
            ptr[0] = 1; ptr[1] = 3; ptr[2] = 5; ptr[3] = 7; ptr[4] = 9; ptr[5] = 11;
            PrefixXor.Run(ptr, 6);
            Assert.AreEqual(1 ^ 3 ^ 5 ^ 7 ^ 9 ^ 11, PrefixXor.RangeXor(ptr, 0, 5));
            Assert.AreEqual(3 ^ 5 ^ 7, PrefixXor.RangeXor(ptr, 1, 3));
        }
    }
}