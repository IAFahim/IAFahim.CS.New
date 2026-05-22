namespace IAFahim.DS.PersistentDsu
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PersistentDsuNode
    {
        public static int NewNode(int* parent, int* size, int* leftChild, int* rightChild, int* allocCnt)
        {
            int idx = ++(*allocCnt);
            parent[idx] = idx;
            size[idx] = 1;
            leftChild[idx] = 0;
            rightChild[idx] = 0;
            return idx;
        }
    }

    public static unsafe class PersistentDsuInit
    {
        public static void Run(int* parent, int n)
        {
            for (int i = 0; i < n; i++) parent[i] = i;
        }

        public static int NewRoot(int* parent, int* size, int* leftChild, int* rightChild, int* prevRoot, int* allocCnt)
        {
            int root = ++(*allocCnt);
            parent[root] = prevRoot != null && *prevRoot != 0 ? parent[*prevRoot] : root;
            size[root] = prevRoot != null && *prevRoot != 0 ? size[*prevRoot] : 1;
            leftChild[root] = *prevRoot;
            rightChild[root] = 0;
            return root;
        }
    }

    public static unsafe class PersistentDsuFind
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* parent, int* leftChild, int* rightChild, int root, int x)
        {
            while (parent[root] != root && parent[root] != 0)
            {
                if (leftChild[root] != 0 && x < root) root = leftChild[root];
                else if (rightChild[root] != 0) root = rightChild[root];
                else break;
            }
            return parent[root] == root ? root : x;
        }
    }

    public static unsafe class PersistentDsuUnion
    {
        public static int Run(int* parent, int* size, int* leftChild, int* rightChild,
            int* prevRoot, int a, int b, int* allocCnt)
        {
            int ra = PersistentDsuFind.Run(parent, leftChild, rightChild, *prevRoot, a);
            int rb = PersistentDsuFind.Run(parent, leftChild, rightChild, *prevRoot, b);
            if (ra == rb) return *prevRoot;
            int newRoot = ++(*allocCnt);
            parent[newRoot] = newRoot;
            size[newRoot] = size[ra] + size[rb];
            leftChild[newRoot] = *prevRoot;
            rightChild[newRoot] = 0;
            if (size[ra] > size[rb])
            {
                parent[ra] = rb;
                size[rb] = size[ra] + size[rb];
                leftChild[newRoot] = ra;
                rightChild[newRoot] = rb;
            }
            else
            {
                parent[rb] = ra;
                size[ra] = size[ra] + size[rb];
                leftChild[newRoot] = rb;
                rightChild[newRoot] = ra;
            }
            return newRoot;
        }
    }
}