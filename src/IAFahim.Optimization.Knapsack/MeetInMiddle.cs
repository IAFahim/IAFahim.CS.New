namespace IAFahim.Optimization.Knapsack
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MeetInMiddle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AccumulateSubset(long* w, long* v, int start, int count, int mask, out long sw, out long sv)
        {
            long lw = 0, lv = 0;
            for (int j = 0; j < count; j++)
            {
                if ((mask & (1 << j)) != 0)
                {
                    lw += w[start + j];
                    lv += v[start + j];
                }
            }
            sw = lw;
            sv = lv;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BuildLeft(long* w, long* v, int half, long* left)
        {
            int leftCount = 1 << half;
            for (int i = 0; i < leftCount; i++)
            {
                long sw, sv;
                AccumulateSubset(w, v, 0, half, i, out sw, out sv);
                left[i * 2] = sw;
                left[i * 2 + 1] = sv;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long BestLeftValue(long* left, int leftCount, long rem)
        {
            long bestLeft = 0;
            for (int i = 0; i < leftCount; i++)
                if (left[i * 2] <= rem && left[i * 2 + 1] > bestLeft) bestLeft = left[i * 2 + 1];
            return bestLeft;
        }

        public static long Run(long* w, long* v, int n, long cap, long* left)
        {
            if (n > 40) return 0;
            int half = n >> 1;
            int leftCount = 1 << half;
            BuildLeft(w, v, half, left);
            long best = 0;
            int rightCount = 1 << (n - half);
            for (int mask = 0; mask < rightCount; mask++)
            {
                long sw, sv;
                AccumulateSubset(w, v, half, n - half, mask, out sw, out sv);
                long rem = cap - sw;
                if (rem < 0) continue;
                long cand = sv + BestLeftValue(left, leftCount, rem);
                if (cand > best) best = cand;
            }
            return best;
        }
    }
}