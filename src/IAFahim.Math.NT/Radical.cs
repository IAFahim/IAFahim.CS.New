namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Radical
    {
        public static long Run(long n)
        {
            long result = 1;
            for (long p = 2; p * p <= n; p++)
            {
                if (n % p == 0)
                {
                    result *= p;
                    while (n % p == 0) n /= p;
                }
            }
            if (n > 1) result *= n;
            return result;
        }
    }
}
