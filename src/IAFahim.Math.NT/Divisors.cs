namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Divisors
    {
        public static int Run(long n, long* divs)
        {
            if (n <= 0) return 0;
            int count = 0;
            for (long i = 1; i * i <= n; i++)
            {
                if (n % i == 0)
                {
                    divs[count++] = i;
                    if (i != n / i)
                        divs[count++] = n / i;
                }
            }
            Sort(divs, count);
            return count;
        }

        private static void Sort(long* ptr, int len)
        {
            for (int i = 1; i < len; i++)
            {
                long key = ptr[i];
                int j = i - 1;
                while (j >= 0 && ptr[j] > key)
                {
                    ptr[j + 1] = ptr[j];
                    j--;
                }
                ptr[j + 1] = key;
            }
        }
    }
}
