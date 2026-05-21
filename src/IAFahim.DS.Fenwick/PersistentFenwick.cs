namespace IAFahim.DS.Fenwick
{
    using System.Runtime.CompilerServices;

    public static unsafe class PersistentFenwickUpdate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* leftChild, int* rightChild, int* sumArr, int* allocCount,
            int prev, int l, int r, int idx, int val)
        {
            int node = ++(*allocCount);
            leftChild[node] = prev != 0 ? leftChild[prev] : 0;
            rightChild[node] = prev != 0 ? rightChild[prev] : 0;
            sumArr[node] = (prev != 0 ? sumArr[prev] : 0) + val;

            if (l == r) return node;

            int mid = (l + r) >> 1;
            if (idx <= mid)
                leftChild[node] = Run(leftChild, rightChild, sumArr, allocCount,
                    prev != 0 ? leftChild[prev] : 0, l, mid, idx, val);
            else
                rightChild[node] = Run(leftChild, rightChild, sumArr, allocCount,
                    prev != 0 ? rightChild[prev] : 0, mid + 1, r, idx, val);

            return node;
        }
    }

    public static unsafe class PersistentFenwickQuery
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* leftChild, int* rightChild, int* sumArr,
            int node, int l, int r, int ql, int qr)
        {
            if (node == 0 || qr < l || ql > r) return 0;
            if (ql <= l && r <= qr) return sumArr[node];
            int mid = (l + r) >> 1;
            return Run(leftChild, rightChild, sumArr, leftChild[node], l, mid, ql, qr) +
                   Run(leftChild, rightChild, sumArr, rightChild[node], mid + 1, r, ql, qr);
        }
    }
}
