namespace IAFahim.String.Tests
{
    using IAFahim.String.SuffixArray;
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class DynamicSuffixArrayTests
    {
        [Fact]
        public void DynamicSuffixArray_Empty_NoOp()
        {
            DynamicStringNode* root = null;
            ulong* powers = stackalloc ulong[10];
            powers[0] = 1;
            for (int i = 1; i < 10; i++) powers[i] = powers[i - 1] * DynamicSuffixArray.BASE;

            ulong h = DynamicSuffixArray.GetSubstringHash(ref root, 0, 0, powers);
            Assert.Equal(0ul, h);
        }

        [Fact]
        public void DynamicSuffixArray_InsertAndCompare()
        {
            const int N = 5; // "ababa"
            byte* str = stackalloc byte[N];
            str[0] = (byte)'a';
            str[1] = (byte)'b';
            str[2] = (byte)'a';
            str[3] = (byte)'b';
            str[4] = (byte)'a';

            ulong* powers = stackalloc ulong[N + 2];
            powers[0] = 1;
            for (int i = 1; i <= N; i++) powers[i] = powers[i - 1] * DynamicSuffixArray.BASE;

            DynamicStringNode* nodes = (DynamicStringNode*)Marshal.AllocHGlobal(N * sizeof(DynamicStringNode));
            DynamicStringNode* root = null;
            Random rng = new Random(42);

            try
            {
                for (int i = 0; i < N; i++)
                {
                    nodes[i].Priority = rng.Next();
                    nodes[i].Size = 1;
                    nodes[i].Value = str[i];
                    nodes[i].Hash = str[i];
                    nodes[i].Left = null;
                    nodes[i].Right = null;
                    DynamicSuffixArray.Insert(ref root, i, &nodes[i], powers);
                }

                // Substring hashes:
                ulong h1 = DynamicSuffixArray.GetSubstringHash(ref root, 0, 2, powers); // "aba"
                ulong h2 = DynamicSuffixArray.GetSubstringHash(ref root, 2, 4, powers); // "aba"
                Assert.Equal(h1, h2);

                // Lcp
                int lcp = DynamicSuffixArray.Lcp(ref root, 0, 2, powers);
                Assert.Equal(3, lcp);

                // Compare: "ababa" (i=0) vs "aba" (i=2)
                // "ababa" > "aba", but wait, they are compared character by character.
                // length of suffix 0 is 5. length of suffix 2 is 3. 
                // "ababa" vs "aba": "ababa" is longer, so it's >. But CompareSuffix logic:
                // If j + lcp == n, return 1.
                int cmp = DynamicSuffixArray.CompareSuffix(ref root, 0, 2, powers);
                Assert.Equal(1, cmp); // 0 > 2

                int cmp2 = DynamicSuffixArray.CompareSuffix(ref root, 1, 3, powers); // "baba" vs "ba"
                Assert.Equal(1, cmp2);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)nodes);
            }
        }
    }
}
