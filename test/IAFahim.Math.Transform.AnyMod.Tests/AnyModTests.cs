namespace IAFahim.Math.Transform.AnyMod.Tests
{
    using NUnit.Framework;

    public sealed unsafe class AnyModTests
    {
        [Test]
        public void Convolution_Small_MatchesNaive()
        {
            const long Mod = 1000000007;
            long* a = stackalloc long[3];
            long* b = stackalloc long[2];
            long* res = stackalloc long[8];
            a[0] = 1; a[1] = 2; a[2] = 3;
            b[0] = 4; b[1] = 5;
            int len = ArbitraryModConvolution.Run(a, 3, b, 2, res, Mod);
            Assert.AreEqual(4, len);
            Assert.AreEqual(4L % Mod, res[0]);
            Assert.AreEqual(13L % Mod, res[1]);
            Assert.AreEqual(22L % Mod, res[2]);
            Assert.AreEqual(15L % Mod, res[3]);
        }
    }
}
