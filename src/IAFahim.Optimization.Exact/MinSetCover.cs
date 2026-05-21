namespace IAFahim.Optimization.Exact
{
    using System;

    public static unsafe class MinSetCover
    {
        public static int Run(int m, int** sets, int* setSizes, int* covered, int* best, int* cur)
        {
            int total = 0;
            for (int i = 0; i < m; i++) total += setSizes[i];
            *best = m + 1;
            cur[0] = 0;
            Search(m, sets, setSizes, covered, 0, 0, best, cur, 0, total);
            return *best;
        }

        private static void Search(int m, int** sets, int* setSizes, int* covered, int covCount, int used, int* best, int* cur, int idx, int remain)
        {
            if (used >= *best) return;
            if (covCount == remain) { if (used < *best) *best = used; return; }
            if (idx == m) return;
            for (int i = idx; i < m; i++)
            {
                int added = 0;
                for (int j = 0; j < setSizes[i]; j++)
                {
                    int elem = sets[i][j];
                    if (covered[elem] == 0) { covered[elem] = 1; added++; }
                }
                if (added > 0)
                {
                    cur[used] = i;
                    Search(m, sets, setSizes, covered, covCount + added, used + 1, best, cur, i + 1, remain);
                }
                for (int j = 0; j < setSizes[i]; j++)
                {
                    int elem = sets[i][j];
                    bool hasOther = false;
                    for (int k = 0; k < m; k++)
                    {
                        if (k == i) continue;
                        for (int t = 0; t < setSizes[k]; t++)
                        {
                            if (sets[k][t] == elem) { hasOther = true; break; }
                        }
                        if (hasOther) break;
                    }
                    if (!hasOther) covered[elem] = 0;
                }
            }
        }
    }
}
