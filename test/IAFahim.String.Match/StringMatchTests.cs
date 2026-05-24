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
        public void PrefixFunction_Empty_NoCrash()
        {
            int* pi = stackalloc int[1];
            PrefixFunction.Run(null, 0, pi);
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
                int dist = EditDistance.Hamming(pa, pb, 3);
                Assert.AreEqual(0, dist);
            }
        }

        [Test]
        public void EditDistance_Hamming_Different()
        {
            byte[] a = new byte[] { 97, 98, 99 };
            byte[] b = new byte[] { 97, 99, 99 };
            fixed (byte* pa = a, pb = b)
            {
                int dist = EditDistance.Hamming(pa, pb, 3);
                Assert.AreEqual(1, dist);
            }
        }

        [Test]
        public void EditDistance_Levenshtein_Same()
        {
            byte[] a = new byte[] { 97, 98, 99 };
            byte[] b = new byte[] { 97, 98, 99 };
            fixed (byte* pa = a, pb = b)
            {
                int dist = EditDistance.Levenshtein(pa, 3, pb, 3, 10);
                Assert.AreEqual(0, dist);
            }
        }

        [Test]
        public void EditDistance_Levenshtein_Delete()
        {
            byte[] a = new byte[] { 97, 98, 99 };
            byte[] b = new byte[] { 97, 99 };
            fixed (byte* pa = a, pb = b)
            {
                int dist = EditDistance.Levenshtein(pa, 3, pb, 2, 10);
                Assert.AreEqual(1, dist);
            }
        }

        [Test]
        public void PatternMatch_Abelian_Matching()
        {
            byte[] a = new byte[] { 97, 98, 98 };
            byte[] b = new byte[] { 98, 98, 97 };
            fixed (byte* pa = a, pb = b)
            {
                bool match = PatternMatch.Abelian(pa, 3, pb, 3);
                Assert.IsTrue(match);
            }
        }

        [Test]
        public void PatternMatch_Abelian_NonMatching()
        {
            byte[] a = new byte[] { 97, 98, 98 };
            byte[] b = new byte[] { 98, 97, 97 };
            fixed (byte* pa = a, pb = b)
            {
                bool match = PatternMatch.Abelian(pa, 3, pb, 3);
                Assert.IsFalse(match);
            }
        }

        [Test]
        public void PatternMatch_Parameterized_Matching()
        {
            byte[] a = new byte[] { 97, 98, 97 };
            byte[] b = new byte[] { 99, 100, 99 };
            fixed (byte* pa = a, pb = b)
            {
                bool match = PatternMatch.Parameterized(pa, 3, pb, 3);
                Assert.IsTrue(match);
            }
        }

        [Test]
        public void PatternMatch_Parameterized_NonMatching()
        {
            byte[] a = new byte[] { 97, 98, 97 };
            byte[] b = new byte[] { 97, 98, 98 };
            fixed (byte* pa = a, pb = b)
            {
                bool match = PatternMatch.Parameterized(pa, 3, pb, 3);
                Assert.IsFalse(match);
            }
        }
    }
}
