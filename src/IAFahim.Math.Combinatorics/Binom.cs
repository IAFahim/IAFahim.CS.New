namespace IAFahim.Math.Combinatorics
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Binom
    {
        public static long Run(long n, long k, long mod)
        {
            if (k < 0 || k > n) return 0;
            if (n == 0 || k == 0 || k == n) return 1;
            long result = 1;
            for (long i = 0; i < k; i++)
            {
                result = (result * (n - i)) % mod;
                result = (result * ModInverse(i + 1, mod)) % mod;
            }
            return result;
        }

        private static long ModInverse(long a, long mod)
        {
            long x, y;
            long g = ExtGcd(a, mod, out x, out y);
            if (g != 1) return -1;
            return (x % mod + mod) % mod;
        }

        private static long ExtGcd(long a, long b, out long x, out long y)
        {
            if (b == 0) { x = 1; y = 0; return a; }
            long x1, y1;
            long g = ExtGcd(b, a % b, out x1, out y1);
            x = y1;
            y = x1 - (a / b) * y1;
            return g;
        }
    }

    public static unsafe class BinomLucas
    {
        public static long Run(long n, long k, long p)
        {
            long result = 1;
            while (n > 0 || k > 0)
            {
                int ni = (int)(n % p);
                int ki = (int)(k % p);
                if (ki > ni) return 0;
                result = (result * Binom.Run(ni, ki, p)) % p;
                n /= p;
                k /= p;
            }
            return result;
        }
    }

    public static unsafe class BinomLarge
    {
        public static long Run(long n, long k, long mod)
        {
            if (k < 0 || k > n) return 0;
            if (n == 0 || k == 0 || k == n) return 1;
            long result = 1;
            for (long i = 0; i < k; i++)
            {
                result = (result * ((n - i) % mod)) % mod;
                result = (result * ModInverse(i + 1, mod)) % mod;
            }
            return result;
        }

        private static long ModInverse(long a, long mod)
        {
            long x, y;
            long g = ExtGcd(a, mod, out x, out y);
            if (g != 1) return -1;
            return (x % mod + mod) % mod;
        }

        private static long ExtGcd(long a, long b, out long x, out long y)
        {
            if (b == 0) { x = 1; y = 0; return a; }
            long x1, y1;
            long g = ExtGcd(b, a % b, out x1, out y1);
            x = y1;
            y = x1 - (a / b) * y1;
            return g;
        }
    }
}