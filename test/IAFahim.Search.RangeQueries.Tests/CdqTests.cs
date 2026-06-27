namespace IAFahim.Search.RangeQueries.Tests
{
    using IAFahim.Search.RangeQueries;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class CdqTests
    {
        [Test]
        public void Offline3DPartialOrder_MatchesBrute_Random()
        {
            Random rng = new Random(7);
            for (int t = 0; t < 120; t++)
            {
                int n = rng.Next(1, 40);
                long[] a = new long[n], b = new long[n], c = new long[n];
                for (int i = 0; i < n; i++) { a[i] = rng.Next(0, 8); b[i] = rng.Next(0, 8); c[i] = rng.Next(0, 8); }
                long[] ans = new long[n];
                Fixed3(a, b, c, n, out long* pa, out long* pb, out long* pc, out long* pans);
                try
                {
                    Offline3DPartialOrder.Run(pa, pb, pc, n, pans);
                    for (int i = 0; i < n; i++)
                    {
                        long brute = 0;
                        for (int j = 0; j < n; j++)
                            if (j != i && a[j] < a[i] && b[j] < b[i] && c[j] < c[i]) brute++;
                        Assert.AreEqual(brute, pans[i], $"t={t} i={i} a={a[i]} b={b[i]} c={c[i]}");
                    }
                }
                finally { Marshal.FreeHGlobal((nint)pa); Marshal.FreeHGlobal((nint)pb); Marshal.FreeHGlobal((nint)pc); Marshal.FreeHGlobal((nint)pans); }
            }
        }

        [Test]
        public void CdqDynamicInversions_MatchesBrute_Random()
        {
            Random rng = new Random(9);
            for (int t = 0; t < 120; t++)
            {
                int n = rng.Next(1, 16);
                int[] perm = new int[n];
                int[] vals = new int[n];
                for (int i = 0; i < n; i++) vals[i] = i + 1;
                for (int i = n - 1; i > 0; i--) { int j = rng.Next(i + 1); int tmp = vals[i]; vals[i] = vals[j]; vals[j] = tmp; }
                for (int i = 0; i < n; i++) perm[i] = vals[i];
                int k = rng.Next(1, n + 1);
                int[] order = new int[n];
                for (int i = 0; i < n; i++) order[i] = i;
                for (int i = n - 1; i > 0; i--) { int j = rng.Next(i + 1); int tmp = order[i]; order[i] = order[j]; order[j] = tmp; }
                int[] removeIdx = new int[k];
                for (int i = 0; i < k; i++) removeIdx[i] = order[i];

                long[] ans = new long[k];
                FixedPerm(perm, n, removeIdx, k, out int* p, out int* ri, out long* pa);
                try
                {
                    CdqDynamicInversions.Run(p, n, ri, k, pa);
                    for (int i = 0; i < k; i++) ans[i] = pa[i];
                }
                finally { Marshal.FreeHGlobal((nint)p); Marshal.FreeHGlobal((nint)ri); Marshal.FreeHGlobal((nint)pa); }

                // brute: present set, remove one at a time, count inversions lost
                bool[] present = new bool[n];
                for (int i = 0; i < n; i++) present[i] = true;
                for (int s = 0; s < k; s++)
                {
                    int e = removeIdx[s];
                    long lost = 0;
                    for (int j = 0; j < n; j++)
                    {
                        if (!present[j] || j == e) continue;
                        bool inv = (j < e && perm[j] > perm[e]) || (j > e && perm[j] < perm[e]);
                        if (inv) lost++;
                    }
                    Assert.AreEqual(lost, ans[s], $"t={t} s={s} e={e}");
                    present[e] = false;
                }
            }
        }

        [Test]
        public void Offline2D_PointAdd_RectangleSum_MatchesBrute()
        {
            Random rng = new Random(13);
            for (int t = 0; t < 80; t++)
            {
                int adds = rng.Next(0, 12);
                int queries = rng.Next(1, 10);
                int E = adds + queries;
                int[] evType = new int[E];
                int[] x = new int[E], y = new int[E];
                long[] delta = new long[E];
                int[] qx1 = new int[E], qx2 = new int[E], qy1 = new int[E], qy2 = new int[E], qIdxOf = new int[E];
                int qCount = 0;
                var pts = new System.Collections.Generic.List<(int x, int y, long d)>();
                for (int i = 0; i < E; i++)
                {
                    if (i < adds)
                    {
                        evType[i] = 0; x[i] = rng.Next(0, 10); y[i] = rng.Next(0, 10); delta[i] = rng.Next(-5, 6);
                        pts.Add((x[i], y[i], delta[i]));
                    }
                    else
                    {
                        evType[i] = 1;
                        int a = rng.Next(0, 10), bb = rng.Next(0, 10);
                        qx1[i] = Math.Min(a, bb); qx2[i] = Math.Max(a, bb);
                        int cc = rng.Next(0, 10), dd = rng.Next(0, 10);
                        qy1[i] = Math.Min(cc, dd); qy2[i] = Math.Max(cc, dd);
                        qIdxOf[i] = qCount++;
                    }
                }
                long[] ans = new long[qCount];
                Fixed2D(evType, x, y, delta, qx1, qx2, qy1, qy2, qIdxOf, E, out int* et, out int* px, out int* py, out long* pd,
                        out int* pq1, out int* pq2, out int* pqy1, out int* pqy2, out int* pqi, out long* pans);
                try
                {
                    Offline2DRangeAddRangeSum.Run(et, px, py, pd, pq1, pq2, pqy1, pqy2, pqi, E, qCount, pans);
                    int qi = 0;
                    for (int i = 0; i < E; i++)
                    {
                        if (evType[i] != 1) continue;
                        long brute = 0;
                        foreach (var p in pts)
                            if (p.x >= qx1[i] && p.x <= qx2[i] && p.y >= qy1[i] && p.y <= qy2[i]) brute += p.d;
                        Assert.AreEqual(brute, pans[qi], $"t={t} qi={qi}");
                        qi++;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)et); Marshal.FreeHGlobal((nint)px); Marshal.FreeHGlobal((nint)py);
                    Marshal.FreeHGlobal((nint)pd); Marshal.FreeHGlobal((nint)pq1); Marshal.FreeHGlobal((nint)pq2);
                    Marshal.FreeHGlobal((nint)pqy1); Marshal.FreeHGlobal((nint)pqy2); Marshal.FreeHGlobal((nint)pqi);
                    Marshal.FreeHGlobal((nint)pans);
                }
            }
        }

        private static void Fixed3(long[] a, long[] b, long[] c, int n, out long* pa, out long* pb, out long* pc, out long* pans)
        {
            pa = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            pb = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            pc = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            pans = (long*)Marshal.AllocHGlobal(sizeof(long) * n);
            for (int i = 0; i < n; i++) { pa[i] = a[i]; pb[i] = b[i]; pc[i] = c[i]; }
        }

        private static void FixedPerm(int[] perm, int n, int[] ri, int k, out int* p, out int* r, out long* a)
        {
            p = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            r = (int*)Marshal.AllocHGlobal(sizeof(int) * k);
            a = (long*)Marshal.AllocHGlobal(sizeof(long) * k);
            for (int i = 0; i < n; i++) p[i] = perm[i];
            for (int i = 0; i < k; i++) r[i] = ri[i];
        }

        private static void Fixed2D(int[] et, int[] x, int[] y, long[] d, int[] q1, int[] q2, int[] y1, int[] y2, int[] qi, int E,
            out int* pet, out int* px, out int* py, out long* pd, out int* pq1, out int* pq2, out int* py1, out int* py2, out int* pqi, out long* pa)
        {
            pet = (int*)Marshal.AllocHGlobal(sizeof(int) * E);
            px = (int*)Marshal.AllocHGlobal(sizeof(int) * E);
            py = (int*)Marshal.AllocHGlobal(sizeof(int) * E);
            pd = (long*)Marshal.AllocHGlobal(sizeof(long) * E);
            pq1 = (int*)Marshal.AllocHGlobal(sizeof(int) * E);
            pq2 = (int*)Marshal.AllocHGlobal(sizeof(int) * E);
            py1 = (int*)Marshal.AllocHGlobal(sizeof(int) * E);
            py2 = (int*)Marshal.AllocHGlobal(sizeof(int) * E);
            pqi = (int*)Marshal.AllocHGlobal(sizeof(int) * E);
            pa = (long*)Marshal.AllocHGlobal(sizeof(long) * E);
            for (int i = 0; i < E; i++) { pet[i] = et[i]; px[i] = x[i]; py[i] = y[i]; pd[i] = d[i]; pq1[i] = q1[i]; pq2[i] = q2[i]; py1[i] = y1[i]; py2[i] = y2[i]; pqi[i] = qi[i]; }
        }
    }
}
