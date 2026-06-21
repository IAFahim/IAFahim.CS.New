namespace IAFahim.Graph.Eertree.Tests
{
    using NUnit.Framework;

    // Key invariant: nodeCount - 2 == number of distinct palindromic substrings.
    // (Nodes 0 and 1 are the two sentinel roots: odd-length -1, even-length 0.)
    public sealed unsafe class EertreeTests
    {
        private static int BuildCount(int* s, int len, int maxNodes, int maxEdges)
        {
            Eertree.Node* nodes = stackalloc Eertree.Node[maxNodes];
            Eertree.Next* next = stackalloc Eertree.Next[maxEdges];
            int nodeCount = 0, nextCount = 0, last = 0, cur = 0;
            Eertree.Build(s, len, nodes, next, ref nodeCount, ref nextCount, ref last, ref cur);
            return nodeCount;
        }

        [Test]
        public void Empty_OnlyTwoRoots()
        {
            int* s = stackalloc int[0];
            int nc = BuildCount(s, 0, 4, 4);
            Assert.AreEqual(2, nc);
        }

        [Test]
        public void SingleChar_OnePalindrome()
        {
            int* s = stackalloc int[1] { (int)'a' };
            int nc = BuildCount(s, 1, 8, 8);
            Assert.AreEqual(3, nc); // 2 roots + "a"
        }

        [Test]
        public void AllSameChars_ThreePalindromes()
        {
            // "aaa": palindromes = {a, aa, aaa} = 3.
            int* s = stackalloc int[3] { (int)'a', (int)'a', (int)'a' };
            int nc = BuildCount(s, 3, 16, 16);
            Assert.AreEqual(5, nc); // 2 roots + 3 palindromes
        }

        [Test]
        public void AllDistinct_NoLongPalindromes()
        {
            // "abc": palindromes = {a, b, c} = 3.
            int* s = stackalloc int[3] { (int)'a', (int)'b', (int)'c' };
            int nc = BuildCount(s, 3, 16, 16);
            Assert.AreEqual(5, nc);
        }

        [Test]
        public void Abba_FourPalindromes()
        {
            // "abba": palindromes = {a, b, bb, abba} = 4.
            int* s = stackalloc int[4] { (int)'a', (int)'b', (int)'b', (int)'a' };
            int nc = BuildCount(s, 4, 16, 16);
            Assert.AreEqual(6, nc);
        }

        [Test]
        public void Ababa_FourPalindromes()
        {
            // "ababa": palindromes = {a, b, aba, ababa} = 4. (bab is NOT a substring)
            // Wait: positions: a(0)b(1)a(2)b(3)a(4). "bab" at positions 1-3 = "bab" YES.
            // So palindromes = {a, b, aba, bab, ababa} = 5.
            int* s = stackalloc int[5] { (int)'a', (int)'b', (int)'a', (int)'b', (int)'a' };
            int nc = BuildCount(s, 5, 16, 16);
            Assert.AreEqual(7, nc); // 2 roots + {a, b, aba, bab, ababa}
        }
    }
}
