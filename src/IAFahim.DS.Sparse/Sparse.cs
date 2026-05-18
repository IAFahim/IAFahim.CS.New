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
                    int base_ = i * log[n] + (j - 1) * n;
                    int prev = i * log[n] + (j - 1) * n;
                    int left = i + (1 << (j - 1));
                    int right = i * log[n] + (j - 1) * n;
                    table[base_ + n] = Math.Min(table[prev], table[right]);
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
                    int idx = i * log[n] + j * n;
                    int left = i * log[n] + (j - 1) * n;
                    int right = (i + (1 << (j - 1))) * log[n] + (j - 1) * n;
                    table[idx] = Math.Min(table[left], table[right]);
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
            int left = l * log[n] + j * n;
            int right = (r - (1 << j) + 1) * log[n] + j * n;
            return Math.Min(table[left], table[right]);
        }

        public static long MinInt64(long* table, int* log, int l, int r, int n)
        {
            int len = r - l + 1;
            int j = log[len];
            int left = l * log[n] + j * n;
            int right = (r - (1 << j) + 1) * log[n] + j * n;
            return Math.Min(table[left], table[right]);
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
        public static void Run(int* arr, int* blocks, int blockSize, int idx, int val)
        {
            arr[idx] = val;
            int block = idx / blockSize;
            int start = block * blockSize;
            int end = Math.Min(start + blockSize - 1, arr[blockSize * 100]);
            int minVal = arr[start];
            for (int i = start + 1; i <= end; i++)
                minVal = Math.Min(minVal, arr[i]);
            blocks[block] = minVal;
        }
    }

    public static unsafe class SqrtQuery
    {
        public static int RangeMin(int* blocks, int blockSize, int l, int r, int n)
        {
            int minVal = int.MaxValue;
            while (l <= r && l % blockSize != 0)
            {
                minVal = Math.Min(minVal, l < n ? l : 0);
                l++;
            }
            while (l + blockSize <= r)
            {
                minVal = Math.Min(minVal, blocks[l / blockSize]);
                l += blockSize;
            }
            while (l <= r)
            {
                minVal = Math.Min(minVal, l < n ? l : 0);
                l++;
            }
            return minVal;
        }
    }

    public static unsafe class WaveletTreeBuild
    {
        public static void Run(int* arr, int* left, int* right, int* b, int node, int l, int r, int maxVal)
        {
            if (l > r || l == r) return;
            int mid = (l + r) >> 1;
            int lo = l, hi = r;
            while (lo <= hi)
            {
                int m = (lo + hi) >> 1;
                if (arr[m] <= mid)
                {
                    left[node * 2] = lo;
                    lo = m + 1;
                }
                else
                {
                    hi = m - 1;
                }
            }
            right[node * 2] = lo - 1;
            for (int i = l; i <= r; i++)
            {
                if (arr[i] <= mid)
                {
                    left[node * 2 + 1] = left[node * 2] + 1;
                }
                else
                {
                    right[node * 2 + 1] = right[node * 2] + 1;
                }
            }
        }
    }

    public static unsafe class WaveletRank
    {
        public static int Run(int* left, int* right, int node, int l, int r, int k, int val)
        {
            if (l > r || k < l || k > r) return 0;
            if (val <= ((l + r) >> 1))
                return left[node * 2 + 1] - left[node * 2] + 1;
            return right[node * 2 + 1] - right[node * 2] + 1;
        }
    }

    public static unsafe class WaveletSelect
    {
        public static int Run(int* left, int* right, int node, int l, int r, int k, int val)
        {
            if (l > r || k < l || k > r) return -1;
            if (l == r) return l;
            int inLeft = left[node * 2 + 1] - left[node * 2];
            if (val <= ((l + r) >> 1))
                return Run(left, right, node * 2, l, (l + r) >> 1, left[node * 2] + k - 1, val);
            return Run(left, right, node * 2 + 1, (l + r) >> 1 + 1, r, right[node * 2] + k - 1, val);
        }
    }

    public static unsafe class WaveletKth
    {
        public static int Run(int* left, int* right, int node, int l, int r, int ql, int qr, int k)
        {
            if (ql > r || qr < l) return -1;
            if (l >= ql && r <= qr) return l + k - 1;
            int mid = (l + r) >> 1;
            int leftCount = Math.Min(qr, mid) - Math.Max(ql, l) + 1;
            if (k <= leftCount)
                return Run(left, right, node * 2, l, mid, ql, Math.Min(qr, mid), k);
            return Run(left, right, node * 2 + 1, mid + 1, r, Math.Max(ql, mid + 1), qr, k - leftCount);
        }
    }

    public static unsafe class WaveletRangeFreq
    {
        public static int Run(int* left, int* right, int node, int l, int r, int ql, int qr, int a, int b)
        {
            if (ql > r || qr < l) return 0;
            if (l >= ql && r <= qr)
            {
                int lo = l, hi = r;
                while (lo <= hi)
                {
                    int m = (lo + hi) >> 1;
                    if (m <= b) lo = m + 1;
                    else hi = m - 1;
                }
                int countB = lo - Math.Max(ql, l);
                lo = l; hi = r;
                while (lo <= hi)
                {
                    int m = (lo + hi) >> 1;
                    if (m >= a) hi = m - 1;
                    else lo = m + 1;
                }
                int countA = Math.Min(qr, r) - lo + 1;
                return countB + countA;
            }
            int mid = (l + r) >> 1;
            return Run(left, right, node * 2, l, mid, ql, qr, a, b) +
                   Run(left, right, node * 2 + 1, mid + 1, r, ql, qr, a, b);
        }
    }
}