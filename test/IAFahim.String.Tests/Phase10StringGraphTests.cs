namespace IAFahim.String.Tests
{
    using IAFahim.String;
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class Phase10StringGraphTests
    {
        private static long NonZeroPoly(long* x, long n)
        {
            return (x[0] - 2) * (x[1] - 3);
        }

        private static long ZeroPoly(long* x, long n)
        {
            return 0;
        }

        [Fact]
        public void ShortestCommonSupersequence_EmptyInput_NoOp()
        {
            byte* a = null;
            byte* b = null;
            byte* c = null;
            int len = Enumeration.ShortestCommonSupersequence(a, 0, b, 0, c);
            Assert.Equal(0, len);
        }

        [Fact]
        public void ShortestCommonSupersequence_Basic()
        {
            byte* a = stackalloc byte[2] { (byte)'a', (byte)'b' };
            byte* b = stackalloc byte[2] { (byte)'b', (byte)'a' };
            byte* c = stackalloc byte[4];
            int len = Enumeration.ShortestCommonSupersequence(a, 2, b, 2, c);
            Assert.Equal(3, len);
            // Possible SCS: "aba" or "bab"
            bool match1 = c[0] == 'a' && c[1] == 'b' && c[2] == 'a';
            bool match2 = c[0] == 'b' && c[1] == 'a' && c[2] == 'b';
            Assert.True(match1 || match2);
        }

        [Fact]
        public void ShortestAbsentSubsequence_EmptyInput_NoOp()
        {
            byte* s = null;
            byte* result = stackalloc byte[2];
            int len = Enumeration.ShortestAbsentSubsequence(s, 0, 2, result);
            Assert.Equal(1, len);
            Assert.Equal(0, result[0]);
        }

        [Fact]
        public void ShortestAbsentSubsequence_Basic()
        {
            byte* s = stackalloc byte[2] { 0, 1 }; // "ab"
            byte* result = stackalloc byte[4];
            int len = Enumeration.ShortestAbsentSubsequence(s, 2, 2, result);
            Assert.Equal(2, len);
            // Lexicographically first shortest absent subsequence of "ab" with alphabet size 2 is "aa" (0, 0)
            Assert.Equal(0, result[0]);
            Assert.Equal(0, result[1]);
        }

        [Fact]
        public void ShortestMissingSubstring_EmptyInput_NoOp()
        {
            byte* s = null;
            byte* result = stackalloc byte[2];
            int len = Enumeration.ShortestMissingSubstring(s, 0, 2, result);
            Assert.Equal(1, len);
            Assert.Equal(0, result[0]);
        }

        [Fact]
        public void ShortestMissingSubstring_Basic()
        {
            byte* s = stackalloc byte[3] { 0, 1, 0 }; // "aba"
            byte* result = stackalloc byte[4];
            int len = Enumeration.ShortestMissingSubstring(s, 3, 2, result);
            Assert.Equal(2, len);
            // Missing length 2: "aa" (0,0) and "bb" (1,1). Lexicographically first is "aa".
            Assert.Equal(0, result[0]);
            Assert.Equal(0, result[1]);
        }

        [Fact]
        public void DeBruijn_SequenceBuild_Basic()
        {
            int n = 3;
            int k = 2;
            int* sequence = stackalloc int[16];
            int seqLen = 0;
            DeBruijn.SequenceBuild(n, k, sequence, &seqLen);
            Assert.Equal(8, seqLen);
        }

        [Fact]
        public void DeBruijn_GraphBuild_Basic()
        {
            int n = 3;
            int k = 2;
            // Vertices = 2^(3-1) = 4. Adjacency size = 4 * 2 = 8
            int* adj = stackalloc int[8];
            DeBruijn.GraphBuild(n, k, adj);
            for (int u = 0; u < 4; u++)
            {
                int v0 = adj[u * k + 0];
                int v1 = adj[u * k + 1];
                Assert.Equal((u * 2) % 4, v0);
                Assert.Equal((u * 2 + 1) % 4, v1);
            }
        }

        [Fact]
        public void DeBruijn_EulerianPath_Basic()
        {
            int n = 3;
            int k = 2;
            int* path = stackalloc int[16];
            int pathLen = 0;
            DeBruijn.EulerianPath(n, k, path, &pathLen);
            // Eulerian path of de bruijn graph of order n has V * k + 1 vertices (or V * k edges visited)
            // Vertices = k^(n-1) = 4, edges = 8, path length should be 9
            Assert.Equal(9, pathLen);
        }

        [Fact]
        public void FreivaldsMatrixVerify_CorrectMatrixProduct_ReturnsTrue()
        {
            int n = 2;
            long* a = stackalloc long[4] { 1, 2, 3, 4 };
            long* b = stackalloc long[4] { 5, 6, 7, 8 };
            // A * B = [19, 22; 43, 50]
            long* c = stackalloc long[4] { 19, 22, 43, 50 };
            bool result = Probabilistic.FreivaldsMatrixVerify(a, b, c, n, 10, 1000000007L);
            Assert.True(result);
        }

        [Fact]
        public void FreivaldsMatrixVerify_IncorrectMatrixProduct_ReturnsFalse()
        {
            int n = 2;
            long* a = stackalloc long[4] { 1, 2, 3, 4 };
            long* b = stackalloc long[4] { 5, 6, 7, 8 };
            long* c = stackalloc long[4] { 19, 22, 43, 51 }; // Incorrect
            bool result = Probabilistic.FreivaldsMatrixVerify(a, b, c, n, 10, 1000000007L);
            Assert.False(result);
        }

        [Fact]
        public void SchwartzZippelTest_IdenticallyZero_ReturnsTrue()
        {
            bool result = Probabilistic.SchwartzZippelTest(&ZeroPoly, 2, 2, 10, 1000000007L);
            Assert.True(result);
        }

        [Fact]
        public void SchwartzZippelTest_NotIdenticallyZero_ReturnsFalse()
        {
            bool result = Probabilistic.SchwartzZippelTest(&NonZeroPoly, 2, 2, 100, 1000000007L);
            Assert.False(result);
        }

        [Fact]
        public void RabinKarpLasVegas_PatternFound_ReturnsCorrectIndex()
        {
            byte* text = stackalloc byte[5] { (byte)'a', (byte)'b', (byte)'a', (byte)'b', (byte)'a' };
            byte* pattern = stackalloc byte[3] { (byte)'b', (byte)'a', (byte)'b' };
            int idx = Probabilistic.RabinKarpLasVegas(text, 5, pattern, 3);
            Assert.Equal(1, idx);
        }

        [Fact]
        public void RabinKarpLasVegas_PatternNotFound_ReturnsMinusOne()
        {
            byte* text = stackalloc byte[5] { (byte)'a', (byte)'b', (byte)'a', (byte)'b', (byte)'a' };
            byte* pattern = stackalloc byte[3] { (byte)'x', (byte)'y', (byte)'z' };
            int idx = Probabilistic.RabinKarpLasVegas(text, 5, pattern, 3);
            Assert.Equal(-1, idx);
        }

        [Fact]
        public void RandomizedMstVerify_CorrectMst_ReturnsTrue()
        {
            int numVertices = 4;
            int numEdges = 5;
            int* u = stackalloc int[5] { 0, 0, 1, 1, 2 };
            int* v = stackalloc int[5] { 1, 2, 2, 3, 3 };
            long* w = stackalloc long[5] { 1, 3, 1, 4, 2 };
            bool* inMst = stackalloc bool[5] { true, false, true, false, true };
            // Tree edges are: (0,1) weight 1, (1,2) weight 1, (2,3) weight 2. Total weight 4.
            // Non-tree: (0,2) weight 3 -> path max (0-1-2) is max(1, 1) = 1 <= 3. Correct.
            // Non-tree: (1,3) weight 4 -> path max (1-2-3) is max(1, 2) = 2 <= 4. Correct.
            bool result = Probabilistic.RandomizedMstVerify(numVertices, numEdges, u, v, w, inMst);
            Assert.True(result);
        }

        [Fact]
        public void RandomizedMstVerify_IncorrectMst_ReturnsFalse()
        {
            int numVertices = 4;
            int numEdges = 5;
            int* u = stackalloc int[5] { 0, 0, 1, 1, 2 };
            int* v = stackalloc int[5] { 1, 2, 2, 3, 3 };
            long* w = stackalloc long[5] { 1, 3, 1, 4, 2 };
            bool* inMst = stackalloc bool[5] { false, true, true, true, false };
            // Tree edges: (0,2) wt 3, (1,2) wt 1, (1,3) wt 4. Total weight 8.
            // Non-tree edge: (2,3) wt 2 -> path (2-1-3) max is max(1,4) = 4 > 2. So NOT MST.
            bool result = Probabilistic.RandomizedMstVerify(numVertices, numEdges, u, v, w, inMst);
            Assert.False(result);
        }

        [Fact]
        public void XmlTreeHash_SameStructureDifferentOrder_ReturnsSameHash()
        {
            // Root has child 1 and child 2
            XmlNode* nodes1 = stackalloc XmlNode[3];
            nodes1[0] = new XmlNode { TagHash = 10, ValueHash = 0, ChildStart = 0, ChildCount = 2 };
            nodes1[1] = new XmlNode { TagHash = 20, ValueHash = 5, ChildStart = 0, ChildCount = 0 };
            nodes1[2] = new XmlNode { TagHash = 30, ValueHash = 6, ChildStart = 0, ChildCount = 0 };

            int* childIndices1 = stackalloc int[2] { 1, 2 };
            uint* hashes1 = stackalloc uint[3];

            uint rootHash1 = SpecialStructures.XmlTreeHash(nodes1, 0, childIndices1, hashes1);

            // Same, but child 2 is before child 1
            XmlNode* nodes2 = stackalloc XmlNode[3];
            nodes2[0] = new XmlNode { TagHash = 10, ValueHash = 0, ChildStart = 0, ChildCount = 2 };
            nodes2[1] = new XmlNode { TagHash = 20, ValueHash = 5, ChildStart = 0, ChildCount = 0 };
            nodes2[2] = new XmlNode { TagHash = 30, ValueHash = 6, ChildStart = 0, ChildCount = 0 };

            int* childIndices2 = stackalloc int[2] { 2, 1 };
            uint* hashes2 = stackalloc uint[3];

            uint rootHash2 = SpecialStructures.XmlTreeHash(nodes2, 0, childIndices2, hashes2);

            Assert.Equal(rootHash1, rootHash2);
        }

        [Fact]
        public void JsonCanonicalHash_SameObjectDifferentKeyOrder_ReturnsSameHash()
        {
            // Element index 0: Object (Type = 1), children: 1 and 2
            JsonElement* elem1 = stackalloc JsonElement[3];
            elem1[0] = new JsonElement { KeyHash = 0, ValueHash = 0, Type = 1, ChildStart = 0, ChildCount = 2 };
            elem1[1] = new JsonElement { KeyHash = 100, ValueHash = 10, Type = 0, ChildStart = 0, ChildCount = 0 };
            elem1[2] = new JsonElement { KeyHash = 200, ValueHash = 20, Type = 0, ChildStart = 0, ChildCount = 0 };

            int* childIndices1 = stackalloc int[2] { 1, 2 };
            uint* hashes1 = stackalloc uint[3];

            uint rootHash1 = SpecialStructures.JsonCanonicalHash(elem1, 0, childIndices1, hashes1);

            JsonElement* elem2 = stackalloc JsonElement[3];
            elem2[0] = new JsonElement { KeyHash = 0, ValueHash = 0, Type = 1, ChildStart = 0, ChildCount = 2 };
            elem2[1] = new JsonElement { KeyHash = 100, ValueHash = 10, Type = 0, ChildStart = 0, ChildCount = 0 };
            elem2[2] = new JsonElement { KeyHash = 200, ValueHash = 20, Type = 0, ChildStart = 0, ChildCount = 0 };

            int* childIndices2 = stackalloc int[2] { 2, 1 }; // swapped children
            uint* hashes2 = stackalloc uint[3];

            uint rootHash2 = SpecialStructures.JsonCanonicalHash(elem2, 0, childIndices2, hashes2);

            Assert.Equal(rootHash1, rootHash2);
        }
    }
}
