namespace IAFahim.Compress.Tests
{
    using NUnit.Framework;

    public sealed unsafe class RestoreCompressedTests
    {
        [Test]
        public void EmptyInput_NoOp()
        {
            int* dst = stackalloc int[0];
            IAFahim.Compress.RestoreCompressed.Run(null, dst, 0);
        }

        [Test]
        public void SingleValue_Restores()
        {
            long* src = stackalloc long[] { 42 };
            int* dst = stackalloc int[1];
            IAFahim.Compress.RestoreCompressed.Run(src, dst, 1);
            Assert.AreEqual(42, dst[0]);
        }

        [Test]
        public void MultipleValues_RestoresAll()
        {
            long* src = stackalloc long[] { -1, 0, 1, int.MaxValue, int.MinValue };
            int* dst = stackalloc int[5];
            IAFahim.Compress.RestoreCompressed.Run(src, dst, 5);
            Assert.AreEqual(-1, dst[0]);
            Assert.AreEqual(0, dst[1]);
            Assert.AreEqual(1, dst[2]);
            Assert.AreEqual(int.MaxValue, dst[3]);
            Assert.AreEqual(int.MinValue, dst[4]);
        }
    }
}