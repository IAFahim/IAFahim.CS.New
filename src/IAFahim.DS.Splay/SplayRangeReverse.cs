namespace IAFahim.DS.Splay
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SplayRevNode
    {
        public int Key;
        public int Size;
        public long Sum;
        public bool Rev;
        public SplayRevNode* Parent;
        public SplayRevNode* Left;
        public SplayRevNode* Right;
    }

    public static unsafe class SplayRangeReverse
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(SplayRevNode* x)
        {
            if (x == null) return;
            SplayRevNode* xl = x->Left;
            SplayRevNode* xr = x->Right;
            int l = xl != null ? xl->Size : 0;
            int r = xr != null ? xr->Size : 0;
            long ls = xl != null ? xl->Sum : 0;
            long rs = xr != null ? xr->Sum : 0;
            x->Size = l + r + 1;
            x->Sum = ls + rs + x->Key;
        }

        public static void Push(SplayRevNode* x)
        {
            if (x == null || !x->Rev) return;
            x->Rev = false;
            SplayRevNode* tmp = x->Left;
            x->Left = x->Right;
            x->Right = tmp;
            if (x->Left != null) x->Left->Rev = !x->Left->Rev;
            if (x->Right != null) x->Right->Rev = !x->Right->Rev;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsRoot(SplayRevNode* x)
        {
            return x->Parent == null || (x->Parent->Left != x && x->Parent->Right != x);
        }

        private static void PushAll(SplayRevNode* x)
        {
            if (!IsRoot(x)) PushAll(x->Parent);
            Push(x);
        }

        private static void Rotate(SplayRevNode** root, SplayRevNode* x)
        {
            SplayRevNode* p = x->Parent;
            SplayRevNode* g = p->Parent;
            bool left = p->Left == x;
            // After one rotation x's subtree equals p's old subtree, so x's
            // aggregates are p's pre-rotation aggregates. Capture them now.
            int oldPSize = p->Size;
            long oldPSum = p->Sum;

            if (left)
            {
                SplayRevNode* xr = x->Right;
                p->Left = xr;
                if (xr != null) xr->Parent = p;
                x->Right = p;
            }
            else
            {
                SplayRevNode* xl = x->Left;
                p->Right = xl;
                if (xl != null) xl->Parent = p;
                x->Left = p;
            }
            p->Parent = x;
            x->Parent = g;

            if (g != null)
            {
                if (g->Left == p) g->Left = x;
                else if (g->Right == p) g->Right = x;
            }
            else *root = x;

            Update(p);
            x->Size = oldPSize;
            x->Sum = oldPSum;
        }

        public static void Splay(SplayRevNode** root, SplayRevNode* x)
        {
            PushAll(x);
            while (!IsRoot(x))
            {
                SplayRevNode* p = x->Parent;
                SplayRevNode* g = p->Parent;
                if (!IsRoot(p))
                {
                    bool zig = (g->Left == p) == (p->Left == x);
                    if (zig) Rotate(root, p);
                    else Rotate(root, x);
                }
                Rotate(root, x);
            }
        }

        // Splays x up until x->Parent == stop, leaving x as a child of stop.
        // Lazy tags along the path from stop down to x are flushed first.
        private static void SplayUnder(SplayRevNode** root, SplayRevNode* x, SplayRevNode* stop)
        {
            PushAll(x);
            while (x->Parent != stop)
            {
                SplayRevNode* p = x->Parent;
                SplayRevNode* g = p->Parent;
                if (g != stop)
                {
                    bool zig = (g->Left == p) == (p->Left == x);
                    if (zig) Rotate(root, p);
                    else Rotate(root, x);
                }
                Rotate(root, x);
            }
        }

        private static SplayRevNode* Kth(SplayRevNode* root, int k)
        {
            SplayRevNode* cur = root;
            while (cur != null)
            {
                Push(cur);
                int ls = cur->Left != null ? cur->Left->Size : 0;
                if (k < ls) { cur = cur->Left; continue; }
                if (k == ls) return cur;
                k -= ls + 1;
                cur = cur->Right;
            }
            return null;
        }

        public static void Reverse(SplayRevNode** root, int l, int r)
        {
            int n = (*root)->Size;
            SplayRevNode* left = l > 0 ? Kth(*root, l - 1) : null;
            SplayRevNode* right = r < n - 1 ? Kth(*root, r + 1) : null;

            SplayRevNode* mid;
            if (left == null && right == null) { mid = *root; }
            else if (left == null) { Splay(root, right); mid = (*root)->Left; }
            else if (right == null) { Splay(root, left); mid = (*root)->Right; }
            else
            {
                Splay(root, left);
                SplayUnder(root, right, left);
                mid = right->Left;
            }

            if (mid != null) mid->Rev = !mid->Rev;
        }
    }
}
