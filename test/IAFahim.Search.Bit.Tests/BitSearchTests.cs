namespace IAFahim.Search.Bit.Tests
{
    using System;
    using Xunit;

    public sealed unsafe class BitSearchTests
    {
        [Fact]
        public void LisLength_Empty_Returns0()
        {
            fixed (int* arr = null)
            {
                Assert.Equal(0, LisLength.Run(0, arr));
            }
        }

        [Fact]
        public void LisLength_SingleElement_Returns1()
        {
            int* arr = stackalloc int[] { 5 };
            Assert.Equal(1, LisLength.Run(1, arr));
        }

        [Fact]
        public void LisLength_AlreadySorted_ReturnsLength()
        {
            int* arr = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(5, LisLength.Run(5, arr));
        }

        [Fact]
        public void LisLength_Reversed_Returns1()
        {
            int* arr = stackalloc int[] { 5, 4, 3, 2, 1 };
            Assert.Equal(1, LisLength.Run(5, arr));
        }

        [Fact]
        public void LisLength_Duplicates_ReturnsCorrect()
        {
            int* arr = stackalloc int[] { 1, 1, 1, 1 };
            Assert.Equal(1, LisLength.Run(4, arr));
        }

        [Fact]
        public void LdsLength_Empty_Returns0()
        {
            fixed (int* arr = null)
            {
                Assert.Equal(0, LdsLength.Run(0, arr));
            }
        }

        [Fact]
        public void LdsLength_Reversed_ReturnsLength()
        {
            int* arr = stackalloc int[] { 5, 4, 3, 2, 1 };
            Assert.Equal(5, LdsLength.Run(5, arr));
        }

        [Fact]
        public void LdsLength_Sorted_Returns1()
        {
            int* arr = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.Equal(1, LdsLength.Run(5, arr));
        }

        [Fact]
        public void BinarySearchLower_Found_ReturnsIndex()
        {
            int* arr = stackalloc int[] { 1, 3, 5, 7, 9 };
            Assert.Equal(2, BinarySearchLower.Run(arr, 5, 5));
        }

        [Fact]
        public void BinarySearchLower_NotFound_ReturnsInsertPosition()
        {
            int* arr = stackalloc int[] { 1, 3, 5, 7, 9 };
            Assert.Equal(5, BinarySearchLower.Run(arr, 5, 10));
            Assert.Equal(0, BinarySearchLower.Run(arr, 5, 0));
        }

        [Fact]
        public void BinarySearchUpper_Found_ReturnsNextIndex()
        {
            int* arr = stackalloc int[] { 1, 3, 5, 7, 9 };
            Assert.Equal(3, BinarySearchUpper.Run(arr, 5, 5));
        }

        [Fact]
        public void BinarySearchUpper_NotFound_ReturnsInsertPosition()
        {
            int* arr = stackalloc int[] { 1, 3, 5, 7, 9 };
            Assert.Equal(5, BinarySearchUpper.Run(arr, 5, 10));
            Assert.Equal(0, BinarySearchUpper.Run(arr, 5, 0));
        }

        [Fact]
        public void InversionCount_Empty_Returns0()
        {
            fixed (int* arr = null)
            {
                Assert.Equal(0L, InversionCount.Run(0, arr));
            }
        }

        [Fact]
        public void InversionCount_Sorted_Returns0()
        {
            int* arr = stackalloc int[] { 1, 2, 3 };
            Assert.Equal(0L, InversionCount.Run(3, arr));
        }

        [Fact]
        public void InversionCount_Reversed_ReturnsCorrect()
        {
            int* arr = stackalloc int[] { 3, 2, 1 };
            Assert.Equal(3L, InversionCount.Run(3, arr));
        }

        [Fact]
        public void PatienceSort_Empty_Returns0()
        {
            fixed (int* arr = null)
            {
                int* piles = stackalloc int[1];
                int* tops = stackalloc int[1];
                Assert.Equal(0, PatienceSort.Run(0, arr, piles, tops));
            }
        }

        [Fact]
        public void PatienceSort_Normal_ReturnsPileCount()
        {
            int* arr = stackalloc int[] { 3, 1, 5, 2, 4 };
            int* piles = stackalloc int[5];
            int* tops = stackalloc int[5];
            int result = PatienceSort.Run(5, arr, piles, tops);
            Assert.True(result > 0);
            Assert.Equal(1, piles[0]);
        }
    }
}