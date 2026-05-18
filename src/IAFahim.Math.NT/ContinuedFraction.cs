namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ContinuedFraction
    {
        public static int Run(long a, long b, long* cf)
        {
            int count = 0;
            while (b != 0)
            {
                cf[count++] = a / b;
                long t = a % b;
                a = b;
                b = t;
            }
            return count;
        }
    }
}
