namespace IAFahim.Unique.Tests
{
    using Xunit;

    public sealed unsafe class UniqueInt64sTests
    {
        [Fact]
        public void EmptyInput_ReturnsZero()
        {
            int count = IAFahim.Unique.UniqueInt64s.Run(null, 0);
            Assert.Equal(0, count);
        }

        [Fact]
        public void SingleElement_ReturnsOne()
        {
            long* ptr = stackalloc long[] { 42L };
            int count = IAFahim.Unique.UniqueInt64s.Run(ptr, 1);
            Assert.Equal(1, count);
            Assert.Equal(42L, ptr[0]);
        }

        [Fact]
        public void AllUnique_ReturnsLen()
        {
            long* ptr = stackalloc long[] { 1, 2, 3 };
            int count = IAFahim.Unique.UniqueInt64s.Run(ptr, 3);
            Assert.Equal(3, count);
        }

        [Fact]
        public void AllDuplicates_ReturnsOne()
        {
            long* ptr = stackalloc long[] { 7, 7, 7 };
            int count = IAFahim.Unique.UniqueInt64s.Run(ptr, 3);
            Assert.Equal(1, count);
            Assert.Equal(7, ptr[0]);
        }

        [Fact]
        public void SomeDuplicates_RemovesDuplicates()
        {
            long* ptr = stackalloc long[] { 1, 1, 2, 3, 3 };
            int count = IAFahim.Unique.UniqueInt64s.Run(ptr, 5);
            Assert.Equal(3, count);
            Assert.Equal(1, ptr[0]);
            Assert.Equal(2, ptr[1]);
            Assert.Equal(3, ptr[2]);
        }
    }
}