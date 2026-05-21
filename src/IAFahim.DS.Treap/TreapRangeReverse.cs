namespace IAFahim.DS.Treap
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct TreapRevNode
    {
        public int Priority;
        public int Size;
        public long Value;
        public long Sum;
        public bool Rev;
        public TreapRevNode* Left;
        public TreapRevNode* Right;
    }

    public static unsafe class TreapRangeReverse
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Size(TreapRevNode* n) => n == null ? 0 : n->Size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Sum(TreapRevNode* n) => n == null ? 0 : n->Sum;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(TreapRevNode* n)
        {
            if (n == null) return;
            n->Size = 1 + Size(n->Left) + Size(n->Right);
            n->Sum = n->Value + Sum(n->Left) + Sum(n->Right);
        }

        public static void Push(TreapRevNode* n)
        {
            if (n == null || !n->Rev) return;
            n->Rev = false;
            TreapRevNode* tmp = n->Left;
            n->Left = n->Right;
            n->Right = tmp;
            if (n->Left != null) n->Left->Rev = !n->Left->Rev;
            if (n->Right != null) n->Right->Rev = !n->Right->Rev;
        }

        public static void Split(TreapRevNode* root, int k,
            TreapRevNode** left, TreapRevNode** right)
        {
            *left = null;
            *right = null;
            if (root == null) return;
            Push(root);
            int ls = Size(root->Left);
            if (ls >= k)
            {
                Split(root->Left, k, left, &root->Left);
                Update(root);
                *right = root;
            }
            else
            {
                Split(root->Right, k - ls - 1, &root->Right, right);
                Update(root);
                *left = root;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TreapRevNode* Merge(TreapRevNode* a, TreapRevNode* b)
        {
            if (a == null) return b;
            if (b == null) return a;
            Push(a);
            Push(b);
            if (a->Priority > b->Priority)
            {
                a->Right = Merge(a->Right, b);
                Update(a);
                return a;
            }
            b->Left = Merge(a, b->Left);
            Update(b);
            return b;
        }

        public static void Reverse(ref TreapRevNode* root, int l, int r)
        {
            TreapRevNode* left = null;
            TreapRevNode* mid = null;
            TreapRevNode* right = null;
            Split(root, l, &left, &mid);
            Split(mid, r - l + 1, &mid, &right);
            if (mid != null) mid->Rev = !mid->Rev;
            root = Merge(left, Merge(mid, right));
        }
    }
}
