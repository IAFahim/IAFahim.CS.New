namespace IAFahim.Search.Specialized.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using IAFahim.Search;

    public sealed unsafe class SchedulingTests
    {
        [Test]
        public void RoundRobinSchedule_EvenTeams_CorrectPairs()
        {
            const int N = 4;
            const int Matches = (N - 1) * (N / 2);
            int* schedule = (int*)Marshal.AllocHGlobal(Matches * 2 * sizeof(int));
            try
            {
                Scheduling.RoundRobinSchedule(N, schedule);

                bool[,] played = new bool[N, N];
                for (int i = 0; i < Matches; i++)
                {
                    int t1 = schedule[i * 2];
                    int t2 = schedule[i * 2 + 1];

                    Assert.AreNotEqual(t1, t2);
                    Assert.IsTrue(t1 >= 0 && t1 < N);
                    Assert.IsTrue(t2 >= 0 && t2 < N);

                    Assert.IsFalse(played[t1, t2]);
                    Assert.IsFalse(played[t2, t1]);

                    played[t1, t2] = true;
                    played[t2, t1] = true;
                }

                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        if (i != j)
                        {
                            Assert.IsTrue(played[i, j]);
                        }
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)schedule);
            }
        }

        [Test]
        public void RoundRobinSchedule_OddTeams_CorrectPairsWithByes()
        {
            const int N = 3;
            // Round-robin schedule size when N is odd is same as N+1 (which is 4)
            const int FakeN = N + 1;
            const int Matches = (FakeN - 1) * (FakeN / 2);
            int* schedule = (int*)Marshal.AllocHGlobal(Matches * 2 * sizeof(int));
            try
            {
                Scheduling.RoundRobinSchedule(N, schedule);

                bool[,] played = new bool[N, N];
                int[] byeCount = new int[N];

                for (int i = 0; i < Matches; i++)
                {
                    int t1 = schedule[i * 2];
                    int t2 = schedule[i * 2 + 1];

                    Assert.AreNotEqual(t1, t2);

                    if (t1 == -1)
                    {
                        Assert.IsTrue(t2 >= 0 && t2 < N);
                        byeCount[t2]++;
                    }
                    else if (t2 == -1)
                    {
                        Assert.IsTrue(t1 >= 0 && t1 < N);
                        byeCount[t1]++;
                    }
                    else
                    {
                        Assert.IsTrue(t1 >= 0 && t1 < N);
                        Assert.IsTrue(t2 >= 0 && t2 < N);
                        Assert.IsFalse(played[t1, t2]);
                        played[t1, t2] = true;
                        played[t2, t1] = true;
                    }
                }

                for (int i = 0; i < N; i++)
                {
                    Assert.AreEqual(1, byeCount[i]);
                    for (int j = 0; j < N; j++)
                    {
                        if (i != j)
                        {
                            Assert.IsTrue(played[i, j]);
                        }
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)schedule);
            }
        }

        [Test]
        public void LatinSquareGenerate_ValidSquare()
        {
            const int N = 4;
            int* grid = (int*)Marshal.AllocHGlobal(N * N * sizeof(int));
            try
            {
                Scheduling.LatinSquareGenerate(N, grid);

                for (int r = 0; r < N; r++)
                {
                    bool[] rowSeen = new bool[N + 1];
                    bool[] colSeen = new bool[N + 1];
                    for (int c = 0; c < N; c++)
                    {
                        int valRow = grid[r * N + c];
                        int valCol = grid[c * N + r];

                        Assert.IsTrue(valRow >= 1 && valRow <= N);
                        Assert.IsTrue(valCol >= 1 && valCol <= N);

                        Assert.IsFalse(rowSeen[valRow]);
                        Assert.IsFalse(colSeen[valCol]);

                        rowSeen[valRow] = true;
                        colSeen[valCol] = true;
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)grid);
            }
        }
    }
}
