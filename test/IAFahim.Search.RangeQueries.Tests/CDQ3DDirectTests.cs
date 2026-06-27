namespace IAFahim.Search.RangeQueries.Tests
{
    using IAFahim.Search.RangeQueries;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class CDQ3DDirectTests
    {
        [Test]
        public void Simple_Dominance()
        {
            // data: (0,0,0),(1,1,1),(2,2,2). queries: (2,2,2)->2, (1,1,1)->1, (0,0,0)->0, (3,3,3)->3
            long[] dx = { 0, 1, 2 };
            long[] dy = { 0, 1, 2 };
            long[] dz = { 0, 1, 2 };
            long[] qx = { 2, 1, 0, 3 };
            long[] qy = { 2, 1, 0, 3 };
            long[] qz = { 2, 1, 0, 3 };
            long[] cnt = new long[4];
            Pin(dx, out long* pdx); Pin(dy, out long* pdy); Pin(dz, out long* pdz);
            Pin(qx, out long* pqx); Pin(qy, out long* pqy); Pin(qz, out long* pqz); Pin(cnt, out long* pcnt);
            try
            {
                CDQ3D.Run(pdx, pdy, pdz, 3, pqx, pqy, pqz, 4, pcnt);
                Assert.AreEqual(2, pcnt[0], "q(2,2,2)");
                Assert.AreEqual(1, pcnt[1], "q(1,1,1)");
                Assert.AreEqual(0, pcnt[2], "q(0,0,0)");
                Assert.AreEqual(3, pcnt[3], "q(3,3,3)");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)pdx); Marshal.FreeHGlobal((nint)pdy); Marshal.FreeHGlobal((nint)pdz);
                Marshal.FreeHGlobal((nint)pqx); Marshal.FreeHGlobal((nint)pqy); Marshal.FreeHGlobal((nint)pqz);
                Marshal.FreeHGlobal((nint)pcnt);
            }
        }

        [Test]
        public void CdqDynamic_KnownCase2()
        {
            int[] perm = { 6, 5, 7, 2, 3, 1, 4 };
            int[] ri = { 5, 6, 2, 1, 4, 3, 0 };
            int n = 7, k = 7;
            int* p = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            int* r = (int*)Marshal.AllocHGlobal(sizeof(int) * k);
            long* a = (long*)Marshal.AllocHGlobal(sizeof(long) * k);
            for (int i = 0; i < n; i++) p[i] = perm[i];
            for (int i = 0; i < k; i++) r[i] = ri[i];
            long got0;
            try { CdqDynamicInversions.Run(p, n, r, k, a); got0 = a[0]; }
            finally { Marshal.FreeHGlobal((nint)p); Marshal.FreeHGlobal((nint)r); Marshal.FreeHGlobal((nint)a); }
            Assert.AreEqual(5, got0);
        }

        [Test]
        public void Negative_Coords_Dominance()
        {
            // mirrors dynamic-inversion left-count: data X=origIdx (0..), Y=-value, Z=-rtime;
            // want count of j with Xj<Xq && Yj<Yq && Zj<Zq with negative Y,Z.
            // data: idx0(v=3,t=5)->(0,-3,-5); idx1(v=1,t=5)->(1,-1,-5); idx2(v=5,t=0)->(2,-5,0)
            // query: e at idx2, te=0 -> (2,-5,0). count j: X<2 && -vj<-5(vj>5 none) -> 0
            long[] dx = { 0, 1, 2 };
            long[] dy = { -3, -1, -5 };
            long[] dz = { -5, -5, 0 };
            long[] qx = { 2 };
            long[] qy = { -5 };
            long[] qz = { 0 };
            long[] cnt = new long[1];
            Pin(dx, out long* pdx); Pin(dy, out long* pdy); Pin(dz, out long* pdz);
            Pin(qx, out long* pqx); Pin(qy, out long* pqy); Pin(qz, out long* pqz); Pin(cnt, out long* pcnt);
            try { CDQ3D.Run(pdx, pdy, pdz, 3, pqx, pqy, pqz, 1, pcnt); Assert.AreEqual(0, pcnt[0], "neg case"); }
            finally { Marshal.FreeHGlobal((nint)pdx); Marshal.FreeHGlobal((nint)pdy); Marshal.FreeHGlobal((nint)pdz); Marshal.FreeHGlobal((nint)pqx); Marshal.FreeHGlobal((nint)pqy); Marshal.FreeHGlobal((nint)pqz); Marshal.FreeHGlobal((nint)pcnt); }

            // second: data idx0(v=1)->(0,-1,-5) dominated by query (1,-1,-5)? X 0<1 T, Y -1<-1 F. ->0
            //        but (0,-2,-5) vs (1,-1,-5): X0<1,Y-2<-1,Z-5<-5 F ->0. Use clearly-dominant:
            long[] dx2 = { 0, 1 };
            long[] dy2 = { -10, -10 };
            long[] dz2 = { -10, -10 };
            long[] qx2 = { 5 };
            long[] qy2 = { -5 };
            long[] qz2 = { -5 };
            long[] cnt2 = new long[1];
            Pin(dx2, out long* pdx2); Pin(dy2, out long* pdy2); Pin(dz2, out long* pdz2);
            Pin(qx2, out long* pqx2); Pin(qy2, out long* pqy2); Pin(qz2, out long* pqz2); Pin(cnt2, out long* pcnt2);
            try { CDQ3D.Run(pdx2, pdy2, pdz2, 2, pqx2, pqy2, pqz2, 1, pcnt2); Assert.AreEqual(2, pcnt2[0], "neg dom"); }
            finally { Marshal.FreeHGlobal((nint)pdx2); Marshal.FreeHGlobal((nint)pdy2); Marshal.FreeHGlobal((nint)pdz2); Marshal.FreeHGlobal((nint)pqx2); Marshal.FreeHGlobal((nint)pqy2); Marshal.FreeHGlobal((nint)pqz2); Marshal.FreeHGlobal((nint)pcnt2); }
        }

        private static void Pin(long[] a, out long* p)
        {
            p = (long*)Marshal.AllocHGlobal(sizeof(long) * a.Length);
            for (int i = 0; i < a.Length; i++) p[i] = a[i];
        }
    }
}
