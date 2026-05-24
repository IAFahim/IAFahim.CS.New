namespace IAFahim.Optimization.Submodular
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SubmodularGreedy
    {
        public static int Run(int n, long* gain, int k, int* selected)
        {
            bool* picked = stackalloc bool[n]; for (int i = 0; i < n; i++) picked[i] = false;
            long total = 0;
            for (int iter = 0; iter < k; iter++)
            {
                int best = FindBestItem(n, iter, gain, picked);
                if (best >= 0) { picked[best] = true; selected[iter] = best; total += gain[iter * n + best]; }
            }
            return (int)total;
        }

        private static int FindBestItem(int n, int iter, long* gain, bool* picked)
        {
            long bestG = 0; int bestIdx = -1;
            for (int i = 0; i < n; i++)
                if (!picked[i] && gain[iter * n + i] > bestG) { bestG = gain[iter * n + i]; bestIdx = i; }
            return bestIdx;
        }

        public static long GreedySetCover(int n, int* elemCounts, int** sets, int m, int* cover)
        {
            bool* covered = stackalloc bool[n]; for (int i = 0; i < n; i++) covered[i] = false;
            int count = 0;
            for (int it = 0; it < m; it++)
            {
                int best = FindBestSet(m, elemCounts, sets, covered, out int bestNew);
                if (best < 0 || bestNew == 0) break;
                cover[count++] = best;
                MarkCovered(elemCounts[best], sets[best], covered);
            }
            return count;
        }

        private static int FindBestSet(int m, int* counts, int** sets, bool* covered, out int bestNew)
        {
            int bestIdx = -1; bestNew = 0;
            for (int i = 0; i < m; i++)
            {
                int curNew = CountNewCoverage(counts[i], sets[i], covered);
                if (curNew > bestNew) { bestNew = curNew; bestIdx = i; }
            }
            return bestIdx;
        }

        private static int CountNewCoverage(int count, int* set, bool* covered)
        {
            int res = 0; for (int j = 0; j < count; j++) if (!covered[set[j]]) res++;
            return res;
        }

        private static void MarkCovered(int count, int* set, bool* covered)
        {
            for (int j = 0; j < count; j++) covered[set[j]] = true;
        }
    }
}
