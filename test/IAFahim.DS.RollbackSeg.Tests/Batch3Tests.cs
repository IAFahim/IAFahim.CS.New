namespace IAFahim.DS.RollbackSeg.Tests
{
    using IAFahim.DS.RollbackSeg;
    using IAFahim.DS.SegmentTree;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class RollbackBasisTests
    {
        [Test]
        public void Insert_Max_Rollback_MatchesBrute()
        {
            const int N = 200;
            long[] vals = new long[N];
            Random rng = new Random(7);
            for (int i = 0; i < N; i++) vals[i] = ((long)rng.Next() << 20) ^ (long)rng.Next();

            long* basis = (long*)Marshal.AllocHGlobal(sizeof(long) * 64);
            long* brute = (long*)Marshal.AllocHGlobal(sizeof(long) * 64);
            int* histSlot = (int*)Marshal.AllocHGlobal(sizeof(int) * 4096);
            byte* histEmpty = (byte*)Marshal.AllocHGlobal(sizeof(byte) * 4096);
            int* top = (int*)Marshal.AllocHGlobal(sizeof(int));
            try
            {
                for (int i = 0; i < 64; i++) { basis[i] = 0; brute[i] = 0; }
                *top = 0;
                for (int i = 0; i < N; i++)
                {
                    LinearBasisRollbackInsert.Run(basis, histSlot, histEmpty, top, vals[i]);
                    InsertBrute(brute, vals[i]);
                    long x = (long)rng.Next() & 0x3FFFFFFFFFFFFFFFL;
                    long fast = LinearBasisRollbackMax.Run(basis, x);
                    long slow = MaxBrute(brute, x);
                    Assert.AreEqual(slow, fast, $"max mismatch after insert {i}");
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)basis);
                Marshal.FreeHGlobal((nint)brute);
                Marshal.FreeHGlobal((nint)histSlot);
                Marshal.FreeHGlobal((nint)histEmpty);
                Marshal.FreeHGlobal((nint)top);
            }
        }

        [Test]
        public void Rollback_RestoresPreviousState()
        {
            long* basis = (long*)Marshal.AllocHGlobal(sizeof(long) * 64);
            int* histSlot = (int*)Marshal.AllocHGlobal(sizeof(int) * 256);
            byte* histEmpty = (byte*)Marshal.AllocHGlobal(sizeof(byte) * 256);
            int* top = (int*)Marshal.AllocHGlobal(sizeof(int));
            try
            {
                for (int i = 0; i < 64; i++) basis[i] = 0;
                *top = 0;
                int cp = LinearBasisRollback.GetCheckpoint(top);
                LinearBasisRollbackInsert.Run(basis, histSlot, histEmpty, top, 13);
                LinearBasisRollbackInsert.Run(basis, histSlot, histEmpty, top, 21);
                long after = LinearBasisRollbackMax.Run(basis, 0);
                LinearBasisRollback.Run(basis, histSlot, histEmpty, top, cp);
                for (int i = 0; i < 64; i++) Assert.AreEqual(0L, basis[i], $"basis[{i}] not cleared by rollback");
                Assert.AreNotEqual(13 | 21, after);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)basis);
                Marshal.FreeHGlobal((nint)histSlot);
                Marshal.FreeHGlobal((nint)histEmpty);
                Marshal.FreeHGlobal((nint)top);
            }
        }

        [Test]
        public void RangeBasis_Rank_Correct()
        {
            long[] vals = { 5, 3, 6, 9, 12, 17 };
            int n = vals.Length;
            long* arr = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            long* buf = (long*)Marshal.AllocHGlobal(sizeof(long) * 64);
            try
            {
                for (int i = 0; i < n; i++) arr[i] = vals[i];
                int rankAll = RangeBasisQuery.Run(arr, n, 0, n - 1, buf);
                Assert.AreEqual(4, rankAll, "{5,3,6,9,12,17}: 6=5^3 and 5=9^12 dependent -> rank 4");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)arr);
                Marshal.FreeHGlobal((nint)buf);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InsertBrute(long* b, long x)
        {
            for (int i = 63; i >= 0; i--)
            {
                if (((x >> i) & 1L) == 0L) continue;
                if (b[i] == 0L) { b[i] = x; return; }
                x ^= b[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long MaxBrute(long* b, long x)
        {
            for (int i = 63; i >= 0; i--)
                if (b[i] != 0L && ((x >> i) & 1L) == 0L) x ^= b[i];
            return x;
        }
    }

    public sealed unsafe class OfflineDeleteSegmentTreeTests
    {
        [Test]
        public void ActiveSumPerTime_MatchesDifferenceArray()
        {
            int m = 4;
            int T = 10;
            int[] s = { 1, 3, 0, 6 };
            int[] e = { 5, 7, 10, 8 };
            long[] v = { 10, 100, 1, 1000 };
            long[] expected = new long[T];
            for (int i = 0; i < m; i++)
                for (int t = s[i]; t < e[i]; t++) expected[t] += v[i];

            int nodes = 4 * (T + 1);
            int* start = (int*)Marshal.AllocHGlobal(sizeof(int) * m);
            int* end = (int*)Marshal.AllocHGlobal(sizeof(int) * m);
            long* val = (long*)Marshal.AllocHGlobal(sizeof(long) * m);
            long* ans = (long*)Marshal.AllocHGlobal(sizeof(long) * T);
            int* ncnt = (int*)Marshal.AllocHGlobal(sizeof(int) * nodes);
            int* noff = (int*)Marshal.AllocHGlobal(sizeof(int) * nodes);
            long* nval = (long*)Marshal.AllocHGlobal(sizeof(long) * (m * 8 + 16));
            try
            {
                for (int i = 0; i < m; i++) { start[i] = s[i]; end[i] = e[i]; val[i] = v[i]; }
                OfflineDeleteSegmentTree.Run(start, end, val, m, T, ans, ncnt, noff, nval);
                for (int t = 0; t < T; t++)
                    Assert.AreEqual(expected[t], ans[t], $"time {t}");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)start);
                Marshal.FreeHGlobal((nint)end);
                Marshal.FreeHGlobal((nint)val);
                Marshal.FreeHGlobal((nint)ans);
                Marshal.FreeHGlobal((nint)ncnt);
                Marshal.FreeHGlobal((nint)noff);
                Marshal.FreeHGlobal((nint)nval);
            }
        }
    }

    public sealed unsafe class LiChaoAndKineticTests
    {
        [Test]
        public void LiChaoRollback_MinLines_MatchesBrute()
        {
            const int Xlo = -50, Xhi = 50;
            const int Size = 4 * (Xhi - Xlo + 1) + 8;
            long[] ma = { 2, -1, 0, 3, -3 };
            long[] ca = { 5, 10, -7, 0, 4 };
            int m = ma.Length;
            long* segM = (long*)Marshal.AllocHGlobal(sizeof(long) * Size);
            long* segC = (long*)Marshal.AllocHGlobal(sizeof(long) * Size);
            int* hNode = (int*)Marshal.AllocHGlobal(sizeof(int) * 8192);
            long* hM = (long*)Marshal.AllocHGlobal(sizeof(long) * 8192);
            long* hC = (long*)Marshal.AllocHGlobal(sizeof(long) * 8192);
            int* top = (int*)Marshal.AllocHGlobal(sizeof(int));
            try
            {
                LiChaoInit.Run(segM, segC, Size);
                *top = 0;
                for (int i = 0; i < m; i++)
                {
                    int cp = LiChaoRollback.GetCheckpoint(top);
                    LiChaoRollback.Add(segM, segC, hNode, hM, hC, top, 1, Xlo, Xhi, ma[i], ca[i]);
                    Assert.IsTrue(*top > cp, $"Add must push history (line {i})");
                    for (int x = Xlo; x <= Xhi; x += 7)
                    {
                        long fast = LiChaoRollback.Query(segM, segC, 1, Xlo, Xhi, x);
                        long slow = long.MaxValue;
                        for (int k = 0; k <= i; k++)
                            slow = Math.Min(slow, ma[k] * x + ca[k]);
                        Assert.AreEqual(slow, fast, $"x={x} after {i + 1} lines");
                    }
                    if (i >= 1)
                    {
                        LiChaoRollback.Rollback(segM, segC, hNode, hM, hC, top, cp);
                        long fastAfter = LiChaoRollback.Query(segM, segC, 1, Xlo, Xhi, 0);
                        long slowAfter = long.MaxValue;
                        for (int k = 0; k < i; k++)
                            slowAfter = Math.Min(slowAfter, ca[k]);
                        Assert.AreEqual(slowAfter, fastAfter, $"rollback restores state before line {i}");
                        LiChaoRollback.Add(segM, segC, hNode, hM, hC, top, 1, Xlo, Xhi, ma[i], ca[i]);
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)segM);
                Marshal.FreeHGlobal((nint)segC);
                Marshal.FreeHGlobal((nint)hNode);
                Marshal.FreeHGlobal((nint)hM);
                Marshal.FreeHGlobal((nint)hC);
                Marshal.FreeHGlobal((nint)top);
            }
        }

        [Test]
        public void OnlineCht_Min_MatchesBrute()
        {
            long[] ma = { -5, -3, -1, 0, 2, 4 };
            long[] ca = { 50, 40, 30, 20, 5, -5 };
            int m = ma.Length;
            long* sl = (long*)Marshal.AllocHGlobal(sizeof(long) * m);
            long* ic = (long*)Marshal.AllocHGlobal(sizeof(long) * m);
            int* head = (int*)Marshal.AllocHGlobal(sizeof(int));
            int* tail = (int*)Marshal.AllocHGlobal(sizeof(int));
            try
            {
                *head = 0; *tail = 0;
                for (int i = 0; i < m; i++)
                {
                    OnlineChtAdd.Run(sl, ic, head, tail, ma[i], ca[i]);
                    *head = 0;
                    for (long x = -20; x <= 20; x += 3)
                    {
                        int savedTail = *tail;
                        long fast = OnlineChtQuery.Run(sl, ic, head, savedTail, x);
                        long slow = long.MaxValue;
                        for (int k = 0; k <= i; k++)
                            slow = Math.Min(slow, ma[k] * x + ca[k]);
                        Assert.AreEqual(slow, fast, $"x={x} after {i + 1} lines");
                        *head = 0;
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)sl);
                Marshal.FreeHGlobal((nint)ic);
                Marshal.FreeHGlobal((nint)head);
                Marshal.FreeHGlobal((nint)tail);
            }
        }

        [Test]
        public void KineticTournament_Winner_AndRange_MinCorrect()
        {
            long[] a = { 2, -1, 0, 3, -2 };
            long[] b = { 0, 5, -3, 1, 4 };
            int n = a.Length;
            int size = 1; while (size < n) size <<= 1;
            long* ta = (long*)Marshal.AllocHGlobal(sizeof(long) * (size * 2));
            long* tb = (long*)Marshal.AllocHGlobal(sizeof(long) * (size * 2));
            long* curT = (long*)Marshal.AllocHGlobal(sizeof(long));
            long* aSrc = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            long* bSrc = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            try
            {
                for (int i = 0; i < n; i++) { aSrc[i] = a[i]; bSrc[i] = b[i]; }
                KineticTournamentBuild.Run(aSrc, bSrc, n, size, ta, tb, curT, 0);
                for (long t = -5; t <= 5; t++)
                {
                    KineticSetTime.Run(ta, tb, size, curT, t);
                    long win = KineticTournamentWinner.Run(ta, tb, curT);
                    long bruteWin = long.MaxValue;
                    for (int i = 0; i < n; i++)
                        bruteWin = Math.Min(bruteWin, a[i] * t + b[i]);
                    Assert.AreEqual(bruteWin, win, $"tournament winner t={t}");

                    long rq = KineticSegmentTreeQuery.Run(size, ta, tb, curT, 1, 3);
                    long bruteRq = long.MaxValue;
                    for (int i = 1; i <= 3; i++)
                        bruteRq = Math.Min(bruteRq, a[i] * t + b[i]);
                    Assert.AreEqual(bruteRq, rq, $"range query [1,3] t={t}");
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ta);
                Marshal.FreeHGlobal((nint)tb);
                Marshal.FreeHGlobal((nint)curT);
                Marshal.FreeHGlobal((nint)aSrc);
                Marshal.FreeHGlobal((nint)bSrc);
            }
        }

        [Test]
        public void DivideConquerHullOpt_Layered_MatchesBrute()
        {
            const int N = 30;
            long* dpPrev = (long*)Marshal.AllocHGlobal(sizeof(long) * N);
            long* dpCur = (long*)Marshal.AllocHGlobal(sizeof(long) * N);
            long* dpCurBrute = (long*)Marshal.AllocHGlobal(sizeof(long) * N);
            long* cost = (long*)Marshal.AllocHGlobal(sizeof(long) * N * N);
            int* opt = (int*)Marshal.AllocHGlobal(sizeof(int) * N);
            Random rng = new Random(3);
            try
            {
                dpPrev[0] = 0;
                for (int i = 1; i < N; i++) dpPrev[i] = rng.Next(0, 100);
                for (int j = 0; j < N; j++)
                    for (int i = 0; i < N; i++)
                        cost[j * N + i] = (long)(i - j) * (i - j);
                DivideConquerHullOptimization.Run(dpPrev, dpCur, cost, N, opt);
                dpCurBrute[0] = 0;
                for (int i = 1; i < N; i++)
                {
                    long best = long.MaxValue;
                    for (int j = 0; j < i; j++)
                        best = Math.Min(best, dpPrev[j] + cost[j * N + i]);
                    dpCurBrute[i] = best;
                }
                for (int i = 1; i < N; i++)
                    Assert.AreEqual(dpCurBrute[i], dpCur[i], $"layered dp i={i}");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)dpPrev);
                Marshal.FreeHGlobal((nint)dpCur);
                Marshal.FreeHGlobal((nint)dpCurBrute);
                Marshal.FreeHGlobal((nint)cost);
                Marshal.FreeHGlobal((nint)opt);
            }
        }
    }
}
