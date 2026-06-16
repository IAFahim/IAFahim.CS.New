namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PrimitiveRoot
    {
        private const int MaxFactors = 64;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Gcd(long a, long b)
        {
            if (a < 0) a = -a;
            if (b < 0) b = -b;
            while (b != 0)
            {
                long t = b;
                b = a % b;
                a = t;
            }
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long PowMod(long a, long e, long mod)
        {
            return IAFahim.Math.NT.ModPow.Run(a, e, mod);
        }

        public static long Run(long n)
        {
            if (n <= 1) return -1;
            if (n == 2) return 1;
            if (n == 4) return 3;

            long phi = Phi.Run(n);

            // Prime factors of phi can exceed int.MaxValue, so they must be stored as long.
            long* factors = stackalloc long[MaxFactors];
            int fc = 0;
            long tmp = phi;
            for (long p = 2; p * p <= tmp; p++)
            {
                if (tmp % p == 0)
                {
                    factors[fc++] = p;
                    while (tmp % p == 0) tmp /= p;
                }
            }
            if (tmp > 1) factors[fc++] = tmp;

            // Hoist the loop-invariant exponents phi/factors[i] out of the candidate loop.
            long* exps = stackalloc long[MaxFactors];
            for (int i = 0; i < fc; i++) exps[i] = phi / factors[i];

            // When n is prime, phi(n) == n-1 and gcd(g, n) == 1 for all 2 <= g < n,
            // so the per-candidate gcd check is redundant.
            bool nIsPrime = phi == n - 1;

            for (long g = 2; g < n; g++)
            {
                if (!nIsPrime && Gcd(g, n) != 1) continue;
                bool ok = true;
                for (int i = 0; i < fc; i++)
                {
                    if (PowMod(g, exps[i], n) == 1)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok) return g;
            }
            return -1;
        }
    }
}
