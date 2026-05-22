namespace IAFahim.Search.RangeQueries
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RangeGcdQuery
    {
        public static long Gcd(long a, long b)
        {
            while (b != 0) { long t = a % b; a = b; b = t; }
            return a;
        }

        public static long Run(long* arr, int n, int l, int r)
        {
            long res = arr[l];
            for (int i = l + 1; i <= r; i++)
                res = Gcd(res, arr[i]);
            return res;
        }
    }

    public static unsafe class RangeLcmQuery
    {
        public static long Gcd(long a, long b)
        {
            while (b != 0) { long t = a % b; a = b; b = t; }
            return a;
        }

        public static long Lcm(long a, long b)
        {
            if (a == 0 || b == 0) return 0;
            return a / Gcd(a, b) * b;
        }

        public static long Run(long* arr, int n, int l, int r)
        {
            long res = arr[l];
            for (int i = l + 1; i <= r; i++)
                res = Lcm(res, arr[i]);
            return res;
        }
    }

    public static unsafe class RangeBitwiseAndQuery
    {
        public static int Run(int* arr, int n, int l, int r)
        {
            int res = ~0;
            for (int i = l; i <= r; i++)
                res &= arr[i];
            return res;
        }

        public static long RunLong(long* arr, int n, int l, int r)
        {
            long res = ~0L;
            for (int i = l; i <= r; i++)
                res &= arr[i];
            return res;
        }
    }

    public static unsafe class RangeBitwiseOrQuery
    {
        public static int Run(int* arr, int n, int l, int r)
        {
            int res = 0;
            for (int i = l; i <= r; i++)
                res |= arr[i];
            return res;
        }

        public static long RunLong(long* arr, int n, int l, int r)
        {
            long res = 0;
            for (int i = l; i <= r; i++)
                res |= arr[i];
            return res;
        }
    }

    public static unsafe class RangeBitwiseXorQuery
    {
        public static int RunInt32(int* arr, int n, int l, int r)
        {
            int res = 0;
            for (int i = l; i <= r; i++)
                res ^= arr[i];
            return res;
        }

        public static long RunInt64(long* arr, int n, int l, int r)
        {
            long res = 0;
            for (int i = l; i <= r; i++)
                res ^= arr[i];
            return res;
        }
    }

    public static unsafe class RangeChminChmaxChadd
    {
        public static void RunMin(long* tree, long* lazy, int node, int l, int r, int ql, int qr, long val)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr)
            {
                if (tree[node] <= val) return;
                tree[node] = val;
                if (l != r)
                {
                    lazy[node * 2] = Math.Min(lazy[node * 2], val);
                    lazy[node * 2 + 1] = Math.Min(lazy[node * 2 + 1], val);
                }
                return;
            }
            int mid = (l + r) >> 1;
            RunMin(tree, lazy, node * 2, l, mid, ql, qr, val);
            RunMin(tree, lazy, node * 2 + 1, mid + 1, r, ql, qr, val);
            tree[node] = Math.Min(tree[node * 2], tree[node * 2 + 1]);
        }

        public static void RunMax(long* tree, long* lazy, int node, int l, int r, int ql, int qr, long val)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr)
            {
                if (tree[node] >= val) return;
                tree[node] = val;
                if (l != r)
                {
                    lazy[node * 2] = Math.Max(lazy[node * 2], val);
                    lazy[node * 2 + 1] = Math.Max(lazy[node * 2 + 1], val);
                }
                return;
            }
            int mid = (l + r) >> 1;
            RunMax(tree, lazy, node * 2, l, mid, ql, qr, val);
            RunMax(tree, lazy, node * 2 + 1, mid + 1, r, ql, qr, val);
            tree[node] = Math.Max(tree[node * 2], tree[node * 2 + 1]);
        }

        public static void RunAdd(long* tree, long* lazy, int node, int l, int r, int ql, int qr, long val)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr)
            {
                tree[node] += val * (r - l + 1);
                if (l != r)
                {
                    lazy[node * 2] += val;
                    lazy[node * 2 + 1] += val;
                }
                return;
            }
            int mid = (l + r) >> 1;
            RunAdd(tree, lazy, node * 2, l, mid, ql, qr, val);
            RunAdd(tree, lazy, node * 2 + 1, mid + 1, r, ql, qr, val);
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }
    }

    public static unsafe class RangeAffineUpdate
    {
        public static void Run(long* tree, long* lazyMul, long* lazyAdd, int node, int l, int r, int ql, int qr, long mul, long add, long mod)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr)
            {
                tree[node] = (tree[node] * mul + add * (r - l + 1)) % mod;
                if (l != r)
                {
                    lazyMul[node * 2] = (lazyMul[node * 2] * mul) % mod;
                    lazyMul[node * 2 + 1] = (lazyMul[node * 2 + 1] * mul) % mod;
                    lazyAdd[node * 2] = (lazyAdd[node * 2] * mul + add) % mod;
                    lazyAdd[node * 2 + 1] = (lazyAdd[node * 2 + 1] * mul + add) % mod;
                }
                return;
            }
            int mid = (l + r) >> 1;
            Run(tree, lazyMul, lazyAdd, node * 2, l, mid, ql, qr, mul, add, mod);
            Run(tree, lazyMul, lazyAdd, node * 2 + 1, mid + 1, r, ql, qr, mul, add, mod);
            tree[node] = (tree[node * 2] + tree[node * 2 + 1]) % mod;
        }
    }

    public static unsafe class RangeAffineQuery
    {
        public static long Run(long* tree, long* lazyMul, long* lazyAdd, int node, int l, int r, int ql, int qr, long mod)
        {
            if (qr < l || ql > r) return 0;
            if (ql <= l && r <= qr) return tree[node];
            int mid = (l + r) >> 1;
            long left = Run(tree, lazyMul, lazyAdd, node * 2, l, mid, ql, qr, mod);
            long right = Run(tree, lazyMul, lazyAdd, node * 2 + 1, mid + 1, r, ql, qr, mod);
            return (left + right) % mod;
        }
    }

    public static unsafe class RangeModuloUpdate
    {
        public static void Run(long* tree, int node, int l, int r, int ql, int qr, long mod)
        {
            if (qr < l || ql > r) return;
            if (l == r) { tree[node] %= mod; return; }
            int mid = (l + r) >> 1;
            Run(tree, node * 2, l, mid, ql, qr, mod);
            Run(tree, node * 2 + 1, mid + 1, r, ql, qr, mod);
            tree[node] = Math.Min(tree[node * 2], tree[node * 2 + 1]);
        }
    }

    public static unsafe class RangeAssignUpdate
    {
        public static void Run(long* tree, long* lazy, bool* hasLazy, int node, int l, int r, int ql, int qr, long val)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr)
            {
                tree[node] = val * (r - l + 1);
                lazy[node * 2] = val;
                lazy[node * 2 + 1] = val;
                hasLazy[node * 2] = true;
                hasLazy[node * 2 + 1] = true;
                hasLazy[node] = false;
                return;
            }
            int mid = (l + r) >> 1;
            if (hasLazy[node] && l != r)
            {
                tree[node * 2] = lazy[node] * (mid - l + 1);
                tree[node * 2 + 1] = lazy[node] * (r - mid);
                lazy[node * 2] = lazy[node];
                lazy[node * 2 + 1] = lazy[node];
                hasLazy[node * 2] = true;
                hasLazy[node * 2 + 1] = true;
                hasLazy[node] = false;
            }
            Run(tree, lazy, hasLazy, node * 2, l, mid, ql, qr, val);
            Run(tree, lazy, hasLazy, node * 2 + 1, mid + 1, r, ql, qr, val);
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }

        public static void RunSetInt32(int* tree, int* lazy, bool* hasLazy, int node, int l, int r, int ql, int qr, int val)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr)
            {
                tree[node] = val * (r - l + 1);
                lazy[node * 2] = val;
                lazy[node * 2 + 1] = val;
                hasLazy[node * 2] = true;
                hasLazy[node * 2 + 1] = true;
                hasLazy[node] = false;
                return;
            }
            int mid = (l + r) >> 1;
            if (hasLazy[node])
            {
                tree[node * 2] = lazy[node] * (mid - l + 1);
                tree[node * 2 + 1] = lazy[node] * (r - mid);
                lazy[node * 2] = lazy[node];
                lazy[node * 2 + 1] = lazy[node];
                hasLazy[node * 2] = true;
                hasLazy[node * 2 + 1] = true;
                hasLazy[node] = false;
            }
            RunSetInt32(tree, lazy, hasLazy, node * 2, l, mid, ql, qr, val);
            RunSetInt32(tree, lazy, hasLazy, node * 2 + 1, mid + 1, r, ql, qr, val);
            tree[node] = tree[node * 2] + tree[node * 2 + 1];
        }
    }

    public static unsafe class RangeMajorityQuery
    {
        public static int Run(int* arr, int n, int l, int r)
        {
            return BoyerMoore.Run(arr, l, r - l + 1);
        }

        public static class BoyerMoore
        {
            public static int Run(int* arr, int start, int len)
            {
                int candidate = 0, count = 0;
                for (int i = 0; i < len; i++)
                {
                    int val = arr[start + i];
                    if (count == 0) { candidate = val; count = 1; }
                    else if (val == candidate) count++;
                    else count--;
                }
                count = 0;
                for (int i = 0; i < len; i++)
                    if (arr[start + i] == candidate) count++;
                return count > (len >> 1) ? candidate : -1;
            }
        }
    }

    public static unsafe class RangeKthSmallest
    {
        public static int Run(int* sorted, int n, int l, int r, int k)
        {
            if (k < 1 || k > r - l + 1) return -1;
            return sorted[l + k - 1];
        }
    }

    public static unsafe class RangeKthLargest
    {
        public static int Run(int* sorted, int n, int l, int r, int k)
        {
            if (k < 1 || k > r - l + 1) return -1;
            return sorted[r - k + 1];
        }
    }

    public static unsafe class RangeMedianQuery
    {
        public static int Run(int* sorted, int n, int l, int r)
        {
            int len = r - l + 1;
            return sorted[l + (len >> 1)];
        }

        public static int RunInt64(long* sorted, int n, int l, int r)
        {
            int len = r - l + 1;
            return (int)sorted[l + (len >> 1)];
        }
    }

    public static unsafe class RangeInversionQuery
    {
        public static long Run(int* arr, int n, int l, int r)
        {
            long inv = 0;
            for (int i = l; i <= r; i++)
            {
                for (int j = i + 1; j <= r; j++)
                {
                    if (arr[i] > arr[j]) inv++;
                }
            }
            return inv;
        }

        public static long RunFenwick(int* arr, int n, int l, int r, int* bit)
        {
            int len = r - l + 1;
            long* compressed = stackalloc long[len];
            int* sorted = stackalloc int[len];
            for (int i = 0; i < len; i++) sorted[i] = arr[l + i];
            for (int i = 1; i < len; i++)
            {
                int key = sorted[i], j = i - 1;
                while (j >= 0 && sorted[j] > key) { sorted[j + 1] = sorted[j]; j--; }
                sorted[j + 1] = key;
            }
            long inv = 0;
            for (int i = 0; i < len; i++)
            {
                int rank = 1;
                for (int j = 0; j < len; j++)
                    if (sorted[j] < sorted[i] || (sorted[j] == sorted[i] && j < i)) rank++;
                for (int j = rank; j <= len; j++) inv += bit[j];
                for (int j = rank; j <= len; j++) bit[j]++;
            }
            return inv;
        }
    }
}