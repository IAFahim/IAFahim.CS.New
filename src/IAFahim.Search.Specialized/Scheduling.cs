namespace IAFahim.Search
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Scheduling
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyBye(bool isOdd, int numTeams, ref int t)
        {
            if (isOdd && t == numTeams - 1) t = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RotateTeams(int* teams, int numTeams)
        {
            int last = teams[numTeams - 1];
            for (int i = numTeams - 1; i > 1; i--)
                teams[i] = teams[i - 1];
            teams[1] = last;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RoundRobinSchedule(int n, int* schedule)
        {
            if (n <= 1) return;

            bool isOdd = (n % 2) != 0;
            int numTeams = isOdd ? n + 1 : n;
            int rounds = numTeams - 1;
            int matchesPerRound = numTeams / 2;

            int* teams = stackalloc int[numTeams];
            for (int i = 0; i < numTeams; i++) teams[i] = i;

            int idx = 0;
            for (int round = 0; round < rounds; round++)
            {
                for (int i = 0; i < matchesPerRound; i++)
                {
                    int t1 = teams[i];
                    int t2 = teams[numTeams - 1 - i];
                    ApplyBye(isOdd, numTeams, ref t1);
                    ApplyBye(isOdd, numTeams, ref t2);
                    schedule[idx++] = t1;
                    schedule[idx++] = t2;
                }
                RotateTeams(teams, numTeams);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LatinSquareGenerate(int n, int* grid)
        {
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    grid[r * n + c] = (r + c) % n + 1;
                }
            }
        }
    }
}
