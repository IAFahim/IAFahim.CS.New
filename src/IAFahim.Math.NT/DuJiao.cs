namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DuJiao
    {
        public static long Phi(long n, long* preSumLarge, long* memo, bool* memoized)
        {
            if (n <= 0) return 0;
            long b = CalculateBlockSize(n);
            if (n <= b) { SievePhi((int)n, preSumLarge); return preSumLarge[n]; }

            long memoSize = n / b + 2;
            InitializeMemo(memoSize, memoized);

            SievePhi((int)b, preSumLarge);
            return GetPhi(n, n, b, preSumLarge, memo, memoized);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long CalculateBlockSize(long n)
        {
            long b = (long)Math.Pow(n, 2.0 / 3.0);
            return Math.Max(10000, Math.Min(b, n));
        }

        private static void InitializeMemo(long size, bool* memoized)
        {
            for (int i = 0; i < size; i++) memoized[i] = false;
        }

        public static long Mobius(long n, long* preSumLarge, long* memo, bool* memoized)
        {
            if (n <= 0) return 0;
            long b = CalculateBlockSize(n);
            if (n <= b) { SieveMobius((int)n, preSumLarge); return preSumLarge[n]; }

            long memoSize = n / b + 2;
            InitializeMemo(memoSize, memoized);

            SieveMobius((int)b, preSumLarge);
            return GetMobius(n, n, b, preSumLarge, memo, memoized);
        }

        private static void SievePhi(int limit, long* preSum)
        {
            int* phi = stackalloc int[limit + 1], primes = stackalloc int[limit + 1];
            bool* isPrime = stackalloc bool[limit + 1];
            InitializeSieve(limit, phi, isPrime);

            int pCount = 0; phi[1] = 1;
            for (int i = 2; i <= limit; i++)
            {
                if (isPrime[i]) { primes[pCount++] = i; phi[i] = i - 1; }
                UpdatePhiSieve(i, limit, primes, pCount, phi, isPrime);
            }
            AccumulatePreSum(limit, phi, preSum);
        }

        private static void InitializeSieve(int limit, int* phi, bool* isPrime)
        {
            for (int i = 0; i <= limit; i++) { phi[i] = i; isPrime[i] = true; }
        }

        private static void UpdatePhiSieve(int i, int limit, int* primes, int pCount, int* phi, bool* isPrime)
        {
            for (int j = 0; j < pCount; j++)
            {
                int p = primes[j];
                long ipL = (long)i * p;
                if (ipL > limit) break;
                int ip = (int)ipL;
                isPrime[ip] = false;
                if (i % p == 0) { phi[ip] = phi[i] * p; break; }
                phi[ip] = phi[i] * (p - 1);
            }
        }

        private static void AccumulatePreSum(int limit, int* vals, long* preSum)
        {
            preSum[0] = 0; for (int i = 1; i <= limit; i++) preSum[i] = preSum[i - 1] + vals[i];
        }

        private static void SieveMobius(int limit, long* preSum)
        {
            int* mu = stackalloc int[limit + 1], primes = stackalloc int[limit + 1];
            bool* isPrime = stackalloc bool[limit + 1];
            for (int i = 0; i <= limit; i++) { mu[i] = 1; isPrime[i] = true; }
            int pCount = 0; mu[1] = 1;
            for (int i = 2; i <= limit; i++)
            {
                if (isPrime[i]) { primes[pCount++] = i; mu[i] = -1; }
                UpdateMobiusSieve(i, limit, primes, pCount, mu, isPrime);
            }
            AccumulatePreSumMobius(limit, mu, preSum);
        }

        private static void UpdateMobiusSieve(int i, int limit, int* primes, int pCount, int* mu, bool* isPrime)
        {
            for (int j = 0; j < pCount; j++)
            {
                int p = primes[j];
                long ipL = (long)i * p;
                if (ipL > limit) break;
                int ip = (int)ipL;
                isPrime[ip] = false;
                if (i % p == 0) { mu[ip] = 0; break; }
                mu[ip] = -mu[i];
            }
        }

        private static void AccumulatePreSumMobius(int limit, int* mu, long* preSum)
        {
            preSum[0] = 0; for (int i = 1; i <= limit; i++) preSum[i] = preSum[i - 1] + mu[i];
        }

        private static long GetPhi(long x, long n, long b, long* preSum, long* memo, bool* memoized)
        {
            if (x <= b) return preSum[x];
            long idx = n / x; if (memoized[idx]) return memo[idx];

            long ans = CalculateArithmeticSum(x);
            for (long l = 2, r; l <= x; l = r + 1)
            {
                long val = x / l; r = x / val;
                ans -= (r - l + 1) * GetPhi(val, n, b, preSum, memo, memoized);
            }
            memo[idx] = ans; memoized[idx] = true; return ans;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long CalculateArithmeticSum(long x) => (x & 1) == 0 ? (x >> 1) * (x + 1) : x * ((x + 1) >> 1);

        private static long GetMobius(long x, long n, long b, long* preSum, long* memo, bool* memoized)
        {
            if (x <= b) return preSum[x];
            long idx = n / x; if (memoized[idx]) return memo[idx];

            long ans = 1;
            for (long l = 2, r; l <= x; l = r + 1)
            {
                long val = x / l; r = x / val;
                ans -= (r - l + 1) * GetMobius(val, n, b, preSum, memo, memoized);
            }
            memo[idx] = ans; memoized[idx] = true; return ans;
        }
    }
}
