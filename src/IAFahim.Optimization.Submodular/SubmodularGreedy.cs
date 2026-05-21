namespace IAFahim.Optimization.Submodular
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SubmodularGreedy
    {
        public static int Run(int n, long* gain, int k, int* selected)
        {
            bool* picked = stackalloc bool[n];
            for (int i = 0; i < n; i++) picked[i] = false;
            long total = 0;
            for (int iter = 0; iter < k; iter++)
            {
                long bestGain = 0;
                int bestIdx = -1;
                for (int i = 0; i < n; i++)
                {
                    if (picked[i]) continue;
                    long curGain = gain[iter * n + i];
                    if (curGain > bestGain)
                    {
                        bestGain = curGain;
                        bestIdx = i;
                    }
                }
                if (bestIdx >= 0)
                {
                    picked[bestIdx] = true;
                    selected[iter] = bestIdx;
                    total += bestGain;
                }
            }
            return (int)total;
        }

        public static long GreedySetCover(int n, int* elemCounts, int** sets, int m, int* cover)
        {
            bool* covered = stackalloc bool[n];
            for (int i = 0; i < n; i++) covered[i] = false;
            int count = 0;
            long cost = 0;
            for (int iters = 0; iters < m; iters++)
            {
                int bestIdx = -1;
                int bestNew = 0;
                for (int i = 0; i < m; i++)
                {
                    int newCover = 0;
                    for (int j = 0; j < elemCounts[i]; j++)
                        if (!covered[sets[i][j]]) newCover++;
                    if (newCover > bestNew)
                    {
                        bestNew = newCover;
                        bestIdx = i;
                    }
                }
                if (bestIdx < 0 || bestNew == 0) break;
                cover[count++] = bestIdx;
                for (int j = 0; j < elemCounts[bestIdx]; j++)
                    covered[sets[bestIdx][j]] = true;
            }
            return cost;
        }
    }
}
