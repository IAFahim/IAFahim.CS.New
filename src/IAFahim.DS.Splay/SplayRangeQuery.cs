namespace IAFahim.DS.Splay
{
    using System.Runtime.CompilerServices;

    public static unsafe class SplayRangeQuery
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SplayRevNode* Kth(SplayRevNode* root, int k)
        {
            SplayRevNode* cur = root;
            while (cur != null)
            {
                SplayRangeReverse.Push(cur);
                SplayRevNode* left = cur->Left;
                int ls = left != null ? left->Size : 0;
                if (k < ls) { cur = left; continue; }
                if (k == ls) return cur;
                k -= ls + 1;
                cur = cur->Right;
            }
            return null;
        }

        // Splays the k-th node (0-based) to the root and returns the sum of indices [0, k].
        // k must be in [0, Size-1]; callers guarantee validity for the negative-k prefix base.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Prefix(SplayRevNode** root, int k)
        {
            if (k < 0) return 0;
            SplayRevNode* node = Kth(*root, k);
            SplayRangeReverse.Splay(root, node);
            SplayRevNode* newRoot = *root;
            SplayRevNode* left = newRoot->Left;
            long ls = left != null ? left->Sum : 0;
            return ls + newRoot->Key;
        }

        public static long QuerySum(SplayRevNode** root, int l, int r)
        {
            // Sum of [l, r] = prefix(r) - prefix(l - 1).
            // Splaying Kth(r) to the root makes (root->Left->Sum + root->Key) the prefix sum
            // of indices [0, r] regardless of tree shape, after lazy reversals are pushed down.
            long upper = Prefix(root, r);
            long lower = Prefix(root, l - 1);
            return upper - lower;
        }
    }
}
