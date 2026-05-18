namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DivisorSum
    {
        public static long Run(long n)
        {
            if (n <= 0) return 0;
            long result = 1;
            for (long p = 2; p * p <= n; p++)
            {
                long sum = 1;
                long pk = 1;
                while (n % p == 0)
                {
                    pk *= p;
                    sum += pk;
                    n /= p;
                }
                result *= sum;
            }
            if (n > 1) result *= (1 + n);
            return result;
        }
    }
}
