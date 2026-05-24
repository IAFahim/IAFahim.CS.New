namespace IAFahim.Optimization.Exact
{
    using System;

    public static unsafe class MinSetCover
    {
        public static int Run(int m, int** sets, int* setSizes, int* covered, int* best, int* cur)
        {
            int total = CalculateTotalElements(m, setSizes);
            *best = m + 1;
            cur[0] = 0;
            Search(m, sets, setSizes, covered, 0, 0, best, cur, 0, total);
            return *best;
        }

        private static int CalculateTotalElements(int m, int* setSizes)
        {
            int total = 0; for (int i = 0; i < m; i++) total += setSizes[i]; return total;
        }

        private static void Search(int m, int** sets, int* setSizes, int* covered, int covCount, int used, int* best, int* cur, int idx, int remain)
        {
            if (used >= *best) return;
            if (covCount == remain) { *best = used; return; }
            if (idx == m) return;

            for (int i = idx; i < m; i++)
            {
                int added = CoverSet(i, sets, setSizes, covered);
                if (added > 0)
                {
                    cur[used] = i;
                    Search(m, sets, setSizes, covered, covCount + added, used + 1, best, cur, i + 1, remain);
                }
                UncoverSet(i, sets, setSizes, covered);
            }
        }

        private static int CoverSet(int i, int** sets, int* setSizes, int* covered)
        {
            int added = 0;
            for (int j = 0; j < setSizes[i]; j++)
            {
                int elem = sets[i][j];
                if (covered[elem] == 0) added++;
                covered[elem]++;
            }
            return added;
        }

        private static void UncoverSet(int i, int** sets, int* setSizes, int* covered)
        {
            for (int j = 0; j < setSizes[i]; j++) covered[sets[i][j]]--;
        }
    }
}