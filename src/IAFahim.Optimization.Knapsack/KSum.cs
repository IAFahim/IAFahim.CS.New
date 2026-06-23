namespace IAFahim.Optimization.Knapsack
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class KSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ProcessMatch(int* a, int n, ref int li, ref int ri, ref long count)
        {
            if (a[li] == a[ri])
            {
                long pairCount = (long)(ri - li + 1) * (ri - li) / 2;
                count += pairCount;
                return true;
            }
            int lv = a[li], rv = a[ri];
            long lc = 0, rc = 0;
            while (li < n && a[li] == lv) { li++; lc++; }
            while (ri >= 0 && a[ri] == rv) { ri--; rc++; }
            count += lc * rc;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(int* a, int n, int k, int target)
        {
            if (k == 2)
            {
                int l = 0, r = n - 1;
                long count = 0;
                while (l < r)
                {
                    long s = (long)a[l] + a[r];
                    if (s == target)
                    {
                        if (ProcessMatch(a, n, ref l, ref r, ref count)) break;
                    }
                    else if (s < target) l++;
                    else r--;
                }
                return (int)count;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FourSum(int* a, int n, int target)
        {
            int count = 0;
            for (int i = 0; i < n - 3; i++)
            {
                for (int j = i + 1; j < n - 2; j++)
                {
                    int l = j + 1, r = n - 1;
                    while (l < r)
                    {
                        long s = (long)a[i] + a[j] + a[l] + a[r];
                        if (s == target) { count++; l++; r--; }
                        else if (s < target) l++;
                        else r--;
                    }
                }
            }
            return count;
        }
    }
}
