namespace IAFahim.Compress.Tests
{
    using Xunit;

    public sealed unsafe class CompressValuesTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            long* dst = stackalloc long[0];
            IAFahim.Compress.CompressValues.Run(null, dst, 0);
        }

        [Fact]
        public void SingleValue_Compresses()
        {
            int* src = stackalloc int[] { 42 };
            long* dst = stackalloc long[1];
            IAFahim.Compress.CompressValues.Run(src, dst, 1);
            Assert.Equal(42, dst[0]);
        }

        [Fact]
        public void MultipleValues_CompressesAll()
        {
            int* src = stackalloc int[] { -1, 0, 1, 2 };
            long* dst = stackalloc long[4];
            IAFahim.Compress.CompressValues.Run(src, dst, 4);
            Assert.Equal(-1, dst[0]);
            Assert.Equal(0, dst[1]);
            Assert.Equal(1, dst[2]);
            Assert.Equal(2, dst[3]);
        }

        [Fact]
        public void LargeValues_PreservesValues()
        {
            int* src = stackalloc int[] { int.MaxValue, int.MinValue };
            long* dst = stackalloc long[2];
            IAFahim.Compress.CompressValues.Run(src, dst, 2);
            Assert.Equal(int.MaxValue, dst[0]);
            Assert.Equal(int.MinValue, dst[1]);
        }
    }

    public sealed unsafe class CompressValuesUniqueTests
    {
        [Fact]
        public void EmptyInput_ReturnsZero()
        {
            long* dst = stackalloc long[0];
            int count = IAFahim.Compress.CompressValues.RunUnique(null, dst, 0);
            Assert.Equal(0, count);
        }

        [Fact]
        public void SingleValue_ReturnsOne()
        {
            int* src = stackalloc int[] { 42 };
            long* dst = stackalloc long[1];
            int count = IAFahim.Compress.CompressValues.RunUnique(src, dst, 1);
            Assert.Equal(1, count);
            Assert.Equal(42, dst[0]);
        }

        [Fact]
        public void AllUnique_ReturnsLen()
        {
            int* src = stackalloc int[] { 3, 1, 4, 1, 5, 9 };
            long* dst = stackalloc long[6];
            int count = IAFahim.Compress.CompressValues.RunUnique(src, dst, 6);
            Assert.Equal(6, count);
        }

        [Fact]
        public void AllDuplicates_ReturnsOne()
        {
            int* src = stackalloc int[] { 7, 7, 7 };
            long* dst = stackalloc long[3];
            int count = IAFahim.Compress.CompressValues.RunUnique(src, dst, 3);
            Assert.Equal(1, count);
            Assert.Equal(7, dst[0]);
        }

        [Fact]
        public void SomeDuplicates_Deduplicates()
        {
            int* src = stackalloc int[] { 1, 1, 2, 3, 3, 3, 4 };
            long* dst = stackalloc long[7];
            int count = IAFahim.Compress.CompressValues.RunUnique(src, dst, 7);
            Assert.Equal(4, count);
            Assert.Equal(1, dst[0]);
            Assert.Equal(2, dst[1]);
            Assert.Equal(3, dst[2]);
            Assert.Equal(4, dst[3]);
        }
    }
}