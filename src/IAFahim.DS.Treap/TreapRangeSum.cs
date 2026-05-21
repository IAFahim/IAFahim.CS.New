namespace IAFahim.DS.Treap
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct TreapSumNode
    {
        public int Priority;
        public int Size;
        public long Value;
        public long Sum;
        public long LazyAdd;
        public TreapSumNode* Left;
        public TreapSumNode* Right;
    }

    public static unsafe class TreapRangeSum
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Size(TreapSumNode* n) => n == null ? 0 : n->Size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Sum(TreapSumNode* n) => n == null ? 0 : n->Sum;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(TreapSumNode* n)
        {
            if (n == null) return;
            n->Size = 1 + Size(n->Left) + Size(n->Right);
            n->Sum = n->Value + Sum(n->Left) + Sum(n->Right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Apply(TreapSumNode* n, long add)
        {
            if (n == null) return;
            n->Value += add;
            n->Sum += (long)n->Size * add;
            n->LazyAdd += add;
        }

        public static void Push(TreapSumNode* n)
        {
            if (n == null || n->LazyAdd == 0) return;
            Apply(n->Left, n->LazyAdd);
            Apply(n->Right, n->LazyAdd);
            n->LazyAdd = 0;
        }

        public static void Split(TreapSumNode* root, int k,
            TreapSumNode** left, TreapSumNode** right)
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
        public static TreapSumNode* Merge(TreapSumNode* a, TreapSumNode* b)
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

        public static void AddRange(ref TreapSumNode* root, int l, int r, long val)
        {
            TreapSumNode* left = null;
            TreapSumNode* mid = null;
            TreapSumNode* right = null;
            Split(root, l, &left, &mid);
            Split(mid, r - l + 1, &mid, &right);
            Apply(mid, val);
            root = Merge(left, Merge(mid, right));
        }

        public static long QuerySum(ref TreapSumNode* root, int l, int r)
        {
            TreapSumNode* left = null;
            TreapSumNode* mid = null;
            TreapSumNode* right = null;
            Split(root, l, &left, &mid);
            Split(mid, r - l + 1, &mid, &right);
            long res = Sum(mid);
            root = Merge(left, Merge(mid, right));
            return res;
        }
    }
}
