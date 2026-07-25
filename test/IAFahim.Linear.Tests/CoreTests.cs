namespace IAFahim.Linear.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class GaussianEliminationTests
    {
        [Test]
        public void Solve_2x2_Identity()
        {
            double* a = stackalloc double[4] { 2, 1, 1, 3 };
            double* b = stackalloc double[2] { 5, 10 };
            double* x = stackalloc double[2];
            Assert.IsTrue(GaussianElimination.Solve(a, b, x, 2));
            Assert.IsTrue(Math.Abs(x[0] - 1) < 1e-9);
            Assert.IsTrue(Math.Abs(x[1] - 3) < 1e-9);
        }

        [Test]
        public void Determinant_Identity3()
        {
            double* a = stackalloc double[9] { 1,0,0, 0,1,0, 0,0,1 };
            Assert.IsTrue(Math.Abs(GaussianElimination.Determinant(a, 3) - 1) < 1e-9);
        }
    }
}
