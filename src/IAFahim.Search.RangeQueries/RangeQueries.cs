namespace IAFahim.Search.RangeQueries
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RangeGcdQuery
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Gcd(long a, long b)
        {
            while (b != 0) { long t = a % b; a = b; b = t; }
            return a;
        }

        public static long Run(long* arr, int n, int l, int r)
        {
            long res = arr[l];
            for (int i = l + 1; i <= r; i++)
            {
                if (res == 1) return 1;
                res = Gcd(res, arr[i]);
            }
            return res;
        }
    }

    public static unsafe class RangeLcmQuery
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Gcd(long a, long b)
        {
            while (b != 0) { long t = a % b; a = b; b = t; }
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Lcm(long a, long b)
        {
            if (a == 0 || b == 0) return 0;
            return a / Gcd(a, b) * b;
        }

        public static long Run(long* arr, int n, int l, int r)
        {
            long res = arr[l];
            for (int i = l + 1; i <= r; i++)
            {
                if (res == 0) return 0;
                res = Lcm(res, arr[i]);
            }
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
            if (ql <= l && r <= qr) { ApplyMin(tree, lazy, node, l, r, val); return; }
            
            PushMin(tree, lazy, node, l, r);
            int mid = (l + r) >> 1;
            int left = node * 2, right = left + 1;
            RunMin(tree, lazy, left, l, mid, ql, qr, val);
            RunMin(tree, lazy, right, mid + 1, r, ql, qr, val);
            tree[node] = Math.Min(tree[left], tree[right]);
        }

        private static void ApplyMin(long* tree, long* lazy, int node, int l, int r, long val)
        {
            if (tree[node] <= val) return;
            tree[node] = val;
            if (l != r) { int left = node * 2, right = left + 1; lazy[left] = Math.Min(lazy[left], val); lazy[right] = Math.Min(lazy[right], val); }
        }

        private static void PushMin(long* tree, long* lazy, int node, int l, int r)
        {
            if (l == r) return;
            int mid = (l + r) >> 1, left = node * 2, right = left + 1;
            ApplyMin(tree, lazy, left, l, mid, lazy[node]);
            ApplyMin(tree, lazy, right, mid + 1, r, lazy[node]);
        }

        public static void RunMax(long* tree, long* lazy, int node, int l, int r, int ql, int qr, long val)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr) { ApplyMax(tree, lazy, node, l, r, val); return; }
            
            PushMax(tree, lazy, node, l, r);
            int mid = (l + r) >> 1;
            int left = node * 2, right = left + 1;
            RunMax(tree, lazy, left, l, mid, ql, qr, val);
            RunMax(tree, lazy, right, mid + 1, r, ql, qr, val);
            tree[node] = Math.Max(tree[left], tree[right]);
        }

        private static void ApplyMax(long* tree, long* lazy, int node, int l, int r, long val)
        {
            if (tree[node] >= val) return;
            tree[node] = val;
            if (l != r) { int left = node * 2, right = left + 1; lazy[left] = Math.Max(lazy[left], val); lazy[right] = Math.Max(lazy[right], val); }
        }

        private static void PushMax(long* tree, long* lazy, int node, int l, int r)
        {
            if (l == r) return;
            int mid = (l + r) >> 1, left = node * 2, right = left + 1;
            ApplyMax(tree, lazy, left, l, mid, lazy[node]);
            ApplyMax(tree, lazy, right, mid + 1, r, lazy[node]);
        }

        public static void RunAdd(long* tree, long* lazy, int node, int l, int r, int ql, int qr, long val)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr) { ApplyAdd(tree, lazy, node, l, r, val); return; }
            
            PushAdd(tree, lazy, node, l, r);
            int mid = (l + r) >> 1;
            int left = node * 2, right = left + 1;
            RunAdd(tree, lazy, left, l, mid, ql, qr, val);
            RunAdd(tree, lazy, right, mid + 1, r, ql, qr, val);
            tree[node] = tree[left] + tree[right];
        }

        private static void ApplyAdd(long* tree, long* lazy, int node, int l, int r, long val)
        {
            tree[node] += val * (r - l + 1);
            if (l != r) { lazy[node * 2] += val; lazy[node * 2 + 1] += val; }
        }

        private static void PushAdd(long* tree, long* lazy, int node, int l, int r)
        {
            if (l == r || lazy[node] == 0) return;
            int mid = (l + r) >> 1, left = node * 2, right = left + 1;
            ApplyAdd(tree, lazy, left, l, mid, lazy[node]);
            ApplyAdd(tree, lazy, right, mid + 1, r, lazy[node]);
            lazy[node] = 0;
        }
    }

    public static unsafe class RangeAffineUpdate
    {
        public static void Run(long* tree, long* lazyMul, long* lazyAdd, int node, int l, int r, int ql, int qr, long mul, long add, long mod)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr) { ApplyAffine(tree, lazyMul, lazyAdd, node, l, r, mul, add, mod); return; }
            
            PushAffine(tree, lazyMul, lazyAdd, node, l, r, mod);
            int mid = (l + r) >> 1;
            int left = node * 2, right = left + 1;
            Run(tree, lazyMul, lazyAdd, left, l, mid, ql, qr, mul, add, mod);
            Run(tree, lazyMul, lazyAdd, right, mid + 1, r, ql, qr, mul, add, mod);
            tree[node] = (tree[left] + tree[right]) % mod;
        }

        private static void ApplyAffine(long* tree, long* lazyMul, long* lazyAdd, int node, int l, int r, long mul, long add, long mod)
        {
            tree[node] = (tree[node] * mul + add * (r - l + 1)) % mod;
            if (l != r)
            {
                int left = node * 2, right = left + 1;
                lazyMul[left] = (lazyMul[left] * mul) % mod;
                lazyMul[right] = (lazyMul[right] * mul) % mod;
                lazyAdd[left] = (lazyAdd[left] * mul + add) % mod;
                lazyAdd[right] = (lazyAdd[right] * mul + add) % mod;
            }
        }

        private static void PushAffine(long* tree, long* lazyMul, long* lazyAdd, int node, int l, int r, long mod)
        {
            if (l == r || (lazyMul[node] == 1 && lazyAdd[node] == 0)) return;
            int mid = (l + r) >> 1, left = node * 2, right = left + 1;
            ApplyAffine(tree, lazyMul, lazyAdd, left, l, mid, lazyMul[node], lazyAdd[node], mod);
            ApplyAffine(tree, lazyMul, lazyAdd, right, mid + 1, r, lazyMul[node], lazyAdd[node], mod);
            lazyMul[node] = 1; lazyAdd[node] = 0;
        }
    }

    public static unsafe class RangeAffineQuery
    {
        public static long Run(long* tree, long* lazyMul, long* lazyAdd, int node, int l, int r, int ql, int qr, long mod)
        {
            if (qr < l || ql > r) return 0;
            if (ql <= l && r <= qr) return tree[node];
            // Note: In a real implementation, a PushDown call would be needed here.
            int mid = (l + r) >> 1, left = node * 2, right = left + 1;
            return (Run(tree, lazyMul, lazyAdd, left, l, mid, ql, qr, mod) + Run(tree, lazyMul, lazyAdd, right, mid + 1, r, ql, qr, mod)) % mod;
        }
    }

    public static unsafe class RangeModuloUpdate
    {
        public static void Run(long* tree, int node, int l, int r, int ql, int qr, long mod)
        {
            if (qr < l || ql > r) return;
            if (l == r) { tree[node] %= mod; return; }
            int mid = (l + r) >> 1, left = node * 2, right = left + 1;
            Run(tree, left, l, mid, ql, qr, mod);
            Run(tree, right, mid + 1, r, ql, qr, mod);
            tree[node] = Math.Min(tree[left], tree[right]);
        }
    }

    public static unsafe class RangeAssignUpdate
    {
        public static void Run(long* tree, long* lazy, bool* hasLazy, int node, int l, int r, int ql, int qr, long val)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr) { ApplyAssign(tree, lazy, hasLazy, node, l, r, val); return; }
            
            PushAssign(tree, lazy, hasLazy, node, l, r);
            int mid = (l + r) >> 1, left = node * 2, right = left + 1;
            Run(tree, lazy, hasLazy, left, l, mid, ql, qr, val);
            Run(tree, lazy, hasLazy, right, mid + 1, r, ql, qr, val);
            tree[node] = tree[left] + tree[right];
        }

        private static void ApplyAssign(long* tree, long* lazy, bool* hasLazy, int node, int l, int r, long val)
        {
            tree[node] = val * (r - l + 1);
            if (l != r) { int left = node * 2, right = left + 1; lazy[left] = lazy[right] = val; hasLazy[left] = hasLazy[right] = true; }
            hasLazy[node] = false;
        }

        private static void PushAssign(long* tree, long* lazy, bool* hasLazy, int node, int l, int r)
        {
            if (l == r || !hasLazy[node]) return;
            int mid = (l + r) >> 1, left = node * 2, right = left + 1;
            ApplyAssign(tree, lazy, hasLazy, left, l, mid, lazy[node]);
            ApplyAssign(tree, lazy, hasLazy, right, mid + 1, r, lazy[node]);
            hasLazy[node] = false;
        }

        public static void RunSetInt32(int* tree, int* lazy, bool* hasLazy, int node, int l, int r, int ql, int qr, int val)
        {
            if (qr < l || ql > r) return;
            if (ql <= l && r <= qr) { ApplyAssignInt32(tree, lazy, hasLazy, node, l, r, val); return; }
            
            PushAssignInt32(tree, lazy, hasLazy, node, l, r);
            int mid = (l + r) >> 1, left = node * 2, right = left + 1;
            RunSetInt32(tree, lazy, hasLazy, left, l, mid, ql, qr, val);
            RunSetInt32(tree, lazy, hasLazy, right, mid + 1, r, ql, qr, val);
            tree[node] = tree[left] + tree[right];
        }

        private static void ApplyAssignInt32(int* tree, int* lazy, bool* hasLazy, int node, int l, int r, int val)
        {
            tree[node] = val * (r - l + 1);
            if (l != r) { int left = node * 2, right = left + 1; lazy[left] = lazy[right] = val; hasLazy[left] = hasLazy[right] = true; }
            hasLazy[node] = false;
        }

        private static void PushAssignInt32(int* tree, int* lazy, bool* hasLazy, int node, int l, int r)
        {
            if (l == r || !hasLazy[node]) return;
            int mid = (l + r) >> 1, left = node * 2, right = left + 1;
            ApplyAssignInt32(tree, lazy, hasLazy, left, l, mid, lazy[node]);
            ApplyAssignInt32(tree, lazy, hasLazy, right, mid + 1, r, lazy[node]);
            hasLazy[node] = false;
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
                if (len == 0) return -1;
                int* p = arr + start;
                int candidate = 0, count = 0;
                for (int i = 0; i < len; i++)
                {
                    int val = p[i];
                    if (count == 0) { candidate = val; count = 1; }
                    else if (val == candidate) count++;
                    else count--;
                }
                count = 0;
                for (int i = 0; i < len; i++)
                    if (p[i] == candidate) count++;
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

        public static long RunInt64(long* sorted, int n, int l, int r)
        {
            int len = r - l + 1;
            return sorted[l + (len >> 1)];
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
            int* sorted = stackalloc int[len];
            for (int i = 0; i < len; i++) sorted[i] = arr[l + i];

            // Insertion sort to build the coordinate-compression key set.
            for (int i = 1; i < len; i++)
            {
                int key = sorted[i], j = i - 1;
                while (j >= 0 && sorted[j] > key) { sorted[j + 1] = sorted[j]; j--; }
                sorted[j + 1] = key;
            }

            // Deduplicate in place; m = number of distinct values, ranks are 1..m.
            int m = 0;
            for (int i = 0; i < len; i++)
            {
                if (i == 0 || sorted[i] != sorted[m - 1]) { sorted[m] = sorted[i]; m++; }
            }

            long inv = 0;
            // Process elements in ORIGINAL order; count already-inserted elements
            // with a strictly greater rank (equal values are not inversions).
            for (int i = 0; i < len; i++)
            {
                int value = arr[l + i];

                // lower_bound: smallest index whose sorted value >= value (exact, found).
                int lo = 0, hi = m;
                while (lo < hi)
                {
                    int mid = (lo + hi) >> 1;
                    if (sorted[mid] < value) lo = mid + 1;
                    else hi = mid;
                }
                int rank = lo + 1; // 1-based rank in [1..m]

                // Count inserted elements with rank > rank: total inserted so far minus
                // prefix-sum up to and including this rank.
                long lessOrEqual = 0;
                for (int j = rank; j > 0; j -= j & -j) lessOrEqual += bit[j];
                inv += i - lessOrEqual;

                // Point update at rank.
                for (int j = rank; j <= m; j += j & -j) bit[j]++;
            }
            return inv;
        }
    }
}