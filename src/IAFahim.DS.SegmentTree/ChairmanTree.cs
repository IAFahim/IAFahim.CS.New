namespace IAFahim.DS.SegmentTree
{
    using System.Runtime.CompilerServices;

    public static unsafe class ChairmanTreeBuild
    {
        public static int Run(int* arr, int n, int lo, int hi,
            int* roots, int* leftChild, int* rightChild, int* cnt, int* allocCount)
        {
            roots[0] = 0;
            for (int i = 0; i < n; i++)
            {
                roots[i + 1] = ChairmanTreeUpdate.Run(
                    roots[i], lo, hi, arr[i], 1,
                    leftChild, rightChild, cnt, allocCount);
            }
            return *allocCount;
        }
    }

    public static unsafe class ChairmanTreeUpdate
    {
        public static int Run(int prev, int lo, int hi, int idx, int val,
            int* leftChild, int* rightChild, int* cnt, int* allocCount)
        {
            int node = ++(*allocCount);
            leftChild[node] = prev != 0 ? leftChild[prev] : 0;
            rightChild[node] = prev != 0 ? rightChild[prev] : 0;
            cnt[node] = (prev != 0 ? cnt[prev] : 0) + val;

            if (lo == hi) return node;

            int mid = lo + ((hi - lo) >> 1);
            if (idx <= mid)
                leftChild[node] = Run(prev != 0 ? leftChild[prev] : 0, lo, mid, idx, val,
                    leftChild, rightChild, cnt, allocCount);
            else
                rightChild[node] = Run(prev != 0 ? rightChild[prev] : 0, mid + 1, hi, idx, val,
                    leftChild, rightChild, cnt, allocCount);

            return node;
        }
    }

    public static unsafe class ChairmanTreeKth
    {
        public static int Run(int lRoot, int rRoot, int lo, int hi, int k,
            int* leftChild, int* rightChild, int* cnt)
        {
            if (lo == hi) return lo;
            int mid = lo + ((hi - lo) >> 1);
            int leftCnt = cnt[leftChild[rRoot]] - cnt[leftChild[lRoot]];
            if (k <= leftCnt)
                return Run(leftChild[lRoot], leftChild[rRoot], lo, mid, k, leftChild, rightChild, cnt);
            return Run(rightChild[lRoot], rightChild[rRoot], mid + 1, hi, k - leftCnt, leftChild, rightChild, cnt);
        }
    }

    public static unsafe class ChairmanTreeCount
    {
        public static int Run(int lRoot, int rRoot, int lo, int hi, int ql, int qr,
            int* leftChild, int* rightChild, int* cnt)
        {
            if (qr < lo || hi < ql) return 0;
            if (ql <= lo && hi <= qr) return cnt[rRoot] - cnt[lRoot];
            int mid = lo + ((hi - lo) >> 1);
            return Run(leftChild[lRoot], leftChild[rRoot], lo, mid, ql, qr, leftChild, rightChild, cnt) +
                   Run(rightChild[lRoot], rightChild[rRoot], mid + 1, hi, ql, qr, leftChild, rightChild, cnt);
        }
    }

    public static unsafe class ChairmanTreeSum
    {
        public static long Run(int lRoot, int rRoot, int lo, int hi, int ql, int qr,
            int* leftChild, int* rightChild, long* sumArr)
        {
            if (qr < lo || hi < ql) return 0;
            if (ql <= lo && hi <= qr) return sumArr[rRoot] - sumArr[lRoot];
            int mid = lo + ((hi - lo) >> 1);
            return Run(leftChild[lRoot], leftChild[rRoot], lo, mid, ql, qr, leftChild, rightChild, sumArr) +
                   Run(rightChild[lRoot], rightChild[rRoot], mid + 1, hi, ql, qr, leftChild, rightChild, sumArr);
        }
    }
}
