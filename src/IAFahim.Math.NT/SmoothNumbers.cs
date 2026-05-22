namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SmoothNumbers
    {
        public static int Generate(int b, long limit, long* result)
        {
            if (limit <= 0 || b < 2)
            {
                if (limit >= 1)
                {
                    result[0] = 1;
                    return 1;
                }
                return 0;
            }

            int* primes = stackalloc int[10000];
            int primeCount = GetPrimes(b, primes);

            int count = 0;
            Gen(0, 1, primeCount, primes, limit, result, ref count);
            QuickSort(result, 0, count - 1);
            return count;
        }

        public static long Count(int b, long limit)
        {
            if (limit <= 0 || b < 2)
            {
                return limit >= 1 ? 1L : 0L;
            }

            int* primes = stackalloc int[10000];
            int primeCount = GetPrimes(b, primes);

            return CountSmooth(0, 1, primeCount, primes, limit);
        }

        private static int GetPrimes(int b, int* primes)
        {
            int count = 0;
            for (int i = 2; i <= b; i++)
            {
                bool isPrime = true;
                for (int j = 2; j * j <= i; j++)
                {
                    if (i % j == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    primes[count++] = i;
                }
            }
            return count;
        }

        private static void Gen(
            int primeIndex,
            long currentValue,
            int primeCount,
            int* primes,
            long limit,
            long* result,
            ref int count)
        {
            result[count++] = currentValue;
            for (int i = primeIndex; i < primeCount; i++)
            {
                int p = primes[i];
                if (limit / (long)p >= currentValue)
                {
                    Gen(i, currentValue * (long)p, primeCount, primes, limit, result, ref count);
                }
            }
        }

        private static long CountSmooth(
            int primeIndex,
            long currentValue,
            int primeCount,
            int* primes,
            long limit)
        {
            long ans = 1;
            for (int i = primeIndex; i < primeCount; i++)
            {
                int p = primes[i];
                if (limit / (long)p >= currentValue)
                {
                    ans += CountSmooth(i, currentValue * (long)p, primeCount, primes, limit);
                }
            }
            return ans;
        }

        private static void QuickSort(long* ptr, int left, int right)
        {
            if (left >= right)
            {
                return;
            }
            long pivot = ptr[left + (right - left) / 2];
            int i = left;
            int j = right;
            while (i <= j)
            {
                while (ptr[i] < pivot)
                {
                    i++;
                }
                while (ptr[j] > pivot)
                {
                    j--;
                }
                if (i <= j)
                {
                    long temp = ptr[i];
                    ptr[i] = ptr[j];
                    ptr[j] = temp;
                    i++;
                    j--;
                }
            }
            QuickSort(ptr, left, j);
            QuickSort(ptr, i, right);
        }
    }
}
