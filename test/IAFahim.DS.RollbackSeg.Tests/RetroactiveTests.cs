namespace IAFahim.DS.RollbackSeg.Tests
{
    using IAFahim.DS.RollbackSeg;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class RetroactiveTests
    {
        [Test]
        public void Connectivity_MatchesBrute()
        {
            Random rng = new Random(71);
            for (int t = 0; t < 150; t++)
            {
                int n = rng.Next(1, 10), T = rng.Next(1, 10), m = rng.Next(0, 12);
                int[] u = new int[m], v = new int[m], st = new int[m], en = new int[m];
                for (int i = 0; i < m; i++)
                {
                    u[i] = rng.Next(n); v[i] = rng.Next(n);
                    int a = rng.Next(0, T), b = rng.Next(0, T);
                    st[i] = Math.Min(a, b); en[i] = Math.Max(a, b);
                }
                int q = rng.Next(1, 12);
                int[] qu = new int[q], qv = new int[q], qt = new int[q], ans = new int[q], brute = new int[q];
                for (int j = 0; j < q; j++) { qu[j] = rng.Next(n); qv[j] = rng.Next(n); qt[j] = rng.Next(0, T); }
                for (int j = 0; j < q; j++)
                {
                    int[] par = new int[n]; for (int i = 0; i < n; i++) par[i] = i;
                    for (int i = 0; i < m; i++) if (qt[j] >= st[i] && qt[j] <= en[i]) Uni(par, u[i], v[i]);
                    brute[j] = Fnd(par, qu[j]) == Fnd(par, qv[j]) ? 1 : 0;
                }
                Pin(u, out int* pu); Pin(v, out int* pv); Pin(st, out int* ps); Pin(en, out int* pe);
                Pin(qu, out int* pqu); Pin(qv, out int* pqv); Pin(qt, out int* pqt); Pin(ans, out int* pa);
                try { RetroactiveConnectivity.Run(n, pu, pv, ps, pe, m, pqu, pqv, pqt, q, T, pa); for (int j = 0; j < q; j++) ans[j] = pa[j]; }
                finally { Free(pu); Free(pv); Free(ps); Free(pe); Free(pqu); Free(pqv); Free(pqt); Free(pa); }
                for (int j = 0; j < q; j++) Assert.AreEqual(brute[j], ans[j], $"t={t} q={j}");
            }
        }

        [Test]
        public void Queue_MatchesBrute()
        {
            Random rng = new Random(72);
            for (int t = 0; t < 200; t++)
            {
                int O = rng.Next(0, 14);
                int[] op = new int[O], val = new int[O];
                var sim = new System.Collections.Generic.Queue<int>();
                for (int i = 0; i < O; i++)
                {
                    if (sim.Count == 0 || rng.Next(2) == 0) { op[i] = 0; val[i] = rng.Next(1, 100); sim.Enqueue(val[i]); }
                    else { op[i] = 1; sim.Dequeue(); }
                }
                int Q = rng.Next(1, 8);
                int[] qt = new int[Q], ans = new int[Q], brute = new int[Q];
                for (int j = 0; j < Q; j++)
                {
                    qt[j] = rng.Next(0, O + 1);
                    var q2 = new System.Collections.Generic.Queue<int>();
                    for (int i = 0; i <= qt[j] && i < O; i++) { if (op[i] == 0) q2.Enqueue(val[i]); else if (q2.Count > 0) q2.Dequeue(); }
                    brute[j] = q2.Count > 0 ? q2.Peek() : 0;
                }
                Pin(op, out int* pop); Pin(val, out int* pval); Pin(qt, out int* pqt); Pin(ans, out int* pa);
                try { RetroactiveQueueInsert.Run(pop, pval, O, pqt, Q, pa); for (int j = 0; j < Q; j++) ans[j] = pa[j]; }
                finally { Free(pop); Free(pval); Free(pqt); Free(pa); }
                for (int j = 0; j < Q; j++) Assert.AreEqual(brute[j], ans[j], $"t={t} q={j} qt={qt[j]}");
            }
        }

        [Test]
        public void PriorityQueue_MatchesBrute()
        {
            Random rng = new Random(73);
            for (int t = 0; t < 200; t++)
            {
                int O = rng.Next(0, 14);
                int[] op = new int[O], val = new int[O];
                var sim = new System.Collections.Generic.SortedSet<int>();
                // SortedSet loses duplicates; use a multiset via list
                var ms = new System.Collections.Generic.List<int>();
                for (int i = 0; i < O; i++)
                {
                    if (ms.Count == 0 || rng.Next(2) == 0) { op[i] = 0; val[i] = rng.Next(1, 50); InsertMs(ms, val[i]); }
                    else { op[i] = 1; ms.RemoveAt(0); }
                }
                int Q = rng.Next(1, 8);
                int[] qt = new int[Q], ans = new int[Q], brute = new int[Q];
                for (int j = 0; j < Q; j++)
                {
                    qt[j] = rng.Next(0, O + 1);
                    var m2 = new System.Collections.Generic.List<int>();
                    for (int i = 0; i <= qt[j] && i < O; i++) { if (op[i] == 0) InsertMs(m2, val[i]); else if (m2.Count > 0) m2.RemoveAt(0); }
                    brute[j] = m2.Count > 0 ? m2[0] : 0;
                }
                Pin(op, out int* pop); Pin(val, out int* pval); Pin(qt, out int* pqt); Pin(ans, out int* pa);
                try { RetroactivePriorityQueueInsert.Run(pop, pval, O, pqt, Q, pa); for (int j = 0; j < Q; j++) ans[j] = pa[j]; }
                finally { Free(pop); Free(pval); Free(pqt); Free(pa); }
                for (int j = 0; j < Q; j++) Assert.AreEqual(brute[j], ans[j], $"t={t} q={j} qt={qt[j]}");
            }
        }

        private static void InsertMs(System.Collections.Generic.List<int> ms, int v)
        {
            int lo = 0, hi = ms.Count;
            while (lo < hi) { int mid = (lo + hi) >> 1; if (ms[mid] < v) lo = mid + 1; else hi = mid; }
            ms.Insert(lo, v);
        }
        private static void Uni(int[] par, int u, int v) { int ru = Fnd(par, u), rv = Fnd(par, v); if (ru != rv) par[ru] = rv; }
        private static int Fnd(int[] par, int x) { while (par[x] != x) { par[x] = par[par[x]]; x = par[x]; } return x; }

        private static void Pin(int[] a, out int* p) { p = (int*)Marshal.AllocHGlobal(sizeof(int) * (a.Length + 1)); for (int i = 0; i < a.Length; i++) p[i] = a[i]; }
        private static void Free(int* p) { Marshal.FreeHGlobal((nint)p); }
    }
}
