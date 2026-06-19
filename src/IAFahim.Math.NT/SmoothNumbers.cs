namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SmoothNumbers
    {
        public static int Generate(int b, long limit, long* result, int* primes)
        {
            int count = 1; result[0] = 1;
            for (int i = 0; i < b; i++)
            {
                int p = primes[i];
                int curCount = count;
                for (int j = 0; j < curCount; j++)
                {
                    long val = result[j];
                    while (val <= limit / p) { val *= p; result[count++] = val; }
                }
                QuickSort(result, 0, count - 1);
            }
            return count;
        }

        private static void QuickSort(long* ptr, int leftIn, int rightIn)
        {
            int left = leftIn, right = rightIn;
            while (left < right)
            {
                long pivot = ptr[left + (right - left) / 2];
                int i = left, j = right;
                while (i <= j)
                {
                    while (ptr[i] < pivot) i++;
                    while (ptr[j] > pivot) j--;
                    if (i <= j) { long t = ptr[i]; ptr[i] = ptr[j]; ptr[j] = t; i++; j--; }
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
