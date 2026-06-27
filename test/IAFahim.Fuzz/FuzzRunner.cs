namespace IAFahim.Fuzz
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    // Seeded RNG input generator + reference-matching runner. Use to fuzz unsafe pointer-based
    // implementations against a slow naive oracle. Deterministic (fixed seed) so a failure is
    // reproducible. Onboard the highest-risk untested unsafe modules first; each gets one
    // [Test] that calls AssertMatchesReference with an appropriate input generator.
    public static unsafe class FuzzRunner
    {
        // Compare a fast int->int pointer transform against a naive oracle over many random inputs.
        // fast/naive both read src[0..n) and write dst[0..n); the runner asserts byte-equality.
        public static void AssertMatchesReference(
            FuzzTransform fast, FuzzTransform naive,
            InputGen gen, int iterations, int seed, int maxN)
        {
            Random rng = new Random(seed);
            for (int it = 0; it < iterations; it++)
            {
                int n = rng.Next(0, maxN + 1);
                int* src = (int*)Marshal.AllocHGlobal(sizeof(int) * (n > 0 ? n : 1));
                int* fastDst = (int*)Marshal.AllocHGlobal(sizeof(int) * (n > 0 ? n : 1));
                int* naiveDst = (int*)Marshal.AllocHGlobal(sizeof(int) * (n > 0 ? n : 1));
                try
                {
                    gen(rng, src, n);
                    for (int i = 0; i < n; i++) { fastDst[i] = 0; naiveDst[i] = 0; }
                    fast(src, n, fastDst);
                    naive(src, n, naiveDst);
                    for (int i = 0; i < n; i++)
                        Assert.AreEqual(naiveDst[i], fastDst[i],
                            $"fuzz divergence iter={it} n={n} idx={i} (seed={seed})");
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)src);
                    Marshal.FreeHGlobal((nint)fastDst);
                    Marshal.FreeHGlobal((nint)naiveDst);
                }
            }
        }

        // Compare a scalar int->int query (e.g. a range query) against an oracle.
        public static void AssertMatchesQuery(
            FuzzQuery fast, FuzzQuery naive,
            QueryGen gen, int iterations, int seed, int maxN)
        {
            Random rng = new Random(seed);
            for (int it = 0; it < iterations; it++)
            {
                int n = rng.Next(1, maxN + 1);
                int* src = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
                try
                {
                    gen(rng, src, n, out int a, out int b, out long key);
                    long fastVal = fast(src, n, a, b, key);
                    long naiveVal = naive(src, n, a, b, key);
                    Assert.AreEqual(naiveVal, fastVal, $"fuzz query divergence iter={it} n={n} a={a} b={b} key={key} (seed={seed})");
                }
                finally { Marshal.FreeHGlobal((nint)src); }
            }
        }

        public delegate void FuzzTransform(int* src, int n, int* dst);
        public delegate long FuzzQuery(int* src, int n, int a, int b, long key);
        public delegate void InputGen(Random rng, int* dst, int n);
        public delegate void QueryGen(Random rng, int* src, int n, out int a, out int b, out long key);

        public static void GenUniform(Random rng, int* dst, int n, int lo, int hi)
        {
            for (int i = 0; i < n; i++) dst[i] = rng.Next(lo, hi);
        }

        public static void GenUniformByte(Random rng, int* dst, int n)
        {
            for (int i = 0; i < n; i++) dst[i] = rng.Next(0, 256);
        }

        public static void GenRangeQuery(Random rng, int* src, int n, out int a, out int b, out long key)
        {
            int lo = rng.Next(0, n), hi = rng.Next(0, n);
            a = lo <= hi ? lo : hi;
            b = lo <= hi ? hi : lo;
            key = rng.Next(-1000, 1000);
        }
    }
}
