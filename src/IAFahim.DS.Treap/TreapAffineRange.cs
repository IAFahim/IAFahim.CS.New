namespace IAFahim.DS.Treap
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct TreapAffineNode
    {
        public int Priority;
        public int Size;
        public long Value;
        public long Sum;
        public long LazyA;
        public long LazyB;
        public bool HasLazy;
        public TreapAffineNode* Left;
        public TreapAffineNode* Right;
    }

    public static unsafe class TreapAffineRange
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Size(TreapAffineNode* n) => n == null ? 0 : n->Size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Sum(TreapAffineNode* n) => n == null ? 0 : n->Sum;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(TreapAffineNode* n)
        {
            if (n == null) return;
            n->Size = 1 + Size(n->Left) + Size(n->Right);
            n->Sum = n->Value + Sum(n->Left) + Sum(n->Right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Apply(TreapAffineNode* n, long a, long b)
        {
            if (n == null) return;
            n->Value = a * n->Value + b;
            n->Sum = a * n->Sum + b * n->Size;
            if (n->HasLazy)
            {
                n->LazyA = a * n->LazyA;
                n->LazyB = a * n->LazyB + b;
            }
            else
            {
                n->LazyA = a;
                n->LazyB = b;
                n->HasLazy = true;
            }
        }

        public static void Push(TreapAffineNode* n)
        {
            if (n == null || !n->HasLazy) return;
            Apply(n->Left, n->LazyA, n->LazyB);
            Apply(n->Right, n->LazyA, n->LazyB);
            n->LazyA = 1;
            n->LazyB = 0;
            n->HasLazy = false;
        }

        public static void Split(TreapAffineNode* root, int k,
            TreapAffineNode** left, TreapAffineNode** right)
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
        public static TreapAffineNode* Merge(TreapAffineNode* a, TreapAffineNode* b)
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

        public static void AffineRange(ref TreapAffineNode* root, int l, int r, long a, long b)
        {
            TreapAffineNode* left = null;
            TreapAffineNode* mid = null;
            TreapAffineNode* right = null;
            Split(root, l, &left, &mid);
            Split(mid, r - l + 1, &mid, &right);
            Apply(mid, a, b);
            root = Merge(left, Merge(mid, right));
        }

        public static long QuerySum(ref TreapAffineNode* root, int l, int r)
        {
            TreapAffineNode* left = null;
            TreapAffineNode* mid = null;
            TreapAffineNode* right = null;
            Split(root, l, &left, &mid);
            Split(mid, r - l + 1, &mid, &right);
            long res = Sum(mid);
            root = Merge(left, Merge(mid, right));
            return res;
        }
    }
}
