namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PrimitiveRoot
    {
        private const int MaxFactors = 64;

        private const long NoRoot = -1;

        private const long MultiplicativeIdentity = 1;

        private const long SmallestGenerator = 2;

        private const int CyclicGroupTwo = 2;

        private const int CyclicGroupFour = 4;

        private const long RootOfCyclicFour = 3;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FactorizeDistinct(long value, long* factors)
        {
            int fc = 0;
            long tmp = value;
            for (long p = SmallestGenerator; p * p <= tmp; p++)
            {
                if (tmp % p == 0)
                {
                    factors[fc++] = p;
                    while (tmp % p == 0) tmp /= p;
                }
            }
            if (tmp > MultiplicativeIdentity) factors[fc++] = tmp;
            return fc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsGenerator(long g, long n, long* exps, int fc)
        {
            for (int i = 0; i < fc; i++)
                if (PowMod(g, exps[i], n) == MultiplicativeIdentity) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long FindSmallestGenerator(long n, long phi, long* exps, int fc)
        {
            bool nIsPrime = phi == n - MultiplicativeIdentity;
            for (long g = SmallestGenerator; g < n; g++)
            {
                if (!nIsPrime && Gcd(g, n) != MultiplicativeIdentity) continue;
                if (IsGenerator(g, n, exps, fc)) return g;
            }
            return NoRoot;
        }

        public static long Run(long n)
        {
            if (n < CyclicGroupTwo) return NoRoot;
            if (n == CyclicGroupTwo) return MultiplicativeIdentity;
            if (n == CyclicGroupFour) return RootOfCyclicFour;
            long phi = Phi.Run(n);
            long* factors = stackalloc long[MaxFactors];
            int fc = FactorizeDistinct(phi, factors);
            long* exps = stackalloc long[MaxFactors];
            for (int i = 0; i < fc; i++) exps[i] = phi / factors[i];
            return FindSmallestGenerator(n, phi, exps, fc);
        }
    }
}
