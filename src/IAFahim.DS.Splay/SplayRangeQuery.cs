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
                int ls = cur->Left != null ? cur->Left->Size : 0;
                if (k < ls) { cur = cur->Left; continue; }
                if (k == ls) return cur;
                k -= ls + 1;
                cur = cur->Right;
            }
            return null;
        }

        public static long QuerySum(SplayRevNode** root, int l, int r)
        {
            int n = (*root)->Size;
            SplayRevNode* left = l > 0 ? Kth(*root, l - 1) : null;
            SplayRevNode* right = r < n - 1 ? Kth(*root, r + 1) : null;

            SplayRevNode* mid;
            if (left == null && right == null)
            {
                mid = *root;
            }
            else if (left == null)
            {
                SplayRangeReverse.Splay(root, right);
                mid = (*root)->Left;
            }
            else if (right == null)
            {
                SplayRangeReverse.Splay(root, left);
                mid = (*root)->Right;
            }
            else
            {
                SplayRangeReverse.Splay(root, left);
                SplayRangeReverse.Splay(root, right);
                mid = right->Left;
            }

            return mid != null ? mid->Sum : 0;
        }
    }
}
