namespace IAFahim.Search.Automaton.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class ModMatrixPowTests
    {
        [Test]
        public void Identity_Power()
        {
            const int N = 2;
            long* a = stackalloc long[N * N];
            long* res = stackalloc long[N * N];
            a[0] = 1; a[1] = 0; a[2] = 0; a[3] = 1;
            ModMatrixPow.Run(N, a, res, 10, 1000000007);
            Assert.AreEqual(1, res[0]);
            Assert.AreEqual(0, res[1]);
            Assert.AreEqual(0, res[2]);
            Assert.AreEqual(1, res[3]);
        }

        [Test]
        public void FibonacciMatrix_Power()
        {
            const int N = 2;
            const long Mod = 1000000007;
            long* a = stackalloc long[N * N];
            long* res = stackalloc long[N * N];
            a[0] = 1; a[1] = 1; a[2] = 1; a[3] = 0;
            ModMatrixPow.Run(N, a, res, 5, Mod);
            Assert.AreEqual(8, res[0]);
            Assert.AreEqual(5, res[1]);
            Assert.AreEqual(5, res[2]);
            Assert.AreEqual(3, res[3]);
        }
    }
}
