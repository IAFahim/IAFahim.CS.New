namespace IAFahim.DS.RollbackStack
{
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
                *currentHistSize -= 2;
                int loser = history[*currentHistSize];
                parity[loser] = history[*currentHistSize + 1];
                parent[loser] = loser;
            }
        }

        // Compression-free walk to the root. Path compression is NOT used because
        // it mutates parent/parity irreversibly without being recorded in history,
        // which would corrupt Rollback.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Find(int* parent, int* parity, int x)
        {
            while (parent[x] != x) x = parent[x];
            return x;
        }

        // Accumulated parity of x relative to its root (compression-free).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ParityToRoot(int* parent, int* parity, int x)
        {
            int acc = 0;
            while (parent[x] != x)
            {
                acc ^= parity[x];
                x = parent[x];
            }
            return acc & 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Union(int* parent, int* parity, int* history, int* histSize, int a, int b)
        {
            int ra = Find(parent, parity, a);
            int rb = Find(parent, parity, b);
            int pa = ParityToRoot(parent, parity, a);
            int pb = ParityToRoot(parent, parity, b);
            if (ra == rb)
            {
                return ((pa ^ pb) & 1) == 0;
            }
            // Edge (a, b) requires opposite colors: parity(a) ^ parity(b) == 1.
            // Attach the larger-indexed root (loser) under the smaller-indexed root,
            // choosing the loser's stored parity so the relation holds across roots.
            // newParity is direction-independent: pa ^ pb ^ 1.
            int newParity = pa ^ pb ^ 1;
            int loser;
            int winner;
            if (ra > rb)
            {
                loser = ra;
                winner = rb;
            }
            else
            {
                loser = rb;
                winner = ra;
            }
            history[(*histSize)++] = loser;
            history[(*histSize)++] = parity[loser];
            parent[loser] = winner;
            parity[loser] = newParity;
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
            where T : unmanaged
        {
            return priority[a] < priority[b];
        }

        public static void HeapifyDown<T>(T* ptr, int* priority, int* heapIdx, int* history, int* histSize, int n, int i)
            where T : unmanaged
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
            where T : unmanaged
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

        // Snapshot captures the history length (the undo marker). Rollback unwinds
        // exactly the swaps recorded since this point, so this MUST return the
        // history length, not the heap size.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Snapshot(int* histSize)
        {
            return *histSize;
        }

        // Undo every swap recorded after targetHistSize by replaying it in reverse,
        // restoring both the heap slots (ptr) and the id->slot mapping (heapIdx).
        // This is O(K) in the number of swaps since the snapshot and reproduces the
        // exact pre-snapshot layout. heapSize is reset to the caller-supplied
        // targetHeapSize captured at snapshot time.
        public static void Rollback<T>(T* ptr, int* priority, int* heapIdx, int* history, int targetHistSize, int* currentHistSize, int* heapSize, int targetHeapSize)
            where T : unmanaged
        {
            while (*currentHistSize > targetHistSize)
            {
                *currentHistSize -= 4;
                int i = history[*currentHistSize];
                int other = history[*currentHistSize + 1];
                heapIdx[i] = history[*currentHistSize + 2];
                heapIdx[other] = history[*currentHistSize + 3];
                T tmp = ptr[i];
                ptr[i] = ptr[other];
                ptr[other] = tmp;
            }
            *heapSize = targetHeapSize;
        }
    }
}