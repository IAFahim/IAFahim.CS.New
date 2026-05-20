namespace IAFahim.Search.Interval.Tests
{
    using System;
    using Xunit;

    public sealed unsafe class IntervalTests
    {
        [Fact]
        public void MergeIntervals_Empty_ReturnsZero()
        {
            fixed (Interval* ptr = null)
            {
                Assert.Equal(0, MergeIntervals.Run(ptr, 0));
            }
        }

        [Fact]
        public void MergeIntervals_Single_ReturnsOne()
        {
            Interval* ptr = stackalloc Interval[1];
            ptr[0] = new Interval { Start = 1, End = 3 };
            Assert.Equal(1, MergeIntervals.Run(ptr, 1));
            Assert.Equal(1, ptr[0].Start);
            Assert.Equal(3, ptr[0].End);
        }

        [Fact]
        public void MergeIntervals_NonOverlapping_ReturnsCount()
        {
            Interval* ptr = stackalloc Interval[3];
            ptr[0] = new Interval { Start = 1, End = 2 };
            ptr[1] = new Interval { Start = 4, End = 5 };
            ptr[2] = new Interval { Start = 7, End = 8 };
            Assert.Equal(3, MergeIntervals.Run(ptr, 3));
        }

        [Fact]
        public void MergeIntervals_Overlapping_Merges()
        {
            Interval* ptr = stackalloc Interval[3];
            ptr[0] = new Interval { Start = 1, End = 4 };
            ptr[1] = new Interval { Start = 2, End = 5 };
            ptr[2] = new Interval { Start = 8, End = 10 };
            int result = MergeIntervals.Run(ptr, 3);
            Assert.Equal(2, result);
            Assert.Equal(1, ptr[0].Start);
            Assert.Equal(5, ptr[0].End);
        }

        [Fact]
        public void MergeIntervals_Adjacent_Merges()
        {
            Interval* ptr = stackalloc Interval[2];
            ptr[0] = new Interval { Start = 1, End = 3 };
            ptr[1] = new Interval { Start = 4, End = 6 };
            int result = MergeIntervals.Run(ptr, 2);
            Assert.Equal(1, result);
            Assert.Equal(1, ptr[0].Start);
            Assert.Equal(6, ptr[0].End);
        }

        [Fact]
        public void MergeIntervals_AllSameStart_Merges()
        {
            Interval* ptr = stackalloc Interval[3];
            ptr[0] = new Interval { Start = 1, End = 3 };
            ptr[1] = new Interval { Start = 1, End = 5 };
            ptr[2] = new Interval { Start = 1, End = 2 };
            int result = MergeIntervals.Run(ptr, 3);
            Assert.Equal(1, result);
            Assert.Equal(1, ptr[0].Start);
            Assert.Equal(5, ptr[0].End);
        }

        [Fact]
        public void IntersectIntervals_NoOverlap_ReturnsZero()
        {
            Interval* a = stackalloc Interval[2];
            Interval* b = stackalloc Interval[2];
            Interval* dst = stackalloc Interval[10];
            a[0] = new Interval { Start = 1, End = 3 };
            a[1] = new Interval { Start = 5, End = 7 };
            b[0] = new Interval { Start = 4, End = 6 };
            b[1] = new Interval { Start = 8, End = 10 };
            Assert.Equal(0, IntersectIntervals.Run(a, 2, b, 2, dst));
        }

        [Fact]
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
            Assert.Equal(2, result);
            Assert.Equal(3, dst[0].Start);
            Assert.Equal(5, dst[0].End);
            Assert.Equal(9, dst[1].Start);
            Assert.Equal(10, dst[1].End);
        }

        [Fact]
        public void NormalizeIntervals_Sorted_ReturnsCount()
        {
            Interval* ptr = stackalloc Interval[3];
            ptr[0] = new Interval { Start = 1, End = 3 };
            ptr[1] = new Interval { Start = 2, End = 4 };
            ptr[2] = new Interval { Start = 5, End = 7 };
            int result = NormalizeIntervals.Run(ptr, 3);
            Assert.Equal(2, result);
        }
    }
}