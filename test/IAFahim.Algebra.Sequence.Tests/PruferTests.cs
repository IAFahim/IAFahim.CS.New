namespace IAFahim.Algebra.Sequence.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class PruferTests
    {
        private const int MOD = 1000000007;

        // Prufer sequence is a length-(n-2) base-n number. Rank/Unrank are inverses.
        [Test]
        public void Rank_Empty_n2_IsZero()
        {
            int* seq = stackalloc int[0];
            Assert.AreEqual(0, Prufer.Rank(seq, 2, MOD));
        }

        [Test]
        public void Rank_SingleElement_n3()
        {
            // n=3, seq length 1. seq=[0] => 0, seq=[1] => 1, seq=[2] => 2.
            int* seq = stackalloc int[1];
            seq[0] = 0; Assert.AreEqual(0, Prufer.Rank(seq, 3, MOD));
            seq[0] = 1; Assert.AreEqual(1, Prufer.Rank(seq, 3, MOD));
            seq[0] = 2; Assert.AreEqual(2, Prufer.Rank(seq, 3, MOD));
        }

        [Test]
        public void Unrank_n3_ReversesRank()
        {
            int* seq = stackalloc int[1];
            for (int r = 0; r < 3; r++)
            {
                Prufer.Unrank(r, 3, MOD, seq);
                Assert.AreEqual(r, Prufer.Rank(seq, 3, MOD));
            }
        }

        [Test]
        public void Unrank_n5_ReversesRank()
        {
            int* seq = stackalloc int[3];
            Random rng = new Random(42);
            for (int trial = 0; trial < 50; trial++)
            {
                long r = rng.Next(125);
                Prufer.Unrank(r, 5, MOD, seq);
                Assert.AreEqual(r, Prufer.Rank(seq, 5, MOD));
            }
        }
    }
}
