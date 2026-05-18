namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DivisorCount
    {
        public static long Run(long n)
        {
            if (n <= 0) return 0;
            long result = 1;
            for (long p = 2; p * p <= n; p++)
            {
                int exp = 0;
                while (n % p == 0)
                {
                    exp++;
                    n /= p;
                }
                result *= (exp + 1);
            }
            if (n > 1) result *= 2;
            return result;
        }
    }
}
