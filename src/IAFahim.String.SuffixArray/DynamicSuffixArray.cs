namespace IAFahim.String.SuffixArray
{
    using System;
    using System.Runtime.CompilerServices;

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public unsafe struct DynamicStringNode
    {
        public int Priority;
        public int Size;
        public byte Value;
        public ulong Hash;
        public DynamicStringNode* Left;
        public DynamicStringNode* Right;
    }

    public static unsafe class DynamicSuffixArray
    {
        public const ulong BASE = 313;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSize(DynamicStringNode* node) => node == null ? 0 : node->Size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetHash(DynamicStringNode* node) => node == null ? 0 : node->Hash;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pull(DynamicStringNode* node, ulong* powers)
        {
            if (node == null) return;
            node->Size = GetSize(node->Left) + 1 + GetSize(node->Right);
            ulong leftHash = GetHash(node->Left);
            ulong rightHash = GetHash(node->Right);
            int rightSize = GetSize(node->Right);
            node->Hash = leftHash * powers[rightSize + 1] + node->Value * powers[rightSize] + rightHash;
        }

        public static void Split(DynamicStringNode* node, int k, DynamicStringNode** left, DynamicStringNode** right, ulong* powers)
        {
            if (node == null)
            {
                *left = null;
                *right = null;
                return;
            }
            int leftSize = GetSize(node->Left);
            if (leftSize >= k)
            {
                *right = node;
                DynamicStringNode* nextLeft = null;
                Split(node->Left, k, left, &nextLeft, powers);
                node->Left = nextLeft;
                Pull(*right, powers);
            }
            else
            {
                *left = node;
                DynamicStringNode* nextRight = null;
                Split(node->Right, k - leftSize - 1, &nextRight, right, powers);
                node->Right = nextRight;
                Pull(*left, powers);
            }
        }

        public static DynamicStringNode* Merge(DynamicStringNode* left, DynamicStringNode* right, ulong* powers)
        {
            if (left == null) return right;
            if (right == null) return left;
            if (left->Priority > right->Priority)
            {
                left->Right = Merge(left->Right, right, powers);
                Pull(left, powers);
                return left;
            }
            else
            {
                right->Left = Merge(left, right->Left, powers);
                Pull(right, powers);
                return right;
            }
        }

        public static void Insert(ref DynamicStringNode* root, int index, DynamicStringNode* node, ulong* powers)
        {
            DynamicStringNode* left = null;
            DynamicStringNode* right = null;
            Split(root, index, &left, &right, powers);
            root = Merge(left, Merge(node, right, powers), powers);
        }

        public static void Erase(ref DynamicStringNode* root, int index, ulong* powers)
        {
            DynamicStringNode* left = null;
            DynamicStringNode* mid = null;
            DynamicStringNode* right = null;
            Split(root, index, &left, &mid, powers);
            Split(mid, 1, &mid, &right, powers);
            root = Merge(left, right, powers);
        }

        public static ulong GetSubstringHash(ref DynamicStringNode* root, int l, int r, ulong* powers)
        {
            if (l > r) return 0;
            DynamicStringNode* left = null;
            DynamicStringNode* mid = null;
            DynamicStringNode* right = null;
            Split(root, l, &left, &mid, powers);
            Split(mid, r - l + 1, &mid, &right, powers);
            ulong h = GetHash(mid);
            root = Merge(left, Merge(mid, right, powers), powers);
            return h;
        }

        public static int Lcp(ref DynamicStringNode* root, int i, int j, ulong* powers)
        {
            int n = GetSize(root);
            int low = 0, high = Math.Min(n - i, n - j);
            int ans = 0;
            while (low <= high)
            {
                int mid = (low + high) / 2;
                if (mid == 0)
                {
                    low = mid + 1;
                    continue;
                }
                ulong h1 = GetSubstringHash(ref root, i, i + mid - 1, powers);
                ulong h2 = GetSubstringHash(ref root, j, j + mid - 1, powers);
                if (h1 == h2)
                {
                    ans = mid;
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            return ans;
        }

        public static int CompareSuffix(ref DynamicStringNode* root, int i, int j, ulong* powers)
        {
            if (i == j) return 0;
            int lcp = Lcp(ref root, i, j, powers);
            int n = GetSize(root);
            if (i + lcp == n) return -1;
            if (j + lcp == n) return 1;

            ulong h1 = GetSubstringHash(ref root, i + lcp, i + lcp, powers);
            ulong h2 = GetSubstringHash(ref root, j + lcp, j + lcp, powers);
            return h1.CompareTo(h2);
        }
    }
}
