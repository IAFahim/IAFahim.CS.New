namespace IAFahim.Search.Bit
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LisLength
    {
        public static int Run(int n, int* arr)
        {
            int* tail = stackalloc int[n];
            int len = 0;
            for (int i = 0; i < n; i++)
            {
                int pos = 0;
                int lo = 0, hi = len;
                while (lo < hi)
                {
                    int mid = (lo + hi) >> 1;
                    if (tail[mid] < arr[i]) lo = mid + 1;
                    else hi = mid;
                }
                pos = lo;
                tail[pos] = arr[i];
                if (pos >= len) len++;
            }
            return len;
        }
    }

    public static unsafe class LdsLength
    {
        public static int Run(int n, int* arr)
        {
            int* tail = stackalloc int[n];
            int len = 0;
            for (int i = 0; i < n; i++)
            {
                int pos = 0;
                int lo = 0, hi = len;
                while (lo < hi)
                {
                    int mid = (lo + hi) >> 1;
                    if (tail[mid] > arr[i]) lo = mid + 1;
                    else hi = mid;
                }
                pos = lo;
                tail[pos] = arr[i];
                if (pos >= len) len++;
            }
            return len;
        }
    }

    public static unsafe class BinarySearchLower
    {
        public static int Run(int* arr, int n, int target)
        {
            int lo = 0, hi = n;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (arr[mid] < target) lo = mid + 1;
                else hi = mid;
            }
            return lo;
        }
    }

    public static unsafe class BinarySearchUpper
    {
        public static int Run(int* arr, int n, int target)
        {
            int lo = 0, hi = n;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (arr[mid] <= target) lo = mid + 1;
                else hi = mid;
            }
            return lo;
        }
    }

    public static unsafe class KthElement
    {
        public static int Run(int* arr, int n, int k)
        {
            long lo = int.MinValue, hi = int.MaxValue;
            while (lo < hi)
            {
                int mid = (int)((lo + hi) >> 1);
                int cnt = 0;
                for (int i = 0; i < n; i++)
                    cnt += (arr[i] <= mid) ? 1 : 0;
                if (cnt >= k) hi = mid;
                else lo = mid + 1;
            }
            return (int)lo;
        }
    }

    public static unsafe class LongestIncreasingSubsequence2D
    {
        public static int Run(int n, int* x, int* y)
        {
            int* dp = stackalloc int[n];
            int len = 0;
            for (int i = 0; i < n; i++)
            {
                int lo = 0, hi = len;
                while (lo < hi)
                {
                    int mid = (lo + hi) >> 1;
                    if (x[dp[mid]] < x[i] || (x[dp[mid]] == x[i] && y[dp[mid]] < y[i])) lo = mid + 1;
                    else hi = mid;
                }
                dp[lo] = i;
                if (lo >= len) len++;
            }
            return len;
        }
    }

    public static unsafe class BitonicLength
    {
        public static int Run(int n, int* arr)
        {
            if (n <= 0) return 0;
            int* inc = stackalloc int[n];
            int* dec = stackalloc int[n];
            int* lisLengths = stackalloc int[n];
            int* ldsLengths = stackalloc int[n];
            int lenI = 0, lenD = 0;
            for (int i = 0; i < n; i++)
            {
                int lo = 0, hi = lenI;
                while (lo < hi)
                {
                    int mid = (lo + hi) >> 1;
                    if (arr[inc[mid]] < arr[i]) lo = mid + 1;
                    else hi = mid;
                }
                inc[lo] = i;
                if (lo >= lenI) lenI++;
                lisLengths[i] = lo + 1;
            }
            for (int i = n - 1; i >= 0; i--)
            {
                int lo = 0, hi = lenD;
                while (lo < hi)
                {
                    int mid = (lo + hi) >> 1;
                    if (arr[dec[mid]] < arr[i]) lo = mid + 1;
                    else hi = mid;
                }
                dec[lo] = i;
                if (lo >= lenD) lenD++;
                ldsLengths[i] = lo + 1;
            }
            int maxLen = 0;
            for (int i = 0; i < n; i++)
            {
                int val = lisLengths[i] + ldsLengths[i] - 1;
                if (val > maxLen) maxLen = val;
            }
            return maxLen;
        }
    }

    public static unsafe class PatienceSort
    {
        public static int Run(int n, int* arr, int* piles, int* tops)
        {
            int numPiles = 0;
            for (int i = 0; i < n; i++)
            {
                int lo = 0, hi = numPiles;
                while (lo < hi)
                {
                    int mid = (lo + hi) >> 1;
                    if (tops[mid] < arr[i]) lo = mid + 1;
                    else hi = mid;
                }
                piles[i] = lo + 1;
                tops[lo] = arr[i];
                if (lo >= numPiles) numPiles++;
            }
            return numPiles;
        }
    }

    public static unsafe class InversionCount
    {
        public static long Run(int n, int* arr)
        {
            int* temp = stackalloc int[n];
            return MergeSortCount(arr, temp, 0, n - 1);
        }

        private static long MergeSortCount(int* arr, int* temp, int l, int r)
        {
            if (l >= r) return 0;
            int m = (l + r) >> 1;
            long cnt = MergeSortCount(arr, temp, l, m);
            cnt += MergeSortCount(arr, temp, m + 1, r);
            int i = l, j = m + 1, k = l;
            while (i <= m && j <= r)
            {
                if (arr[i] <= arr[j]) temp[k++] = arr[i++];
                else { temp[k++] = arr[j++]; cnt += m - i + 1; }
            }
            while (i <= m) temp[k++] = arr[i++];
            while (j <= r) temp[k++] = arr[j++];
            for (i = l; i <= r; i++) arr[i] = temp[i];
            return cnt;
        }
    }
}
