namespace IAFahim.DS.SegmentTree
{
    using System.Runtime.CompilerServices;

    public static unsafe class MergeSortTreeBuild
    {
        public static void Run(int* arr, int n, int** nodes, int* sizes, int* pool, ref int poolSize)
        {
            Build(arr, n, 0, 0, n - 1, nodes, sizes, pool, ref poolSize);
        }

        private static void Build(int* arr, int n, int node, int l, int r,
            int** nodes, int* sizes, int* pool, ref int poolSize)
        {
            int len = r - l + 1;
            nodes[node] = pool + poolSize;
            sizes[node] = len;
            poolSize += len;

            for (int i = l; i <= r; i++) nodes[node][i - l] = arr[i];
            Sort(nodes[node], len);

            if (l == r) return;
            int mid = (l + r) >> 1;
            Build(arr, n, 2 * node + 1, l, mid, nodes, sizes, pool, ref poolSize);
            Build(arr, n, 2 * node + 2, mid + 1, r, nodes, sizes, pool, ref poolSize);
        }

        private static void Sort(int* a, int n)
        {
            for (int i = 1; i < n; i++)
            {
                int key = a[i];
                int j = i - 1;
                while (j >= 0 && a[j] > key) { a[j + 1] = a[j]; j--; }
                a[j + 1] = key;
            }
        }
    }

    public static unsafe class MergeSortTreeKth
    {
        public static int Run(int* arr, int n, int ql, int qr, int k,
            int** nodes, int* sizes)
        {
            int lo = -1, hi = 1 << 30;
            while (lo < hi - 1)
            {
                int mid = lo + ((hi - lo) >> 1);
                if (CountLessEqual(0, 0, n - 1, ql, qr, mid, nodes, sizes) >= k)
                    hi = mid;
                else
                    lo = mid;
            }
            return hi;
        }

        private static int CountLessEqual(int node, int l, int r, int ql, int qr, int val,
            int** nodes, int* sizes)
        {
            if (qr < l || r < ql) return 0;
            if (ql <= l && r <= qr)
                return UpperBound(nodes[node], sizes[node], val);
            int mid = (l + r) >> 1;
            return CountLessEqual(2 * node + 1, l, mid, ql, qr, val, nodes, sizes) +
                   CountLessEqual(2 * node + 2, mid + 1, r, ql, qr, val, nodes, sizes);
        }

        private static int UpperBound(int* a, int n, int val)
        {
            int lo = 0, hi = n;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (a[mid] <= val) lo = mid + 1;
                else hi = mid;
            }
            return lo;
        }
    }

    public static unsafe class MergeSortTreeCountLess
    {
        public static int Run(int* arr, int n, int ql, int qr, int val,
            int** nodes, int* sizes)
        {
            return Count(0, 0, n - 1, ql, qr, val, nodes, sizes);
        }

        private static int Count(int node, int l, int r, int ql, int qr, int val,
            int** nodes, int* sizes)
        {
            if (qr < l || r < ql) return 0;
            if (ql <= l && r <= qr)
                return LowerBound(nodes[node], sizes[node], val);
            int mid = (l + r) >> 1;
            return Count(2 * node + 1, l, mid, ql, qr, val, nodes, sizes) +
                   Count(2 * node + 2, mid + 1, r, ql, qr, val, nodes, sizes);
        }

        private static int LowerBound(int* a, int n, int val)
        {
            int lo = 0, hi = n;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (a[mid] < val) lo = mid + 1;
                else hi = mid;
            }
            return lo;
        }
    }

    public static unsafe class MergeSortTreeCountGreater
    {
        public static int Run(int* arr, int n, int ql, int qr, int val,
            int** nodes, int* sizes)
        {
            return Count(0, 0, n - 1, ql, qr, val, nodes, sizes);
        }

        private static int Count(int node, int l, int r, int ql, int qr, int val,
            int** nodes, int* sizes)
        {
            if (qr < l || r < ql) return 0;
            if (ql <= l && r <= qr)
                return sizes[node] - UpperBound(nodes[node], sizes[node], val);
            int mid = (l + r) >> 1;
            return Count(2 * node + 1, l, mid, ql, qr, val, nodes, sizes) +
                   Count(2 * node + 2, mid + 1, r, ql, qr, val, nodes, sizes);
        }

        private static int UpperBound(int* a, int n, int val)
        {
            int lo = 0, hi = n;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                if (a[mid] <= val) lo = mid + 1;
                else hi = mid;
            }
            return lo;
        }
    }

    public static unsafe class MergeSortTreePredecessor
    {
        public static bool TryRun(int* arr, int n, int ql, int qr, int val,
            int** nodes, int* sizes, out int result)
        {
            result = int.MinValue;
            bool found = false;
            Find(0, 0, n - 1, ql, qr, val, nodes, sizes, ref result, ref found);
            return found;
        }

        private static void Find(int node, int l, int r, int ql, int qr, int val,
            int** nodes, int* sizes, ref int result, ref bool found)
        {
            if (qr < l || r < ql) return;
            if (ql <= l && r <= qr)
            {
                int idx = LowerBound(nodes[node], sizes[node], val) - 1;
                if (idx >= 0) { if (!found || nodes[node][idx] > result) { result = nodes[node][idx]; found = true; } }
                return;
            }
            int mid = (l + r) >> 1;
            Find(2 * node + 1, l, mid, ql, qr, val, nodes, sizes, ref result, ref found);
            Find(2 * node + 2, mid + 1, r, ql, qr, val, nodes, sizes, ref result, ref found);
        }

        private static int LowerBound(int* a, int n, int val)
        {
            int lo = 0, hi = n;
            while (lo < hi) { int mid = (lo + hi) >> 1; if (a[mid] < val) lo = mid + 1; else hi = mid; }
            return lo;
        }
    }

    public static unsafe class MergeSortTreeSuccessor
    {
        public static bool TryRun(int* arr, int n, int ql, int qr, int val,
            int** nodes, int* sizes, out int result)
        {
            result = int.MaxValue;
            bool found = false;
            Find(0, 0, n - 1, ql, qr, val, nodes, sizes, ref result, ref found);
            return found;
        }

        private static void Find(int node, int l, int r, int ql, int qr, int val,
            int** nodes, int* sizes, ref int result, ref bool found)
        {
            if (qr < l || r < ql) return;
            if (ql <= l && r <= qr)
            {
                int idx = UpperBound(nodes[node], sizes[node], val);
                if (idx < sizes[node]) { if (!found || nodes[node][idx] < result) { result = nodes[node][idx]; found = true; } }
                return;
            }
            int mid = (l + r) >> 1;
            Find(2 * node + 1, l, mid, ql, qr, val, nodes, sizes, ref result, ref found);
            Find(2 * node + 2, mid + 1, r, ql, qr, val, nodes, sizes, ref result, ref found);
        }

        private static int UpperBound(int* a, int n, int val)
        {
            int lo = 0, hi = n;
            while (lo < hi) { int mid = (lo + hi) >> 1; if (a[mid] <= val) lo = mid + 1; else hi = mid; }
            return lo;
        }
    }
}
