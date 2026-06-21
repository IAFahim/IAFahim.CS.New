using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.MeetInMiddle
{
    public static unsafe class MeetInMiddle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SubsetSumCount(int* values, int len, int target)
        {
            int half = len >> 1;
            int leftCount = 1 << half;
            int rightCount = 1 << (len - half);
            long* leftSums = stackalloc long[leftCount];
            long* rightSums = stackalloc long[rightCount];
            for (int mask = 0; mask < leftCount; mask++)
            {
                long sum = 0;
                for (int i = 0; i < half; i++)
                {
                    if ((mask & (1 << i)) != 0)
                        sum += values[i];
                }
                leftSums[mask] = sum;
            }
            for (int mask = 0; mask < rightCount; mask++)
            {
                long sum = 0;
                for (int i = 0; i < len - half; i++)
                {
                    if ((mask & (1 << i)) != 0)
                        sum += values[half + i];
                }
                rightSums[mask] = sum;
            }

            SortLongs(rightSums, rightCount);

            int count = 0;
            for (int i = 0; i < leftCount; i++)
            {
                long rem = target - leftSums[i];
                int lo = LowerBoundLong(rightSums, rightCount, rem);
                int hi = UpperBoundLong(rightSums, rightCount, rem);
                count += hi - lo;
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasSubsetSum(int* values, int len, int target)
        {
            int half = len >> 1;
            int leftCount = 1 << half;
            int rightCount = 1 << (len - half);
            long* leftSums = stackalloc long[leftCount];
            long* rightSums = stackalloc long[rightCount];
            for (int mask = 0; mask < leftCount; mask++)
            {
                long sum = 0;
                for (int i = 0; i < half; i++)
                {
                    if ((mask & (1 << i)) != 0)
                        sum += values[i];
                }
                leftSums[mask] = sum;
            }
            for (int mask = 0; mask < rightCount; mask++)
            {
                long sum = 0;
                for (int i = 0; i < len - half; i++)
                {
                    if ((mask & (1 << i)) != 0)
                        sum += values[half + i];
                }
                rightSums[mask] = sum;
            }

            SortLongs(rightSums, rightCount);

            for (int i = 0; i < leftCount; i++)
            {
                long rem = target - leftSums[i];
                int idx = LowerBoundLong(rightSums, rightCount, rem);
                if (idx < rightCount && rightSums[idx] == rem)
                    return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SortLongs(long* arr, int len)
        {
            for (int i = (len >> 1) - 1; i >= 0; i--) SiftDown(arr, i, len);
            for (int end = len - 1; end > 0; end--)
            {
                long t = arr[0]; arr[0] = arr[end]; arr[end] = t;
                SiftDown(arr, 0, end);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SiftDown(long* a, int i, int n)
        {
            while (true)
            {
                int l = 2 * i + 1, r = 2 * i + 2, m = i;
                if (l < n && a[l] > a[m]) m = l;
                if (r < n && a[r] > a[m]) m = r;
                if (m == i) break;
                long t = a[i]; a[i] = a[m]; a[m] = t;
                i = m;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LowerBoundLong(long* arr, int len, long val)
        {
            int lo = 0, hi = len;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (arr[mid] < val) lo = mid + 1;
                else hi = mid;
            }
            return lo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int UpperBoundLong(long* arr, int len, long val)
        {
            int lo = 0, hi = len;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (arr[mid] <= val) lo = mid + 1;
                else hi = mid;
            }
            return lo;
        }
    }
}
