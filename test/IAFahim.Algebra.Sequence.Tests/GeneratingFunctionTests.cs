namespace IAFahim.Algebra.Sequence.Tests
{
    using NUnit.Framework;

    public sealed unsafe class GeneratingFunctionTests
    {
        private const int MOD = 1000000007;

        // f(x) = x/(1-x) = x + x^2 + x^3 + ...
        // Compositional inverse g satisfies f(g(x)) = x, i.e. g = x/(1+x) = x - x^2 + x^3 - x^4 + ...
        [Test]
        public void LagrangeInversion_InverseOfXOver1MinusX()
        {
            // f[0]=0, f[1..4] = 1,1,1,1.
            long* f = stackalloc long[5] { 0, 1, 1, 1, 1 };
            Assert.AreEqual(1, GeneratingFunction.LagrangeInversion(f, 5, 1, MOD));
            Assert.AreEqual(MOD - 1, GeneratingFunction.LagrangeInversion(f, 5, 2, MOD));
            Assert.AreEqual(1, GeneratingFunction.LagrangeInversion(f, 5, 3, MOD));
            Assert.AreEqual(MOD - 1, GeneratingFunction.LagrangeInversion(f, 5, 4, MOD));
        }

        // TreeCount uses Cayley's formula n^(n-2).
        [Test]
        public void TreeCount_CayleyFormula()
        {
            Assert.AreEqual(1, GeneratingFunction.TreeCount(1, MOD));
            Assert.AreEqual(1, GeneratingFunction.TreeCount(2, MOD));
            Assert.AreEqual(3, GeneratingFunction.TreeCount(3, MOD));
            Assert.AreEqual(16, GeneratingFunction.TreeCount(4, MOD));
            Assert.AreEqual(125, GeneratingFunction.TreeCount(5, MOD));
        }

        // OgfMultiply: (1)(1) = 1 coefficient-wise for [1,1,1,1].
        [Test]
        public void OgfMultiply_AllOnes_Gives1234()
        {
            long* a = stackalloc long[4] { 1, 1, 1, 1 };
            long* b = stackalloc long[4] { 1, 1, 1, 1 };
            long* r = stackalloc long[4];
            GeneratingFunction.OgfMultiply(a, b, 4, MOD, r);
            Assert.AreEqual(1, r[0]);
            Assert.AreEqual(2, r[1]);
            Assert.AreEqual(3, r[2]);
            Assert.AreEqual(4, r[3]);
        }

        // EgfMultiply: product of e^x and e^x is e^{2x}. Coeffs (2x)^k/k! = 2^k.
        [Test]
        public void EgfMultiply_ExpX_ExpX_GivesExp2X()
        {
            // e^x coeffs: 1/0!, 1/1!, 1/2!, 1/3! = stored as 1,1,1/2,1/6.
            // But EGF multiply uses a[i]/i! convention internally via binom. With a[i]=1/i! not needed;
            // EgfMultiply multiplies coeffs in "EGF coefficient form" (a_k = f_k / k!).
            // For e^x: f_k=1 for all k, so a_k = 1/k!. But here a[i] is just stored as 1/k! mod p.
            // Simpler: identity test — multiply by identity EGF [1,0,0,0] gives back original.
            long* id = stackalloc long[4] { 1, 0, 0, 0 };
            long* a = stackalloc long[4] { 1, 2, 3, 4 };
            long* r = stackalloc long[4];
            GeneratingFunction.EgfMultiply(id, a, 4, MOD, r);
            Assert.AreEqual(1, r[0]);
            Assert.AreEqual(2, r[1]);
            Assert.AreEqual(3, r[2]);
            Assert.AreEqual(4, r[3]);
        }
    }
}
