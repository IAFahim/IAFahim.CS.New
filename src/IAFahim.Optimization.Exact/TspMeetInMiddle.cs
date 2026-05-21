namespace IAFahim.Optimization.Exact
{
    using System;

    public static unsafe class TspMeetInMiddle
    {
        public static long Run(int n, long* w, long inf)
        {
            if (n <= 2) return 0;
            if (n <= 20)
            {
                int half = n / 2;
                int maxMask = 1 << half;
                long* left = stackalloc long[maxMask * half];
                for (int i = 0; i < maxMask * half; i++) left[i] = inf;
                left[0] = 0;
                for (int mask = 1; mask < maxMask; mask++)
                {
                    int bits = 0;
                    for (int b = 0; b < half; b++)
                        if ((mask & (1 << b)) != 0) bits++;
                    int i = bits - 1;
                    int sub = mask ^ (1 << i);
                    for (int j = 0; j < half; j++)
                    {
                        if ((sub & (1 << j)) == 0) continue;
                        long wji = w[j * n + i];
                        if (wji != inf && left[sub * half + j] != inf)
                        {
                            long cand = left[sub * half + j] + wji;
                            if (cand < left[mask * half + i]) left[mask * half + i] = cand;
                        }
                    }
                }
                return inf;
            }
            return inf;
        }
    }
}
