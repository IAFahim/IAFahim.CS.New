namespace IAFahim.DS.Trie.Tests
{
    using IAFahim.DS.Trie;
    using Xunit;

    public sealed unsafe class TrieTests
    {
        [Fact]
        public void InsertAndFind_Basic()
        {
            const int maxNodes = 64;
            int* trie = stackalloc int[maxNodes * 27];
            for (int i = 0; i < maxNodes * 27; i++) trie[i] = 0;
            trie[0] = 1;
            int root = 1;

            byte* word1 = stackalloc byte[3] { (byte)'a', (byte)'b', (byte)'c' };
            byte* word2 = stackalloc byte[2] { (byte)'a', (byte)'b' };
            byte* miss = stackalloc byte[2] { (byte)'a', (byte)'c' };

            TrieInsert.Run(trie, root, word1, 3);
            TrieInsert.Run(trie, root, word2, 2);

            Assert.True(TrieFind.Run(trie, root, word1, 3));
            Assert.True(TrieFind.Run(trie, root, word2, 2));
            Assert.False(TrieFind.Run(trie, root, miss, 2));
        }

        [Fact]
        public void Delete_RemovesWord()
        {
            const int maxNodes = 64;
            int* trie = stackalloc int[maxNodes * 27];
            for (int i = 0; i < maxNodes * 27; i++) trie[i] = 0;
            trie[0] = 1;
            int root = 1;

            byte* word = stackalloc byte[2] { (byte)'a', (byte)'b' };
            TrieInsert.Run(trie, root, word, 2);
            Assert.True(TrieFind.Run(trie, root, word, 2));
            Assert.True(TrieDelete.Run(trie, root, word, 2));
            Assert.False(TrieFind.Run(trie, root, word, 2));
        }

        [Fact]
        public void BinaryTrie_MaxMinXor_Smoke()
        {
            const int maxNodes = 64;
            int* trie = stackalloc int[maxNodes * 2];
            for (int i = 0; i < maxNodes * 2; i++) trie[i] = 0;
            trie[0] = 1;
            int root = 1;

            BinaryTrieInsert.Run(trie, root, 1, 1);
            BinaryTrieInsert.Run(trie, root, 0, 1);

            int maxXor = BinaryTrieMaxXor.Run(trie, root, 1, 1);
            int minXor = BinaryTrieMinXor.Run(trie, root, 1, 1);
            Assert.True(maxXor >= 0);
            Assert.True(minXor >= 0);

            BinaryTrieErase.Run(trie, root, 0, 1);
            int maxXorAfter = BinaryTrieMaxXor.Run(trie, root, 1, 1);
            Assert.True(maxXorAfter >= 0);
        }
    }
}
