namespace IAFahim.Search.DifferenceArray.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class DifferenceArrayTests
    {
        [Test]
        public void Apply_EmptyDiff_NoOp()
        {
            int* diff = stackalloc int[0];
            Diff.Apply(diff, 0, 0, 0, 1);
        }

        [Test]
        public void Apply_OutOfBounds_NoOp()
        {
            int* diff = stackalloc int[10];
            for (int i = 0; i < 10; i++) diff[i] = 0;
            Diff.Apply(diff, 10, 10, 15, 5);
            for (int i = 0; i < 10; i++) Assert.AreEqual(0, diff[i]);
        }

        [Test]
        public void Apply_Normal_AddsCorrectly()
        {
            int* diff = stackalloc int[10];
            for (int i = 0; i < 10; i++) diff[i] = 0;
            Diff.Apply(diff, 10, 2, 5, 3);
            Assert.AreEqual(3, diff[2]);
            Assert.AreEqual(-3, diff[6]);
        }

        [Test]
        public void Apply_EndAtLastIndex_NoUnderflow()
        {
            int* diff = stackalloc int[10];
            for (int i = 0; i < 10; i++) diff[i] = 0;
            Diff.Apply(diff, 10, 0, 9, 5);
            Assert.AreEqual(5, diff[0]);
            Assert.AreEqual(0, diff[10]);
        }

        [Test]
        public void Build_Empty_NoOp()
        {
            int* output = stackalloc int[0];
            int* diff = stackalloc int[0];
            Diff.Build(output, diff, 0);
        }

        [Test]
        public void Build_Normal_ComputesCorrectPrefix()
        {
            int* output = stackalloc int[5];
            int* diff = stackalloc int[5];
            diff[0] = 1; diff[1] = 2; diff[2] = 0; diff[3] = -1; diff[4] = -2;
            Diff.Build(output, diff, 5);
            Assert.AreEqual(1, output[0]);
            Assert.AreEqual(3, output[1]);
            Assert.AreEqual(3, output[2]);
            Assert.AreEqual(2, output[3]);
            Assert.AreEqual(0, output[4]);
        }

        [Test]
        public void Build_AlreadyFlat_NoChange()
        {
            int* output = stackalloc int[5];
            int* diff = stackalloc int[5];
            for (int i = 0; i < 5; i++) diff[i] = 0;
            Diff.Build(output, diff, 5);
            for (int i = 0; i < 5; i++) Assert.AreEqual(0, output[i]);
        }

        [Test]
        public void PrefixFromDiff_Normal_CreatesCorrectPrefix()
        {
            int* prefix = stackalloc int[6];
            int* diff = stackalloc int[5];
            diff[0] = 2; diff[1] = 3; diff[2] = -1; diff[3] = 0; diff[4] = -4;
            Diff.PrefixFromDiff(prefix, diff, 5);
            Assert.AreEqual(5, prefix[0]);
            Assert.AreEqual(2, prefix[1]);
            Assert.AreEqual(5, prefix[2]);
            Assert.AreEqual(4, prefix[3]);
            Assert.AreEqual(4, prefix[4]);
            Assert.AreEqual(0, prefix[5]);
        }

        [Test]
        public void RangeSum_ValidIndex_ReturnsCorrect()
        {
            int* prefix = stackalloc int[6];
            prefix[0] = 5;
            prefix[1] = 3; prefix[2] = 7; prefix[3] = 10; prefix[4] = 12; prefix[5] = 15;
            Assert.AreEqual(7, Diff.RangeSum(prefix, 1));
        }

        [Test]
        public void RangeSum_InvalidIndex_ReturnsZero()
        {
            int* prefix = stackalloc int[6];
            prefix[0] = 5;
            Assert.AreEqual(0, Diff.RangeSum(prefix, 5));
        }
    }
}