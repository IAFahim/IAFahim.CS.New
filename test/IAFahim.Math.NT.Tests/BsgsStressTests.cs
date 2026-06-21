namespace IAFahim.Math.NT.Tests
{
    using NUnit.Framework;

    public sealed unsafe class BsgsStressTests
    {
        // Small prime where brute force covers the full multiplicative order.
        // mod=101, a=2 has order 100 (2 is a primitive root mod 101).
        private const long MOD = 101L;
        private const int ORDER = 100;

        private static long BruteLog(long a, long b, long m)
        {
            long cur = 1 % m;
            for (int x = 0; x < m; x++)
            {
                if (cur == b % m) return x;
                cur = cur * a % m;
            }
            return -1;
        }

        // Every b in [1, 100] has a unique discrete log in [0, 100). BSGS must find it.
        [Test]
        public void Bsgs_EveryTarget_HasSolution()
        {
            int m = 1;
            while ((long)m * m < MOD) m++;
            long* sk = stackalloc long[m + 16];
            long* sv = stackalloc long[m + 16];
            int misses = 0;
            for (long b = 1; b < MOD; b++)
            {
                long got = Bsgs.Run(2L, b, MOD, sk, sv);
                long brute = BruteLog(2L, b, MOD);
                Assert.AreNotEqual(-1, brute, $"b={b}: brute should always find a solution");
                if (got == -1)
                {
                    misses++;
                    TestContext.WriteLine($"b={b}: BSGS returned -1 but brute found x={brute}");
                }
                else
                {
                    long check = ModPowSlow(2L, got, MOD);
                    Assert.AreEqual(b, check, $"b={b}: BSGS returned {got} but 2^{got}={check} != {b}");
                }
            }
            Assert.AreEqual(0, misses, $"BSGS missed {misses} solvable targets (hash table overflow)");
        }

        private static long ModPowSlow(long b, long e, long m)
        {
            b %= m; if (b < 0) b += m;
            long r = 1 % m;
            while (e > 0)
            {
                if ((e & 1) != 0) r = r * b % m;
                b = b * b % m;
                e >>= 1;
            }
            return r;
        }
    }
}
