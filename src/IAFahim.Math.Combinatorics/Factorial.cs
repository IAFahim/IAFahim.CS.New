namespace IAFahim.Math.Combinatorics
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Factorial
    {
        public static void Run(long* fact, long* invFact, int n, long mod)
        {
            fact[0] = 1;
            for (int i = 1; i <= n; i++)
                fact[i] = (fact[i - 1] * i) % mod;
            invFact[n] = ModPow(fact[n], mod - 2, mod);
            for (int i = n; i > 0; i--)
                invFact[i - 1] = (invFact[i] * i) % mod;
        }

        public static long Run(long n, long mod)
        {
            long result = 1;
            for (long i = 2; i <= n; i++)
                result = (result * i) % mod;
            return result;
        }

        private static long ModPow(long b, long e, long mod)
        {
            long result = 1;
            b %= mod;
            while (e > 0)
            {
                if ((e & 1) == 1) result = (result * b) % mod;
                b = (b * b) % mod;
                e >>= 1;
            }
            return result;
        }
    }
}