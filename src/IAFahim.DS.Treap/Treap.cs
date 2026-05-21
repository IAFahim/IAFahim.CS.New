namespace IAFahim.DS.Treap
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct TreapNode
    {
        public int Key;
        public int Priority;
        public int Size;
        public bool Rev;
        public long Sum;
        public TreapNode* Left;
        public TreapNode* Right;
    }

    public static unsafe class Treap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SizeOf(TreapNode* node)
        {
            return node == null ? 0 : node->Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long SumOf(TreapNode* node)
        {
            return node == null ? 0 : node->Sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(TreapNode* node)
        {
            if (node == null) return;
            node->Size = 1 + SizeOf(node->Left) + SizeOf(node->Right);
            node->Sum = node->Key + SumOf(node->Left) + SumOf(node->Right);
        }

        public static void Push(TreapNode* node)
        {
            if (node == null || !node->Rev) return;
            node->Rev = false;
            TreapNode* tmp = node->Left;
            node->Left = node->Right;
            node->Right = tmp;
            if (node->Left != null) node->Left->Rev = !node->Left->Rev;
            if (node->Right != null) node->Right->Rev = !node->Right->Rev;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TreapNode* Merge(TreapNode* a, TreapNode* b)
        {
            if (a == null) return b;
            if (b == null) return a;
            if (a->Priority < b->Priority)
            {
                Push(a);
                a->Right = Merge(a->Right, b);
                Update(a);
                return a;
            }
            else
            {
                Push(b);
                b->Left = Merge(a, b->Left);
                Update(b);
                return b;
            }
        }

        public static void Split(TreapNode* root, int key, TreapNode** left, TreapNode** right)
        {
            *left = null;
            *right = null;
            if (root == null) return;
            Push(root);
            if (root->Key < key)
            {
                Split(root->Right, key, &root->Right, right);
                Update(root);
                *left = root;
            }
            else
            {
                Split(root->Left, key, left, &root->Left);
                Update(root);
                *right = root;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Insert(TreapNode** root, TreapNode* node)
        {
            if (*root == null)
            {
                *root = node;
                return;
            }
            if (node->Priority < (*root)->Priority)
            {
                Push(*root);
                Split(*root, node->Key, &node->Left, &node->Right);
                Update(node);
                *root = node;
            }
            else
            {
                Push(*root);
                if (node->Key < (*root)->Key)
                    Insert(&(*root)->Left, node);
                else
                    Insert(&(*root)->Right, node);
                Update(*root);
            }
        }

        public static void Erase(TreapNode** root, int key)
        {
            if (*root == null) return;
            Push(*root);
            if ((*root)->Key == key)
            {
                TreapNode* merged = Merge((*root)->Left, (*root)->Right);
                *root = merged;
            }
            else if (key < (*root)->Key)
            {
                Erase(&(*root)->Left, key);
                Update(*root);
            }
            else
            {
                Erase(&(*root)->Right, key);
                Update(*root);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TreapNode* Find(TreapNode* root, int key)
        {
            while (root != null)
            {
                Push(root);
                if (root->Key == key) return root;
                if (key < root->Key) root = root->Left;
                else root = root->Right;
            }
            return null;
        }

        public static TreapNode* Kth(TreapNode* root, int k)
        {
            if (root == null) return null;
            Push(root);
            int leftSize = SizeOf(root->Left);
            if (k < leftSize) return Kth(root->Left, k);
            if (k == leftSize) return root;
            return Kth(root->Right, k - leftSize - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Rank(TreapNode* root, int key)
        {
            int result = 0;
            TreapNode* cur = root;
            while (cur != null)
            {
                Push(cur);
                if (cur->Key < key)
                {
                    result += 1 + SizeOf(cur->Left);
                    cur = cur->Right;
                }
                else
                {
                    cur = cur->Left;
                }
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reverse(TreapNode* node)
        {
            if (node != null) node->Rev = !node->Rev;
        }

        public static long RangeQuery(TreapNode** root, int l, int r)
        {
            TreapNode* left = null;
            TreapNode* mid = null;
            TreapNode* right = null;
            Split(root, l, &left, &mid);
            TreapNode** midRef = &mid;
            Split(midRef, r + 1, &mid, &right);
            long result = SumOf(mid);
            *root = Merge(left, Merge(mid, right));
            return result;
        }
    }
}