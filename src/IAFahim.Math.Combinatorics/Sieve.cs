namespace IAFahim.Math.Combinatorics
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SievePrimes
    {
        public static int Run(int* primes, bool* isPrime, int n)
        {
            int count = 0;
            for (int i = 0; i <= n; i++)
                isPrime[i] = true;
            isPrime[0] = false;
            if (n >= 1) isPrime[1] = false;
            for (int p = 2; p * p <= n; p++)
            {
                if (isPrime[p])
                {
                    for (int i = p * p; i <= n; i += p)
                        isPrime[i] = false;
                }
            }
            for (int i = 2; i <= n; i++)
            {
                if (isPrime[i])
                    primes[count++] = i;
            }
            return count;
        }
    }

    public static unsafe class LinearSieve
    {
        public static int Run(int* primes, int* lp, int n)
        {
            int count = 0;
            for (int i = 2; i <= n; i++)
            {
                if (lp[i] == 0)
                {
                    lp[i] = i;
                    primes[count++] = i;
                }
                for (int j = 0; j < count && primes[j] <= lp[i] && (long)i * primes[j] <= n; j++)
                {
                    lp[i * primes[j]] = primes[j];
                }
            }
            return count;
        }
    }

    public static unsafe class SegmentedSieve
    {
        public static int Run(long low, long high, int* primes, int primeCount, int* result)
        {
            int count = 0;
            long size = high - low + 1;
            bool* isPrime = stackalloc bool[(int)size];
            for (long i = 0; i < size; i++)
                isPrime[i] = true;
            if (low == 0) { isPrime[0] = false; }
            if (low == 1) { isPrime[0] = false; }
            long limit = (long)Math.Sqrt(high) + 1;
            for (int i = 0; i < primeCount && primes[i] <= limit; i++)
            {
                long start = ((low + primes[i] - 1) / primes[i]) * primes[i];
                if (start == primes[i]) start = primes[i] * 2;
                for (long j = start; j <= high; j += primes[i])
                    isPrime[j - low] = false;
            }
            for (long i = low; i <= high; i++)
            {
                if (isPrime[i - low])
                    result[count++] = (int)i;
            }
            return count;
        }
    }

    public static unsafe class IsPrime
    {
        private static long ModMul(long a, long b, long mod)
        {
            long result = 0;
            a %= mod;
            while (b > 0)
            {
                if ((b & 1) == 1) result = (result + a) % mod;
                a = (a * 2) % mod;
                b >>= 1;
            }
            return result;
        }

        private static long ModPow(long b, long e, long mod)
        {
            long result = 1;
            b %= mod;
            while (e > 0)
            {
                if ((e & 1) == 1) result = ModMul(result, b, mod);
                b = ModMul(b, b, mod);
                e >>= 1;
            }
            return result;
        }

        public static bool Run(long n)
        {
            if (n < 2) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;
            long d = n - 1;
            int s = 0;
            while ((d & 1) == 0)
            {
                d >>= 1;
                s++;
            }
            long[] witnesses = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 };
            for (int i = 0; i < witnesses.Length; i++)
            {
                long a = witnesses[i];
                if (a >= n) continue;
                long x = ModPow(a, d, n);
                if (x == 1 || x == n - 1) continue;
                bool composite = true;
                for (int r = 0; r < s - 1; r++)
                {
                    x = ModMul(x, x, n);
                    if (x == n - 1) { composite = false; break; }
                }
                if (composite) return false;
            }
            return true;
        }
    }
}