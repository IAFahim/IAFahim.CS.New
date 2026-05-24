namespace IAFahim.Compress.Tests
{
    using Xunit;

    public sealed unsafe class RestoreCompressedTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            int* dst = stackalloc int[0];
            IAFahim.Compress.RestoreCompressed.Run(null, dst, 0);
        }

        [Fact]
        public void SingleValue_Restores()
        {
            long* src = stackalloc long[] { 42 };
            int* dst = stackalloc int[1];
            IAFahim.Compress.RestoreCompressed.Run(src, dst, 1);
            Assert.Equal(42, dst[0]);
        }

        [Fact]
        public void MultipleValues_RestoresAll()
        {
            long* src = stackalloc long[] { -1, 0, 1, int.MaxValue, int.MinValue };
            int* dst = stackalloc int[5];
            IAFahim.Compress.RestoreCompressed.Run(src, dst, 5);
            Assert.Equal(-1, dst[0]);
            Assert.Equal(0, dst[1]);
            Assert.Equal(1, dst[2]);
            Assert.Equal(int.MaxValue, dst[3]);
            Assert.Equal(int.MinValue, dst[4]);
        }
    }
}