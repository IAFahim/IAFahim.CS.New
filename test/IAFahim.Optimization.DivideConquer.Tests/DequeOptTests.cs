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

        [Test]
        public void Run_LowerEnvelopeAtPoints()
        {
            const int n = 3;
            long* dp = stackalloc long[n];
            DequeOpt.Quad* quads = stackalloc DequeOpt.Quad[n + 1];
            for (int i = 0; i <= n; i++)
            {
                quads[i].A = 0;
                quads[i].B = -i;
                quads[i].C = i * i;
            }
            int* deque = stackalloc int[n + 2];
            int head = 0, tail = 0;
            deque[0] = 0;
            tail = 1;
            DequeOpt.Run(dp, n, quads, deque, &head, &tail);
            Assert.IsTrue(tail > head);
            for (int i = 0; i < n; i++)
                Assert.IsTrue(dp[i] <= quads[0].C);
        }

        [Test]
        public void LagrangianRelaxation_Search()
        {
            long* w = stackalloc long[] { 10, 5, 3 };
            long lo = LagrangianRelaxation.Search(w, 3, 1, 0, 20);
            Assert.IsTrue(lo >= 0 && lo <= 20);
        }
    }
}
