namespace IAFahim.Optimization.Exact
{
    using System.Runtime.CompilerServices;

    public static unsafe class MinSetCover
    {
        public static int Run(int m, int** sets, int* setSizes, int* covered, int* best, int* cur)
        {
            int universe = CalculateDistinctElements(m, sets, setSizes, covered);
            *best = m + 1;
            cur[0] = 0;
            Search(m, sets, setSizes, covered, 0, 0, best, cur, 0, universe);
            return *best;
        }

        private static int CalculateDistinctElements(int m, int** sets, int* setSizes, int* covered)
        {
            int distinct = 0;
            for (int i = 0; i < m; i++)
            {
                int sz = setSizes[i];
                int* s = sets[i];
                for (int j = 0; j < sz; j++)
                {
                    int elem = s[j];
                    if (covered[elem] == 0) { covered[elem] = 1; distinct++; }
                }
            }
            for (int i = 0; i < m; i++)
            {
                int sz = setSizes[i];
                int* s = sets[i];
                for (int j = 0; j < sz; j++) covered[s[j]] = 0;
            }
            return distinct;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CoverSet(int i, int** sets, int* setSizes, int* covered)
        {
            int added = 0;
            int sz = setSizes[i];
            int* s = sets[i];
            for (int j = 0; j < sz; j++)
            {
                int elem = s[j];
                if (covered[elem] == 0) added++;
                covered[elem]++;
            }
            return added;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UncoverSet(int i, int** sets, int* setSizes, int* covered)
        {
            int sz = setSizes[i];
            int* s = sets[i];
            for (int j = 0; j < sz; j++) covered[s[j]]--;
        }
    }
}