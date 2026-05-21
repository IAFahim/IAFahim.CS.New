namespace IAFahim.Optimization.Knapsack
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class KSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(int* a, int n, int k, int target)
        {
            if (k == 2)
            {
                int l = 0, r = n - 1, count = 0;
                while (l < r)
                {
                    int s = a[l] + a[r];
                    if (s == target) { count++; l++; r--; }
                    else if (s < target) l++;
                    else r--;
                }
                return count;
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
                        int s = a[i] + a[j] + a[l] + a[r];
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
