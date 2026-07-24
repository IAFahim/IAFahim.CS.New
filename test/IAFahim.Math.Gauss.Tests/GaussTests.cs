namespace IAFahim.Math.Gauss.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class GaussTests
    {
        [Test]
        public void Solve_TwoByTwo()
        {
            double* a = stackalloc double[4];
            double* b = stackalloc double[2];
            double* x = stackalloc double[2];
            a[0] = 2; a[1] = 1;
            a[2] = 1; a[3] = 3;
            b[0] = 5; b[1] = 5;
            int rank = GaussEliminationDouble.Run(a, b, x, 2, 2);
            Assert.AreEqual(2, rank);
            Assert.AreEqual(2.0, x[0], 1e-9);
            Assert.AreEqual(1.0, x[1], 1e-9);
        }

        [Test]
        public void Solve_Identity()
        {
            double* a = stackalloc double[4];
            double* b = stackalloc double[2];
            double* x = stackalloc double[2];
            a[0] = 1; a[1] = 0;
            a[2] = 0; a[3] = 1;
            b[0] = 3; b[1] = 7;
            GaussEliminationDouble.Run(a, b, x, 2, 2);
            Assert.AreEqual(3.0, x[0], 1e-12);
            Assert.AreEqual(7.0, x[1], 1e-12);
        }
    }
}
