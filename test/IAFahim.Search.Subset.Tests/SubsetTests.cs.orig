namespace IAFahim.Search.Subset.Tests
{
    using Xunit;

    public sealed unsafe class EnumerateSubsetsTests
    {
        [Fact]
        public void Count()
        {
            Assert.Equal(8, EnumerateSubsets.Count(7));
        }

        [Fact]
        public void Run()
        {
            int* dst = stackalloc int[8];
            EnumerateSubsets.Run(7, dst);
            Assert.Equal(7, dst[0]);
            Assert.Equal(0, dst[7]);
        }
    }

    public sealed unsafe class EnumerateSupersetsTests
    {
        [Fact]
        public void Run()
        {
            int* dst = stackalloc int[4];
            int count = EnumerateSupersets.Run(2, 7, dst);
            Assert.Equal(3, count);
        }
    }

    public sealed unsafe class EnumerateMasksTests
    {
        [Fact]
        public void Count()
        {
            Assert.Equal(8, EnumerateMasks.Count(3));
        }

        [Fact]
        public void CountPopBits()
        {
            Assert.Equal(3, EnumerateMasks.CountPopBits(7));
            Assert.Equal(1, EnumerateMasks.CountPopBits(4));
            Assert.Equal(0, EnumerateMasks.CountPopBits(0));
        }
    }
}