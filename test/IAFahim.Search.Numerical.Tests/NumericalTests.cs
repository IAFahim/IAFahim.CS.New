namespace IAFahim.Search.Numerical.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class NumericalTests
    {
        [Test]
        public void Simpson_IntegralOfX2_OnUnitInterval()
        {
            double* coeff = stackalloc double[3];
            coeff[0] = 1;
            coeff[1] = 0;
            coeff[2] = 0;
            double got = SimpsonIntegral.Run(coeff, 0, 1, 100);
            Assert.AreEqual(1.0 / 3.0, got, 1e-6);
        }

        [Test]
        public void GaussLegendre_IntegralOfX2()
        {
            double* coeff = stackalloc double[3];
            coeff[0] = 1;
            coeff[1] = 0;
            coeff[2] = 0;
            double got = GaussLegendre.Run(8, 0, 1, coeff);
            Assert.AreEqual(1.0 / 3.0, got, 1e-8);
        }

        [Test]
        public void AdaptiveSimpson_IntegralOfX2()
        {
            double* coeff = stackalloc double[3];
            coeff[0] = 1;
            coeff[1] = 0;
            coeff[2] = 0;
            double got = AdaptiveSimpson.Run(coeff, 0, 1, 1e-10, 20);
            Assert.AreEqual(1.0 / 3.0, got, 1e-8);
        }

        [Test]
        public void TernaryReal_LinearIncreasing_MinimizesAtLeft()
        {
            double* f = stackalloc double[2];
            f[0] = 1;
            f[1] = 0;
            double x = TernaryReal.Run(f, 80, 0, 10);
            Assert.IsTrue(x < 1.0);
        }
    }
}
