namespace IAFahim.String.Tests
{
    using IAFahim.String.SuffixArray;
    using NUnit.Framework;

    public sealed unsafe class LcpIntervalTests
    {
        [Test]
        public void BananaQueryAna_ReturnsMatchingSuffixInterval()
        {
            int* sa = stackalloc int[6];
            int* lcp = stackalloc int[6];
            int* interval = stackalloc int[2];

            sa[0] = 5;
            sa[1] = 3;
            sa[2] = 1;
            sa[3] = 0;
            sa[4] = 4;
            sa[5] = 2;

            lcp[0] = 0;
            lcp[1] = 1;
            lcp[2] = 3;
            lcp[3] = 0;
            lcp[4] = 0;
            lcp[5] = 2;

            int count = LcpInterval.Find(sa, lcp, 6, interval, 2, 3);

            Assert.AreEqual(2, count);
            Assert.AreEqual(1, interval[0]);
            Assert.AreEqual(2, interval[1]);
        }

        [Test]
        public void ZeroLengthQuery_ReturnsFullInterval()
        {
            int* sa = stackalloc int[3];
            int* lcp = stackalloc int[3];
            int* interval = stackalloc int[2];

            int count = LcpInterval.Find(sa, lcp, 3, interval, 1, 0);

            Assert.AreEqual(3, count);
            Assert.AreEqual(0, interval[0]);
            Assert.AreEqual(2, interval[1]);
        }

        [Test]
        public void InvalidQueryStart_ReturnsEmptyInterval()
        {
            int* sa = stackalloc int[3];
            int* lcp = stackalloc int[3];
            int* interval = stackalloc int[2];

            int count = LcpInterval.Find(sa, lcp, 3, interval, 3, 1);

            Assert.AreEqual(0, count);
            Assert.AreEqual(-1, interval[0]);
            Assert.AreEqual(-1, interval[1]);
        }
    }
}
