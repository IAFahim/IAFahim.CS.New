namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Mobius
    {
        public static int Run(long n)
        {
            if (n == 1) return 1;
            int count = 0;
            for (long p = 2; p * p <= n; p++)
            {
                if (n % p == 0)
                {
                    n /= p;
                    count++;
                    if (n % p == 0) return 0;
                }
            }
            if (n > 1) count++;
            return (count & 1) == 0 ? 1 : -1;
        }
    }
}
