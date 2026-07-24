namespace IAFahim.DP.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class Knapsack01Tests
    {
        [Test]
        public void Empty_Zero()
        {
            long* dp = stackalloc long[1];
            dp[0] = 0;
            Assert.AreEqual(0, Knapsack01.Run(0, 0, null, null, dp));
        }

        [Test]
        public void Classic_Instance()
        {
            const int N = 3;
            const long Cap = 5;
            long* w = stackalloc long[N];
            long* v = stackalloc long[N];
            w[0] = 2; w[1] = 3; w[2] = 4;
            v[0] = 3; v[1] = 4; v[2] = 5;
            int cols = (int)Cap + 1;
            long* dp = (long*)Marshal.AllocHGlobal((N + 1) * cols * sizeof(long));
            try
            {
                for (int i = 0; i < (N + 1) * cols; i++) dp[i] = 0;
                long ans = Knapsack01.Run(N, Cap, w, v, dp);
                Assert.AreEqual(7, ans);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)dp);
            }
        }
    }

    public sealed unsafe class SmawkTests
    {
        [Test]
        public void RowMinima_NotColumnMinima()
        {
            const int N = 3, M = 3;
            long* mat = stackalloc long[N * M];
            long* dp = stackalloc long[N];
            mat[0] = 9; mat[1] = 1; mat[2] = 8;
            mat[3] = 2; mat[4] = 7; mat[5] = 3;
            mat[6] = 6; mat[7] = 4; mat[8] = 5;
            long overall = Smawk.Run(N, M, mat, dp);
            Assert.AreEqual(1, dp[0]);
            Assert.AreEqual(2, dp[1]);
            Assert.AreEqual(4, dp[2]);
            Assert.AreEqual(1, overall);
        }

        [Test]
        public void Empty_Zero()
        {
            Assert.AreEqual(0, Smawk.Run(0, 0, null, null));
        }
    }

    public sealed unsafe class SubsetSumDpTests
    {
        [Test]
        public void SingleItem_MatchesTarget()
        {
            long* arr = stackalloc long[1];
            arr[0] = 5;
            bool* dp = stackalloc bool[8];
            Assert.IsTrue(IAFahim.DP.SubsetSum.Run(1, 5, arr, dp));
            Assert.IsTrue(dp[0]);
            Assert.IsTrue(dp[5]);
            Assert.IsFalse(IAFahim.DP.SubsetSum.Run(1, 4, arr, dp));
        }
    }

    public sealed unsafe class SosDpTests
    {
        [Test]
        public void SingleBit_Propagates()
        {
            const int Bits = 3;
            const int N = 1 << Bits;
            long* f = stackalloc long[N];
            for (int i = 0; i < N; i++) f[i] = i == 1 ? 1 : 0;
            SosDp.Run(Bits, f);
            Assert.AreEqual(1, f[1]);
            Assert.AreEqual(1, f[3]);
            Assert.AreEqual(1, f[5]);
            Assert.AreEqual(1, f[7]);
            Assert.AreEqual(0, f[2]);
        }
    }

    public sealed unsafe class MinPlusConvolutionTests
    {
        [Test]
        public void MatrixMinPlus_MatchesBrute()
        {
            const int N = 2;
            long* a = stackalloc long[N * N];
            long* b = stackalloc long[N * N];
            long* c = stackalloc long[N * N];
            a[0] = 1; a[1] = 4; a[2] = 2; a[3] = 3;
            b[0] = 0; b[1] = 5; b[2] = 1; b[3] = 2;
            MinPlusConvolution.Run(N, a, b, c);
            Assert.AreEqual(1, c[0]);
            Assert.AreEqual(6, c[1]);
            Assert.AreEqual(2, c[2]);
            Assert.AreEqual(5, c[3]);
        }
    }

    public sealed unsafe class ConvexHullTrickTests
    {
        [Test]
        public void AddQuery_LowerEnvelope_DecreasingSlopes()
        {
            long* ms = stackalloc long[8];
            long* bs = stackalloc long[8];
            int sz = 0;
            ConvexHullTrickAdd.AddLine(2, 0, ms, bs, &sz);
            ConvexHullTrickAdd.AddLine(0, 5, ms, bs, &sz);
            Assert.AreEqual(2, sz);
            Assert.AreEqual(0, ConvexHullTrickAdd.Query(0, ms, bs, sz));
            Assert.AreEqual(5, ConvexHullTrickAdd.Query(10, ms, bs, sz));
            Assert.AreEqual(4, ConvexHullTrickAdd.Query(2, ms, bs, sz));
        }
    }
}
