namespace IAFahim.DS.Treap
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct TreapMinNode
    {
        public int Priority;
        public int Size;
        public long Value;
        public long Min;
        public long LazyAssign;
        public bool HasAssign;
        public TreapMinNode* Left;
        public TreapMinNode* Right;
    }

    public static unsafe class TreapRangeMin
    {
        private const long MaxVal = long.MaxValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Size(TreapMinNode* n) => n == null ? 0 : n->Size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Min(TreapMinNode* n) => n == null ? MaxVal : n->Min;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(TreapMinNode* n)
        {
            if (n == null) return;
            n->Size = 1 + Size(n->Left) + Size(n->Right);
            long lmin = Min(n->Left);
            long rmin = Min(n->Right);
            long vmin = n->Value;
            n->Min = lmin < rmin ? (lmin < vmin ? lmin : vmin) : (rmin < vmin ? rmin : vmin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Apply(TreapMinNode* n, long val)
        {
            if (n == null) return;
            n->Value = val;
            n->Min = val;
            n->LazyAssign = val;
            n->HasAssign = true;
        }

        public static void Push(TreapMinNode* n)
        {
            if (n == null || !n->HasAssign) return;
            Apply(n->Left, n->LazyAssign);
            Apply(n->Right, n->LazyAssign);
            n->HasAssign = false;
        }

        public static void Split(TreapMinNode* root, int k,
            TreapMinNode** left, TreapMinNode** right)
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
        public static TreapMinNode* Merge(TreapMinNode* a, TreapMinNode* b)
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

        public static long QueryMin(ref TreapMinNode* root, int l, int r)
        {
            TreapMinNode* left = null;
            TreapMinNode* mid = null;
            TreapMinNode* right = null;
            Split(root, l, &left, &mid);
            Split(mid, r - l + 1, &mid, &right);
            long res = Min(mid);
            root = Merge(left, Merge(mid, right));
            return res;
        }

        public static void AssignRange(ref TreapMinNode* root, int l, int r, long val)
        {
            TreapMinNode* left = null;
            TreapMinNode* mid = null;
            TreapMinNode* right = null;
            Split(root, l, &left, &mid);
            Split(mid, r - l + 1, &mid, &right);
            Apply(mid, val);
            root = Merge(left, Merge(mid, right));
        }
    }
}
