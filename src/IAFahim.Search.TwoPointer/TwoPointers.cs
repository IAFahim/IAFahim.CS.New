using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.TwoPointer
{
    public static unsafe class TwoPointers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountPairsWithSum(int* a, int aLen, int* b, int bLen, int target)
        {
            int left = 0;
            int right = bLen - 1;
            int count = 0;
            while (left < aLen && right >= 0)
            {
                int sum = a[left] + b[right];
                if (sum == target)
                {
                    count++;
                    left++;
                    right--;
                }
                else if (sum < target)
                {
                    left++;
                }
                else
                {
                    right--;
                }
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasPairWithSum(int* a, int aLen, int* b, int bLen, int target)
        {
            int left = 0;
            int right = bLen - 1;
            while (left < aLen && right >= 0)
            {
                int sum = a[left] + b[right];
                if (sum == target) return true;
                if (sum < target) left++;
                else right--;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MergeSorted<T>(T* a, int aLen, T* b, int bLen, T* dst) where T : unmanaged, IComparable<T>
        {
            int i = 0, j = 0, k = 0;
            while (i < aLen && j < bLen)
            {
                dst[k++] = a[i].CompareTo(b[j]) <= 0 ? a[i++] : b[j++];
            }
            while (i < aLen) dst[k++] = a[i++];
            while (j < bLen) dst[k++] = b[j++];
            return k;
        }
    }
}