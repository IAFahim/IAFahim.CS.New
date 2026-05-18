namespace IAFahim.DS.LinkCut
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct LctNode
    {
        public int Index;
        public long Value;
        public long PathSum;
        public bool Rev;
        public LctNode* Left;
        public LctNode* Right;
        public LctNode* Parent;
    }

    public static unsafe class LinkCut
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Update(LctNode* x)
        {
            x->PathSum = x->Value;
            if (x->Left != null)
                x->PathSum += x->Left->PathSum;
            if (x->Right != null)
                x->PathSum += x->Right->PathSum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Push(LctNode* x)
        {
            if (x->Rev)
            {
                LctNode* tmp = x->Left;
                x->Left = x->Right;
                x->Right = tmp;
                if (x->Left != null)
                    x->Left->Rev = !x->Left->Rev;
                if (x->Right != null)
                    x->Right->Rev = !x->Right->Rev;
                x->Rev = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsRoot(LctNode* x)
        {
            LctNode* p = x->Parent;
            if (p == null) return true;
            return p->Left != x && p->Right != x;
        }

        private static void Rotate(LctNode* x)
        {
            LctNode* p = x->Parent;
            LctNode* g = p->Parent;
            if (p == null) return;
            if (p->Left == x)
            {
                p->Left = x->Right;
                if (x->Right != null)
                    x->Right->Parent = p;
                x->Right = p;
            }
            else
            {
                p->Right = x->Left;
                if (x->Left != null)
                    x->Left->Parent = p;
                x->Left = p;
            }
            p->Parent = x;
            x->Parent = g;
            if (g != null)
            {
                if (g->Left == p)
                    g->Left = x;
                else if (g->Right == p)
                    g->Right = x;
            }
            Update(p);
            Update(x);
        }

        private static void PushTo(LctNode* x)
        {
            if (!IsRoot(x))
                PushTo(x->Parent);
            Push(x);
        }

        private static void Splay(LctNode* x)
        {
            PushTo(x);
            while (!IsRoot(x))
            {
                LctNode* p = x->Parent;
                LctNode* g = p->Parent;
                if (!IsRoot(p))
                {
                    if ((p->Left == x) != (g->Left == p))
                        Rotate(x);
                    else
                        Rotate(p);
                }
                Rotate(x);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Access(LctNode* x)
        {
            LctNode* last = null;
            while (x != null)
            {
                Splay(x);
                x->Right = last;
                Update(x);
                last = x;
                x = x->Parent;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MakeRoot(LctNode* x)
        {
            Access(x);
            Splay(x);
            x->Rev = !x->Rev;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LctNode* FindRoot(LctNode* x)
        {
            Access(x);
            Splay(x);
            while (true)
            {
                Push(x);
                if (x->Left == null) break;
                x = x->Left;
            }
            Splay(x);
            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Link(LctNode* x, LctNode* y)
        {
            MakeRoot(x);
            x->Parent = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Cut(LctNode* x, LctNode* y)
        {
            MakeRoot(x);
            Access(y);
            Splay(y);
            if (y->Left == x)
            {
                x->Parent = null;
                y->Left = null;
                Update(y);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Query(LctNode* x, LctNode* y)
        {
            MakeRoot(x);
            Access(y);
            Splay(y);
            return y->PathSum;
        }
    }
}