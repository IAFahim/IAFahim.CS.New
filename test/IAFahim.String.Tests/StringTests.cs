namespace IAFahim.String.Tests
{
    using IAFahim.String;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class StringTests
    {
        [Test]
        public void ManacherOdd_Basic()
        {
            byte* s = stackalloc byte[5] { (byte)'a', (byte)'b', (byte)'a', (byte)'b', (byte)'a' };
            int* d = stackalloc int[5];
            ManacherOdd.Run(s, 5, d);
            Assert.IsTrue(d[2] > 0);
        }

        [Test]
        public void KmpPrefix_Basic()
        {
            byte* pat = stackalloc byte[3] { (byte)'a', (byte)'b', (byte)'a' };
            int* pi = stackalloc int[3];
            KmpPrefix.Run(pat, 3, pi);
            Assert.AreEqual(0, pi[0]);
            Assert.IsTrue(pi[1] >= 0);
        }

        [Test]
        public void ZAlgorithm_Basic()
        {
            byte* s = stackalloc byte[6] { (byte)'a', (byte)'b', (byte)'a', (byte)'a', (byte)'b', (byte)'c' };
            int* z = stackalloc int[6];
            ZAlgorithm.Run(s, 6, z);
            Assert.AreEqual(6, z[0]);
            Assert.IsTrue(z[1] >= 0);
        }

        [Test]
        public void EditDistance_Basic()
        {
            byte* a = stackalloc byte[3] { (byte)'a', (byte)'b', (byte)'c' };
            byte* b = stackalloc byte[3] { (byte)'b', (byte)'d', (byte)'c' };
            int dist = EditDistance.Run(a, 3, b, 3);
            Assert.AreEqual(2, dist);
        }

        [Test]
        public void Lcs_Basic()
        {
            byte* a = stackalloc byte[4] { (byte)'a', (byte)'b', (byte)'c', (byte)'d' };
            byte* b = stackalloc byte[4] { (byte)'b', (byte)'c', (byte)'e', (byte)'f' };
            byte* res = stackalloc byte[4];
            int len = Lcs.Run(a, 4, b, 4, res);
            Assert.AreEqual(2, len);
        }

        [Test]
        public void MinCyclicShift_Sorted()
        {
            byte* s = stackalloc byte[3] { (byte)'a', (byte)'a', (byte)'b' };
            int idx = MinCyclicShift.Run(s, 3);
            Assert.AreEqual(0, idx);
        }

        [Test]
        public void AhoBuild_Basic()
        {
            const int alphabet = 26;
            const int maxNodes = 10;
            int* next = (int*)Marshal.AllocHGlobal(maxNodes * alphabet * sizeof(int));
            int* link = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            try
            {
                for (int i = 0; i < maxNodes * alphabet; i++) next[i] = 0;
                for (int i = 0; i < maxNodes; i++) link[i] = 0;
                int nodeCount = 1;
                byte* pat = stackalloc byte[3] { (byte)'a', (byte)'b', (byte)'c' };
                int cur = 0;
                for (int i = 0; i < 3; i++)
                {
                    int c = pat[i] - 'a';
                    int nextNode = next[cur * alphabet + c];
                    if (nextNode == 0)
                    {
                        nextNode = nodeCount++;
                        next[cur * alphabet + c] = nextNode;
                    }
                    cur = nextNode;
                }
                AhoBuild.Run(next, link, nodeCount, alphabet);
                Assert.IsTrue(nodeCount > 1);
            }
            finally { Marshal.FreeHGlobal((nint)next); Marshal.FreeHGlobal((nint)link); }
        }

        [Test]
        public void PalindromicTree_Basic()
        {
            const int maxLen = 100;
            int* next = (int*)Marshal.AllocHGlobal(maxLen * 256 * sizeof(int));
            int* link = (int*)Marshal.AllocHGlobal(maxLen * sizeof(int));
            int* len = (int*)Marshal.AllocHGlobal(maxLen * sizeof(int));
            try
            {
                for (int i = 0; i < maxLen * 256; i++) next[i] = 0;
                for (int i = 0; i < maxLen; i++) { link[i] = 0; len[i] = 0; }
                len[0] = 2;
                len[1] = 0;
                len[2] = -1;
                link[1] = 2;
                link[2] = 2;
                int last = 1;
                byte* s = stackalloc byte[5] { (byte)'a', (byte)'b', (byte)'a', (byte)'b', (byte)'a' };
                for (int i = 0; i < 5; i++)
                    PalindromicTreeAdd.Run(len, link, next, &last, s, i);
                Assert.IsTrue(len[0] > 1);
            }
            finally { Marshal.FreeHGlobal((nint)next); Marshal.FreeHGlobal((nint)link); Marshal.FreeHGlobal((nint)len); }
        }
    }
}
