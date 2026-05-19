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

            long d = n - 1;
            int r = 0;
            while ((d & 1) == 0)
            {
                d >>= 1;
                r++;
            }

            for (int i = 0; i < SmallPrimes.Length; i++)
            {
                long a = SmallPrimes[i];
                if (a >= n) continue;
                long x = ModPow.Run(a, d, n);
                if (x == 1 || x == n - 1) continue;
                bool composite = true;
                for (int j = 0; j < r - 1; j++)
                {
                    x = ModMul.Run(x, x, n);
                    if (x == n - 1)
                    {
                        composite = false;
                        break;
                    }
                }
                if (composite) return false;
            }
            return true;
        }
    }

    public static unsafe class ModMul
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long a, long b, long mod)
        {
            long result = 0;
            a %= mod;
            if (a < 0) a += mod;
            if (a <= long.MaxValue / 2 && mod <= long.MaxValue / 2)
            {
                while (b > 0)
                {
                    if ((b & 1) == 1)
                        result = (result + a) % mod;
                    a = (a + a) % mod;
                    b >>= 1;
                }
            }
            else
            {
                while (b > 0)
                {
                    if ((b & 1) == 1)
                        result = ((result % mod) + (a % mod)) % mod;
                    a = ((a % mod) + (a % mod)) % mod;
                    b >>= 1;
                }
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
