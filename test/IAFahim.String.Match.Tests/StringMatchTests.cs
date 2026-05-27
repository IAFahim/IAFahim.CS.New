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
            ZAlgorithm.Run(null, 0, z);
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
    }
}
