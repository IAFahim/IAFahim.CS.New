namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MillerRabin
    {
        private static readonly long[] SmallPrimes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 };

        public static bool Run(long n)
        {
            if (n < 2) return false;
            if (n < 4) return true;
            if (n % 2 == 0 || n % 3 == 0) return false;

            Decompose(n - 1, out long d, out int r);
            for (int i = 0; i < SmallPrimes.Length; i++)
            {
                if (SmallPrimes[i] >= n) continue;
                if (IsComposite(n, SmallPrimes[i], d, r)) return false;
            }
            return true;
        }

        private static void Decompose(long n_minus_1, out long d, out int r)
        {
            d = n_minus_1; r = 0;
            while ((d & 1) == 0) { d >>= 1; r++; }
        }

        private static bool IsComposite(long n, long a, long d, int r)
        {
            long x = ModPow.Run(a, d, n);
            if (x == 1 || x == n - 1) return false;
            for (int j = 0; j < r - 1; j++)
            {
                x = ModMul.Run(x, x, n);
                if (x == n - 1) return false;
            }
            return true;
        }
    }

    public static unsafe class ModMul
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long b, long mod)
        {
            if (mod <= 1) return 0;
            long result = 0;
            a %= mod;
            if (a < 0) a += mod;
            b %= mod;
            if (b < 0) b += mod;

            while (b > 0)
            {
                if ((b & 1) == 1)
                {
                    result = (mod - result <= a) ? result - (mod - a) : result + a;
                }
                a = (mod - a <= a) ? a - (mod - a) : a + a;
                b >>= 1;
            }
            return result;
        }
    }

    public static unsafe class ModPow
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long b, long e, long mod)
        {
            long result = 1;
            b %= mod;
            if (b < 0) b += mod;
            while (e > 0)
            {
                if ((e & 1) == 1)
                    result = ModMul.Run(result, b, mod);
                e >>= 1;
                b = ModMul.Run(b, b, mod);
            }
            return result;
        }
    }
}
