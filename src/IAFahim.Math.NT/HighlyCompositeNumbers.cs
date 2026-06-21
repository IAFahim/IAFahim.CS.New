namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct HighlyCompositeCandidate
    {
        public long Value;
        public int Divisors;
    }

    public static unsafe class HighlyCompositeNumbers
    {
        private static readonly int[] Primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };

        public static int Run(long limit, long* result, HighlyCompositeCandidate* scratch)
        {
            int count = 0;
            Dfs(0, 1, 1, 60, limit, scratch, ref count);
            QuickSort(scratch, 0, count - 1);

            int outCount = 0;
            int maxDiv = 0;
            for (int i = 0; i < count; i++)
            {
                if (scratch[i].Divisors > maxDiv)
                {
                    result[outCount++] = scratch[i].Value;
                    maxDiv = scratch[i].Divisors;
                }
            }
            return outCount;
        }

        private static void Dfs(int pIdx, long cur, int div, int lastE, long limit, HighlyCompositeCandidate* scratch, ref int count)
        {
            scratch[count++] = new HighlyCompositeCandidate { Value = cur, Divisors = div };
            if (pIdx >= Primes.Length) return;

            long p = Primes[pIdx];
            for (int e = 1; e <= lastE; e++)
            {
                if (cur > limit / p) break;
                cur *= p;
                Dfs(pIdx + 1, cur, div * (e + 1), e, limit, scratch, ref count);
            }
        }

        private static void QuickSort(HighlyCompositeCandidate* ptr, int leftIn, int rightIn)
        {
            int left = leftIn, right = rightIn;
            while (left < right)
            {
                long pivot = ptr[left + (right - left) / 2].Value;
                int i = left, j = right;
                while (i <= j)
                {
                    while (ptr[i].Value < pivot) i++;
                    while (ptr[j].Value > pivot) j--;
                    if (i <= j) { HighlyCompositeCandidate t = ptr[i]; ptr[i] = ptr[j]; ptr[j] = t; i++; j--; }
                }
                if (j - left < right - i)
                {
                    QuickSort(ptr, left, j);
                    left = i;
                }
                else
                {
                    QuickSort(ptr, i, right);
                    right = j;
                }
            }
        }
    }
}
