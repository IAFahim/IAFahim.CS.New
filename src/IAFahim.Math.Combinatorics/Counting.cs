namespace IAFahim.Math.Combinatorics
{
    using System;
    using System.Runtime.CompilerServices;

    internal static unsafe class MathHelper
    {
        public static long ModInverse(long a, long mod)
        {
            long x, y;
            long g = ExtGcd(a, mod, out x, out y);
            if (g != 1) return 0;
            return (x % mod + mod) % mod;
        }

        public static long ExtGcd(long a, long b, out long x, out long y)
        {
            if (b == 0) { x = 1; y = 0; return a; }
            long x1, y1;
            long g = ExtGcd(b, a % b, out x1, out y1);
            x = y1;
            y = x1 - (a / b) * y1;
            return g;
        }
    }

    public static unsafe class PermuteCount
    {
        public static long Run(int n, long mod)
        {
            long result = 1;
            for (int i = 2; i <= n; i++)
                result = (result * i) % mod;
            return result;
        }
    }

    public static unsafe class MultisetPermutations
    {
        public static long Run(int n, int* counts, int k, long mod)
        {
            long factN = 1;
            for (int i = 2; i <= n; i++)
                factN = (factN * i) % mod;
            long result = factN;
            for (int i = 0; i < k; i++)
            {
                long factC = 1;
                for (int j = 2; j <= counts[i]; j++)
                    factC = (factC * j) % mod;
                result = (result * MathHelper.ModInverse(factC, mod)) % mod;
            }
            return result;
        }
    }

    public static unsafe class Catalan
    {
        public static long Run(long n, long mod)
        {
            if (n <= 0) return 1;
            // C_n = (2n choose n) / (n + 1)
            // C_n = product_{k=2}^n (n+k)/k
            long result = 1;
            for (long k = 2; k <= n; k++)
            {
                result = (result * (n + k)) % mod;
                result = (result * MathHelper.ModInverse(k, mod)) % mod;
            }
            return result;
        }
    }

    public static unsafe class StirlingFirst
    {
        public static long Run(long n, long k, long mod)
        {
            if (k == 0 && n == 0) return 1;
            if (k == 0 || n < k) return 0;
            long* s = stackalloc long[(int)k + 1];
            for (int i = 0; i <= k; i++) s[i] = 0;
            s[0] = 1;
            for (long i = 1; i <= n; i++)
            {
                for (long j = k; j >= 1; j--)
                {
                    s[j] = ((s[j - 1] + ((i - 1) * s[j]) % mod) % mod);
                }
                s[0] = 0;
            }
            return s[k];
        }
    }

    public static unsafe class StirlingSecond
    {
        public static long Run(long n, long k, long mod)
        {
            if (k == 0 && n == 0) return 1;
            if (k == 0 || n < k) return 0;
            long* s = stackalloc long[(int)k + 1];
            for (int i = 0; i <= k; i++) s[i] = 0;
            s[0] = 1;
            for (long i = 1; i <= n; i++)
            {
                for (long j = k; j >= 1; j--)
                {
                    s[j] = ((s[j - 1] + (j * s[j]) % mod) % mod);
                }
                s[0] = 0;
            }
            return s[k];
        }
    }

    public static unsafe class BellNumbers
    {
        public static long Run(long n, long mod)
        {
            if (n == 0) return 1;
            int nn = (int)n;
            long* prev = stackalloc long[nn + 1];
            long* cur = stackalloc long[nn + 1];
            prev[0] = 1;
            for (int i = 1; i <= nn; i++)
            {
                cur[0] = prev[i - 1];
                for (int j = 1; j <= i; j++)
                    cur[j] = (cur[j - 1] + prev[j - 1]) % mod;
                long* tmp = prev; prev = cur; cur = tmp;
            }
            return prev[0];
        }
    }

    public static unsafe class PartitionNumbers
    {
        public static long Run(long n, long mod)
        {
            if (n == 0) return 1;
            long* dp = stackalloc long[(int)n + 1];
            for (int i = 0; i <= n; i++) dp[i] = 0;
            dp[0] = 1;
            for (long i = 1; i <= n; i++)
            {
                for (long j = i; j <= n; j++)
                {
                    dp[j] = (dp[j] + dp[j - i]) % mod;
                }
            }
            return dp[n];
        }
    }

    public static unsafe class Derangements
    {
        public static long Run(long n, long mod)
        {
            if (n == 0) return 1;
            if (n == 1) return 0;
            long a = 1, b = 0, c = 1;
            for (long i = 2; i <= n; i++)
            {
                c = ((i - 1) * (a + b)) % mod;
                a = b;
                b = c;
            }
            return c;
        }
    }

    public static unsafe class StarsBars
    {
        public static long Run(long n, long k, long mod)
        {
            return Binom.Run(n + k - 1, k, mod);
        }
    }
}