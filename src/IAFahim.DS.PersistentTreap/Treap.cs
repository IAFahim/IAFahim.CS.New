namespace IAFahim.DS.PersistentTreap
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PersistentTreapNode
    {
        // Knuth's multiplicative hash constant (2^32 / golden ratio).
        private const uint KnuthHash = 2654435761u;

        public static int NewNode<T>(T* nodes, int* left, int* right, int* prio, int* size, T val, int* allocCnt)
            where T : unmanaged, IComparable<T>
        {
            int idx = ++(*allocCnt);
            left[idx] = 0; right[idx] = 0;
            prio[idx] = (int)((uint)idx * KnuthHash);
            size[idx] = 1; nodes[idx] = val;
            return idx;
        }

        public static int CloneNode<T>(T* nodes, int* left, int* right, int* prio, int* size, int src, int* allocCnt)
            where T : unmanaged
        {
            if (src == 0) return 0;
            int idx = ++(*allocCnt);
            left[idx] = left[src]; right[idx] = right[src]; prio[idx] = prio[src]; size[idx] = size[src]; nodes[idx] = nodes[src];
            return idx;
        }

        // Precondition (null-sentinel contract): index 0 is the null node and size[0] == 0.
        // NewNode/CloneNode only ever write indices >= 1 (++(*allocCnt)), so size[0] stays 0
        // for the lifetime of a zero-initialized buffer, making the child reads branch-free.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update(int* left, int* right, int* size, int x)
        {
            if (x == 0) return;
            size[x] = 1 + size[left[x]] + size[right[x]];
        }
    }

    public static unsafe class PersistentTreapSplit
    {
        public static void Run<T>(T* nodes, int* left, int* right, int* prio, int* size, int root, T key, int* outLeft, int* outRight, int* allocCnt)
            where T : unmanaged, IComparable<T>
        {
            if (root == 0) { *outLeft = 0; *outRight = 0; return; }
            int newNode = PersistentTreapNode.CloneNode(nodes, left, right, prio, size, root, allocCnt);
            if (nodes[newNode].CompareTo(key) <= 0) { *outLeft = newNode; Run(nodes, left, right, prio, size, right[newNode], key, &right[newNode], outRight, allocCnt); }
            else { *outRight = newNode; Run(nodes, left, right, prio, size, left[newNode], key, outLeft, &left[newNode], allocCnt); }
            PersistentTreapNode.Update(left, right, size, newNode);
        }
    }

    public static unsafe class PersistentTreapMerge
    {
        public static int Run<T>(T* nodes, int* left, int* right, int* prio, int* size, int l, int r, int* allocCnt)
            where T : unmanaged, IComparable<T>
        {
            if (l == 0 || r == 0) return l != 0 ? l : r;
            int newNode;
            if (prio[l] > prio[r]) { newNode = PersistentTreapNode.CloneNode(nodes, left, right, prio, size, l, allocCnt); right[newNode] = Run(nodes, left, right, prio, size, right[newNode], r, allocCnt); }
            else { newNode = PersistentTreapNode.CloneNode(nodes, left, right, prio, size, r, allocCnt); left[newNode] = Run(nodes, left, right, prio, size, l, left[newNode], allocCnt); }
            PersistentTreapNode.Update(left, right, size, newNode);
            return newNode;
        }
    }

    public static unsafe class PersistentTreapInsert
    {
        public static int Run<T>(T* nodes, int* left, int* right, int* prio, int* size, int* allocCnt, int root, T val)
            where T : unmanaged, IComparable<T>
        {
            int l = 0, r = 0; PersistentTreapSplit.Run(nodes, left, right, prio, size, root, val, &l, &r, allocCnt);
            int mid = PersistentTreapNode.NewNode(nodes, left, right, prio, size, val, allocCnt);
            return PersistentTreapMerge.Run(nodes, left, right, prio, size, PersistentTreapMerge.Run(nodes, left, right, prio, size, l, mid, allocCnt), r, allocCnt);
        }
    }

    public static unsafe class PersistentTreapErase
    {
        // Removes a single occurrence of val (if present) and returns the new root.
        // Persistent: clones every node on the descent path, leaving the input tree intact.
        public static int Run<T>(T* nodes, int* left, int* right, int* prio, int* size, int* allocCnt, int root, T val)
            where T : unmanaged, IComparable<T>
        {
            if (root == 0) return 0;
            int cmp = nodes[root].CompareTo(val);
            if (cmp == 0)
            {
                // Found a matching node: drop it by merging its two children.
                return PersistentTreapMerge.Run(nodes, left, right, prio, size, left[root], right[root], allocCnt);
            }
            int newNode = PersistentTreapNode.CloneNode(nodes, left, right, prio, size, root, allocCnt);
            if (cmp < 0)
            {
                right[newNode] = Run(nodes, left, right, prio, size, allocCnt, right[newNode], val);
            }
            else
            {
                left[newNode] = Run(nodes, left, right, prio, size, allocCnt, left[newNode], val);
            }
            PersistentTreapNode.Update(left, right, size, newNode);
            return newNode;
        }
    }

    public static unsafe class PersistentTreapFind
    {
        public static bool Run<T>(T* nodes, int* left, int* right, int root, T val)
            where T : unmanaged, IComparable<T>
        {
            int cur = root;
            while (cur != 0)
            {
                int cmp = nodes[cur].CompareTo(val);
                if (cmp == 0) return true;
                int* child = cmp < 0 ? right : left;
                cur = child[cur];
            }
            return false;
        }
    }
}
