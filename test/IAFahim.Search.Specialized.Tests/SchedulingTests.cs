namespace IAFahim.Search.Specialized.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;
    using IAFahim.Search;

    public sealed unsafe class SchedulingTests
    {
        [Fact]
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

                    Assert.NotEqual(t1, t2);
                    Assert.True(t1 >= 0 && t1 < N);
                    Assert.True(t2 >= 0 && t2 < N);

                    Assert.False(played[t1, t2]);
                    Assert.False(played[t2, t1]);

                    played[t1, t2] = true;
                    played[t2, t1] = true;
                }

                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        if (i != j)
                        {
                            Assert.True(played[i, j]);
                        }
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)schedule);
            }
        }

        [Fact]
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

                    Assert.NotEqual(t1, t2);

                    if (t1 == -1)
                    {
                        Assert.True(t2 >= 0 && t2 < N);
                        byeCount[t2]++;
                    }
                    else if (t2 == -1)
                    {
                        Assert.True(t1 >= 0 && t1 < N);
                        byeCount[t1]++;
                    }
                    else
                    {
                        Assert.True(t1 >= 0 && t1 < N);
                        Assert.True(t2 >= 0 && t2 < N);
                        Assert.False(played[t1, t2]);
                        played[t1, t2] = true;
                        played[t2, t1] = true;
                    }
                }

                for (int i = 0; i < N; i++)
                {
                    Assert.Equal(1, byeCount[i]);
                    for (int j = 0; j < N; j++)
                    {
                        if (i != j)
                        {
                            Assert.True(played[i, j]);
                        }
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)schedule);
            }
        }

        [Fact]
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

                        Assert.True(valRow >= 1 && valRow <= N);
                        Assert.True(valCol >= 1 && valCol <= N);

                        Assert.False(rowSeen[valRow]);
                        Assert.False(colSeen[valCol]);

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
