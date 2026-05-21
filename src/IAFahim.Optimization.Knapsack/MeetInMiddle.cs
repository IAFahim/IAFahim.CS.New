namespace IAFahim.Optimization.Knapsack
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MeetInMiddle
    {
        public static long Run(long* w, long* v, int n, long cap)
        {
            if (n <= 40)
            {
                int half = n >> 1;
                int leftCount = 1 << half;
                long* left = stackalloc long[leftCount * 2];
                for (int i = 0; i < leftCount; i++)
                {
                    long sw = 0, sv = 0;
                    for (int j = 0; j < half; j++)
                    {
                        if ((i & (1 << j)) != 0)
                        {
                            sw += w[j];
                            sv += v[j];
                        }
                    }
                    left[i * 2] = sw;
                    left[i * 2 + 1] = sv;
                }
                long best = 0;
                for (int mask = 0; mask < (1 << (n - half)); mask++)
                {
                    long sw = 0, sv = 0;
                    for (int j = 0; j < n - half; j++)
                    {
                        if ((mask & (1 << j)) != 0)
                        {
                            sw += w[half + j];
                            sv += v[half + j];
                        }
                    }
                    long rem = cap - sw;
                    if (rem < 0) continue;
                    long bestLeft = 0;
                    for (int i = 0; i < leftCount; i++)
                        if (left[i * 2] <= rem && left[i * 2 + 1] > bestLeft)
                            bestLeft = left[i * 2 + 1];
                    long cand = sv + bestLeft;
                    if (cand > best) best = cand;
                }
                return best;
            }
            return 0;
        }
    }
}
