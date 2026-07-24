namespace IAFahim.Math.Transform.Ntt.Tests
{
    using NUnit.Framework;

    public sealed unsafe class NttTests
    {
        private const long Mod = 998244353;
        private const long G = 3;

        [Test]
        public void Convolution_Identity()
        {
            long* a = stackalloc long[3];
            long* b = stackalloc long[2];
            long* res = stackalloc long[8];
            a[0] = 1; a[1] = 2; a[2] = 3;
            b[0] = 4; b[1] = 5;
            int len = NttConvolution.Run(a, 3, b, 2, res, Mod, G);
            Assert.AreEqual(4, len);
            Assert.AreEqual(4L, res[0]);
            Assert.AreEqual(13L, res[1]);
            Assert.AreEqual(22L, res[2]);
            Assert.AreEqual(15L, res[3]);
        }

        [Test]
        public void ForwardInverse_Recovers()
        {
            const int N = 8;
            long* a = stackalloc long[N];
            long* orig = stackalloc long[N];
            long* roots = stackalloc long[N];
            long* invRoots = stackalloc long[N];
            for (int i = 0; i < N; i++)
            {
                a[i] = (i * 17 + 3) % Mod;
                orig[i] = a[i];
            }
            NttInit.Run(3, Mod, G, roots, invRoots);
            NttTransform.Forward(a, N, Mod, roots);
            NttTransform.Inverse(a, N, Mod, invRoots);
            for (int i = 0; i < N; i++)
                Assert.AreEqual(orig[i], a[i]);
        }
    }
}
