namespace IAFahim.Sort.Merge.Tests
{
    using System;
    using Xunit;

    public sealed unsafe class MergeTests
    {
        [Fact]
        public void MergeSorted_BothEmpty_ProducesEmpty()
        {
            int* a = stackalloc int[0];
            int* b = stackalloc int[0];
            int* dst = stackalloc int[0];
            MergeSorted.Run(a, 0, b, 0, dst);
        }

        [Fact]
        public void MergeSorted_LeftEmpty_CopiesRight()
        {
            int* a = stackalloc int[0];
            int* b = stackalloc int[3];
            b[0] = 1; b[1] = 3; b[2] = 5;
            int* dst = stackalloc int[3];
            MergeSorted.Run(a, 0, b, 3, dst);
            Assert.Equal(1, dst[0]);
            Assert.Equal(3, dst[1]);
            Assert.Equal(5, dst[2]);
        }

        [Fact]
        public void MergeSorted_RightEmpty_CopiesLeft()
        {
            int* a = stackalloc int[3];
            a[0] = 2; a[1] = 4; a[2] = 6;
            int* b = stackalloc int[0];
            int* dst = stackalloc int[3];
            MergeSorted.Run(a, 3, b, 0, dst);
            Assert.Equal(2, dst[0]);
            Assert.Equal(4, dst[1]);
            Assert.Equal(6, dst[2]);
        }

        [Fact]
        public void MergeSorted_BothHaveData_MergesCorrectly()
        {
            int* a = stackalloc int[3];
            a[0] = 1; a[1] = 3; a[2] = 5;
            int* b = stackalloc int[3];
            b[0] = 2; b[1] = 4; b[2] = 6;
            int* dst = stackalloc int[6];
            MergeSorted.Run(a, 3, b, 3, dst);
            Assert.Equal(1, dst[0]);
            Assert.Equal(2, dst[1]);
            Assert.Equal(3, dst[2]);
            Assert.Equal(4, dst[3]);
            Assert.Equal(5, dst[4]);
            Assert.Equal(6, dst[5]);
        }

        [Fact]
        public void MergeSorted_Duplicates_MergesCorrectly()
        {
            int* a = stackalloc int[2];
            a[0] = 1; a[1] = 3;
            int* b = stackalloc int[2];
            b[0] = 1; b[1] = 3;
            int* dst = stackalloc int[4];
            MergeSorted.Run(a, 2, b, 2, dst);
            Assert.Equal(1, dst[0]);
            Assert.Equal(1, dst[1]);
            Assert.Equal(3, dst[2]);
            Assert.Equal(3, dst[3]);
        }

        [Fact]
        public void MergeSorted_LargerLeft_MergesCorrectly()
        {
            int* a = stackalloc int[4];
            a[0] = 1; a[1] = 3; a[2] = 5; a[3] = 7;
            int* b = stackalloc int[2];
            b[0] = 2; b[1] = 4;
            int* dst = stackalloc int[6];
            MergeSorted.Run(a, 4, b, 2, dst);
            Assert.Equal(1, dst[0]);
            Assert.Equal(2, dst[1]);
            Assert.Equal(3, dst[2]);
            Assert.Equal(4, dst[3]);
            Assert.Equal(5, dst[4]);
            Assert.Equal(7, dst[5]);
        }

        [Fact]
        public void MergeSorted_InPlace_Empty_NoOp()
        {
            fixed (int* ptr = null)
            {
                MergeSorted.RunInPlace(ptr, 0);
            }
        }

        [Fact]
        public void MergeSorted_InPlace_Single_NoOp()
        {
            int* ptr = stackalloc int[1];
            ptr[0] = 5;
            MergeSorted.RunInPlace(ptr, 1);
            Assert.Equal(5, ptr[0]);
        }

        [Fact]
        public void MergeSorted_InPlace_Reversed_Sorts()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 4; ptr[2] = 3; ptr[3] = 2; ptr[4] = 1;
            MergeSorted.RunInPlace(ptr, 5);
            Assert.Equal(1, ptr[0]);
            Assert.Equal(2, ptr[1]);
            Assert.Equal(3, ptr[2]);
            Assert.Equal(4, ptr[3]);
            Assert.Equal(5, ptr[4]);
        }

        [Fact]
        public void MergeSorted_InPlace_AlreadySorted_NoChange()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 1; ptr[1] = 2; ptr[2] = 3; ptr[3] = 4; ptr[4] = 5;
            MergeSorted.RunInPlace(ptr, 5);
            Assert.Equal(1, ptr[0]);
            Assert.Equal(2, ptr[1]);
            Assert.Equal(3, ptr[2]);
            Assert.Equal(4, ptr[3]);
            Assert.Equal(5, ptr[4]);
        }

        [Fact]
        public void MergeSorted_InPlace_Duplicates_Sorts()
        {
            int* ptr = stackalloc int[6];
            ptr[0] = 3; ptr[1] = 1; ptr[2] = 3; ptr[3] = 1; ptr[4] = 3; ptr[5] = 1;
            MergeSorted.RunInPlace(ptr, 6);
            Assert.Equal(1, ptr[0]);
            Assert.Equal(1, ptr[1]);
            Assert.Equal(1, ptr[2]);
            Assert.Equal(3, ptr[3]);
            Assert.Equal(3, ptr[4]);
            Assert.Equal(3, ptr[5]);
        }
    }
}