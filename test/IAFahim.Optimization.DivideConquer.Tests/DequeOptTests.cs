namespace IAFahim.Optimization.DivideConquer.Tests
{
    using NUnit.Framework;

    public sealed unsafe class DequeOptTests
    {
        [Test]
        public void Eval_MatchesQuadratic()
        {
            DequeOpt.Quad q;
            q.A = 1; q.B = -2; q.C = 1;
            Assert.AreEqual(0, DequeOpt.Eval(q, 1));
            Assert.AreEqual(1, DequeOpt.Eval(q, 0));
            Assert.AreEqual(1, DequeOpt.Eval(q, 2));
        }

        [Test]
        public void IntersectX_TwoLines()
        {
            DequeOpt.Quad p; p.A = 0; p.B = 2; p.C = 0;
            DequeOpt.Quad q; q.A = 0; q.B = 0; q.C = 10;
            long x = DequeOpt.IntersectX(p, q);
            Assert.AreEqual(5, x);
        }
    }
}
