namespace IAFahim.DS.Splay
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SplayNode
    {
        public int Key;
        public int Size;
        public SplayNode* Parent;
        public SplayNode* Left;
        public SplayNode* Right;
    }

    public static unsafe class Splay
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(SplayNode* x)
        {
            if (x == null) return;
            int l = x->Left != null ? x->Left->Size : 0;
            int r = x->Right != null ? x->Right->Size : 0;
            x->Size = l + r + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsLeftChild(SplayNode* x)
        {
            return x->Parent != null && x->Parent->Left == x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsRightChild(SplayNode* x)
        {
            return x->Parent != null && x->Parent->Right == x;
        }

        public static void Rotate(SplayNode** root, SplayNode* x)
        {
            if (x == null || x->Parent == null) return;
            SplayNode* p = x->Parent;
            SplayNode* g = p->Parent;
            bool left = IsLeftChild(x);
            if (g == null)
            {
                if (left)
                {
                    SplayNode* xr = x->Right;
                    p->Left = xr;
                    if (xr != null) xr->Parent = p;
                    x->Right = p;
                    p->Parent = x;
                    x->Parent = null;
                }
                else
                {
                    SplayNode* xl = x->Left;
                    p->Right = xl;
                    if (xl != null) xl->Parent = p;
                    x->Left = p;
                    p->Parent = x;
                    x->Parent = null;
                }
                *root = x;
                Update(p);
                Update(x);
                return;
            }
            if (left)
            {
                SplayNode* xr = x->Right;
                p->Left = xr;
                if (xr != null) xr->Parent = p;
                x->Right = p;
                p->Parent = x;
            }
            else
            {
                SplayNode* xl = x->Left;
                p->Right = xl;
                if (xl != null) xl->Parent = p;
                x->Left = p;
                p->Parent = x;
            }
            bool wasLeft = IsLeftChild(p);
            if (wasLeft) g->Left = x;
            else g->Right = x;
            x->Parent = g;
            Update(p);
            Update(x);
            while (g != null)
            {
                Update(g);
                g = g->Parent;
            }
        }

        public static void Splay_(SplayNode** root, SplayNode* x)
        {
            if (x == null || root == null) return;
            while (x->Parent != null)
            {
                SplayNode* p = x->Parent;
                SplayNode* g = p->Parent;
                if (g == null)
                {
                    Rotate(root, x);
                }
                else if ((IsLeftChild(x) && IsLeftChild(p)) || (IsRightChild(x) && IsRightChild(p)))
                {
                    Rotate(root, p);
                    Rotate(root, x);
                }
                else
                {
                    Rotate(root, x);
                    Rotate(root, x);
                }
            }
            *root = x;
        }

        public static SplayNode* Access(SplayNode** root, int idx)
        {
            if (root == null || *root == null) return null;
            SplayNode* cur = *root;
            while (cur != null)
            {
                int leftSize = cur->Left != null ? cur->Left->Size : 0;
                if (idx < leftSize)
                {
                    cur = cur->Left;
                }
                else if (idx == leftSize)
                {
                    Splay_(root, cur);
                    return cur;
                }
                else
                {
                    idx -= leftSize + 1;
                    cur = cur->Right;
                }
            }
            return null;
        }
    }
}