namespace IAFahim.Optimization.Games.Tests
{
    using NUnit.Framework;

    public sealed unsafe class GrundyTests
    {
        [Test]
        public void Mex_Empty_Zero()
        {
            Assert.AreEqual(0, Grundy.Mex(null, 0));
        }

        [Test]
        public void Mex_SkipsPresent()
        {
            int* v = stackalloc int[3];
            v[0] = 0; v[1] = 1; v[2] = 3;
            Assert.AreEqual(2, Grundy.Mex(v, 3));
        }

        [Test]
        public void SpragueGrundy_NimHeapMoves()
        {
            const int N = 5;
            int* moves = stackalloc int[N * 10];
            int* counts = stackalloc int[N];
            int* g = stackalloc int[N];
            int* scratch = stackalloc int[10];
            for (int i = 0; i < N; i++)
            {
                counts[i] = i;
                for (int j = 0; j < i; j++) moves[i * 10 + j] = j;
            }
            Grundy.SpragueGrundy(moves, counts, N, g, scratch);
            for (int i = 0; i < N; i++)
                Assert.AreEqual(i, g[i]);
        }
    }

    public sealed unsafe class RetrogradeTests
    {
        [Test]
        public void TerminalLose_PredecessorIsWin()
        {
            const int N = 2;
            bool* win = stackalloc bool[N];
            bool* lose = stackalloc bool[N];
            int* from = stackalloc int[1];
            int* to = stackalloc int[1];
            win[0] = false; lose[0] = false;
            win[1] = false; lose[1] = true;
            from[0] = 0; to[0] = 1;
            Retrograde.Solve(N, win, lose, from, to, 1);
            Assert.IsTrue(win[0]);
            Assert.IsFalse(lose[0]);
            Assert.IsTrue(lose[1]);
        }

        [Test]
        public void AllMovesToWin_IsLose()
        {
            const int N = 3;
            bool* win = stackalloc bool[N];
            bool* lose = stackalloc bool[N];
            int* from = stackalloc int[2];
            int* to = stackalloc int[2];
            for (int i = 0; i < N; i++) { win[i] = false; lose[i] = false; }
            lose[1] = false; win[1] = true;
            lose[2] = false; win[2] = true;
            from[0] = 0; to[0] = 1;
            from[1] = 0; to[1] = 2;
            Retrograde.Solve(N, win, lose, from, to, 2);
            Assert.IsTrue(lose[0]);
            Assert.IsFalse(win[0]);
        }
    }
}
