namespace IAFahim.DS.Treap
{
    using System.Runtime.CompilerServices;

    public static unsafe class TreapRangeRotate
    {
        public static TreapRevNode* Rotate(ref TreapRevNode* root, int l, int r, int k)
        {
            int len = r - l + 1;
            k = ((k % len) + len) % len;
            if (k == 0) return root;

            TreapRevNode* left = null;
            TreapRevNode* seg = null;
            TreapRevNode* right = null;
            TreapRangeReverse.Split(root, l, &left, &seg);
            TreapRangeReverse.Split(seg, len, &seg, &right);

            TreapRevNode* front = null;
            TreapRevNode* back = null;
            TreapRangeReverse.Split(seg, k, &front, &back);

            root = TreapRangeReverse.Merge(left, TreapRangeReverse.Merge(back, TreapRangeReverse.Merge(front, right)));
            return root;
        }
    }
}
