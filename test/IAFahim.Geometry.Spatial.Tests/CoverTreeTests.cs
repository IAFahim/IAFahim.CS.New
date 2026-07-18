namespace IAFahim.Geometry.Spatial.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class CoverTreeTests
    {
        [Test]
        public void Empty_NearestNegative()
        {
            Assert.AreEqual(-1, CoverTree.Nearest(null, 0, 0, 0));
        }

        [Test]
        public void Build_NearestMatchesBruteForce()
        {
            const int N = 16;
            double* xs = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            double* ys = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            CoverTree.Node* nodes = (CoverTree.Node*)Marshal.AllocHGlobal(N * sizeof(CoverTree.Node));
            try
            {
                for (int i = 0; i < N; i++)
                {
                    xs[i] = (i % 4) * 1.0;
                    ys[i] = (i / 4) * 1.0;
                }
                int built = CoverTree.Build(xs, ys, N, nodes);
                Assert.AreEqual(N, built);

                double[] queriesX = { 0.1, 3.0, 1.5, 2.2, 0.0, 2.9 };
                double[] queriesY = { 0.1, 3.0, 1.4, 0.0, 2.0, 1.1 };
                for (int q = 0; q < queriesX.Length; q++)
                {
                    int got = CoverTree.Nearest(nodes, N, queriesX[q], queriesY[q]);
                    Assert.IsTrue(got >= 0 && got < N);
                    double gotD = Dist2(xs[got], ys[got], queriesX[q], queriesY[q]);
                    double wantD = BruteDist2(xs, ys, N, queriesX[q], queriesY[q]);
                    Assert.AreEqual(wantD, gotD, 1e-12);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)xs);
                Marshal.FreeHGlobal((nint)ys);
                Marshal.FreeHGlobal((nint)nodes);
            }
        }

        [Test]
        public void Build_ParentLinksFormTree()
        {
            const int N = 8;
            double* xs = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            double* ys = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            CoverTree.Node* nodes = (CoverTree.Node*)Marshal.AllocHGlobal(N * sizeof(CoverTree.Node));
            try
            {
                for (int i = 0; i < N; i++)
                {
                    xs[i] = i;
                    ys[i] = i * 0.5;
                }
                CoverTree.Build(xs, ys, N, nodes);
                int roots = 0;
                for (int i = 0; i < N; i++)
                {
                    if (nodes[i].Next < 0) roots++;
                    else Assert.IsTrue(nodes[i].Next >= 0 && nodes[i].Next < N);
                }
                Assert.AreEqual(1, roots);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)xs);
                Marshal.FreeHGlobal((nint)ys);
                Marshal.FreeHGlobal((nint)nodes);
            }
        }

        private static double Dist2(double x1, double y1, double x2, double y2)
        {
            double dx = x1 - x2;
            double dy = y1 - y2;
            return dx * dx + dy * dy;
        }

        private static double BruteDist2(double* xs, double* ys, int n, double qx, double qy)
        {
            double bd = double.MaxValue;
            for (int i = 0; i < n; i++)
            {
                double d = Dist2(xs[i], ys[i], qx, qy);
                if (d < bd) bd = d;
            }
            return bd;
        }
    }
}
