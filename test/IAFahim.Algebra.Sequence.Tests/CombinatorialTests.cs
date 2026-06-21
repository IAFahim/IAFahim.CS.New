namespace IAFahim.Algebra.Sequence.Tests
{
    using NUnit.Framework;

    // Verifies Combinatorial number sequences against known OEIS values.
    public sealed class CombinatorialTests
    {
        private const int MOD = 1000000007;

        // Eulerian A(n,k): row n=3 is [1,4,1], n=4 is [1,11,11,1], n=5 is [1,26,66,26,1].
        [Test]
        public void Eulerian_Row3_141()
        {
            Assert.AreEqual(1, Combinatorial.Eulerian(3, 0, MOD));
            Assert.AreEqual(4, Combinatorial.Eulerian(3, 1, MOD));
            Assert.AreEqual(1, Combinatorial.Eulerian(3, 2, MOD));
        }

        [Test]
        public void Eulerian_Row4_1110111()
        {
            Assert.AreEqual(1, Combinatorial.Eulerian(4, 0, MOD));
            Assert.AreEqual(11, Combinatorial.Eulerian(4, 1, MOD));
            Assert.AreEqual(11, Combinatorial.Eulerian(4, 2, MOD));
            Assert.AreEqual(1, Combinatorial.Eulerian(4, 3, MOD));
        }

        [Test]
        public void Eulerian_OutOfRange_Zero()
        {
            Assert.AreEqual(0, Combinatorial.Eulerian(3, -1, MOD));
            Assert.AreEqual(0, Combinatorial.Eulerian(3, 3, MOD));
            Assert.AreEqual(0, Combinatorial.Eulerian(0, 0, MOD));
        }

        // Narayana N(n,k): N(4,1)=1, N(4,2)=6, N(4,3)=6, N(4,4)=1.
        [Test]
        public void Narayana_N4_16661()
        {
            Assert.AreEqual(1, Combinatorial.Narayana(4, 1, MOD));
            Assert.AreEqual(6, Combinatorial.Narayana(4, 2, MOD));
            Assert.AreEqual(6, Combinatorial.Narayana(4, 3, MOD));
            Assert.AreEqual(1, Combinatorial.Narayana(4, 4, MOD));
        }

        [Test]
        public void Narayana_N3_1331()
        {
            Assert.AreEqual(1, Combinatorial.Narayana(3, 1, MOD));
            Assert.AreEqual(3, Combinatorial.Narayana(3, 2, MOD));
            Assert.AreEqual(1, Combinatorial.Narayana(3, 3, MOD));
        }

        [Test]
        public void Narayana_SumRowEqualsCatalan()
        {
            // Sum of N(n,k) over k=1..n = Catalan(n). Catalan(4)=14.
            long sum = 0;
            for (int k = 1; k <= 4; k++) sum += Combinatorial.Narayana(4, k, MOD);
            sum %= MOD;
            // Catalan(4) = 14.
            Assert.AreEqual(14, sum);
        }

        // Lah L(n,k): L(3,1)=6, L(3,2)=6, L(4,2)=36.
        [Test]
        public void Lah_KnownValues()
        {
            Assert.AreEqual(6, Combinatorial.Lah(3, 1, MOD));
            Assert.AreEqual(6, Combinatorial.Lah(3, 2, MOD));
            Assert.AreEqual(36, Combinatorial.Lah(4, 2, MOD));
        }

        [Test]
        public void Lah_OutOfRange_Zero()
        {
            Assert.AreEqual(0, Combinatorial.Lah(3, 0, MOD));
            Assert.AreEqual(0, Combinatorial.Lah(3, 4, MOD));
        }

        // Gaussian binomial [n choose k]_q.
        [Test]
        public void QBinomial_3Choose1_AtQ2_Is7()
        {
            // [3 choose 1]_2 = (2^3-1)/(2-1) = 7.
            Assert.AreEqual(7, Combinatorial.QBinomial(3, 1, 2, MOD));
        }

        [Test]
        public void QBinomial_4Choose2_AtQ2_Is35()
        {
            // [4 choose 2]_2 = (2^4-1)(2^3-1)/((2^2-1)(2-1)) = 15*7/3 = 35.
            Assert.AreEqual(35, Combinatorial.QBinomial(4, 2, 2, MOD));
        }

        [Test]
        public void QBinomial_AtQ1_EqualsBinomial()
        {
            // [n choose k]_1 = C(n,k) by L'Hopital.
            Assert.AreEqual(6, Combinatorial.QBinomial(4, 2, 1, MOD));
            Assert.AreEqual(3, Combinatorial.QBinomial(3, 1, 1, MOD));
            Assert.AreEqual(3, Combinatorial.QBinomial(3, 2, 1, MOD));
        }

        [Test]
        public void GaussianBinomial_AliasMatchesQBinomial()
        {
            Assert.AreEqual(Combinatorial.QBinomial(4, 2, 2, MOD), Combinatorial.GaussianBinomial(4, 2, 2, MOD));
        }

        // Young tableaux via hook length formula: f(shape) = n! / prod(hooks).
        [Test]
        public unsafe void YoungTableaux_Shape21_TwoTableaux()
        {
            int* shape = stackalloc int[2] { 2, 1 };
            Assert.AreEqual(2, Combinatorial.YoungTableaux(shape, 2, MOD));
        }

        [Test]
        public unsafe void YoungTableaux_Shape33_FortyTwo()
        {
            // Shape (3,3): f = 6! / (5*4*3*3*2*1) = 720/360 = ... wait.
            // Actually (3,3) hooks: (0,0)=4,(0,1)=3,(0,2)=2,(1,0)=3,(1,1)=2,(1,2)=1.
            // f = 720/(4*3*2*3*2*1) = 720/144 = 5.
            int* shape = stackalloc int[2] { 3, 3 };
            Assert.AreEqual(5, Combinatorial.YoungTableaux(shape, 2, MOD));
        }

        [Test]
        public unsafe void YoungTableaux_SingleRow_OneTableaux()
        {
            // Shape (n): always exactly 1 tableau (strictly increasing).
            int* shape = stackalloc int[1] { 5 };
            Assert.AreEqual(1, Combinatorial.YoungTableaux(shape, 1, MOD));
        }

        [Test]
        public unsafe void HookLength_AliasMatchesYoungTableaux()
        {
            int* shape = stackalloc int[2] { 2, 1 };
            Assert.AreEqual(Combinatorial.YoungTableaux(shape, 2, MOD), Combinatorial.HookLength(shape, 2, MOD));
        }
    }
}
