namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PowerfulNumbers
    {
        public static int Generate(long limit, long* result)
        {
            if (limit <= 0) return 0;
            int count = BuildRawPowerful(limit, result);
            QuickSort(result, 0, count - 1);
            return Unique(result, count);
        }

        private static int BuildRawPowerful(long limit, long* res)
        {
            int count = 0;
            for (long b = 1; b * b * b <= limit; b++)
            {
                long b3 = b * b * b, maxA = (long)Math.Sqrt((double)(limit / b3));
                while (maxA > 0 && maxA * maxA * b3 > limit) maxA--;
                for (long a = 1; a <= maxA; a++) res[count++] = a * a * b3;
            }
            return count;
        }

        private static int Unique(long* res, int count)
        {
            if (count <= 0) return 0;
            int uniqueCount = 1;
            for (int i = 1; i < count; i++) if (res[i] != res[i - 1]) res[uniqueCount++] = res[i];
            return uniqueCount;
        }

        private static void QuickSort(long* ptr, int left, int right)
        {
            if (left >= right) return;
            long pivot = ptr[left + (right - left) / 2];
            int i = left, j = right;
            while (i <= j)
            {
                while (ptr[i] < pivot) i++;
                while (ptr[j] > pivot) j--;
                if (i <= j) { long t = ptr[i]; ptr[i] = ptr[j]; ptr[j] = t; i++; j--; }
            }
            QuickSort(ptr, left, j); QuickSort(ptr, i, right);
        }
    }
}
