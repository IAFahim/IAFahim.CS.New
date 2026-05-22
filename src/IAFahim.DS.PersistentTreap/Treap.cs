namespace IAFahim.DS.PersistentTreap
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PersistentTreapNode
    {
        public static int NewNode<T>(T* nodes, int* left, int* right, int* prio, T val, int* allocCnt)
            where T : unmanaged, IComparable<T>
        {
            int idx = ++(*allocCnt);
            left[idx] = 0;
            right[idx] = 0;
            prio[idx] = (int)((uint)(idx * 2654435761) >> 0);
            nodes[idx] = val;
            return idx;
        }
    }

    public static unsafe class PersistentTreapSplit
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(T* nodes, int* left, int* right, int root, T key, int* outLeft, int* outRight)
            where T : unmanaged, IComparable<T>
        {
            if (root == 0)
            {
                *outLeft = 0;
                *outRight = 0;
                return;
            }
            if (nodes[root].CompareTo(key) <= 0)
            {
                *outLeft = root;
                Run(nodes, left, right, right[root], key, outRight, outRight);
            }
            else
            {
                *outRight = root;
                Run(nodes, left, right, left[root], key, outLeft, outLeft);
            }
        }
    }

    public static unsafe class PersistentTreapMerge
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run<T>(T* nodes, int* left, int* right, int l, int r)
            where T : unmanaged, IComparable<T>
        {
            if (l == 0 || r == 0) return l != 0 ? l : r;
            if (left[r] > left[l])
            {
                int newRight = Run(nodes, left, right, right[l], r);
                right[l] = newRight;
                return l;
            }
            else
            {
                int newLeft = Run(nodes, left, right, l, left[r]);
                left[r] = newLeft;
                return r;
            }
        }
    }

    public static unsafe class PersistentTreapInsert
    {
        public static int Run<T>(T* nodes, int* left, int* right, int* prio, int* allocCnt, int root, T val)
            where T : unmanaged, IComparable<T>
        {
            int l = 0, r = 0;
            PersistentTreapSplit.Run(nodes, left, right, root, val, &l, &r);
            int newNode = PersistentTreapNode.NewNode(nodes, left, right, prio, val, allocCnt);
            return PersistentTreapMerge.Run(nodes, left, right, PersistentTreapMerge.Run(nodes, left, right, l, newNode), r);
        }
    }

    public static unsafe class PersistentTreapErase
    {
        public static int Run<T>(T* nodes, int* left, int* right, int root, T val)
            where T : unmanaged, IComparable<T>
        {
            if (root == 0) return 0;
            int cmp = nodes[root].CompareTo(val);
            if (cmp == 0)
            {
                return PersistentTreapMerge.Run(nodes, left, right, left[root], right[root]);
            }
            if (cmp < 0)
            {
                int newRight = Run(nodes, left, right, right[root], val);
                right[root] = newRight;
            }
            else
            {
                int newLeft = Run(nodes, left, right, left[root], val);
                left[root] = newLeft;
            }
            return root;
        }
    }

    public static unsafe class PersistentTreapFind
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run<T>(T* nodes, int* left, int* right, int root, T val)
            where T : unmanaged, IComparable<T>
        {
            int cur = root;
            while (cur != 0)
            {
                int cmp = nodes[cur].CompareTo(val);
                if (cmp == 0) return true;
                cur = cmp < 0 ? right[cur] : left[cur];
            }
            return false;
        }
    }

    public static unsafe class PersistentTreapKth
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Run<T>(T* nodes, int* left, int* right, int* size, int root, int k)
            where T : unmanaged
        {
            int cur = root;
            while (cur != 0)
            {
                int leftSize = left[cur] != 0 ? size[left[cur]] : 0;
                if (k < leftSize)
                {
                    cur = left[cur];
                }
                else if (k == leftSize)
                {
                    return nodes[cur];
                }
                else
                {
                    k -= leftSize + 1;
                    cur = right[cur];
                }
            }
            return default;
        }
    }

    public static unsafe class PersistentTreapRank
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run<T>(T* nodes, int* left, int* right, int* size, int root, T val)
            where T : unmanaged, IComparable<T>
        {
            int cur = root;
            int rank = 0;
            while (cur != 0)
            {
                int cmp = nodes[cur].CompareTo(val);
                if (cmp < 0)
                {
                    rank += (left[cur] != 0 ? size[left[cur]] : 0) + 1;
                    cur = right[cur];
                }
                else
                {
                    cur = left[cur];
                }
            }
            return rank;
        }
    }
}