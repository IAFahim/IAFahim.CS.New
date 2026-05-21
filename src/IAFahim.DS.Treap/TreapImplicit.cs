namespace IAFahim.DS.Treap
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct TreapImplicitNode
    {
        public int Priority;
        public int Size;
        public long Value;
        public long Sum;
        public long Lazy;
        public bool Rev;
        public bool HasLazy;
        public TreapImplicitNode* Left;
        public TreapImplicitNode* Right;
    }

    public static unsafe class TreapImplicit
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Size(TreapImplicitNode* n) => n == null ? 0 : n->Size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Sum(TreapImplicitNode* n) => n == null ? 0 : n->Sum;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(TreapImplicitNode* n)
        {
            if (n == null) return;
            n->Size = 1 + Size(n->Left) + Size(n->Right);
            n->Sum = n->Value + Sum(n->Left) + Sum(n->Right);
        }

        public static void Push(TreapImplicitNode* n)
        {
            if (n == null) return;
            if (n->Rev)
            {
                TreapImplicitNode* tmp = n->Left;
                n->Left = n->Right;
                n->Right = tmp;
                if (n->Left != null) n->Left->Rev = !n->Left->Rev;
                if (n->Right != null) n->Right->Rev = !n->Right->Rev;
                n->Rev = false;
            }
            if (n->HasLazy)
            {
                ApplyLazy(n->Left, n->Lazy);
                ApplyLazy(n->Right, n->Lazy);
                n->HasLazy = false;
                n->Lazy = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyLazy(TreapImplicitNode* n, long lazy)
        {
            if (n == null) return;
            n->Value += lazy;
            n->Sum += (long)n->Size * lazy;
            n->Lazy += lazy;
            n->HasLazy = true;
        }

        public static void SplitBySize(TreapImplicitNode* root, int k,
            TreapImplicitNode** left, TreapImplicitNode** right)
        {
            *left = null;
            *right = null;
            if (root == null) return;
            Push(root);
            int leftSize = Size(root->Left);
            if (leftSize >= k)
            {
                SplitBySize(root->Left, k, left, &root->Left);
                Update(root);
                *right = root;
            }
            else
            {
                SplitBySize(root->Right, k - leftSize - 1, &root->Right, right);
                Update(root);
                *left = root;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TreapImplicitNode* Merge(TreapImplicitNode* a, TreapImplicitNode* b)
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
            else
            {
                b->Left = Merge(a, b->Left);
                Update(b);
                return b;
            }
        }

        public static void AddRange(ref TreapImplicitNode* root, int l, int r, long val)
        {
            TreapImplicitNode* left = null;
            TreapImplicitNode* mid = null;
            TreapImplicitNode* right = null;
            SplitBySize(root, l, &left, &mid);
            SplitBySize(mid, r - l + 1, &mid, &right);
            ApplyLazy(mid, val);
            root = Merge(left, Merge(mid, right));
        }

        public static long QueryRange(ref TreapImplicitNode* root, int l, int r)
        {
            TreapImplicitNode* left = null;
            TreapImplicitNode* mid = null;
            TreapImplicitNode* right = null;
            SplitBySize(root, l, &left, &mid);
            SplitBySize(mid, r - l + 1, &mid, &right);
            long result = Sum(mid);
            root = Merge(left, Merge(mid, right));
            return result;
        }

        public static void ReverseRange(ref TreapImplicitNode* root, int l, int r)
        {
            TreapImplicitNode* left = null;
            TreapImplicitNode* mid = null;
            TreapImplicitNode* right = null;
            SplitBySize(root, l, &left, &mid);
            SplitBySize(mid, r - l + 1, &mid, &right);
            if (mid != null) mid->Rev = !mid->Rev;
            root = Merge(left, Merge(mid, right));
        }
    }
}
