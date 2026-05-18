namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Factorize
    {
        public static int Run(long n, long* factors)
        {
            if (n <= 1) return 0;
            int count = 0;
            Factor(n, factors, ref count);
            Sort(factors, count);
            return count;
        }

        private static void Factor(long n, long* factors, ref int count)
        {
            if (n <= 1) return;
            if (MillerRabin.Run(n))
            {
                factors[count++] = n;
                return;
            }
            long d = PollardRho.Run(n);
            Factor(d, factors, ref count);
            Factor(n / d, factors, ref count);
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
