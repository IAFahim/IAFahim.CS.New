namespace IAFahim.DS.RollbackStack
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RollbackStack
    {
        public static void Init(void* mem, int capacity)
        {
            *(int*)mem = 0;
        }

        public static int Snapshot(void* mem)
        {
            return *(int*)mem;
        }

        public static void Rollback(void* mem, int targetSize, int sizeOfT)
        {
            *(int*)mem = targetSize;
        }
    }

    public static unsafe class UndoableUnionFind
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Snapshot(int* parent, int* size, int* history, int histSize)
        {
            return histSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rollback(int* parent, int* size, int* history, int targetHistSize, int* currentHistSize)
        {
            while (*currentHistSize > targetHistSize)
            {
                *currentHistSize -= 2;
                int child = history[*currentHistSize];
                int sz = history[*currentHistSize + 1];
                int par = parent[child];
                parent[child] = child;
                size[par] = sz;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Find(int* parent, int x)
        {
            while (parent[x] != x) x = parent[x];
            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Union(int* parent, int* size, int* history, int* histSize, int a, int b)
        {
            int ra = Find(parent, a);
            int rb = Find(parent, b);
            if (ra == rb) return false;
            if (size[ra] < size[rb])
            {
                int tmp = ra; ra = rb; rb = tmp;
            }
            history[(*histSize)++] = rb;
            history[(*histSize)++] = size[ra];
            parent[rb] = ra;
            size[ra] += size[rb];
            return true;
        }
    }

    public static unsafe class UndoableBipartiteDsu
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Snapshot(int* parent, int* parity, int* history, int histSize)
        {
            return histSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rollback(int* parent, int* parity, int* history, int targetHistSize, int* currentHistSize)
        {
            while (*currentHistSize > targetHistSize)
            {
                *currentHistSize -= 3;
                int node = history[*currentHistSize + 2];
                parity[node] = history[*currentHistSize + 1];
                parent[node] = history[*currentHistSize];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Find(int* parent, int* parity, int x)
        {
            int root = x;
            int acc = 0;
            while (parent[root] != root)
            {
                acc ^= parity[root];
                root = parent[root];
            }
            parity[x] ^= acc;
            parent[x] = root;
            return root;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Union(int* parent, int* parity, int* history, int* histSize, int a, int b)
        {
            int ra = Find(parent, parity, a);
            int rb = Find(parent, parity, b);
            if (ra == rb)
            {
                return ((parity[a] ^ parity[b]) & 1) == 0;
            }
            int pa = parent[a];
            int pb = parent[b];
            history[(*histSize)++] = pa;
            history[(*histSize)++] = parity[pa];
            history[(*histSize)++] = pb;
            if (parity[a] == parity[b])
            {
                if (pa > pb)
                {
                    parent[pa] = pb;
                    parity[pa] = 1;
                }
                else
                {
                    parent[pb] = pa;
                    parity[pb] = 1;
                }
            }
            else
            {
                if (pa > pb)
                {
                    parent[pa] = pb;
                    parity[pa] = 0;
                }
                else
                {
                    parent[pb] = pa;
                    parity[pb] = 0;
                }
            }
            return true;
        }
    }

    public static unsafe class UndoableHeap
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Parent(int i) => (i - 1) >> 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Left(int i) => (i << 1) + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Right(int i) => (i << 1) + 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Less<T>(T* ptr, int* priority, int a, int b)
            where T : unmanaged, IComparable<T>
        {
            return priority[a] < priority[b];
        }

        public static void HeapifyDown<T>(T* ptr, int* priority, int* heapIdx, int* history, int* histSize, int n, int i)
            where T : unmanaged, IComparable<T>
        {
            while (true)
            {
                int smallest = i;
                int l = Left(i);
                int r = Right(i);
                if (l < n && Less(ptr, priority, l, smallest)) smallest = l;
                if (r < n && Less(ptr, priority, r, smallest)) smallest = r;
                if (smallest == i) break;
                history[(*histSize)++] = i;
                history[(*histSize)++] = smallest;
                history[(*histSize)++] = heapIdx[i];
                history[(*histSize)++] = heapIdx[smallest];
                int tmpIdx = heapIdx[i];
                heapIdx[i] = heapIdx[smallest];
                heapIdx[smallest] = tmpIdx;
                T tmp = ptr[i];
                ptr[i] = ptr[smallest];
                ptr[smallest] = tmp;
                i = smallest;
            }
        }

        public static void HeapifyUp<T>(T* ptr, int* priority, int* heapIdx, int* history, int* histSize, int i)
            where T : unmanaged, IComparable<T>
        {
            while (i > 0)
            {
                int p = Parent(i);
                if (!Less(ptr, priority, i, p)) break;
                history[(*histSize)++] = p;
                history[(*histSize)++] = i;
                history[(*histSize)++] = heapIdx[p];
                history[(*histSize)++] = heapIdx[i];
                int tmpIdx = heapIdx[p];
                heapIdx[p] = heapIdx[i];
                heapIdx[i] = tmpIdx;
                T tmp = ptr[p];
                ptr[p] = ptr[i];
                ptr[i] = tmp;
                i = p;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Snapshot(int* heapSize)
        {
            return *heapSize;
        }

        public static void Rollback<T>(T* ptr, int* priority, int* heapIdx, int* history, int targetSize, int* currentHistSize, int* heapSize)
            where T : unmanaged, IComparable<T>
        {
            *heapSize = targetSize;
            for (int i = (targetSize - 1) >> 1; i >= 0; i--)
            {
                HeapifyDown(ptr, priority, heapIdx, history, currentHistSize, targetSize, i);
            }
        }
    }
}