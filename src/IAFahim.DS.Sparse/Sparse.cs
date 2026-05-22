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
            if (l == r) { b[node] = 0; return; }
            int mid = (l + r) >> 1;
            int lo = l, hi = r;
            while (lo <= hi)
            {
                int m = (lo + hi) >> 1;
                if (arr[m] <= mid) lo = m + 1;
                else hi = m - 1;
            }
            int leftCount = lo - l;
            b[node] = leftCount;
            left[node * 2] = l;
            left[node * 2 + 1] = lo;
            right[node * 2] = lo - 1;
            right[node * 2 + 1] = r;
            Run(arr, left, right, b, node * 2, l, mid, maxVal);
            Run(arr, left, right, b, node * 2 + 1, mid + 1, r, maxVal);
        }

        public static void RunIndex(int* data, int n, int maxVal, int* left, int* right, int* b, int node, int l, int r)
        {
            if (l > r) return;
            if (l == r) { b[node] = l; return; }
            int mid = (l + r) >> 1;
            int lo = l, hi = r;
            while (lo <= hi)
            {
                int m = (lo + hi) >> 1;
                if (data[m] <= mid) lo = m + 1;
                else hi = m - 1;
            }
            int leftCount = lo - l;
            b[node] = leftCount;
            left[node * 2] = l;
            left[node * 2 + 1] = lo;
            right[node * 2] = lo - 1;
            right[node * 2 + 1] = r;
            RunIndex(data, n, maxVal, left, right, b, node * 2, l, mid);
            RunIndex(data, n, maxVal, left, right, b, node * 2 + 1, mid + 1, r);
        }
    }

    public static unsafe class WaveletRank
    {
        public static int Run(int* left, int* right, int* b, int node, int l, int r, int k, int val)
        {
            if (l > r || k < l || k > r) return 0;
            if (l == r) return 1;
            int leftCount = b[node];
            if (val <= right[node * 2])
                return Run(left, right, b, node * 2, l, l + leftCount - 1, k, val);
            return Run(left, right, b, node * 2 + 1, l + leftCount, r, k, val);
        }
    }

    public static unsafe class WaveletSelect
    {
        public static int Run(int* left, int* right, int* b, int node, int l, int r, int k, int val)
        {
            if (l > r || k < l || k > r) return -1;
            if (l == r) return l;
            int leftCount = b[node];
            if (val <= right[node * 2])
                return Run(left, right, b, node * 2, l, l + leftCount - 1, k, val);
            return Run(left, right, b, node * 2 + 1, l + leftCount, r, k, val);
        }
    }

    public static unsafe class WaveletKth
    {
        public static int Run(int* left, int* right, int* b, int node, int l, int r, int ql, int qr, int k)
        {
            if (ql > r || qr < l || k < 1) return -1;
            if (l == r) return l;
            int leftCount = b[node];
            int inLeft = Math.Min(qr, right[node * 2]) - Math.Max(ql, l) + 1;
            inLeft = Math.Max(0, inLeft);
            if (k <= inLeft)
                return Run(left, right, b, node * 2, l, l + leftCount - 1, ql, Math.Min(qr, right[node * 2]), k);
            return Run(left, right, b, node * 2 + 1, l + leftCount, r, Math.Max(ql, l + leftCount), qr, k - inLeft);
        }
    }

    public static unsafe class WaveletRangeFreq
    {
        public static int Run(int* left, int* right, int* b, int node, int l, int r, int ql, int qr, int a, int b_)
        {
            if (ql > r || qr < l) return 0;
            if (l >= ql && r <= qr)
            {
                if (a <= left[node] && right[node] <= b_) return r - l + 1;
                return 0;
            }
            int leftCount = b[node];
            int mid = (left[node] + right[node]) >> 1;
            int leftL = l, leftR = l + leftCount - 1;
            int rightL = l + leftCount, rightR = r;
            return Run(left, right, b, node * 2, leftL, leftR, ql, qr, a, b_) +
                   Run(left, right, b, node * 2 + 1, rightL, rightR, ql, qr, a, b_);
        }
    }

    public static unsafe class WaveletTreeRangeSum
    {
        public static int Run(int* left, int* right, int* b, int node, int l, int r, int ql, int qr, int a, int b_, int* data)
        {
            if (ql > r || qr < l) return 0;
            if (l >= ql && r <= qr)
            {
                int sum = 0;
                for (int i = l; i <= r; i++)
                    if (a <= data[i] && data[i] <= b_) sum++;
                return sum;
            }
            int leftCount = b[node];
            return Run(left, right, b, node * 2, l, l + leftCount - 1, ql, qr, a, b_, data) +
                   Run(left, right, b, node * 2 + 1, l + leftCount, r, ql, qr, a, b_, data);
        }
    }

    public static unsafe class WaveletTreeLessThan
    {
        public static int Run(int* left, int* right, int* b, int node, int l, int r, int k, int val)
        {
            if (l > r || k > r) return 0;
            if (l == r) return (val >= left[node]) ? (k >= l ? 1 : 0) : 0;
            int leftCount = b[node];
            int inLeft = Math.Min(r, leftCount) - Math.Max(l, 0) + 1;
            inLeft = Math.Max(0, inLeft);
            if (val <= right[node * 2])
                return Run(left, right, b, node * 2, l, l + leftCount - 1, k, val);
            return inLeft + Run(left, right, b, node * 2 + 1, l + leftCount, r, k, val);
        }
    }

    public static unsafe class SuccinctWaveletBuild
    {
        public static int Run(int* data, int n, int maxVal, int* bitmaps, int* ranks, int* mids, int log)
        {
            for (int b = 0; b < log; b++)
            {
                int ones = 0;
                for (int i = 0; i < n; i++)
                {
                    bitmaps[b * (n + 1) + i] = ((data[i] >> b) & 1);
                    if (bitmaps[b * (n + 1) + i] == 1) ones++;
                }
                bitmaps[b * (n + 1) + n] = 0;
                ranks[b * (n + 1) + 0] = 0;
                for (int i = 1; i <= n; i++)
                    ranks[b * (n + 1) + i] = ranks[b * (n + 1) + i - 1] + bitmaps[b * (n + 1) + i - 1];
                mids[b] = ones;
            }
            return log;
        }
    }

    public static unsafe class SuccinctWaveletRank
    {
        public static int Run(int* bitmaps, int* ranks, int* mids, int b, int l, int r, int k, int val)
        {
            if (l > r || k < l || k > r) return 0;
            int* rk = ranks + b * (r + 1);
            int* bm = bitmaps + b * (r + 1);
            if (k == l && l == r) return 1;
            int leftCount = rk[r + 1] - rk[l];
            int bit = (val >> b) & 1;
            if (bit == 0) return Run(bitmaps, ranks, mids, b + 1, rk[l], rk[r + 1] - 1, k, val);
            int mid = mids[b];
            return Run(bitmaps, ranks, mids, b + 1, mid + (l - rk[l]), mid + (r - rk[r + 1] + 1) - 1, k, val);
        }
    }

    public static unsafe class SuccinctWaveletSelect
    {
        public static int Run(int* bitmaps, int* ranks, int* mids, int b, int l, int r, int k, int val)
        {
            if (l > r || k < l || k > r) return -1;
            if (l == r) return l;
            int* rk = ranks + b * (r + 1);
            int* bm = bitmaps + b * (r + 1);
            int leftCount = rk[r + 1] - rk[l];
            int bit = (val >> b) & 1;
            if (bit == 0) return Run(bitmaps, ranks, mids, b + 1, rk[l], rk[r + 1] - 1, k, val);
            int mid = mids[b];
            return Run(bitmaps, ranks, mids, b + 1, mid + (l - rk[l]), mid + (r - rk[r + 1] + 1) - 1, k, val);
        }
    }
}
