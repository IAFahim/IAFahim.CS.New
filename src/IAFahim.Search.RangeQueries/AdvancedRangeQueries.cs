namespace IAFahim.Search.RangeQueries
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class RangeSuccessorQuery
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* arr, int n, int l, int r, long key)
        {
            long best = long.MaxValue;
            for (int i = l; i <= r; i++)
            {
                long v = arr[i];
                if (v >= key && v < best) best = v;
            }
            return best == long.MaxValue ? long.MinValue : best;
        }
    }

    public static unsafe class RangePredecessorQuery
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* arr, int n, int l, int r, long key)
        {
            long best = long.MinValue;
            for (int i = l; i <= r; i++)
            {
                long v = arr[i];
                if (v <= key && v > best) best = v;
            }
            return best == long.MinValue ? long.MaxValue : best;
        }
    }

    public static unsafe class RangeDistinctCount
    {
        public static int Run(int* arr, int n, int l, int r)
        {
            if (l > r) return 0;
            int len = r - l + 1;
            int* buf = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            for (int i = 0; i < len; i++) buf[i] = arr[l + i];
            HeapSortInt(buf, len);
            int count = 1;
            for (int i = 1; i < len; i++)
                if (buf[i] != buf[i - 1]) count++;
            Marshal.FreeHGlobal((IntPtr)buf);
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HeapSortInt(int* a, int len)
        {
            for (int i = (len >> 1) - 1; i >= 0; i--) SiftDownInt(a, i, len);
            for (int i = len - 1; i > 0; i--)
            {
                int t = a[0]; a[0] = a[i]; a[i] = t;
                SiftDownInt(a, 0, i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SiftDownInt(int* a, int i, int len)
        {
            int half = len >> 1;
            while (i < half)
            {
                int child = (i << 1) + 1;
                int right = child + 1;
                if (right < len && a[right] > a[child]) child = right;
                if (a[child] <= a[i]) break;
                int t = a[i]; a[i] = a[child]; a[child] = t;
                i = child;
            }
        }
    }

    public static unsafe class RangeChminChmaxSum
    {
        public struct Node
        {
            public long Sum;
            public long Mx;
            public long Smx;
            public long Mn;
            public long Smn;
            public int Mxcnt;
            public int Mncnt;
        }

        private const long Neg = long.MinValue;
        private const long Pos = long.MaxValue;

        public static void Build(int* arr, int n, Node* nodes)
        {
            if (n <= 0) return;
            BuildRec(arr, nodes, 1, 0, n - 1);
        }

        private static void BuildRec(int* arr, Node* nodes, int ni, int l, int r)
        {
            if (l == r)
            {
                long v = arr[l];
                ref Node leaf = ref nodes[ni];
                leaf.Sum = v; leaf.Mx = v; leaf.Mn = v;
                leaf.Smx = Neg; leaf.Smn = Pos;
                leaf.Mxcnt = 1; leaf.Mncnt = 1;
                return;
            }
            int mid = (l + r) >> 1;
            int lc = ni << 1;
            int rc = lc | 1;
            BuildRec(arr, nodes, lc, l, mid);
            BuildRec(arr, nodes, rc, mid + 1, r);
            Pull(nodes, ni, lc, rc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Pull(Node* nodes, int ni, int lc, int rc)
        {
            ref Node p = ref nodes[ni];
            ref Node a = ref nodes[lc];
            ref Node b = ref nodes[rc];
            p.Sum = a.Sum + b.Sum;
            if (a.Mx == b.Mx)
            {
                p.Mx = a.Mx; p.Mxcnt = a.Mxcnt + b.Mxcnt; p.Smx = Math.Max(a.Smx, b.Smx);
            }
            else if (a.Mx > b.Mx)
            {
                p.Mx = a.Mx; p.Mxcnt = a.Mxcnt; p.Smx = Math.Max(a.Smx, b.Mx);
            }
            else
            {
                p.Mx = b.Mx; p.Mxcnt = b.Mxcnt; p.Smx = Math.Max(a.Mx, b.Smx);
            }
            if (a.Mn == b.Mn)
            {
                p.Mn = a.Mn; p.Mncnt = a.Mncnt + b.Mncnt; p.Smn = Math.Min(a.Smn, b.Smn);
            }
            else if (a.Mn < b.Mn)
            {
                p.Mn = a.Mn; p.Mncnt = a.Mncnt; p.Smn = Math.Min(a.Smn, b.Mn);
            }
            else
            {
                p.Mn = b.Mn; p.Mncnt = b.Mncnt; p.Smn = Math.Min(a.Mn, b.Smn);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyChmin(Node* nodes, int ni, int len, long bound)
        {
            ref Node p = ref nodes[ni];
            long d = p.Mx - bound;
            p.Sum -= d * p.Mxcnt;
            if (p.Mx == p.Mn) p.Mn = bound;
            else if (p.Smn == p.Mx) p.Smn = bound;
            p.Mx = bound;
            if (p.Mx == p.Mn)
            {
                p.Smx = Neg; p.Smn = Pos;
                p.Mxcnt = len; p.Mncnt = len;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyChmax(Node* nodes, int ni, int len, long bound)
        {
            ref Node p = ref nodes[ni];
            long d = bound - p.Mn;
            p.Sum += d * p.Mncnt;
            if (p.Mn == p.Mx) p.Mx = bound;
            else if (p.Smx == p.Mn) p.Smx = bound;
            p.Mn = bound;
            if (p.Mn == p.Mx)
            {
                p.Smx = Neg; p.Smn = Pos;
                p.Mxcnt = len; p.Mncnt = len;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Pushdown(Node* nodes, int ni, int l, int r)
        {
            if (l == r) return;
            int mid = (l + r) >> 1;
            int lc = ni << 1;
            int rc = lc | 1;
            long pmx = nodes[ni].Mx;
            long pmn = nodes[ni].Mn;
            int leftLen = mid - l + 1;
            int rightLen = r - mid;
            if (nodes[lc].Mx > pmx) ApplyChmin(nodes, lc, leftLen, pmx);
            if (nodes[rc].Mx > pmx) ApplyChmin(nodes, rc, rightLen, pmx);
            if (nodes[lc].Mn < pmn) ApplyChmax(nodes, lc, leftLen, pmn);
            if (nodes[rc].Mn < pmn) ApplyChmax(nodes, rc, rightLen, pmn);
        }

        public static void Chmin(Node* nodes, int ni, int l, int r, int ql, int qr, long x)
        {
            ref Node p = ref nodes[ni];
            if (qr < l || ql > r || x >= p.Mx) return;
            if (ql <= l && r <= qr && p.Smx < x)
            {
                ApplyChmin(nodes, ni, r - l + 1, x);
                return;
            }
            Pushdown(nodes, ni, l, r);
            int mid = (l + r) >> 1;
            int lc = ni << 1;
            int rc = lc | 1;
            Chmin(nodes, lc, l, mid, ql, qr, x);
            Chmin(nodes, rc, mid + 1, r, ql, qr, x);
            Pull(nodes, ni, lc, rc);
        }

        public static void Chmax(Node* nodes, int ni, int l, int r, int ql, int qr, long x)
        {
            ref Node p = ref nodes[ni];
            if (qr < l || ql > r || x <= p.Mn) return;
            if (ql <= l && r <= qr && p.Smn > x)
            {
                ApplyChmax(nodes, ni, r - l + 1, x);
                return;
            }
            Pushdown(nodes, ni, l, r);
            int mid = (l + r) >> 1;
            int lc = ni << 1;
            int rc = lc | 1;
            Chmax(nodes, lc, l, mid, ql, qr, x);
            Chmax(nodes, rc, mid + 1, r, ql, qr, x);
            Pull(nodes, ni, lc, rc);
        }

        public static long QuerySum(Node* nodes, int ni, int l, int r, int ql, int qr)
        {
            if (qr < l || ql > r) return 0;
            if (ql <= l && r <= qr) return nodes[ni].Sum;
            Pushdown(nodes, ni, l, r);
            int mid = (l + r) >> 1;
            int lc = ni << 1;
            int rc = lc | 1;
            return QuerySum(nodes, lc, l, mid, ql, qr) + QuerySum(nodes, rc, mid + 1, r, ql, qr);
        }
    }
}
