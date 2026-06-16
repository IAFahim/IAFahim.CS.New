namespace IAFahim.DS.SegmentTree
{
    using System.Runtime.CompilerServices;

    public static unsafe class PersistentLazySegmentUpdate
    {
        public static int Run(int prev, int lo, int hi, int ql, int qr, long val,
            int* leftChild, int* rightChild, long* sumArr, long* lazyArr, int* cntArr, int* allocCount)
        {
            int node = ++(*allocCount);
            if (prev != 0)
            {
                leftChild[node] = leftChild[prev];
                rightChild[node] = rightChild[prev];
                sumArr[node] = sumArr[prev];
                lazyArr[node] = lazyArr[prev];
                cntArr[node] = cntArr[prev];
            }
            else
            {
                leftChild[node] = 0;
                rightChild[node] = 0;
                sumArr[node] = 0;
                lazyArr[node] = 0;
                cntArr[node] = 0;
            }

            if (ql <= lo && hi <= qr)
            {
                lazyArr[node] += val;
                sumArr[node] += val * cntArr[node];
                return node;
            }

            int mid = lo + ((hi - lo) >> 1);
            if (ql <= mid)
                leftChild[node] = Run(leftChild[node], lo, mid, ql, qr, val,
                    leftChild, rightChild, sumArr, lazyArr, cntArr, allocCount);
            if (qr > mid)
                rightChild[node] = Run(rightChild[node], mid + 1, hi, ql, qr, val,
                    leftChild, rightChild, sumArr, lazyArr, cntArr, allocCount);

            sumArr[node] = sumArr[leftChild[node]] + sumArr[rightChild[node]] + lazyArr[node] * cntArr[node];
            return node;
        }
    }

    public static unsafe class PersistentLazySegmentQuery
    {
        public static long Run(int node, int lo, int hi, int ql, int qr, long inherited,
            int* leftChild, int* rightChild, long* sumArr, long* lazyArr, int* cntArr)
        {
            if (node == 0 || qr < lo || hi < ql) return 0;
            if (ql <= lo && hi <= qr)
            {
                int cnt = cntArr[node];
                return sumArr[node] + inherited * cnt;
            }
            long lazy = inherited + lazyArr[node];
            int mid = lo + ((hi - lo) >> 1);
            return Run(leftChild[node], lo, mid, ql, qr, lazy, leftChild, rightChild, sumArr, lazyArr, cntArr) +
                   Run(rightChild[node], mid + 1, hi, ql, qr, lazy, leftChild, rightChild, sumArr, lazyArr, cntArr);
        }
    }
}
