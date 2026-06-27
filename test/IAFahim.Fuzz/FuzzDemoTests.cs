namespace IAFahim.Fuzz
{
    using IAFahim.Search.RangeQueries;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    // Demonstration: fuzz RangeDistinctCount against a naive oracle. Proves the harness runs and
    // would surface off-by-one/overflow bugs the 2-judge pass can miss. Add one such test per
    // high-risk module (onboarding list in FIXES_BACKLOG §N5).
    public sealed unsafe class FuzzDemoTests
    {
        [Test]
        public void RangeDistinctCount_FuzzVsBrute()
        {
            FuzzRunner.AssertMatchesQuery(
                fast: (src, n, a, b, key) => RangeDistinctCount.Run(src, n, a, b),
                naive: (src, n, a, b, key) => BruteDistinct(src, n, a, b),
                gen: (rng, src, n, out a, out b, out key) =>
                {
                    FuzzRunner.GenUniform(rng, src, n, 0, 12);
                    FuzzRunner.GenRangeQuery(rng, src, n, out a, out b, out key);
                },
                iterations: 2000, seed: 1337, maxN: 64);
        }

        private static long BruteDistinct(int* src, int n, int a, int b)
        {
            if (a > b) return 0;
            int count = 0;
            for (int i = a; i <= b; i++)
            {
                bool seen = false;
                for (int j = a; j < i; j++) if (src[j] == src[i]) { seen = true; break; }
                if (!seen) count++;
            }
            return count;
        }
    }
}
