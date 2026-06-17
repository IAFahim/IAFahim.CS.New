namespace IAFahim.Graph.Matching.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class AssignmentTests
    {
        [Test]
        public void Hungarian_Square_KnownOptimum()
        {
            // rows x cols, optimal = 1+1+1 = 3 (assign diag).
            int n = 3, m = 3;
            int[] mat = { 1, 9, 9, 9, 1, 9, 9, 9, 1 };
            int* cost = Make(mat);
            int* ml = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* mr = (int*)Marshal.AllocHGlobal(m * sizeof(int));
            try
            {
                AssignmentHungarianRectangular.Run(cost, n, m, ml, mr);
                AssertTotalCost(cost, n, m, ml, 3);
                for (int i = 0; i < n; i++) Assert.AreEqual(i, ml[i]);
                for (int j = 0; j < m; j++) Assert.AreEqual(j, mr[j]);
            }
            finally { Marshal.FreeHGlobal((nint)cost); Marshal.FreeHGlobal((nint)ml); Marshal.FreeHGlobal((nint)mr); }
        }

        [Test]
        public void Hungarian_Rectangular_MoreCols_LeavesColsUnmatched()
        {
            // 2 rows, 3 cols: only 2 cols matched.
            int n = 2, m = 3;
            int[] mat = { 1, 5, 9, 5, 1, 9 }; // row0->col0(1), row1->col1(1) total 2
            int* cost = Make(mat);
            int* ml = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* mr = (int*)Marshal.AllocHGlobal(m * sizeof(int));
            try
            {
                AssignmentHungarianRectangular.Run(cost, n, m, ml, mr);
                AssertTotalCost(cost, n, m, ml, 2);
                int matchedCols = 0;
                for (int j = 0; j < m; j++) if (mr[j] >= 0) matchedCols++;
                Assert.AreEqual(n, matchedCols);
            }
            finally { Marshal.FreeHGlobal((nint)cost); Marshal.FreeHGlobal((nint)ml); Marshal.FreeHGlobal((nint)mr); }
        }

        [Test]
        public void Auction_Square_MatchesHungarian()
        {
            int n = 4;
            int[] mat = { 9, 2, 7, 8, 6, 4, 3, 7, 5, 8, 1, 8, 7, 6, 9, 4 };
            int* cost = Make(mat);
            int* match = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* prices = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* ml = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* mr = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                AssignmentAuctionAlgorithm.Run(cost, n, match, prices);
                AssignmentHungarianRectangular.Run(cost, n, n, ml, mr);
                int auctionTotal = TotalCostSquare(cost, n, match);
                int hungarianTotal = TotalCostSquare(cost, n, ml);
                Assert.AreEqual(hungarianTotal, auctionTotal);
                // permutation check
                bool[] seen = new bool[n];
                for (int i = 0; i < n; i++) { Assert.IsTrue(match[i] >= 0 && match[i] < n); Assert.IsFalse(seen[match[i]]); seen[match[i]] = true; }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)cost); Marshal.FreeHGlobal((nint)match);
                Marshal.FreeHGlobal((nint)prices); Marshal.FreeHGlobal((nint)ml); Marshal.FreeHGlobal((nint)mr);
            }
        }

        private static int* Make(int[] a)
        {
            int* p = (int*)Marshal.AllocHGlobal(a.Length * sizeof(int));
            for (int i = 0; i < a.Length; i++) p[i] = a[i];
            return p;
        }

        private static int TotalCostSquare(int* cost, int n, int* match)
        {
            int total = 0;
            for (int i = 0; i < n; i++) total += cost[i * n + match[i]];
            return total;
        }

        private static void AssertTotalCost(int* cost, int n, int m, int* matchLeft, int expected)
        {
            int total = 0;
            for (int i = 0; i < n; i++) if (matchLeft[i] >= 0) total += cost[i * m + matchLeft[i]];
            Assert.AreEqual(expected, total);
        }
    }

    public sealed unsafe class HospitalResidentsTests
    {
        [Test]
        public void EachResidentAtMostOneHospital_CapacityRespected()
        {
            int numR = 4, numH = 2;
            // resident prefs: each lists both hospitals in some order.
            int[] rp = { 0, 1, 0, 1, 1, 0, 1, 0 }; // r0:0,1 r1:0,1 r2:1,0 r3:1,0
            int[] hp = { 0, 1, 2, 3, 0, 1, 2, 3 }; // both hospitals rank residents 0<1<2<3
            int[] cap = { 2, 2 };
            int* rpref = Pin(rp), hpref = Pin(hp), hcap = Pin(cap);
            int* match = (int*)Marshal.AllocHGlobal(numR * sizeof(int));
            try
            {
                HospitalResidentsMatching.Run(rpref, hpref, hcap, numR, numH, match);
                int[] fill = new int[numH];
                bool[] residentUsed = new bool[numR];
                for (int i = 0; i < numR; i++)
                {
                    int h = match[i];
                    if (h < 0) continue;
                    Assert.IsTrue(h >= 0 && h < numH);
                    Assert.IsFalse(residentUsed[i]);
                    residentUsed[i] = true;
                    fill[h]++;
                }
                for (int h = 0; h < numH; h++) Assert.IsTrue(fill[h] <= cap[h]);
            }
            finally { Marshal.FreeHGlobal((nint)rpref); Marshal.FreeHGlobal((nint)hpref); Marshal.FreeHGlobal((nint)hcap); Marshal.FreeHGlobal((nint)match); }
        }

        private static int* Pin(int[] a)
        {
            int* p = (int*)Marshal.AllocHGlobal(a.Length * sizeof(int));
            for (int i = 0; i < a.Length; i++) p[i] = a[i];
            return p;
        }
    }
}
