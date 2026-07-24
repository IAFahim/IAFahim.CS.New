namespace IAFahim.Search.Interval.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class IntervalTests
    {
        [Test]
        public void MergeIntervals_Empty_ReturnsZero()
        {
            Interval* ptr = null;
            Assert.AreEqual(0, MergeIntervals.Run(ptr, 0));
        }

        [Test]
        public void MergeIntervals_Single_ReturnsOne()
        {
            Interval* ptr = stackalloc Interval[1];
            ptr[0] = new Interval { Start = 1, End = 3 };
            Assert.AreEqual(1, MergeIntervals.Run(ptr, 1));
            Assert.AreEqual(1, ptr[0].Start);
            Assert.AreEqual(3, ptr[0].End);
        }

        [Test]
        public void MergeIntervals_NonOverlapping_ReturnsCount()
        {
            Interval* ptr = stackalloc Interval[3];
            ptr[0] = new Interval { Start = 1, End = 2 };
            ptr[1] = new Interval { Start = 4, End = 5 };
            ptr[2] = new Interval { Start = 7, End = 8 };
            Assert.AreEqual(3, MergeIntervals.Run(ptr, 3));
        }

        [Test]
        public void MergeIntervals_Overlapping_Merges()
        {
            Interval* ptr = stackalloc Interval[3];
            ptr[0] = new Interval { Start = 1, End = 4 };
            ptr[1] = new Interval { Start = 2, End = 5 };
            ptr[2] = new Interval { Start = 8, End = 10 };
            int result = MergeIntervals.Run(ptr, 3);
            Assert.AreEqual(2, result);
            Assert.AreEqual(1, ptr[0].Start);
            Assert.AreEqual(5, ptr[0].End);
        }

        [Test]
        public void MergeIntervals_Adjacent_Merges()
        {
            Interval* ptr = stackalloc Interval[2];
            ptr[0] = new Interval { Start = 1, End = 3 };
            ptr[1] = new Interval { Start = 3, End = 6 };
            int result = MergeIntervals.Run(ptr, 2);
            Assert.AreEqual(1, result);
            Assert.AreEqual(1, ptr[0].Start);
            Assert.AreEqual(6, ptr[0].End);
        }

        [Test]
        public void MergeIntervals_AllSameStart_Merges()
        {
            Interval* ptr = stackalloc Interval[3];
            ptr[0] = new Interval { Start = 1, End = 3 };
            ptr[1] = new Interval { Start = 1, End = 5 };
            ptr[2] = new Interval { Start = 1, End = 2 };
            int result = MergeIntervals.Run(ptr, 3);
            Assert.AreEqual(1, result);
            Assert.AreEqual(1, ptr[0].Start);
            Assert.AreEqual(5, ptr[0].End);
        }

        [Test]
        public void IntersectIntervals_NoOverlap_ReturnsZero()
        {
            Interval* a = stackalloc Interval[2];
            Interval* b = stackalloc Interval[2];
            Interval* dst = stackalloc Interval[10];
            a[0] = new Interval { Start = 1, End = 3 };
            a[1] = new Interval { Start = 5, End = 7 };
            b[0] = new Interval { Start = 11, End = 12 };
            b[1] = new Interval { Start = 13, End = 14 };
            Assert.AreEqual(0, IntersectIntervals.Run(a, 2, b, 2, dst));
        }

        [Test]
        public void IntersectIntervals_Overlap_ReturnsCorrect()
        {
            Interval* a = stackalloc Interval[2];
            Interval* b = stackalloc Interval[2];
            Interval* dst = stackalloc Interval[10];
            a[0] = new Interval { Start = 1, End = 5 };
            a[1] = new Interval { Start = 8, End = 10 };
            b[0] = new Interval { Start = 3, End = 7 };
            b[1] = new Interval { Start = 9, End = 12 };
            int result = IntersectIntervals.Run(a, 2, b, 2, dst);
            Assert.AreEqual(2, result);
            Assert.AreEqual(3, dst[0].Start);
            Assert.AreEqual(5, dst[0].End);
            Assert.AreEqual(9, dst[1].Start);
            Assert.AreEqual(10, dst[1].End);
        }

        [Test]
        public void NormalizeIntervals_Sorted_ReturnsCount()
        {
            Interval* ptr = stackalloc Interval[3];
            ptr[0] = new Interval { Start = 1, End = 3 };
            ptr[1] = new Interval { Start = 2, End = 4 };
            ptr[2] = new Interval { Start = 5, End = 7 };
            int result = NormalizeIntervals.Run(ptr, 3);
            Assert.AreEqual(2, result);
        }

        [Test]
        public void CountContained_SignedPoints()
        {
            int* starts = stackalloc int[] { -5, 0, 2 };
            int* ends = stackalloc int[] { -1, 4, 2 };
            Assert.AreEqual(1, IntervalSearch.CountContained(starts, ends, 3, -3));
            Assert.AreEqual(1, IntervalSearch.CountContained(starts, ends, 3, 0));
            Assert.AreEqual(1, IntervalSearch.CountContained(starts, ends, 3, 2));
            Assert.AreEqual(0, IntervalSearch.CountContained(starts, ends, 3, 10));
        }

        [Test]
        public void CountOverlapping_AndFindFirst()
        {
            int* starts = stackalloc int[] { 1, 5, 10 };
            int* ends = stackalloc int[] { 4, 8, 12 };
            Assert.AreEqual(2, IntervalSearch.CountOverlapping(starts, ends, 3, 3, 6));
            Assert.AreEqual(1, IntervalSearch.FindFirstOverlapping(starts, ends, 3, 6, 7));
            Assert.AreEqual(-1, IntervalSearch.FindFirstOverlapping(starts, ends, 3, 20, 21));
        }

        [Test]
        public void SortByStart_OrdersIntervals()
        {
            int* starts = stackalloc int[] { 5, 1, 3 };
            int* ends = stackalloc int[] { 6, 2, 4 };
            IntervalSearch.SortByStart(starts, ends, 3);
            Assert.AreEqual(1, starts[0]);
            Assert.AreEqual(3, starts[1]);
            Assert.AreEqual(5, starts[2]);
            Assert.AreEqual(2, ends[0]);
        }
    }
}