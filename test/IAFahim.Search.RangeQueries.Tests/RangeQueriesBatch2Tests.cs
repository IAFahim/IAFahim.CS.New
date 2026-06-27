namespace IAFahim.Search.RangeQueries.Tests
{
    using IAFahim.Search.RangeQueries;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class AdvancedRangeQueriesTests
    {
        [Test]
        public void SuccessorPredecessor_Definition()
        {
            long[] vals = { 5, 1, 9, 3, 7, 2, 8 };
            int n = vals.Length;
            long* arr = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            try
            {
                for (int i = 0; i < n; i++) arr[i] = vals[i];
                Assert.AreEqual(5, RangeSuccessorQuery.Run(arr, n, 0, 6, 4), "succ>=4 in {5,1,9,3,7,2,8}");
                Assert.AreEqual(7, RangeSuccessorQuery.Run(arr, n, 0, 6, 6), "succ>=6");
                Assert.AreEqual(3, RangePredecessorQuery.Run(arr, n, 0, 6, 4), "pred<=4");
                Assert.AreEqual(long.MinValue, RangeSuccessorQuery.Run(arr, n, 0, 6, 100), "succ none");
                Assert.AreEqual(long.MaxValue, RangePredecessorQuery.Run(arr, n, 0, 6, 0), "pred none");
                Assert.AreEqual(9, RangeSuccessorQuery.Run(arr, n, 0, 6, 9), "succ exact");
            }
            finally { Marshal.FreeHGlobal((nint)arr); }
        }

        [Test]
        public void DistinctCount_Cases()
        {
            int[] vals = { 3, 1, 3, 2, 1, 1, 4 };
            int n = vals.Length;
            int* arr = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            try
            {
                for (int i = 0; i < n; i++) arr[i] = vals[i];
                Assert.AreEqual(4, RangeDistinctCount.Run(arr, n, 0, 6));
                Assert.AreEqual(1, RangeDistinctCount.Run(arr, n, 1, 1));
                Assert.AreEqual(2, RangeDistinctCount.Run(arr, n, 0, 1));
                Assert.AreEqual(2, RangeDistinctCount.Run(arr, n, 4, 6));
                Assert.AreEqual(0, RangeDistinctCount.Run(arr, n, 3, 2), "empty range");
            }
            finally { Marshal.FreeHGlobal((nint)arr); }
        }
    }

    public sealed unsafe class RangeChminChmaxSumTests
    {
        [Test]
        public void Beats_MatchesNaive_RandomOps()
        {
            const int N = 200;
            const int Ops = 500;
            int seed = 42;
            Random rng = new Random(seed);

            int* src = (int*)Marshal.AllocHGlobal(sizeof(int) * N);
            long* naive = (long*)Marshal.AllocHGlobal(sizeof(long) * N);
            RangeChminChmaxSum.Node* nodes =
                (RangeChminChmaxSum.Node*)Marshal.AllocHGlobal(sizeof(RangeChminChmaxSum.Node) * (4 * N));
            try
            {
                for (int i = 0; i < N; i++)
                {
                    int v = rng.Next(1, 50);
                    src[i] = v;
                    naive[i] = v;
                }
                RangeChminChmaxSum.Build(src, N, nodes);

                for (int op = 0; op < Ops; op++)
                {
                    int l = rng.Next(0, N);
                    int r = rng.Next(l, N);
                    int kind = rng.Next(0, 3);
                    if (kind == 0)
                    {
                        long x = rng.Next(1, 60);
                        RangeChminChmaxSum.Chmin(nodes, 1, 0, N - 1, l, r, x);
                        for (int i = l; i <= r; i++)
                            if (naive[i] > x) naive[i] = x;
                    }
                    else if (kind == 1)
                    {
                        long x = rng.Next(0, 50);
                        RangeChminChmaxSum.Chmax(nodes, 1, 0, N - 1, l, r, x);
                        for (int i = l; i <= r; i++)
                            if (naive[i] < x) naive[i] = x;
                    }
                    else
                    {
                        long expected = 0;
                        for (int i = l; i <= r; i++) expected += naive[i];
                        long actual = RangeChminChmaxSum.QuerySum(nodes, 1, 0, N - 1, l, r);
                        Assert.AreEqual(expected, actual,
                            $"sum mismatch op={op} l={l} r={r}");
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)src);
                Marshal.FreeHGlobal((nint)naive);
                Marshal.FreeHGlobal((nint)nodes);
            }
        }

        [Test]
        public void Beats_FullSum_AfterChminChmax()
        {
            const int N = 8;
            int[] vals = { 10, 20, 30, 40, 50, 60, 70, 80 };
            int* src = (int*)Marshal.AllocHGlobal(sizeof(int) * N);
            RangeChminChmaxSum.Node* nodes =
                (RangeChminChmaxSum.Node*)Marshal.AllocHGlobal(sizeof(RangeChminChmaxSum.Node) * (4 * N));
            try
            {
                for (int i = 0; i < N; i++) src[i] = vals[i];
                RangeChminChmaxSum.Build(src, N, nodes);

                Assert.AreEqual(360, RangeChminChmaxSum.QuerySum(nodes, 1, 0, N - 1, 0, N - 1));
                RangeChminChmaxSum.Chmin(nodes, 1, 0, N - 1, 0, N - 1, 45);
                Assert.AreEqual(10 + 20 + 30 + 40 + 45 + 45 + 45 + 45,
                    RangeChminChmaxSum.QuerySum(nodes, 1, 0, N - 1, 0, N - 1));
                RangeChminChmaxSum.Chmax(nodes, 1, 0, N - 1, 0, N - 1, 25);
                Assert.AreEqual(25 + 25 + 30 + 40 + 45 + 45 + 45 + 45,
                    RangeChminChmaxSum.QuerySum(nodes, 1, 0, N - 1, 0, N - 1));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)src);
                Marshal.FreeHGlobal((nint)nodes);
            }
        }
    }

    public sealed unsafe class OfflineAndStaticTests
    {
        [Test]
        public void OfflineRangeCount_MatchesBrute()
        {
            int[] vals = { 5, 1, 9, 3, 7, 2, 8, 4, 6, 0 };
            int n = vals.Length;
            int q = 6;
            int[] ls = { 0, 2, 1, 0, 5, 3 };
            int[] rs = { 9, 7, 4, 3, 8, 3 };
            int[] xs = { 5, 9, 7, 5, 6, 3 };

            int* arr = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            int* ql = (int*)Marshal.AllocHGlobal(sizeof(int) * q);
            int* qr = (int*)Marshal.AllocHGlobal(sizeof(int) * q);
            int* qx = (int*)Marshal.AllocHGlobal(sizeof(int) * q);
            int* ans = (int*)Marshal.AllocHGlobal(sizeof(int) * q);
            try
            {
                for (int i = 0; i < n; i++) arr[i] = vals[i];
                for (int j = 0; j < q; j++) { ql[j] = ls[j]; qr[j] = rs[j]; qx[j] = xs[j]; }
                OfflineRangeCount.Run(arr, n, ql, qr, qx, q, ans);
                for (int j = 0; j < q; j++)
                {
                    int brute = 0;
                    for (int i = ls[j]; i <= rs[j]; i++)
                        if (vals[i] <= xs[j]) brute++;
                    Assert.AreEqual(brute, ans[j], $"OfflineRangeCount q={j}");
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)arr);
                Marshal.FreeHGlobal((nint)ql);
                Marshal.FreeHGlobal((nint)qr);
                Marshal.FreeHGlobal((nint)qx);
                Marshal.FreeHGlobal((nint)ans);
            }
        }

        [Test]
        public void StaticRangeMode_Mex_Inversions()
        {
            int[] vals = { 4, 2, 4, 2, 1, 0, 3, 2 };
            int n = vals.Length;
            int* arr = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            try
            {
                for (int i = 0; i < n; i++) arr[i] = vals[i];

                Assert.AreEqual(4, StaticRangeMode.Run(arr, n, 0, 2), "mode of {4,2,4}");
                Assert.AreEqual(2, StaticRangeMode.Run(arr, n, 0, 7), "mode of full: 2 appears 3x");

                Assert.AreEqual(3, StaticRangeMex.Run(arr, n, 0, 5), "mex of {4,2,4,2,1,0}: 0,1,2 present -> 3");
                Assert.AreEqual(0, StaticRangeMex.Run(arr, n, 0, 0), "mex of {4}");
                Assert.AreEqual(5, StaticRangeMex.Run(arr, n, 0, 7), "mex of {0,1,2,3,4} present, missing 5");

                Assert.AreEqual(1, StaticRangeInversions.Run(arr, n, 0, 2), "inv of {4,2,4} = 1");
                long bruteInv = 0;
                for (int i = 0; i < n; i++)
                    for (int j = i + 1; j < n; j++)
                        if (vals[i] > vals[j]) bruteInv++;
                Assert.AreEqual(bruteInv, StaticRangeInversions.Run(arr, n, 0, n - 1), "full inversions");
            }
            finally { Marshal.FreeHGlobal((nint)arr); }
        }
    }
}
