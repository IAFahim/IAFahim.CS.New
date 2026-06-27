namespace IAFahim.Search.RangeQueries.Tests
{
    using IAFahim.Search.RangeQueries;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class TimeSegmentTreeTests
    {
        [Test]
        public void SegmentTreeOverTimeAdd_MatchesBrute()
        {
            Random rng = new Random(21);
            for (int t = 0; t < 100; t++)
            {
                int T = rng.Next(1, 15), m = rng.Next(0, 12);
                int[] l = new int[m], r = new int[m]; long[] d = new long[m];
                long[] brute = new long[T];
                for (int i = 0; i < m; i++)
                {
                    int a = rng.Next(0, T), b = rng.Next(0, T);
                    l[i] = Math.Min(a, b); r[i] = Math.Max(a, b);
                    d[i] = rng.Next(-5, 6);
                    for (int k = l[i]; k <= r[i]; k++) brute[k] += d[i];
                }
                long[] ans = new long[T];
                PinI(l, out int* pl); PinI(r, out int* pr); PinL(d, out long* pd); PinL(ans, out long* pa);
                try { SegmentTreeOverTimeAdd.Run(pl, pr, pd, m, T, pa); for (int i = 0; i < T; i++) ans[i] = pa[i]; }
                finally { Marshal.FreeHGlobal((nint)pl); Marshal.FreeHGlobal((nint)pr); Marshal.FreeHGlobal((nint)pd); Marshal.FreeHGlobal((nint)pa); }
                for (int i = 0; i < T; i++) Assert.AreEqual(brute[i], ans[i], $"t={t} time={i}");
            }
        }

        [Test]
        public void DivideConquerOnTime_MatchesBrute()
        {
            Random rng = new Random(33);
            for (int t = 0; t < 150; t++)
            {
                int n = rng.Next(1, 10), T = rng.Next(1, 10), m = rng.Next(0, 12);
                int[] eu = new int[m], ev = new int[m], el = new int[m], er = new int[m];
                for (int i = 0; i < m; i++)
                {
                    eu[i] = rng.Next(n); ev[i] = rng.Next(n);
                    int a = rng.Next(0, T), b = rng.Next(0, T);
                    el[i] = Math.Min(a, b); er[i] = Math.Max(a, b);
                }
                int q = rng.Next(1, 12);
                int[] qu = new int[q], qv = new int[q], qt = new int[q];
                for (int j = 0; j < q; j++) { qu[j] = rng.Next(n); qv[j] = rng.Next(n); qt[j] = rng.Next(0, T); }

                int[] ans = new int[q];
                int[] brute = new int[q];
                for (int j = 0; j < q; j++)
                {
                    int[] par = new int[n];
                    for (int i = 0; i < n; i++) par[i] = i;
                    for (int i = 0; i < m; i++)
                        if (qt[j] >= el[i] && qt[j] <= er[i]) Union(par, eu[i], ev[i]);
                    brute[j] = Find(par, qu[j]) == Find(par, qv[j]) ? 1 : 0;
                }

                PinI(eu, out int* peu); PinI(ev, out int* pev); PinI(el, out int* pel); PinI(er, out int* per);
                PinI(qu, out int* pqu); PinI(qv, out int* pqv); PinI(qt, out int* pqt); PinI(ans, out int* pans);
                try { DivideConquerOnTime.Run(n, peu, pev, pel, per, m, pqu, pqv, pqt, q, T, pans); for (int j = 0; j < q; j++) ans[j] = pans[j]; }
                finally
                {
                    Marshal.FreeHGlobal((nint)peu); Marshal.FreeHGlobal((nint)pev); Marshal.FreeHGlobal((nint)pel); Marshal.FreeHGlobal((nint)per);
                    Marshal.FreeHGlobal((nint)pqu); Marshal.FreeHGlobal((nint)pqv); Marshal.FreeHGlobal((nint)pqt); Marshal.FreeHGlobal((nint)pans);
                }
                for (int j = 0; j < q; j++) Assert.AreEqual(brute[j], ans[j], $"t={t} q={j} u={qu[j]} v={qv[j]} time={qt[j]}");
            }
        }

        [Test]
        public void SegmentTreeOverTimeDfs_ComponentCount_MatchesBrute()
        {
            Random rng = new Random(44);
            for (int t = 0; t < 150; t++)
            {
                int n = rng.Next(1, 10), T = rng.Next(1, 10), m = rng.Next(0, 12);
                int[] eu = new int[m], ev = new int[m], el = new int[m], er = new int[m];
                for (int i = 0; i < m; i++)
                {
                    eu[i] = rng.Next(n); ev[i] = rng.Next(n);
                    int a = rng.Next(0, T), b = rng.Next(0, T);
                    el[i] = Math.Min(a, b); er[i] = Math.Max(a, b);
                }
                int[] brute = new int[T];
                for (int tm = 0; tm < T; tm++)
                {
                    int[] par = new int[n];
                    for (int i = 0; i < n; i++) par[i] = i;
                    for (int i = 0; i < m; i++)
                        if (tm >= el[i] && tm <= er[i]) Union(par, eu[i], ev[i]);
                    int c = 0; for (int i = 0; i < n; i++) if (Find(par, i) == i) c++;
                    brute[tm] = c;
                }
                int[] ans = new int[T];
                PinI(eu, out int* peu); PinI(ev, out int* pev); PinI(el, out int* pel); PinI(er, out int* per); PinI(ans, out int* pans);
                try { SegmentTreeOverTimeDfs.Run(n, peu, pev, pel, per, m, T, pans); for (int tm = 0; tm < T; tm++) ans[tm] = pans[tm]; }
                finally { Marshal.FreeHGlobal((nint)peu); Marshal.FreeHGlobal((nint)pev); Marshal.FreeHGlobal((nint)pel); Marshal.FreeHGlobal((nint)per); Marshal.FreeHGlobal((nint)pans); }
                for (int tm = 0; tm < T; tm++) Assert.AreEqual(brute[tm], ans[tm], $"t={t} time={tm}");
            }
        }

        private static void Union(int[] par, int u, int v)
        { int ru = Find(par, u), rv = Find(par, v); if (ru != rv) par[ru] = rv; }
        private static int Find(int[] par, int x) { while (par[x] != x) { par[x] = par[par[x]]; x = par[x]; } return x; }

        private static void PinI(int[] a, out int* p) { p = (int*)Marshal.AllocHGlobal(sizeof(int) * a.Length); for (int i = 0; i < a.Length; i++) p[i] = a[i]; }
        private static void PinL(long[] a, out long* p) { p = (long*)Marshal.AllocHGlobal(sizeof(long) * a.Length); for (int i = 0; i < a.Length; i++) p[i] = a[i]; }
    }
}
