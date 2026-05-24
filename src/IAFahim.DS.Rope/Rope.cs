namespace IAFahim.DS.Rope
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct RopeNode
    {
        public int Size;
        public int Priority;
        public byte Value;
        public RopeNode* Left;
        public RopeNode* Right;
    }

    internal static unsafe class RopeShared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Size(RopeNode* n) => n == null ? 0 : n->Size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(RopeNode* n)
        {
            if (n == null) return;
            n->Size = 1 + Size(n->Left) + Size(n->Right);
        }

        public static void SplitAt(RopeNode* root, int k, RopeNode** left, RopeNode** right)
        {
            *left = null; *right = null;
            if (root == null) return;
            int ls = Size(root->Left);
            if (ls >= k)
            {
                SplitAt(root->Left, k, left, &root->Left);
                Update(root); *right = root;
            }
            else
            {
                SplitAt(root->Right, k - ls - 1, &root->Right, right);
                Update(root); *left = root;
            }
        }

        public static RopeNode* Merge(RopeNode* a, RopeNode* b)
        {
            if (a == null) return b;
            if (b == null) return a;
            if (a->Priority > b->Priority)
            {
                a->Right = Merge(a->Right, b);
                Update(a); return a;
            }
            b->Left = Merge(a, b->Left);
            Update(b); return b;
        }
    }

    public static unsafe class RopeInsert
    {
        public static RopeNode* Run(RopeNode* root, int pos, RopeNode* node)
        {
            RopeNode* left = null; RopeNode* right = null;
            RopeShared.SplitAt(root, pos, &left, &right);
            return RopeShared.Merge(left, RopeShared.Merge(node, right));
        }
    }

    public static unsafe class RopeErase
    {
        public static RopeNode* Run(RopeNode* root, int pos, int len)
        {
            RopeNode* left = null; RopeNode* mid = null; RopeNode* right = null;
            RopeShared.SplitAt(root, pos, &left, &mid);
            RopeShared.SplitAt(mid, len, &mid, &right);
            return RopeShared.Merge(left, right);
        }
    }

    public static unsafe class RopeSubstring
    {
        public static RopeNode* Run(RopeNode* root, int pos, int len, byte* buf, out int count)
        {
            RopeNode* left = null; RopeNode* mid = null; RopeNode* right = null;
            RopeShared.SplitAt(root, pos, &left, &mid);
            RopeShared.SplitAt(mid, len, &mid, &right);
            int idx = 0;
            Collect(mid, buf, ref idx);
            count = idx;
            return RopeShared.Merge(left, RopeShared.Merge(mid, right));
        }

        private static void Collect(RopeNode* node, byte* buf, ref int idx)
        {
            if (node == null) return;
            Collect(node->Left, buf, ref idx);
            buf[idx++] = node->Value;
            Collect(node->Right, buf, ref idx);
        }
    }
}
