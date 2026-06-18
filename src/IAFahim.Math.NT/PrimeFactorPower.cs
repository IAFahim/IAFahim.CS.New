namespace IAFahim.Math.NT
{
    using System.Runtime.CompilerServices;

    public static unsafe class PrimeFactorPower
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int* minPrime, int* outPrimes, int* outExponents)
        {
            int count = 0;
            while (n > 1)
            {
                int p = minPrime[n];
                int e = 0;
                while (n % p == 0) { n /= p; e++; }
                outPrimes[count] = p;
                outExponents[count] = e;
                count++;
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(long n, long* outPrimes, int* outExponents)
        {
            int count = 0;
            for (long p = 2; p * p <= n; p++)
            {
                if (n % p != 0) continue;
                int e = 0;
                while (n % p == 0) { n /= p; e++; }
                outPrimes[count] = p;
                outExponents[count] = e;
                count++;
            }
            if (n > 1)
            {
                outPrimes[count] = n;
                outExponents[count] = 1;
                count++;
            }
            return count;
        }
    }

    public static unsafe class PrimeFactorPowerSum
    {
        public static long Run(long n, delegate* managed<long, int, long> f)
        {
            long sum = 0;
            for (long p = 2; p * p <= n; p++)
            {
                if (n % p != 0) continue;
                int e = 0;
                while (n % p == 0) { n /= p; e++; }
                sum += f(p, e);
            }
            if (n > 1) sum += f(n, 1);
            return sum;
        }
    }
}
