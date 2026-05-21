namespace IAFahim.DS.Sparse
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SparseTableBuild
    {
        public static void RunInt32(int* arr, int* table, int* log, int n)
        {
            for (int i = 0; i < n; i++)
                table[i] = arr[i];
            for (int j = 1; (1 << j) <= n; j++)
            {
                for (int i = 0; i + (1 << j) <= n; i++)
                {
                    int left = table[i + (j - 1) * n];
                    int right = table[i + (1 << (j - 1)) + (j - 1) * n];
                    table[i + j * n] = left < right ? left : right;
                }
            }
        }

        public static void RunInt64(long* arr, long* table, int* log, int n)
        {
            for (int i = 0; i < n; i++)
                table[i] = arr[i];
            for (int j = 1; (1 << j) <= n; j++)
            {
                for (int i = 0; i + (1 << j) <= n; i++)
                {
                    long left = table[i + (j - 1) * n];
                    long right = table[i + (1 << (j - 1)) + (j - 1) * n];
                    table[i + j * n] = left < right ? left : right;
                }
            }
        }
    }

    public static unsafe class SparseTableQuery
    {
        public static int MinInt32(int* table, int* log, int l, int r, int n)
        {
            int len = r - l + 1;
            int j = log[len];
            int left = table[l + j * n];
            int right = table[r - (1 << j) + 1 + j * n];
            return left < right ? left : right;
        }

        public static long MinInt64(long* table, int* log, int l, int r, int n)
        {
            int len = r - l + 1;
            int j = log[len];
            long left = table[l + j * n];
            long right = table[r - (1 << j) + 1 + j * n];
            return left < right ? left : right;
        }
    }

    public static unsafe class DisjointSparseBuild
    {
        public static void RunInt64(long* arr, long* table, int* blockSize, int n)
        {
            int b = 0;
            while ((1 << b) <= n) b++;
            b--;
            int sz = 1 << b;
            for (int i = 0; i < n; i++)
            {
                int base_ = i * b;
                table[base_] = arr[i];
                for (int j = 1; j < sz && i + j < n; j++)
                    table[base_ + j] = Math.Min(table[base_ + j - 1], arr[i + j]);
            }
        }
    }

    public static unsafe class DisjointSparseQuery
    {
        public static long RangeMinInt64(long* table, int* blockSize, int l, int r)
        {
            int b = 0;
            while ((1 << b) <= r - l + 1) b++;
            b--;
            int sz = 1 << b;
            int blockL = l / sz;
            int blockR = r / sz;
            if (blockL == blockR)
            {
                long res = long.MaxValue;
                for (int i = l; i <= r; i++)
                    res = Math.Min(res, table[i * b]);
                return res;
            }
            long leftRes = long.MaxValue;
            int bsz = 0;
            while ((1 << bsz) <= r - l + 1) bsz++;
            bsz--;
            for (int i = l; i < (blockL + 1) * sz; i++)
                leftRes = Math.Min(leftRes, table[i * bsz]);
            long rightRes = long.MaxValue;
            for (int i = blockR * sz; i <= r; i++)
                rightRes = Math.Min(rightRes, table[i * bsz]);
            return Math.Min(leftRes, rightRes);
        }
    }

    public static unsafe class SqrtDecomposeBuild
    {
        public static void Run(int* arr, int* blocks, int* blockSize, int n)
        {
            int b = 0;
            while ((b + 1) * (b + 1) <= n) b++;
            *blockSize = b == 0 ? n : b;
            for (int i = 0; i < n; i++)
            {
                int block = i / (*blockSize);
                if (i == 0 || i % (*blockSize) == 0)
                    blocks[block] = arr[i];
                else
                    blocks[block] = Math.Min(blocks[block], arr[i]);
            }
        }
    }

    public static unsafe class SqrtUpdate
    {
        public static void Run(int* arr, int* blocks, int blockSize, int idx, int val, int n)
        {
            arr[idx] = val;
            int block = idx / blockSize;
            int start = block * blockSize;
            int end = Math.Min(start + blockSize - 1, n - 1);
            int minVal = arr[start];
            for (int i = start + 1; i <= end; i++)
                minVal = Math.Min(minVal, arr[i]);
            blocks[block] = minVal;
        }
    }

    public static unsafe class SqrtQuery
    {
        public static int RangeMin(int* arr, int* blocks, int blockSize, int l, int r, int n)
        {
            int minVal = int.MaxValue;
            while (l <= r && l % blockSize != 0)
            {
                minVal = Math.Min(minVal, arr[l]);
                l++;
            }
            while (l + blockSize <= r)
            {
                minVal = Math.Min(minVal, blocks[l / blockSize]);
                l += blockSize;
            }
            while (l <= r)
            {
                minVal = Math.Min(minVal, arr[l]);
                l++;
            }
            return minVal;
        }
    }

    public static unsafe class WaveletTreeBuild
    {
        public static void Run(int* arr, int* left, int* right, int* b, int node, int l, int r, int maxVal)
        {
            if (l > r) return;
            left[node] = l;
            right[node] = r;
            if (l == r) return;
            int mid = (l + r) >> 1;
            int lo = l, hi = r;
            while (lo <= hi)
            {
                int m = (lo + hi) >> 1;
                if (arr[m] <= mid)
                {
                    lo = m + 1;
                }
                else
                {
                    hi = m - 1;
                }
            }
            int leftCount = lo - l;
            left[node * 2] = l;
            left[node * 2 + 1] = lo;
            right[node * 2] = lo - 1;
            right[node * 2 + 1] = r;
            Run(arr, left, right, b, node * 2, l, mid, maxVal);
            Run(arr, left, right, b, node * 2 + 1, mid + 1, r, maxVal);
        }
    }

    public static unsafe class WaveletRank
    {
        public static int Run(int* left, int* right, int node, int l, int r, int k, int val)
        {
            if (l > r || k < l || k > r) return 0;
            if (l == r) return 1;
            int mid = (left[node] + right[node]) >> 1;
            if (val <= mid)
                return Run(left, right, node * 2, l, r, k, val);
            return Run(left, right, node * 2 + 1, l, r, k, val);
        }
    }

    public static unsafe class WaveletSelect
    {
        public static int Run(int* left, int* right, int node, int l, int r, int k, int val)
        {
            if (l > r || k < l || k > r) return -1;
            if (l == r) return l;
            int leftCount = left[node * 2 + 1] - left[node * 2];
            int mid = (left[node] + right[node]) >> 1;
            if (val <= mid)
                return Run(left, right, node * 2, l, r, k, val);
            return Run(left, right, node * 2 + 1, l, r, k, val);
        }
    }

    public static unsafe class WaveletKth
    {
        public static int Run(int* left, int* right, int node, int l, int r, int ql, int qr, int k)
        {
            if (ql > r || qr < l) return -1;
            if (l == r) return l;
            int leftCount = left[node * 2 + 1] - left[node * 2];
            int mid = (left[node] + right[node]) >> 1;
            int inLeft = Math.Min(qr, mid) - Math.Max(ql, l) + 1;
            if (k <= inLeft)
                return Run(left, right, node * 2, l, mid, ql, Math.Min(qr, mid), k);
            return Run(left, right, node * 2 + 1, mid + 1, r, Math.Max(ql, mid + 1), qr, k - inLeft);
        }
    }

    public static unsafe class WaveletRangeFreq
    {
        public static int Run(int* left, int* right, int node, int l, int r, int ql, int qr, int a, int b)
        {
            if (ql > r || qr < l) return 0;
            if (l >= ql && r <= qr)
            {
                if (a <= left[node] && right[node] <= b) return r - l + 1;
                return 0;
            }
            int mid = (left[node] + right[node]) >> 1;
            return Run(left, right, node * 2, l, mid, ql, qr, a, b) +
                   Run(left, right, node * 2 + 1, mid + 1, r, ql, qr, a, b);
        }
    }
}
