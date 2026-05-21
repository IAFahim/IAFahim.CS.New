namespace IAFahim.DS.SegmentTree
{
    using System.Runtime.CompilerServices;

    public static unsafe class MergeableSegmentTreeMerge
    {
        public static int Run(int aNode, int bNode,
            int* leftChild, int* rightChild, int* sumArr, int* allocCount)
        {
            if (aNode == 0) return bNode;
            if (bNode == 0) return aNode;

            int node = ++(*allocCount);
            sumArr[node] = sumArr[aNode] + sumArr[bNode];
            leftChild[node] = Run(leftChild[aNode], leftChild[bNode],
                leftChild, rightChild, sumArr, allocCount);
            rightChild[node] = Run(rightChild[aNode], rightChild[bNode],
                leftChild, rightChild, sumArr, allocCount);
            return node;
        }
    }

    public static unsafe class MergeableSegmentTreeUpdate
    {
        public static void Run(int* root, int lo, int hi, int idx, int val,
            int* leftChild, int* rightChild, int* sumArr, int* allocCount)
        {
            if (*root == 0) *root = ++(*allocCount);
            int node = *root;
            sumArr[node] += val;
            if (lo == hi) return;
            int mid = lo + ((hi - lo) >> 1);
            if (idx <= mid)
            {
                if (leftChild[node] == 0) leftChild[node] = ++(*allocCount);
                Run(&leftChild[node], lo, mid, idx, val, leftChild, rightChild, sumArr, allocCount);
            }
            else
            {
                if (rightChild[node] == 0) rightChild[node] = ++(*allocCount);
                Run(&rightChild[node], mid + 1, hi, idx, val, leftChild, rightChild, sumArr, allocCount);
            }
        }
    }

    public static unsafe class MergeableSegmentTreeQuery
    {
        public static int Run(int node, int lo, int hi, int ql, int qr,
            int* leftChild, int* rightChild, int* sumArr)
        {
            if (node == 0 || qr < lo || hi < ql) return 0;
            if (ql <= lo && hi <= qr) return sumArr[node];
            int mid = lo + ((hi - lo) >> 1);
            return Run(leftChild[node], lo, mid, ql, qr, leftChild, rightChild, sumArr) +
                   Run(rightChild[node], mid + 1, hi, ql, qr, leftChild, rightChild, sumArr);
        }
    }
}
