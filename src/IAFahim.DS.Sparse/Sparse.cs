namespace IAFahim.DS.Sparse
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SparseTableBuild
    {
        public static void RunInt32(int* arr, int* table, int* log, int n)
        {
            InitializeLevel0(arr, table, n);
            for (int j = 1; (1 << j) <= n; j++)
            {
                ComputeNextLevelInt32(table, n, j);
            }
        }

        private static void InitializeLevel0(int* arr, int* table, int n)
        {
            for (int i = 0; i < n; i++) table[i] = arr[i];
        }

        private static void ComputeNextLevelInt32(int* table, int n, int j)
        {
            int offset = j * n;
            int prevOffset = (j - 1) * n;
            int half = 1 << (j - 1);
            for (int i = 0; i + (1 << j) <= n; i++)
            {
                int left = table[i + prevOffset];
                int right = table[i + half + prevOffset];
                table[i + offset] = left < right ? left : right;
            }
        }

        public static void RunInt64(long* arr, long* table, int* log, int n)
        {
            InitializeLevel0Int64(arr, table, n);
            for (int j = 1; (1 << j) <= n; j++)
            {
                ComputeNextLevelInt64(table, n, j);
            }
        }

        private static void InitializeLevel0Int64(long* arr, long* table, int n)
        {
            for (int i = 0; i < n; i++) table[i] = arr[i];
        }

        private static void ComputeNextLevelInt64(long* table, int n, int j)
        {
            int offset = j * n;
            int prevOffset = (j - 1) * n;
            int half = 1 << (j - 1);
            for (int i = 0; i + (1 << j) <= n; i++)
            {
                long left = table[i + prevOffset];
                long right = table[i + half + prevOffset];
                table[i + offset] = left < right ? left : right;
            }
        }
    }

    public static unsafe class DisjointSparseBuild
    {
        public static void RunInt64(long* arr, long* table, int* blockSize, int n)
        {
            int levels = CalculateLevels(n);
            *blockSize = n;
            InitializeLevel0(arr, table, n);

            for (int j = 0; j < levels; j++)
            {
                ComputeLevelInt64(arr, table, n, j);
            }
        }

        private static int CalculateLevels(int n)
        {
            int levels = 0;
            while ((1 << levels) < n) levels++;
            return levels == 0 ? 1 : levels;
        }

        private static void InitializeLevel0(long* arr, long* table, int n)
        {
            for (int i = 0; i < n; i++) table[i] = arr[i];
        }

        private static void ComputeLevelInt64(long* arr, long* table, int n, int j)
        {
            int half = 1 << j;
            int blockLen = 1 << (j + 1);
            for (int start = 0; start < n; start += blockLen)
            {
                int mid = Math.Min(start + half - 1, n - 1);
                ProcessLeftBlock(arr, table, n, j, start, mid);
                if (mid + 1 < n)
                {
                    int end = Math.Min(start + blockLen - 1, n - 1);
                    ProcessRightBlock(arr, table, n, j, mid + 1, end);
                }
            }
        }

        private static void ProcessLeftBlock(long* arr, long* table, int n, int j, int start, int mid)
        {
            long cur = arr[mid];
            table[j * n + mid] = cur;
            for (int i = mid - 1; i >= start; i--)
            {
                cur = cur < arr[i] ? cur : arr[i];
                table[j * n + i] = cur;
            }
        }

        private static void ProcessRightBlock(long* arr, long* table, int n, int j, int start, int end)
        {
            long cur = arr[start];
            table[j * n + start] = cur;
            for (int i = start + 1; i <= end; i++)
            {
                cur = cur < arr[i] ? cur : arr[i];
                table[j * n + i] = cur;
            }
        }
    }
}
