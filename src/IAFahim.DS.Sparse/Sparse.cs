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
            // Precompute loop bound
            int maxI = n - (1 << j);
            for (int i = 0; i <= maxI; i++)
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
            // Precompute loop bound
            int maxI = n - (1 << j);
            for (int i = 0; i <= maxI; i++)
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

            for (int j = 0; j < levels; j++)
            {
                int half = 1 << j;
                int blockLen = 1 << (j + 1);
                for (int start = 0; start < n; start += blockLen)
                {
                    int mid = start + half - 1;
                    if (mid >= n) mid = n - 1;
                    
                    long cur = arr[mid];
                    table[j * n + mid] = cur;
                    for (int i = mid - 1; i >= start; i--)
                    {
                        cur = cur < arr[i] ? cur : arr[i];
                        table[j * n + i] = cur;
                    }

                    if (mid + 1 < n)
                    {
                        int end = start + blockLen - 1;
                        if (end >= n) end = n - 1;
                        cur = arr[mid + 1];
                        table[j * n + mid + 1] = cur;
                        for (int i = mid + 2; i <= end; i++)
                        {
                            cur = cur < arr[i] ? cur : arr[i];
                            table[j * n + i] = cur;
                        }
                    }
                }
            }
        }

        private static int CalculateLevels(int n)
        {
            int levels = 0;
            while ((1 << levels) < n) levels++;
            return levels == 0 ? 1 : levels;
        }
    }

    public static unsafe class SparseTableQuery
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinInt32(int* table, int* log, int l, int r, int n)
        {
            int j = log[r - l + 1];
            int left = table[j * n + l];
            int right = table[j * n + r - (1 << j) + 1];
            return left < right ? left : right;
        }
    }

    public static unsafe class SqrtDecomposeBuild
    {
        public static void Run(int* arr, int* block, int* blockSize, int n)
        {
            int bSize = (int)Math.Sqrt(n);
            if (bSize == 0) bSize = 1;
            *blockSize = bSize;
            for (int i = 0; i < (n + bSize - 1) / bSize; i++) block[i] = int.MaxValue;
            for (int i = 0; i < n; i++)
            {
                int bIdx = i / bSize;
                if (arr[i] < block[bIdx]) block[bIdx] = arr[i];
            }
        }
    }

    public static unsafe class SqrtQuery
    {
        public static int RangeMin(int* arr, int* block, int blockSize, int l, int r, int n)
        {
            int res = int.MaxValue;
            int bl = l / blockSize, br = r / blockSize;
            if (bl == br)
            {
                for (int i = l; i <= r; i++) if (arr[i] < res) res = arr[i];
            }
            else
            {
                for (int i = l; i < (bl + 1) * blockSize; i++) if (arr[i] < res) res = arr[i];
                for (int i = bl + 1; i < br; i++) if (block[i] < res) res = block[i];
                for (int i = br * blockSize; i <= r; i++) if (arr[i] < res) res = arr[i];
            }
            return res;
        }
    }

    public static unsafe class DisjointSparseQuery
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RangeMinInt64(long* table, int* blockSize, int l, int r)
        {
            if (l > r) { int tmp = l; l = r; r = tmp; }
            if (l == r) return table[l];
            int n = *blockSize;
            int j = BitOperations_Log2((uint)(l ^ r));
            long left = table[j * n + l];
            long right = table[j * n + r];
            return left < right ? left : right;
        }

        private static int BitOperations_Log2(uint value)
        {
            int res = 0;
            if ((value & 0xFFFF0000) != 0) { value >>= 16; res |= 16; }
            if ((value & 0xFF00) != 0) { value >>= 8; res |= 8; }
            if ((value & 0xF0) != 0) { value >>= 4; res |= 4; }
            if ((value & 0xC) != 0) { value >>= 2; res |= 2; }
            if ((value & 0x2) != 0) { res |= 1; }
            return res;
        }
    }
}
