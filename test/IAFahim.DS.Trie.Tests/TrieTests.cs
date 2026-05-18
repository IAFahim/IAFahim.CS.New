namespace IAFahim.DS.Trie.Tests
{
    using IAFahim.DS.Trie;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class TrieTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            TrieInsert.Run(null, 0, 0);
            Assert.False(TrieFind.Run(null, 0, 0));
        }

        [Fact]
        public void InsertAndFind_Basic()
        {
            const int maxNodes = 100;
            int* next0 = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int* next1 = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int* cnt = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            try
            {
                for (int i = 0; i < maxNodes; i++) { next0[i] = -1; next1[i] = -1; cnt[i] = 0; }
                int nodeCount = 1;

                TrieInsert.Run(&nodeCount, next0, next1, cnt, 1);
                TrieInsert.Run(&nodeCount, next0, next1, cnt, 2);
                TrieInsert.Run(&nodeCount, next0, next1, cnt, 3);

                Assert.True(TrieFind.Run(&nodeCount, next0, next1, cnt, 1));
                Assert.True(TrieFind.Run(&nodeCount, next0, next1, cnt, 2));
                Assert.True(TrieFind.Run(&nodeCount, next0, next1, cnt, 3));
                Assert.False(TrieFind.Run(&nodeCount, next0, next1, cnt, 4));
            }
            finally { Marshal.FreeHGlobal((nint)next0); Marshal.FreeHGlobal((nint)next1); Marshal.FreeHGlobal((nint)cnt); }
        }

        [Fact]
        public void PrefixCount_AfterMultipleInserts()
        {
            const int maxNodes = 100;
            int* next0 = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int* next1 = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int* cnt = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            try
            {
                for (int i = 0; i < maxNodes; i++) { next0[i] = -1; next1[i] = -1; cnt[i] = 0; }
                int nodeCount = 1;

                TrieInsert.Run(&nodeCount, next0, next1, cnt, 1);
                TrieInsert.Run(&nodeCount, next0, next1, cnt, 2);
                TrieInsert.Run(&nodeCount, next0, next1, cnt, 3);
                TrieInsert.Run(&nodeCount, next0, next1, cnt, 1);

                Assert.Equal(2, TriePrefixCount.Run(&nodeCount, next0, next1, cnt, 1));
            }
            finally { Marshal.FreeHGlobal((nint)next0); Marshal.FreeHGlobal((nint)next1); Marshal.FreeHGlobal((nint)cnt); }
        }

        [Fact]
        public void BinaryTrie_MaxXor()
        {
            const int maxNodes = 1000;
            int* child = (int*)Marshal.AllocHGlobal(maxNodes * 2 * sizeof(int));
            int* cnt = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            try
            {
                for (int i = 0; i < maxNodes * 2; i++) child[i] = -1;
                for (int i = 0; i < maxNodes; i++) cnt[i] = 0;
                int nodeCount = 1;

                int* vals = stackalloc int[5];
                vals[0] = 1; vals[1] = 4; vals[2] = 5; vals[3] = 7; vals[4] = 9;
                for (int i = 0; i < 5; i++)
                    BinaryTrieInsert.Run(&nodeCount, child, cnt, vals[i], 31);

                int x = 6;
                int result = BinaryTrieMaxXor.Run(&nodeCount, child, cnt, x, 31);
                Assert.True(result >= x);
            }
            finally { Marshal.FreeHGlobal((nint)child); Marshal.FreeHGlobal((nint)cnt); }
        }

        [Fact]
        public void BinaryTrie_Erase()
        {
            const int maxNodes = 100;
            int* child = (int*)Marshal.AllocHGlobal(maxNodes * 2 * sizeof(int));
            int* cnt = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            try
            {
                for (int i = 0; i < maxNodes * 2; i++) child[i] = -1;
                for (int i = 0; i < maxNodes; i++) cnt[i] = 0;
                int nodeCount = 1;

                BinaryTrieInsert.Run(&nodeCount, child, cnt, 5, 31);
                BinaryTrieInsert.Run(&nodeCount, child, cnt, 5, 31);
                BinaryTrieErase.Run(&nodeCount, child, cnt, 5, 31);
                Assert.True(BinaryTrieFind.Run(&nodeCount, child, cnt, 5, 31));
                BinaryTrieErase.Run(&nodeCount, child, cnt, 5, 31);
                Assert.False(BinaryTrieFind.Run(&nodeCount, child, cnt, 5, 31));
            }
            finally { Marshal.FreeHGlobal((nint)child); Marshal.FreeHGlobal((nint)cnt); }
        }
    }
}