namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct HighlyCompositeCandidate
    {
        public long Value;
        public long Divisors;
    }

    public static unsafe class HighlyCompositeNumbers
    {
        private const int MaxCandidates = 20000;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetPrime(int index)
        {
            switch (index)
            {
                case 0: return 2;
                case 1: return 3;
                case 2: return 5;
                case 3: return 7;
                case 4: return 11;
                case 5: return 13;
                case 6: return 17;
                case 7: return 19;
                case 8: return 23;
                case 9: return 29;
                case 10: return 31;
                case 11: return 37;
                case 12: return 41;
                case 13: return 43;
                case 14: return 47;
                default: return 53;
            }
        }

        public static int Run(long limit, long* result, HighlyCompositeCandidate* scratch)
        {
            if (limit <= 0)
            {
                return 0;
            }

            int count = 0;
            Generate(0, 1, 1, 60, limit, scratch, ref count);
            QuickSort(scratch, 0, count - 1);

            int outCount = 0;
            long maxDivisors = 0;
            for (int i = 0; i < count; i++)
            {
                if (scratch[i].Divisors > maxDivisors)
                {
                    maxDivisors = scratch[i].Divisors;
                    result[outCount++] = scratch[i].Value;
                }
            }
            return outCount;
        }

        private static void Generate(
            int primeIndex,
            long currentValue,
            long currentDivisors,
            int lastExponent,
            long limit,
            HighlyCompositeCandidate* candidates,
            ref int count)
        {
            if (count >= MaxCandidates)
            {
                return;
            }

            candidates[count].Value = currentValue;
            candidates[count].Divisors = currentDivisors;
            count++;

            if (primeIndex >= 15)
            {
                return;
            }

            int p = GetPrime(primeIndex);
            long nextValue = currentValue;
            for (int e = 1; e <= lastExponent; e++)
            {
                if (limit / p < nextValue)
                {
                    break;
                }
                nextValue *= (long)p;
                Generate(
                    primeIndex + 1,
                    nextValue,
                    currentDivisors * (long)(e + 1),
                    e,
                    limit,
                    candidates,
                    ref count);
            }
        }

        private static void QuickSort(HighlyCompositeCandidate* ptr, int left, int right)
        {
            if (left >= right)
            {
                return;
            }
            HighlyCompositeCandidate pivot = ptr[left + (right - left) / 2];
            int i = left;
            int j = right;
            while (i <= j)
            {
                while (ptr[i].Value < pivot.Value)
                {
                    i++;
                }
                while (ptr[j].Value > pivot.Value)
                {
                    j--;
                }
                if (i <= j)
                {
                    HighlyCompositeCandidate temp = ptr[i];
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