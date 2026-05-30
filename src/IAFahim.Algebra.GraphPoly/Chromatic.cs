namespace IAFahim.Algebra.GraphPoly
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Chromatic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsIndependentSet(int n, bool* adj, int mask)
        {
            for (int i = 0; i < n; i++)
            {
                if ((mask & (1 << i)) == 0) continue;
                for (int j = i + 1; j < n; j++)
                {
                    if ((mask & (1 << j)) == 0) continue;
                    long index = (long)i * (long)n + (long)j;
                    if (adj[index]) return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PopCount(int mask)
        {
            int bits = 0;
            while (mask > 0)
            {
                if ((mask & 1) != 0) bits++;
                mask >>= 1;
            }
            return bits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModPow(long b, long e, long mod)
        {
            long r = 1L;
            b %= mod;
            if (b < 0L) b += mod;
            while (e > 0L)
            {
                if ((e & 1L) != 0L) r = (r * b) % mod;
                b = (b * b) % mod;
                e /= 2L;
            }
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Subset(int n, bool* adj, int MOD, long* coeffs)
        {
            int size = 1 << n;
            long* F = stackalloc long[size];
            for (int mask = 0; mask < size; mask++)
            {
                F[mask] = IsIndependentSet(n, adj, mask) ? 1L : 0L;
            }

            for (int i = 0; i < n; i++)
            {
                int bit = 1 << i;
                for (int mask = 0; mask < size; mask++)
                {
                    if ((mask & bit) != 0)
                    {
                        F[mask] = (F[mask] + F[mask ^ bit]) % MOD;
                    }
                }
            }

            long* y = stackalloc long[n + 1];
            for (int k = 0; k <= n; k++)
            {
                long sum = 0L;
                for (int mask = 0; mask < size; mask++)
                {
                    long term = ModPow(F[mask], (long)k, MOD);
                    int pop = PopCount(mask);
                    if ((n - pop) % 2 != 0)
                    {
                        sum = (sum - term + MOD) % MOD;
                    }
                    else
                    {
                        sum = (sum + term) % MOD;
                    }
                }
                y[k] = sum;
            }

            Interpolate(n, y, MOD, coeffs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Interpolate(int n, long* y, int MOD, long* coeffs)
        {
            for (int i = 0; i <= n; i++) coeffs[i] = 0L;

            long* poly = stackalloc long[n + 2];
            poly[0] = 1L;
            for (int i = 1; i <= n + 1; i++) poly[i] = 0L;
            int polyLen = 1;

            for (int i = 0; i <= n; i++)
            {
                long* nextPoly = stackalloc long[polyLen + 1];
                for (int j = 0; j <= polyLen; j++) nextPoly[j] = 0L;

                for (int j = 0; j < polyLen; j++)
                {
                    nextPoly[j + 1] = (nextPoly[j + 1] + poly[j]) % MOD;
                    long sub = (poly[j] * i) % MOD;
                    nextPoly[j] = (nextPoly[j] - sub + MOD) % MOD;
                }
                polyLen++;
                for (int j = 0; j < polyLen; j++) poly[j] = nextPoly[j];
            }

            long* temp = stackalloc long[polyLen];
            long* q = stackalloc long[polyLen];
            long* r = stackalloc long[polyLen];

            for (int i = 0; i <= n; i++)
            {
                long den = 1L;
                for (int j = 0; j <= n; j++)
                {
                    if (i != j)
                    {
                        long diff = (i - j + MOD) % MOD;
                        den = (den * diff) % MOD;
                    }
                }

                long invDen = ModPow(den, MOD - 2, MOD);
                long factor = (y[i] * invDen) % MOD;

                long root = (long)i % MOD;
                long lastQ = 0L;
                for (int j = polyLen - 1; j > 0; j--)
                {
                    q[j - 1] = (poly[j] + lastQ * root) % MOD;
                    lastQ = q[j - 1];
                }

                for (int j = 0; j < polyLen - 1; j++)
                {
                    long add = (q[j] * factor) % MOD;
                    coeffs[j] = (coeffs[j] + add) % MOD;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NumberDp(int n, bool* adj, int MOD)
        {
            long* coeffs = stackalloc long[n + 1];
            Subset(n, adj, MOD, coeffs);

            for (int k = 1; k <= n; k++)
            {
                long val = 0L;
                long xPow = 1L;
                for (int i = 0; i <= n; i++)
                {
                    val = (val + coeffs[i] * xPow) % MOD;
                    xPow = (xPow * (long)k) % MOD;
                }
                if (val > 0L) return k;
            }
            return n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DeletionContraction(int n, bool* adj, int edges, int* from, int* to, int MOD, long* coeffs)
        {
            Subset(n, adj, MOD, coeffs);
        }
    }
}
