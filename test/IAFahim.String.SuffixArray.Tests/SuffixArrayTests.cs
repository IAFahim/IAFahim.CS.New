namespace IAFahim.String.SuffixArray.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using NUnit.Framework;

    public sealed unsafe class SuffixArrayTests
    {
        [Test]
        public void Empty_NoOp()
        {
            SuffixArray.Build(null, 0, null, null, null, null, null);
        }

        [Test]
        public void Single_Char()
        {
            byte c = (byte)'a';
            int* sa = stackalloc int[1];
            int* rank = stackalloc int[2];
            int* tmpSa = stackalloc int[1];
            int* count = stackalloc int[256];
            int* tmpRank = stackalloc int[1];
            SuffixArray.Build(&c, 1, sa, rank, tmpSa, count, tmpRank);
            Assert.AreEqual(0, sa[0]);
        }

        [Test]
        public void Banana_KnownOrder()
        {
            byte[] s = Encoding.ASCII.GetBytes("banana");
            const int N = 6;
            int* sa = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            int* rank = (int*)Marshal.AllocHGlobal(N * 2 * sizeof(int));
            int* tmpSa = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            int* count = (int*)Marshal.AllocHGlobal(Math.Max(256, N * 2) * sizeof(int));
            int* tmpRank = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                fixed (byte* p = s)
                {
                    SuffixArray.Build(p, N, sa, rank, tmpSa, count, tmpRank);
                }
                int[] expected = { 5, 3, 1, 0, 4, 2 };
                for (int i = 0; i < N; i++)
                    Assert.AreEqual(expected[i], sa[i]);
                for (int i = 1; i < N; i++)
                {
                    string prev = Encoding.ASCII.GetString(s, sa[i - 1], N - sa[i - 1]);
                    string cur = Encoding.ASCII.GetString(s, sa[i], N - sa[i]);
                    Assert.IsTrue(string.CompareOrdinal(prev, cur) < 0);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)sa);
                Marshal.FreeHGlobal((nint)rank);
                Marshal.FreeHGlobal((nint)tmpSa);
                Marshal.FreeHGlobal((nint)count);
                Marshal.FreeHGlobal((nint)tmpRank);
            }
        }

        [Test]
        public void Locate_Find_ExactAndLongerPattern()
        {
            byte[] s = Encoding.ASCII.GetBytes("banana");
            const int N = 6;
            int* sa = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            int* rank = (int*)Marshal.AllocHGlobal(N * 2 * sizeof(int));
            int* tmpSa = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            int* count = (int*)Marshal.AllocHGlobal(Math.Max(256, N * 2) * sizeof(int));
            int* tmpRank = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                fixed (byte* p = s)
                {
                    SuffixArray.Build(p, N, sa, rank, tmpSa, count, tmpRank);
                    byte* pat = stackalloc byte[] { (byte)'a', (byte)'n', (byte)'a' };
                    int pos = Locate.Find(sa, N, p, N, pat, 3);
                    Assert.IsTrue(pos == 1 || pos == 3);
                    byte* longPat = stackalloc byte[] { (byte)'a', (byte)'n', (byte)'a', (byte)'x' };
                    Assert.AreEqual(-1, Locate.Find(sa, N, p, N, longPat, 4));
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)sa);
                Marshal.FreeHGlobal((nint)rank);
                Marshal.FreeHGlobal((nint)tmpSa);
                Marshal.FreeHGlobal((nint)count);
                Marshal.FreeHGlobal((nint)tmpRank);
            }
        }
    }
}
