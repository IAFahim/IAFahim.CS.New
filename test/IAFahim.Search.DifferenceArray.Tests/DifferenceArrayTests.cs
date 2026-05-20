namespace IAFahim.Search.DifferenceArray.Tests
{
    using System;
    using Xunit;

    public sealed unsafe class DifferenceArrayTests
    {
        [Fact]
        public void Apply_EmptyDiff_NoOp()
        {
            int* diff = stackalloc int[0];
            Diff.Apply(diff, 0, 0, 0, 1);
        }

        [Fact]
        public void Apply_OutOfBounds_NoOp()
        {
            int* diff = stackalloc int[10];
            for (int i = 0; i < 10; i++) diff[i] = 0;
            Diff.Apply(diff, 10, 10, 15, 5);
            for (int i = 0; i < 10; i++) Assert.Equal(0, diff[i]);
        }

        [Fact]
        public void Apply_Normal_AddsCorrectly()
        {
            int* diff = stackalloc int[10];
            for (int i = 0; i < 10; i++) diff[i] = 0;
            Diff.Apply(diff, 10, 2, 5, 3);
            Assert.Equal(3, diff[2]);
            Assert.Equal(-3, diff[6]);
        }

        [Fact]
        public void Apply_EndAtLastIndex_NoUnderflow()
        {
            int* diff = stackalloc int[10];
            for (int i = 0; i < 10; i++) diff[i] = 0;
            Diff.Apply(diff, 10, 0, 9, 5);
            Assert.Equal(5, diff[0]);
            Assert.Equal(0, diff[10]);
        }

        [Fact]
        public void Build_Empty_NoOp()
        {
            int* output = stackalloc int[0];
            int* diff = stackalloc int[0];
            Diff.Build(output, diff, 0);
        }

        [Fact]
        public void Build_Normal_ComputesCorrectPrefix()
        {
            int* output = stackalloc int[5];
            int* diff = stackalloc int[5];
            diff[0] = 1; diff[1] = 2; diff[2] = 0; diff[3] = -1; diff[4] = -2;
            Diff.Build(output, diff, 5);
            Assert.Equal(1, output[0]);
            Assert.Equal(3, output[1]);
            Assert.Equal(3, output[2]);
            Assert.Equal(2, output[3]);
            Assert.Equal(0, output[4]);
        }

        [Fact]
        public void Build_AlreadyFlat_NoChange()
        {
            int* output = stackalloc int[5];
            int* diff = stackalloc int[5];
            for (int i = 0; i < 5; i++) diff[i] = 0;
            Diff.Build(output, diff, 5);
            for (int i = 0; i < 5; i++) Assert.Equal(0, output[i]);
        }

        [Fact]
        public void PrefixFromDiff_Normal_CreatesCorrectPrefix()
        {
            int* prefix = stackalloc int[6];
            int* diff = stackalloc int[5];
            diff[0] = 2; diff[1] = 3; diff[2] = -1; diff[3] = 0; diff[4] = -4;
            Diff.PrefixFromDiff(prefix, diff, 5);
            Assert.Equal(5, prefix[0]);
            Assert.Equal(2, prefix[1]);
            Assert.Equal(5, prefix[2]);
            Assert.Equal(4, prefix[3]);
            Assert.Equal(4, prefix[4]);
            Assert.Equal(0, prefix[5]);
        }

        [Fact]
        public void RangeSum_ValidIndex_ReturnsCorrect()
        {
            int* prefix = stackalloc int[6];
            prefix[0] = 5;
            prefix[1] = 3; prefix[2] = 7; prefix[3] = 10; prefix[4] = 12; prefix[5] = 15;
            Assert.Equal(7, Diff.RangeSum(prefix, 2));
        }

        [Fact]
        public void RangeSum_InvalidIndex_ReturnsZero()
        {
            int* prefix = stackalloc int[6];
            prefix[0] = 5;
            Assert.Equal(0, Diff.RangeSum(prefix, 5));
        }
    }
}