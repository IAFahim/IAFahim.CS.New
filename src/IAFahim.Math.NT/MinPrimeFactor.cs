namespace IAFahim.Math.NT
{
    using System.Runtime.CompilerServices;

    public static unsafe class MinPrimeFactor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long n)
        {
            if (n < 2) return 0;
            if ((n & 1) == 0) return 2;
            for (long p = 3; p * p <= n; p += 2)
                if (n % p == 0) return p;
            return n;
        }
    }
}
