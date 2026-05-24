namespace IAFahim.Optimization.Matroid
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MatroidGreedy
    {
        public static long Run(int n, int* set, int setSize, long* weight, delegate*<int*, int, int, bool> independent)
        {
            int* order = stackalloc int[n]; for (int i = 0; i < n; i++) order[i] = i;
            SortByWeight(order, n, weight);

            long total = 0;
            int* cur = stackalloc int[n]; int curSize = 0;
            for (int i = 0; i < n; i++)
            {
                int e = order[i]; cur[curSize] = e;
                if (independent(cur, curSize + 1, e)) { total += weight[e]; curSize++; }
            }
            return total;
        }

        private static void SortByWeight(int* order, int n, long* weight)
        {
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    if (weight[order[i]] < weight[order[j]]) { int t = order[i]; order[i] = order[j]; order[j] = t; }
        }
    }
}
