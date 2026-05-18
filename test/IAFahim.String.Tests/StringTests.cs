namespace IAFahim.String.Tests
{
    using IAFahim.String;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class StringTests
    {
        [Fact]
        public void ManacherOdd_Basic()
        {
            byte* s = stackalloc byte[5] { (byte)'a', (byte)'b', (byte)'a', (byte)'b', (byte)'a' };
            int* d = stackalloc int[5];
            ManacherOdd.Run(s, 5, d);
            Assert.True(d[2] > 0);
        }

        [Fact]
        public void KmpPrefix_Basic()
        {
            byte* pat = stackalloc byte[3] { (byte)'a', (byte)'b', (byte)'a' };
            int* pi = stackalloc int[3];
            KmpPrefix.Run(pat, 3, pi);
            Assert.Equal(0, pi[0]);
            Assert.True(pi[1] >= 0);
        }

        [Fact]
        public void ZAlgorithm_Basic()
        {
            byte* s = stackalloc byte[6] { (byte)'a', (byte)'b', (byte)'a', (byte)'a', (byte)'b', (byte)'c' };
            int* z = stackalloc int[6];
            ZAlgorithm.Run(s, 6, z);
            Assert.Equal(6, z[0]);
            Assert.True(z[1] >= 0);
        }

        [Fact]
        public void EditDistance_Basic()
        {
            byte* a = stackalloc byte[3] { (byte)'a', (byte)'b', (byte)'c' };
            byte* b = stackalloc byte[3] { (byte)'b', (byte)'d', (byte)'c' };
            int dist = EditDistance.Run(a, 3, b, 3);
            Assert.Equal(2, dist);
        }

        [Fact]
        public void Lcs_Basic()
        {
            byte* a = stackalloc byte[4] { (byte)'a', (byte)'b', (byte)'c', (byte)'d' };
            byte* b = stackalloc byte[4] { (byte)'b', (byte)'c', (byte)'e', (byte)'f' };
            int len = Lcs.Run(a, 4, b, 4);
            Assert.Equal(2, len);
        }

        [Fact]
        public void MinCyclicShift_Sorted()
        {
            byte* s = stackalloc byte[3] { (byte)'a', (byte)'a', (byte)'b' };
            int idx = MinCyclicShift.Run(s, 3);
            Assert.Equal(0, idx);
        }

        [Fact]
        public void AhoBuild_Basic()
        {
            const int alphabet = 26;
            const int maxNodes = 10;
            int* next = (int*)Marshal.AllocHGlobal(maxNodes * alphabet * sizeof(int));
            int* link = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int* out_ = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            try
            {
                for (int i = 0; i < maxNodes * alphabet; i++) next[i] = -1;
                for (int i = 0; i < maxNodes; i++) { link[i] = 0; out_[i] = 0; }
                int nodeCount = 1;
                byte* pat = stackalloc byte[3] { (byte)'a', (byte)'b', (byte)'c' };
                AhoBuild.Run(&nodeCount, next, link, out_, pat, 3, alphabet);
                Assert.True(nodeCount > 1);
            }
            finally { Marshal.FreeHGlobal((nint)next); Marshal.FreeHGlobal((nint)link); Marshal.FreeHGlobal((nint)out_); }
        }

        [Fact]
        public void PalindromicTree_Basic()
        {
            const int maxLen = 100;
            int* next = (int*)Marshal.AllocHGlobal(maxLen * 256 * sizeof(int));
            int* link = (int*)Marshal.AllocHGlobal(maxLen * sizeof(int));
            int* len = (int*)Marshal.AllocHGlobal(maxLen * sizeof(int));
            int* cnt = (int*)Marshal.AllocHGlobal(maxLen * sizeof(int));
            try
            {
                for (int i = 0; i < maxLen * 256; i++) next[i] = -1;
                for (int i = 0; i < maxLen; i++) { link[i] = 0; len[i] = 0; cnt[i] = 0; }
                int last = 0, sz = 2;
                byte* s = stackalloc byte[5] { (byte)'a', (byte)'b', (byte)'a', (byte)'b', (byte)'a' };
                for (int i = 0; i < 5; i++)
                    PalindromicTreeAdd.Run(&last, &sz, next, link, len, cnt, s[i]);
                Assert.True(sz > 2);
            }
            finally { Marshal.FreeHGlobal((nint)next); Marshal.FreeHGlobal((nint)link); Marshal.FreeHGlobal((nint)len); Marshal.FreeHGlobal((nint)cnt); }
        }
    }
}