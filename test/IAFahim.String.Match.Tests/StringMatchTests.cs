namespace IAFahim.String.Match.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class StringMatchTests
    {
        [Test]
        public void ZAlgorithm_Empty_NoCrash()
        {
            int* z = stackalloc int[1];
            ZAlgorithm.Run((byte*)null, 0, z);
        }

        [Test]
        public void ZAlgorithm_SingleChar_ZIsLength()
        {
            byte[] arr = new byte[] { 97 };
            fixed (byte* ptr = arr)
            {
                int* z = stackalloc int[1];
                ZAlgorithm.Run(ptr, 1, z);
                Assert.AreEqual(1, z[0]);
            }
        }

        [Test]
        public void ZAlgorithm_AllSame_ZCorrect()
        {
            byte[] arr = new byte[] { 97, 97, 97, 97 };
            fixed (byte* ptr = arr)
            {
                int* z = stackalloc int[4];
                ZAlgorithm.Run(ptr, 4, z);
                Assert.AreEqual(4, z[0]);
                Assert.AreEqual(3, z[1]);
                Assert.AreEqual(2, z[2]);
                Assert.AreEqual(1, z[3]);
            }
        }

        [Test]
        public void PrefixFunction_AllSame_PiCorrect()
        {
            byte[] arr = new byte[] { 97, 97, 97 };
            fixed (byte* ptr = arr)
            {
                int* pi = stackalloc int[3];
                PrefixFunction.Run(ptr, 3, pi);
                Assert.AreEqual(0, pi[0]);
                Assert.AreEqual(1, pi[1]);
                Assert.AreEqual(2, pi[2]);
            }
        }

        [Test]
        public void PrefixFunction_AbPattern_PiCorrect()
        {
            byte[] arr = new byte[] { 97, 98, 97 };
            fixed (byte* ptr = arr)
            {
                int* pi = stackalloc int[3];
                PrefixFunction.Run(ptr, 3, pi);
                Assert.AreEqual(0, pi[0]);
                Assert.AreEqual(0, pi[1]);
                Assert.AreEqual(1, pi[2]);
            }
        }

        [Test]
        public void EditDistance_Hamming_Same()
        {
            byte[] a = new byte[] { 97, 98, 99 };
            byte[] b = new byte[] { 97, 98, 99 };
            fixed (byte* pa = a, pb = b)
            {
                Assert.AreEqual(0, EditDistance.Hamming(pa, pb, 3));
            }
        }

        [Test]
        public void EditDistance_Hamming_Different()
        {
            byte[] a = new byte[] { 97, 98, 99 };
            byte[] b = new byte[] { 97, 99, 99 };
            fixed (byte* pa = a, pb = b)
            {
                Assert.AreEqual(1, EditDistance.Hamming(pa, pb, 3));
            }
        }

        [Test]
        public void EditDistance_Levenshtein_Same()
        {
            byte[] a = new byte[] { 97, 98, 99 };
            byte[] b = new byte[] { 97, 98, 99 };
            fixed (byte* pa = a, pb = b)
            {
                Assert.AreEqual(0, EditDistance.Levenshtein(pa, 3, pb, 3, 10));
            }
        }

        [Test]
        public void EditDistance_Levenshtein_Delete()
        {
            byte[] a = new byte[] { 97, 98, 99 };
            byte[] b = new byte[] { 97, 99 };
            fixed (byte* pa = a, pb = b)
            {
                Assert.AreEqual(1, EditDistance.Levenshtein(pa, 3, pb, 2, 10));
            }
        }

        [Test]
        public void RollingHash_Compute_SameStrings()
        {
            byte[] a = new byte[] { 97, 98, 99 };
            byte[] b = new byte[] { 97, 98, 99 };
            fixed (byte* pa = a, pb = b)
            {
                Assert.AreEqual(RollingHash.Compute(pa, 3), RollingHash.Compute(pb, 3));
            }
        }

        [Test]
        public void RollingHash_Compute_DifferentStrings()
        {
            byte[] a = new byte[] { 97, 98, 99 };
            byte[] b = new byte[] { 97, 98, 100 };
            fixed (byte* pa = a, pb = b)
            {
                Assert.AreNotEqual(RollingHash.Compute(pa, 3), RollingHash.Compute(pb, 3));
            }
        }

        [Test]
        public void RollingHash_Query_Substring()
        {
            byte[] arr = new byte[] { 97, 98, 99, 100, 101 };
            fixed (byte* ptr = arr)
            {
                ulong* prefix = stackalloc ulong[6];
                ulong* power = stackalloc ulong[6];
                RollingHash.Build(ptr, 5, prefix, power);
                ulong h1 = RollingHash.Query(prefix, power, 0, 3);
                ulong h2 = RollingHash.Query(prefix, power, 2, 5);
                Assert.AreNotEqual(h1, h2);
            }
        }

        [Test]
        public void PatternMatch_Abelian_Matching()
        {
            byte[] a = new byte[] { 97, 98, 98 };
            byte[] b = new byte[] { 98, 98, 97 };
            fixed (byte* pa = a, pb = b)
            {
                int* cntA = stackalloc int[256];
                int* cntB = stackalloc int[256];
                Assert.IsTrue(PatternMatch.Abelian(pa, 3, pb, 3, cntA, cntB));
            }
        }

        [Test]
        public void PatternMatch_Abelian_NonMatching()
        {
            byte[] a = new byte[] { 97, 98, 98 };
            byte[] b = new byte[] { 98, 97, 97 };
            fixed (byte* pa = a, pb = b)
            {
                int* cntA = stackalloc int[256];
                int* cntB = stackalloc int[256];
                Assert.IsFalse(PatternMatch.Abelian(pa, 3, pb, 3, cntA, cntB));
            }
        }

        [Test]
        public void PatternMatch_Parameterized_Matching()
        {
            byte[] a = new byte[] { 97, 98, 97 };
            byte[] b = new byte[] { 99, 100, 99 };
            fixed (byte* pa = a, pb = b)
            {
                int* mapA = stackalloc int[3]; int* mapB = stackalloc int[3];
                Assert.IsTrue(PatternMatch.Parameterized(pa, 3, pb, 3, mapA, mapB));
            }
        }

        [Test]
        public void MainLorentz_FindRuns()
        {
            byte[] arr = new byte[] { 97, 97, 97, 97 };
            fixed (byte* ptr = arr)
            {
                MainLorentz.Run* runs = stackalloc MainLorentz.Run[4];
                int count = MainLorentz.Find(ptr, 4, runs);
                Assert.IsTrue(count > 0);
            }
        }

        [Test]
        public void Crochemore_FindReps()
        {
            byte[] arr = new byte[] { 97, 97, 97, 97 };
            fixed (byte* ptr = arr)
            {
                Crochemore.Repetition* reps = stackalloc Crochemore.Repetition[4];
                int count = Crochemore.Find(ptr, 4, reps);
                Assert.IsTrue(count > 0);
            }
        }

        [Test]
        public void ApproximateMatch_LandauVishkin()
        {
            byte[] text = new byte[] { (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e' };
            byte[] pattern = new byte[] { (byte)'b', (byte)'x', (byte)'d' };
            int* results = stackalloc int[5];
            int count = 0;
            int* curr = stackalloc int[4];
            int* prev = stackalloc int[4];
            for (int i = 0; i < 4; i++) curr[i] = prev[i] = 0;
            fixed (byte* pt = text, pp = pattern)
            {
                ApproximateMatch.LandauVishkin(pt, 5, pp, 3, 1, results, &count, curr, prev);
            }
            Assert.AreEqual(1, count);
            Assert.AreEqual(1, results[0]);
        }

        [Test]
        public void AhoCorasick_BuildAndSearch()
        {
            const int sigma = 256;
            AhoCorasick.State* st = stackalloc AhoCorasick.State[100];
            int size = 0;
            AhoCorasick.Build(st, ref size, sigma);
            
            int* pat1 = stackalloc int[2] { (int)'h', (int)'e' };
            AhoCorasick.AddPattern(st, ref size, sigma, pat1, 2, 1);
            
            int* pat2 = stackalloc int[3] { (int)'s', (int)'h', (int)'e' };
            AhoCorasick.AddPattern(st, ref size, sigma, pat2, 3, 2);
            
            int* queue = stackalloc int[100];
            AhoCorasick.BuildLinks(st, size, sigma, queue);
            
            byte[] text = new byte[] { (byte)'s', (byte)'h', (byte)'e' };
            int* matches = stackalloc int[10];
            int matchCount = 0;
            fixed (byte* pt = text)
            {
                matchCount = AhoCorasick.Search(st, sigma, pt, 3, matches);
            }
            
            Assert.AreEqual(2, matchCount);
            Assert.AreEqual(2, matches[0]);
            Assert.AreEqual(1, matches[1]);
        }

        [Test]
        public void EditDistance_Ukkonen_WithinAndBeyondK()
        {
            byte[] a = new byte[] { (byte)'k', (byte)'i', (byte)'t', (byte)'t', (byte)'e', (byte)'n' };
            byte[] b = new byte[] { (byte)'s', (byte)'i', (byte)'t', (byte)'t', (byte)'i', (byte)'n', (byte)'g' };
            int* v = stackalloc int[16];
            fixed (byte* pa = a, pb = b)
            {
                Assert.IsTrue(EditDistance.Ukkonen(pa, 6, pb, 7, 3, v, null));
                Assert.IsFalse(EditDistance.Ukkonen(pa, 6, pb, 7, 2, v, null));
                Assert.IsTrue(EditDistance.Ukkonen(pa, 6, pa, 6, 0, v, null));
            }
        }

        [Test]
        public void MainLorentz_Find_AaaHasPeriodOneRun()
        {
            byte[] s = new byte[] { (byte)'a', (byte)'a', (byte)'a', (byte)'a' };
            MainLorentz.Run* runs = stackalloc MainLorentz.Run[16];
            fixed (byte* ps = s)
            {
                int c = MainLorentz.Find(ps, 4, runs);
                Assert.IsTrue(c >= 1);
                bool hasP1 = false;
                for (int i = 0; i < c; i++)
                    if (runs[i].Period == 1 && runs[i].Length >= 2) hasP1 = true;
                Assert.IsTrue(hasP1);
            }
        }

        [Test]
        public void Runs_Count_PlateauCountsOnce()
        {
            // Synthetic LCP: indices 1..3 share LCP>=2 plateau — must count once at left edge.
            int* lcp = stackalloc int[5] { 0, 2, 2, 2, 0 };
            int* sa = stackalloc int[5] { 0, 1, 2, 3, 4 };
            Assert.AreEqual(1, Runs.Count(lcp, sa, 5));
        }

        [Test]
        public void Runs_FindLyndonRuns_Repeating()
        {
            byte[] s = new byte[] { (byte)'a', (byte)'a', (byte)'a', (byte)'a' };
            int* starts = stackalloc int[8];
            int* lengths = stackalloc int[8];
            fixed (byte* ps = s)
            {
                int c = Runs.FindLyndonRuns(ps, 4, starts, lengths);
                Assert.IsTrue(c >= 1);
                Assert.IsTrue(lengths[0] >= 2);
            }
        }

        [Test]
        public void AhoOffline_Query_MatchesOnGoFail()
        {
            // Manual 2-state automaton: state 0 -'a'-> 1 (out=0), fail[1]=0
            int* go = stackalloc int[2 * 256];
            for (int i = 0; i < 2 * 256; i++) go[i] = -1;
            go[0 * 256 + (int)'a'] = 1;
            int* fail = stackalloc int[2] { 0, 0 };
            int* out_ = stackalloc int[2] { -1, 0 };
            int* matches = stackalloc int[8];
            byte[] text = new byte[] { (byte)'x', (byte)'a', (byte)'y', (byte)'a' };
            fixed (byte* pt = text)
            {
                int c = AhoOffline.Query(pt, 4, go, fail, out_, 1, matches);
                Assert.AreEqual(2, c);
                Assert.AreEqual(1, matches[0]);
                Assert.AreEqual(3, matches[1]);
            }
        }
    }
}
