namespace IAFahim.Graph.DeferredImpl.Tests
{
    using IAFahim.Graph.Functional;
    using IAFahim.Graph.Cactus;
    using IAFahim.Graph.TreeIsomorphism;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class FunctionalGraphRerootTests
    {
        [Test]
        public void Reroot_PathReversal_NewRootHasNoParent()
        {
            // tree: 0<-1<-2<-3 (parent: p[3]=2,p[2]=1,p[1]=0,p[0]=-1). reroot at 3.
            int[] p = { -1, 0, 1, 2 };
            int n = 4;
            int* pp = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            int* res = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            try
            {
                for (int i = 0; i < n; i++) pp[i] = p[i];
                Assert.IsTrue(FunctionalGraphReroot.Run(pp, n, 3, res));
                Assert.AreEqual(-1, res[3], "new root has no parent");
                Assert.AreEqual(3, res[2], "2's parent is now 3");
                Assert.AreEqual(2, res[1], "1's parent is now 2");
                Assert.AreEqual(1, res[0], "0's parent is now 1");
            }
            finally { Marshal.FreeHGlobal((nint)pp); Marshal.FreeHGlobal((nint)res); }
        }

        [Test]
        public void Reroot_InvalidInput_ReturnsFalse()
        {
            int[] p = { -1, 0 };
            int* pp = (int*)Marshal.AllocHGlobal(sizeof(int) * 2);
            int* res = (int*)Marshal.AllocHGlobal(sizeof(int) * 2);
            try
            {
                pp[0] = -1; pp[1] = 0;
                Assert.IsFalse(FunctionalGraphReroot.Run(pp, 2, 5, res), "u out of range");
            }
            finally { Marshal.FreeHGlobal((nint)pp); Marshal.FreeHGlobal((nint)res); }
        }
    }

    public sealed unsafe class CactusShortestPathTests
    {
        [Test]
        public void Cactus_TwoPathsAroundCycle_PicksShorter()
        {
            // square cycle 0-1-2-3-0, all weight 1; plus shortcut? keep pure cactus: just the cycle.
            // 0-1(1),1-2(1),2-3(1),3-0(1). shortest 0->2 = 2.
            int n = 4;
            int m = 8;
            int* head = MakeArr(n, -1);
            int* to = MakeArr(m, 0);
            int* nx = MakeArr(m, -1);
            int* wt = MakeArr(m, 1);
            int[] us = { 0, 1, 2, 3 };
            int[] vs = { 1, 2, 3, 0 };
            int edge = 0;
            for (int i = 0; i < 4; i++)
            {
                AddEdge(head, to, nx, us[i], vs[i], ref edge);
                AddEdge(head, to, nx, vs[i], us[i], ref edge);
            }
            try
            {
                Assert.AreEqual(2, CactusShortestPath.Run(head, to, nx, wt, n, m, 0, 2), "0->2 around cycle");
                Assert.AreEqual(0, CactusShortestPath.Run(head, to, nx, wt, n, m, 1, 1), "u==v");
                Assert.AreEqual(2, CactusShortestPath.Run(head, to, nx, wt, n, m, 3, 1), "3->1");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)head); Marshal.FreeHGlobal((nint)to);
                Marshal.FreeHGlobal((nint)nx); Marshal.FreeHGlobal((nint)wt);
            }
        }

        private static void AddEdge(int* head, int* to, int* nx, int u, int v, ref int edge)
        {
            to[edge] = v; nx[edge] = head[u]; head[u] = edge; edge++;
        }

        private static int* MakeArr(int len, int val)
        {
            int* a = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            for (int i = 0; i < len; i++) a[i] = val;
            return a;
        }
    }

    public sealed unsafe class UnorderedTreeEditDistanceTests
    {
        [Test]
        public void Constrained_Identity_IsZero()
        {
            int[] p = { -1, 0, 0, 1, 1 };
            int n = 5;
            int* pp = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            try
            {
                for (int i = 0; i < n; i++) pp[i] = p[i];
                Assert.AreEqual(0, UnorderedTreeEditDistance.RunConstrained(pp, n, pp, n), "identical trees -> 0");
            }
            finally { Marshal.FreeHGlobal((nint)pp); }
        }

        [Test]
        public void Constrained_InsertDeleteSubtree()
        {
            // T1: root 0 with child 1 (chain of 2). T2: single node.
            int[] p1 = { -1, 0 };
            int[] p2 = { -1 };
            Fixed(p1, out int* a1); Fixed(p2, out int* a2);
            try
            {
                Assert.AreEqual(1, UnorderedTreeEditDistance.RunConstrained(a1, 2, a2, 1), "delete the child subtree (size 1)");
            }
            finally { Marshal.FreeHGlobal((nint)a1); Marshal.FreeHGlobal((nint)a2); }
        }

        [Test]
        public void Constrained_MatchesBrute_Symmetric()
        {
            Random rng = new Random(11);
            for (int t = 0; t < 300; t++)
            {
                int n1 = rng.Next(1, 6), n2 = rng.Next(1, 6);
                int[] p1 = RandomTree(rng, n1), p2 = RandomTree(rng, n2);
                Fixed(p1, out int* a1); Fixed(p2, out int* a2);
                try
                {
                    int d12 = UnorderedTreeEditDistance.RunConstrained(a1, n1, a2, n2);
                    int d21 = UnorderedTreeEditDistance.RunConstrained(a2, n2, a1, n1);
                    int b12 = BruteConstrained(p1, p2);
                    int b21 = BruteConstrained(p2, p1);
                    Assert.AreEqual(b12, d12, $"t={t}: impl vs brute (dir 1) n1={n1} n2={n2}");
                    Assert.AreEqual(b21, d21, $"t={t}: impl vs brute (dir 2) p1=[{string.Join(",",p1)}] p2=[{string.Join(",",p2)}]");
                    Assert.AreEqual(b12, b21, $"t={t}: brute symmetric");
                    Assert.IsTrue(d12 >= 0 && d12 <= n1 + n2, $"range t={t}");
                    if (SameSet(p1, p2, n1, n2)) Assert.AreEqual(0, d12, $"equal trees t={t}");
                }
                finally { Marshal.FreeHGlobal((nint)a1); Marshal.FreeHGlobal((nint)a2); }
            }
        }

        private static int BruteConstrained(int[] p, int[] q)
        {
            int n = p.Length, m = q.Length;
            int r1 = Array.IndexOf(p, -1), r2 = Array.IndexOf(q, -1);
            var cb1 = new System.Collections.Generic.List<int>[n];
            var cb2 = new System.Collections.Generic.List<int>[m];
            for (int i = 0; i < n; i++) cb1[i] = new System.Collections.Generic.List<int>();
            for (int i = 0; i < m; i++) cb2[i] = new System.Collections.Generic.List<int>();
            for (int i = 0; i < n; i++) if (p[i] >= 0) cb1[p[i]].Add(i);
            for (int i = 0; i < m; i++) if (q[i] >= 0) cb2[q[i]].Add(i);
            int s1 = Size(p, r1), s2 = Size(q, r2);
            return B(r1, r2, cb1, cb2, p, q);

            int Size(int[] pp, int root)
            {
                int s = 1;
                foreach (int c in Ch(pp, root)) s += Size(pp, c);
                return s;
            }
            System.Collections.Generic.IEnumerable<int> Ch(int[] pp, int x)
            {
                for (int i = 0; i < pp.Length; i++) if (pp[i] == x) yield return i;
            }
            int B(int a, int b, System.Collections.Generic.List<int>[] c1, System.Collections.Generic.List<int>[] c2, int[] pp, int[] qq)
            {
                var ca = c1[a]; var cb = c2[b];
                if (ca.Count == 0 && cb.Count == 0) return 0;
                int best = int.MaxValue;
                bool[] usedB = new bool[cb.Count];
                Rec(0, 0, usedB);
                return best;
                void Rec(int i, int acc, bool[] ub)
                {
                    if (i == ca.Count)
                    {
                        for (int j = 0; j < cb.Count; j++) if (!ub[j]) acc += Size(qq, cb[j]);
                        if (acc < best) best = acc;
                        return;
                    }
                    Rec(i + 1, acc + Size(pp, ca[i]), ub);
                    for (int j = 0; j < cb.Count; j++)
                        if (!ub[j])
                        {
                            ub[j] = true;
                            Rec(i + 1, acc + B(ca[i], cb[j], c1, c2, pp, qq), ub);
                            ub[j] = false;
                        }
                }
            }
        }

        private static void Fixed(int[] arr, out int* p)
        {
            p = (int*)Marshal.AllocHGlobal(sizeof(int) * arr.Length);
            for (int i = 0; i < arr.Length; i++) p[i] = arr[i];
        }

        private static int[] RandomTree(Random rng, int n)
        {
            int[] p = new int[n];
            for (int i = 0; i < n; i++) p[i] = i == 0 ? -1 : rng.Next(0, i);
            return p;
        }

        private static bool SameSet(int[] a, int[] b, int n1, int n2)
        {
            if (n1 != n2) return false;
            for (int i = 0; i < n1; i++) if (a[i] != b[i]) return false;
            return true;
        }
    }
}
