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
            x->Size = 1 + (x->Left != null ? x->Left->Size : 0) + (x->Right != null ? x->Right->Size : 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsLeftChild(SplayNode* x) => x->Parent != null && x->Parent->Left == x;

        public static void Rotate(SplayNode** root, SplayNode* x)
        {
            SplayNode* p = x->Parent;
            SplayNode* g = p->Parent;
            if (IsLeftChild(x))
            {
                p->Left = x->Right;
                if (x->Right != null) x->Right->Parent = p;
                x->Right = p;
            }
            else
            {
                p->Right = x->Left;
                if (x->Left != null) x->Left->Parent = p;
                x->Left = p;
            }
            p->Parent = x;
            x->Parent = g;
            if (g != null)
            {
                if (g->Left == p) g->Left = x;
                else g->Right = x;
            }
            else *root = x;
            Update(p);
            Update(x);
        }

        public static void Splay_(SplayNode** root, SplayNode* x)
        {
            while (x->Parent != null)
            {
                SplayNode* p = x->Parent;
                SplayNode* g = p->Parent;
                if (g != null)
                {
                    if (IsLeftChild(x) == IsLeftChild(p)) Rotate(root, p);
                    else Rotate(root, x);
                }
                Rotate(root, x);
            }
        }
    }
}
