namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PowerfulNumbers
    {
        public static int Generate(long limit, long* result)
        {
            if (limit <= 0)
            {
                return 0;
            }
            int count = 0;
            for (long b = 1; b * b * b <= limit; b++)
            {
                long b3 = b * b * b;
                long maxA = (long)Math.Sqrt((double)(limit / b3));
                for (long a = 1; a <= maxA; a++)
                {
                    result[count++] = a * a * b3;
                }
            }
            QuickSort(result, 0, count - 1);
            int uniqueCount = 0;
            if (count > 0)
            {
                result[uniqueCount++] = result[0];
                for (int i = 1; i < count; i++)
                {
                    if (result[i] != result[i - 1])
                    {
                        result[uniqueCount++] = result[i];
                    }
                }
            }
            return uniqueCount;
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
