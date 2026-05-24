namespace IAFahim.Compress.Tests
{
    using NUnit.Framework;

    public sealed unsafe class CompressValuesTests
    {
        [Test]
        public void EmptyInput_NoOp()
        {
            long* dst = stackalloc long[0];
            IAFahim.Compress.CompressValues.Run(null, dst, 0);
        }

        [Test]
        public void SingleValue_Compresses()
        {
            int* src = stackalloc int[] { 42 };
            long* dst = stackalloc long[1];
            IAFahim.Compress.CompressValues.Run(src, dst, 1);
            Assert.AreEqual(42, dst[0]);
        }

        [Test]
        public void MultipleValues_CompressesAll()
        {
            int* src = stackalloc int[] { -1, 0, 1, 2 };
            long* dst = stackalloc long[4];
            IAFahim.Compress.CompressValues.Run(src, dst, 4);
            Assert.AreEqual(-1, dst[0]);
            Assert.AreEqual(0, dst[1]);
            Assert.AreEqual(1, dst[2]);
            Assert.AreEqual(2, dst[3]);
        }

        [Test]
        public void LargeValues_PreservesValues()
        {
            int* src = stackalloc int[] { int.MaxValue, int.MinValue };
            long* dst = stackalloc long[2];
            IAFahim.Compress.CompressValues.Run(src, dst, 2);
            Assert.AreEqual(int.MaxValue, dst[0]);
            Assert.AreEqual(int.MinValue, dst[1]);
        }
    }

    public sealed unsafe class CompressValuesUniqueTests
    {
        [Test]
        public void EmptyInput_ReturnsZero()
        {
            long* dst = stackalloc long[0];
            int count = IAFahim.Compress.CompressValues.RunUnique(null, dst, 0);
            Assert.AreEqual(0, count);
        }

        [Test]
        public void SingleValue_ReturnsOne()
        {
            int* src = stackalloc int[] { 42 };
            long* dst = stackalloc long[1];
            int count = IAFahim.Compress.CompressValues.RunUnique(src, dst, 1);
            Assert.AreEqual(1, count);
            Assert.AreEqual(42, dst[0]);
        }

        [Test]
        public void AllUnique_ReturnsLen()
        {
            int* src = stackalloc int[] { 3, 1, 4, 1, 5, 9 };
            long* dst = stackalloc long[6];
            int count = IAFahim.Compress.CompressValues.RunUnique(src, dst, 6);
            Assert.AreEqual(6, count);
        }

        [Test]
        public void AllDuplicates_ReturnsOne()
        {
            int* src = stackalloc int[] { 7, 7, 7 };
            long* dst = stackalloc long[3];
            int count = IAFahim.Compress.CompressValues.RunUnique(src, dst, 3);
            Assert.AreEqual(1, count);
            Assert.AreEqual(7, dst[0]);
        }

        [Test]
        public void SomeDuplicates_Deduplicates()
        {
            int* src = stackalloc int[] { 1, 1, 2, 3, 3, 3, 4 };
            long* dst = stackalloc long[7];
            int count = IAFahim.Compress.CompressValues.RunUnique(src, dst, 7);
            Assert.AreEqual(4, count);
            Assert.AreEqual(1, dst[0]);
            Assert.AreEqual(2, dst[1]);
            Assert.AreEqual(3, dst[2]);
            Assert.AreEqual(4, dst[3]);
        }
    }
}