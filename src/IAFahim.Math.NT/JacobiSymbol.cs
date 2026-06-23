namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class JacobiSymbol
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void StripPowersOfTwo(ref long a, long n, ref int result)
        {
            while ((a & 1) == 0)
            {
                a >>= 1;
                long r = n % 8;
                if (r == 3 || r == 5) result = -result;
            }
        }

        public static int Run(long a, long n)
        {
            if (n <= 0 || (n & 1) == 0) return 0;
            a = ((a % n) + n) % n;
            int result = 1;
            while (a != 0)
            {
                StripPowersOfTwo(ref a, n, ref result);
                long tmp = a;
                a = n;
                n = tmp;
                if (a % 4 == 3 && n % 4 == 3) result = -result;
                a = a % n;
            }
            return n == 1 ? result : 0;
        }
    }
}
