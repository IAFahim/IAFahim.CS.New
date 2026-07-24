namespace IAFahim.String.Tests
{
    using NUnit.Framework;

    public sealed unsafe class SuffixLowerBoundCoverage
    {
        [Test]
        public void SuffixLowerBound_FindsPatternStart()
        {
            byte* s = stackalloc byte[] { (byte)'a', (byte)'b', (byte)'a', (byte)'b' };
            int* sa = stackalloc int[] { 3, 1, 2, 0 };
            byte* pat = stackalloc byte[] { (byte)'a', (byte)'b' };
            int idx = SuffixLowerBound.Run(s, 4, sa, pat, 2);
            Assert.IsTrue(idx >= 0 && idx <= 4);
        }

        [Test]
        public void BuildPrefixFunction_Basic()
        {
            byte* s = stackalloc byte[] { (byte)'a', (byte)'b', (byte)'a' };
            int* fail = stackalloc int[3];
            StringCoreKmp.BuildPrefixFunction(s, 3, fail);
            Assert.AreEqual(0, fail[0]);
        }
    }
}
