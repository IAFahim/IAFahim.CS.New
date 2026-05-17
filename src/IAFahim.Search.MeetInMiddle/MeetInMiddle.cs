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
            int count = 0;
            for (int i = 0; i < leftCount; i++)
            {
                long rem = target - leftSums[i];
                for (int j = 0; j < rightCount; j++)
                {
                    if (leftSums[i] + rightSums[j] == target)
                        count++;
                }
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
            for (int i = 0; i < leftCount; i++)
            {
                long rem = target - leftSums[i];
                for (int j = 0; j < rightCount; j++)
                {
                    if (leftSums[i] + rightSums[j] == target)
                        return true;
                }
            }
            return false;
        }
    }
}