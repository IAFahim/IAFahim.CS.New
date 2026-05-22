namespace IAFahim.DS.SegmentTree
{
    using System.Runtime.CompilerServices;

    public static unsafe class ChairmanTreeBuild
    {
        public static int Run(int* arr, int n, int lo, int hi,
            int* roots, int* leftChild, int* rightChild, int* cnt, long* sumArr, int* allocCount)
        {
            roots[0] = 0;
            for (int i = 0; i < n; i++)
            {
                roots[i + 1] = ChairmanTreeUpdate.Run(
                    roots[i], lo, hi, arr[i], 1, arr[i],
                    leftChild, rightChild, cnt, sumArr, allocCount);
            }
            return *allocCount;
        }
    }

    public static unsafe class ChairmanTreeUpdate
    {
        public static int Run(int prev, int lo, int hi, int idx, int addCount, long addSum,
            int* leftChild, int* rightChild, int* cnt, long* sumArr, int* allocCount)
        {
            int node = ++(*allocCount);
            leftChild[node] = prev != 0 ? leftChild[prev] : 0;
            rightChild[node] = prev != 0 ? rightChild[prev] : 0;
            cnt[node] = (prev != 0 ? cnt[prev] : 0) + addCount;
            sumArr[node] = (prev != 0 ? sumArr[prev] : 0) + addSum;

            if (lo == hi) return node;

            int mid = lo + ((hi - lo) >> 1);
            if (idx <= mid)
                leftChild[node] = Run(prev != 0 ? leftChild[prev] : 0, lo, mid, idx, addCount, addSum,
                    leftChild, rightChild, cnt, sumArr, allocCount);
            else
                rightChild[node] = Run(prev != 0 ? rightChild[prev] : 0, mid + 1, hi, idx, addCount, addSum,
                    leftChild, rightChild, cnt, sumArr, allocCount);

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
            int lCnt = lRoot != 0 ? cnt[leftChild[lRoot]] : 0;
            int rCnt = rRoot != 0 ? cnt[leftChild[rRoot]] : 0;
            int leftCount = rCnt - lCnt;
            if (k <= leftCount)
                return Run(lRoot != 0 ? leftChild[lRoot] : 0, rRoot != 0 ? leftChild[rRoot] : 0, lo, mid, k, leftChild, rightChild, cnt);
            return Run(lRoot != 0 ? rightChild[lRoot] : 0, rRoot != 0 ? rightChild[rRoot] : 0, mid + 1, hi, k - leftCount, leftChild, rightChild, cnt);
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
            return Run(lRoot != 0 ? leftChild[lRoot] : 0, rRoot != 0 ? leftChild[rRoot] : 0, lo, mid, ql, qr, leftChild, rightChild, cnt) +
                   Run(lRoot != 0 ? rightChild[lRoot] : 0, rRoot != 0 ? rightChild[rRoot] : 0, mid + 1, hi, ql, qr, leftChild, rightChild, cnt);
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
            return Run(lRoot != 0 ? leftChild[lRoot] : 0, rRoot != 0 ? leftChild[rRoot] : 0, lo, mid, ql, qr, leftChild, rightChild, sumArr) +
                   Run(lRoot != 0 ? rightChild[lRoot] : 0, rRoot != 0 ? rightChild[rRoot] : 0, mid + 1, hi, ql, qr, leftChild, rightChild, sumArr);
        }
    }
}
