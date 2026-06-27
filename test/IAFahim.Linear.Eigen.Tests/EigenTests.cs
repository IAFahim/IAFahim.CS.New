namespace IAFahim.Linear.Eigen.Tests
{
    using IAFahim.Linear.Eigen;
    using System;
    using NUnit.Framework;

    public sealed class EigenTests
    {
        [Test]
        public void SymmetricEigen_KnownDiagonalAndRotation()
        {
            double[] a = { 3, 0, 0, 0, 1, 0, 0, 0, 2 };
            double[] vals = new double[3];
            double[] vecs = new double[9];
            unsafe
            {
                fixed (double* ap = a, vp = vals, vecp = vecs)
                    SymmetricEigen3.Run(ap, vp, vecp);
            }
            Assert.AreEqual(1.0, vals[0], 1e-9);
            Assert.AreEqual(2.0, vals[1], 1e-9);
            Assert.AreEqual(3.0, vals[2], 1e-9);

            double[] sym = { 4, 1, 0, 1, 3, 0, 0, 0, 5 };
            unsafe
            {
                fixed (double* ap = sym, vp = vals, vecp = vecs)
                    SymmetricEigen3.Run(ap, vp, vecp);
            }
            Assert.IsTrue(Reconstructs(sym, vals, vecs), "A reconstructed from V D V^T");
        }

        [Test]
        public void SymmetricEigen_RandomReconstructs()
        {
            Random rng = new Random(55);
            for (int t = 0; t < 100; t++)
            {
                double[] a = new double[9];
                for (int i = 0; i < 9; i++) a[i] = rng.NextDouble() * 10 - 5;
                a[1] = a[3]; a[2] = a[6]; a[5] = a[7];
                double[] vals = new double[3];
                double[] vecs = new double[9];
                unsafe
                {
                    fixed (double* ap = a, vp = vals, vecp = vecs)
                        SymmetricEigen3.Run(ap, vp, vecp);
                }
                Assert.IsTrue(Reconstructs(a, vals, vecs), $"reconstruct t={t}");
                Assert.IsTrue(Orthonormal(vecs), $"orthonormal V t={t}");
            }
        }

        [Test]
        public void Svd_RandomReconstructs_Orthogonal()
        {
            Random rng = new Random(99);
            for (int t = 0; t < 50; t++)
            {
                double[] a = new double[9];
                for (int i = 0; i < 9; i++) a[i] = rng.NextDouble() * 10 - 5;
                double[] u = new double[9], s = new double[3], v = new double[9];
                unsafe
                {
                    fixed (double* ap = a, up = u, sp = s, vp = v)
                        Svd3.Run(ap, up, sp, vp);
                }
                Assert.IsTrue(Orthonormal(u), $"orthonormal U t={t}");
                Assert.IsTrue(Orthonormal(v), $"orthonormal V t={t}");
                double[] recon = new double[9];
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                    {
                        double sum = 0;
                        for (int k = 0; k < 3; k++) sum += u[i * 3 + k] * s[k] * v[j * 3 + k];
                        recon[i * 3 + j] = sum;
                    }
                for (int i = 0; i < 9; i++)
                    Assert.AreEqual(a[i], recon[i], 1e-6, $"svd reconstruct t={t} idx={i}");
            }
        }

        private static bool Reconstructs(double[] a, double[] vals, double[] vecs)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < 3; k++) sum += vecs[i * 3 + k] * vals[k] * vecs[j * 3 + k];
                    if (Math.Abs(sum - a[i * 3 + j]) > 1e-6) return false;
                }
            return true;
        }

        private static bool Orthonormal(double[] m)
        {
            for (int c = 0; c < 3; c++)
            {
                double norm = 0;
                for (int r = 0; r < 3; r++) norm += m[r * 3 + c] * m[r * 3 + c];
                if (Math.Abs(norm - 1.0) > 1e-6) return false;
            }
            for (int c1 = 0; c1 < 3; c1++)
                for (int c2 = c1 + 1; c2 < 3; c2++)
                {
                    double dot = 0;
                    for (int r = 0; r < 3; r++) dot += m[r * 3 + c1] * m[r * 3 + c2];
                    if (Math.Abs(dot) > 1e-6) return false;
                }
            return true;
        }
    }
}
