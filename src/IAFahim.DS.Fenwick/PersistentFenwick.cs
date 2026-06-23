namespace IAFahim.DS.Fenwick
{
    using System.Runtime.CompilerServices;

    public static unsafe class PersistentFenwickUpdate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyNodeFromPrev(int* leftChild, int* rightChild, int* sumArr, int node, int curPrev, int val)
        {
            leftChild[node] = curPrev != 0 ? leftChild[curPrev] : 0;
            rightChild[node] = curPrev != 0 ? rightChild[curPrev] : 0;
            sumArr[node] = (curPrev != 0 ? sumArr[curPrev] : 0) + val;
        }

        public static int Run(int* leftChild, int* rightChild, int* sumArr, int* allocCount,
            int prev, int lIn, int rIn, int idx, int val)
        {
            int first = ++(*allocCount);
            int curPrev = prev;
            int l = lIn, r = rIn;
            int node = first;
            bool firstNode = true;
            while (true)
            {
                CopyNodeFromPrev(leftChild, rightChild, sumArr, node, curPrev, val);

                if (l == r)
                {
                    if (!firstNode) break;
                    return node;
                }

                int mid = l + ((r - l) >> 1);
                int child = ++(*allocCount);
                if (idx <= mid)
                {
                    leftChild[node] = child;
                    curPrev = curPrev != 0 ? leftChild[curPrev] : 0;
                    r = mid;
                }
                else
                {
                    rightChild[node] = child;
                    curPrev = curPrev != 0 ? rightChild[curPrev] : 0;
                    l = mid + 1;
                }
                node = child;
                firstNode = false;
            }
            return first;
        }
    }

    public static unsafe class PersistentFenwickQuery
    {
        public static int Run(int* leftChild, int* rightChild, int* sumArr,
            int node, int l, int r, int ql, int qr)
        {
            if (node == 0 || qr < l || ql > r) return 0;
            if (ql <= l && r <= qr) return sumArr[node];
            int mid = l + ((r - l) >> 1);
            return Run(leftChild, rightChild, sumArr, leftChild[node], l, mid, ql, qr) +
                   Run(leftChild, rightChild, sumArr, rightChild[node], mid + 1, r, ql, qr);
        }
    }
}
