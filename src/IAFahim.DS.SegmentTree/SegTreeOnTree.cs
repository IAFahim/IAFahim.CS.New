namespace IAFahim.DS.SegmentTree
{
    using System.Runtime.CompilerServices;

    public static unsafe class SegmentTreeOnTreeMerge
    {
        public static int Run(int aRoot, int bRoot, int lo, int hi,
            int* leftChild, int* rightChild, int* sumArr, int* allocCount)
        {
            if (aRoot == 0) return bRoot;
            if (bRoot == 0) return aRoot;

            if (lo == hi)
            {
                sumArr[aRoot] += sumArr[bRoot];
                return aRoot;
            }

            int mid = lo + ((hi - lo) >> 1);
            leftChild[aRoot] = Run(leftChild[aRoot], leftChild[bRoot], lo, mid,
                leftChild, rightChild, sumArr, allocCount);
            rightChild[aRoot] = Run(rightChild[aRoot], rightChild[bRoot], mid + 1, hi,
                leftChild, rightChild, sumArr, allocCount);
            sumArr[aRoot] = sumArr[leftChild[aRoot]] + sumArr[rightChild[aRoot]];
            return aRoot;
        }
    }

    public static unsafe class PersistentMergeableSegmentTree
    {
        public static int Update(int prev, int lo, int hi, int idx, int val,
            int* leftChild, int* rightChild, int* sumArr, int* allocCount)
        {
            int node = ++(*allocCount);
            leftChild[node] = prev != 0 ? leftChild[prev] : 0;
            rightChild[node] = prev != 0 ? rightChild[prev] : 0;
            sumArr[node] = (prev != 0 ? sumArr[prev] : 0) + val;

            if (lo == hi) return node;

            int mid = lo + ((hi - lo) >> 1);
            if (idx <= mid)
                leftChild[node] = Update(leftChild[node], lo, mid, idx, val,
                    leftChild, rightChild, sumArr, allocCount);
            else
                rightChild[node] = Update(rightChild[node], mid + 1, hi, idx, val,
                    leftChild, rightChild, sumArr, allocCount);

            return node;
        }

        public static int Merge(int aNode, int bNode, int lo, int hi,
            int* leftChild, int* rightChild, int* sumArr, int* allocCount)
        {
            if (aNode == 0) return bNode;
            if (bNode == 0) return aNode;

            int node = ++(*allocCount);
            sumArr[node] = sumArr[aNode] + sumArr[bNode];

            if (lo == hi) return node;

            int mid = lo + ((hi - lo) >> 1);
            leftChild[node] = Merge(leftChild[aNode], leftChild[bNode], lo, mid,
                leftChild, rightChild, sumArr, allocCount);
            rightChild[node] = Merge(rightChild[aNode], rightChild[bNode], mid + 1, hi,
                leftChild, rightChild, sumArr, allocCount);
            return node;
        }

        public static int Query(int node, int lo, int hi, int ql, int qr,
            int* leftChild, int* rightChild, int* sumArr)
        {
            if (node == 0 || qr < lo || hi < ql) return 0;
            if (ql <= lo && hi <= qr) return sumArr[node];
            int mid = lo + ((hi - lo) >> 1);
            return Query(leftChild[node], lo, mid, ql, qr, leftChild, rightChild, sumArr) +
                   Query(rightChild[node], mid + 1, hi, ql, qr, leftChild, rightChild, sumArr);
        }
    }
}
