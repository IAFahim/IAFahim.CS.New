namespace IAFahim.Search.Bit.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class BitSearchTests
    {
        [Test]
        public void LisLength_Empty_Returns0()
        {
            int* arr = null;
            Assert.AreEqual(0, LisLength.Run(0, arr));
        }

        [Test]
        public void LisLength_SingleElement_Returns1()
        {
            int* arr = stackalloc int[] { 5 };
            Assert.AreEqual(1, LisLength.Run(1, arr));
        }

        [Test]
        public void LisLength_AlreadySorted_ReturnsLength()
        {
            int* arr = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.AreEqual(5, LisLength.Run(5, arr));
        }

        [Test]
        public void LisLength_Reversed_Returns1()
        {
            int* arr = stackalloc int[] { 5, 4, 3, 2, 1 };
            Assert.AreEqual(1, LisLength.Run(5, arr));
        }

        [Test]
        public void LisLength_Duplicates_ReturnsCorrect()
        {
            int* arr = stackalloc int[] { 1, 1, 1, 1 };
            Assert.AreEqual(1, LisLength.Run(4, arr));
        }

        [Test]
        public void LdsLength_Empty_Returns0()
        {
            int* arr = null;
            Assert.AreEqual(0, LdsLength.Run(0, arr));
        }

        [Test]
        public void LdsLength_Reversed_ReturnsLength()
        {
            int* arr = stackalloc int[] { 5, 4, 3, 2, 1 };
            Assert.AreEqual(5, LdsLength.Run(5, arr));
        }

        [Test]
        public void LdsLength_Sorted_Returns1()
        {
            int* arr = stackalloc int[] { 1, 2, 3, 4, 5 };
            Assert.AreEqual(1, LdsLength.Run(5, arr));
        }

        [Test]
        public void BinarySearchLower_Found_ReturnsIndex()
        {
            int* arr = stackalloc int[] { 1, 3, 5, 7, 9 };
            Assert.AreEqual(2, BinarySearchLower.Run(arr, 5, 5));
        }

        [Test]
        public void BinarySearchLower_NotFound_ReturnsInsertPosition()
        {
            int* arr = stackalloc int[] { 1, 3, 5, 7, 9 };
            Assert.AreEqual(5, BinarySearchLower.Run(arr, 5, 10));
            Assert.AreEqual(0, BinarySearchLower.Run(arr, 5, 0));
        }

        [Test]
        public void BinarySearchUpper_Found_ReturnsNextIndex()
        {
            int* arr = stackalloc int[] { 1, 3, 5, 7, 9 };
            Assert.AreEqual(3, BinarySearchUpper.Run(arr, 5, 5));
        }

        [Test]
        public void BinarySearchUpper_NotFound_ReturnsInsertPosition()
        {
            int* arr = stackalloc int[] { 1, 3, 5, 7, 9 };
            Assert.AreEqual(5, BinarySearchUpper.Run(arr, 5, 10));
            Assert.AreEqual(0, BinarySearchUpper.Run(arr, 5, 0));
        }

        [Test]
        public void InversionCount_Empty_Returns0()
        {
            int* arr = null;
            Assert.AreEqual(0L, InversionCount.Run(0, arr));
        }

        [Test]
        public void InversionCount_Sorted_Returns0()
        {
            int* arr = stackalloc int[] { 1, 2, 3 };
            Assert.AreEqual(0L, InversionCount.Run(3, arr));
        }

        [Test]
        public void InversionCount_Reversed_ReturnsCorrect()
        {
            int* arr = stackalloc int[] { 3, 2, 1 };
            Assert.AreEqual(3L, InversionCount.Run(3, arr));
        }

        [Test]
        public void PatienceSort_Empty_Returns0()
        {
            int* arr = null;
            int* piles = stackalloc int[1];
            int* tops = stackalloc int[1];
            Assert.AreEqual(0, PatienceSort.Run(0, arr, piles, tops));
        }

        [Test]
        public void PatienceSort_Normal_ReturnsPileCount()
        {
            int* arr = stackalloc int[] { 3, 1, 5, 2, 4 };
            int* piles = stackalloc int[5];
            int* tops = stackalloc int[5];
            int result = PatienceSort.Run(5, arr, piles, tops);
            Assert.IsTrue(result > 0);
            Assert.AreEqual(1, piles[0]);
        }
    }
}