namespace IAFahim.DS.Dsu
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class DsuInit
    {
        public static void Run(int* parent, int* size, int n)
        {
            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
                size[i] = 1;
            }
        }
    }

    public static unsafe class DsuFind
    {
        public static int Run(int* parent, int x)
        {
            int root = x;
            while (parent[root] != root)
            {
                root = parent[root];
            }
            return root;
        }

        public static int RunPathCompression(int* parent, int x)
        {
            int root = x;
            while (parent[root] != root) root = parent[root];
            while (parent[x] != root)
            {
                int next = parent[x];
                parent[x] = root;
                x = next;
            }
            return root;
        }
    }

    public static unsafe class DsuUnion
    {
        public static bool Run(int* parent, int* size, int a, int b)
        {
            int ra = DsuFind.Run(parent, a);
            int rb = DsuFind.Run(parent, b);
            if (ra == rb) return false;
            if (size[ra] < size[rb])
            {
                parent[ra] = rb;
                size[rb] += size[ra];
            }
            else
            {
                parent[rb] = ra;
                size[ra] += size[rb];
            }
            return true;
        }
    }

    public static unsafe class DsuSame
    {
        public static bool Run(int* parent, int a, int b)
        {
            return DsuFind.Run(parent, a) == DsuFind.Run(parent, b);
        }
    }

    public static unsafe class DsuSize
    {
        public static int Run(int* parent, int* size, int x)
        {
            return size[DsuFind.Run(parent, x)];
        }
    }

    public static unsafe class DsuRollbackSnapshot
    {
        public static int Run(int* history, int histSize)
        {
            return histSize;
        }
    }

    public static unsafe class DsuRollback
    {
        public static void Run(int* parent, int* size, int* history, int targetHistSize, int* currentHistSize)
        {
            while (*currentHistSize > targetHistSize)
            {
                RollbackStep(parent, size, history, currentHistSize);
            }
        }

        private static void RollbackStep(int* parent, int* size, int* history, int* currentHistSize)
        {
            *currentHistSize -= 3;
            int par = history[*currentHistSize];
            int child = history[*currentHistSize + 1];
            int sz = history[*currentHistSize + 2];
            parent[child] = child;
            size[par] = sz;
        }
    }

    public static unsafe class DsuRollbackUnion
    {
        public static bool Run(int* parent, int* size, int* history, int* histSize, int a, int b)
        {
            int ra = FindSimple(parent, a);
            int rb = FindSimple(parent, b);
            if (ra == rb) return false;
            if (size[ra] < size[rb]) Swap(ref ra, ref rb);
            
            RecordHistory(history, histSize, ra, rb, size[ra]);
            parent[rb] = ra;
            size[ra] += size[rb];
            return true;
        }

        private static int FindSimple(int* parent, int x)
        {
            while (parent[x] != x) x = parent[x];
            return x;
        }

        private static void Swap(ref int a, ref int b) { int t = a; a = b; b = t; }

        private static void RecordHistory(int* history, int* histSize, int ra, int rb, int sizeRa)
        {
            history[(*histSize)++] = ra;
            history[(*histSize)++] = rb;
            history[(*histSize)++] = sizeRa;
        }
    }

    public static unsafe class DsuBipartiteAdd
    {
        public static bool Run(int* parent, int* parity, int* size, int* hist, int* histSize, int a, int b)
        {
            int ra = FindBipartiteRoot(parent, parity, a, out int pa);
            int rb = FindBipartiteRoot(parent, parity, b, out int pb);
            if (ra == rb) return (pa ^ pb) != 0;

            RecordBipartiteHistory(parent, size, hist, histSize, ra, rb);
            UpdateBipartiteMerge(parent, parity, size, ra, rb, pa, pb);
            return true;
        }

        private static int FindBipartiteRoot(int* parent, int* parity, int x, out int p)
        {
            p = 0;
            while (parent[x] != x) { p ^= parity[x]; x = parent[x]; }
            return x;
        }

        private static void RecordBipartiteHistory(int* parent, int* size, int* hist, int* histSize, int ra, int rb)
        {
            hist[(*histSize)++] = ra;
            hist[(*histSize)++] = rb;
            hist[(*histSize)++] = size[ra];
            hist[(*histSize)++] = size[rb];
            hist[(*histSize)++] = parent[ra];
            hist[(*histSize)++] = parent[rb];
        }

        private static void UpdateBipartiteMerge(int* parent, int* parity, int* size, int ra, int rb, int pa, int pb)
        {
            if (size[ra] > size[rb])
            {
                parent[rb] = ra;
                parity[rb] = pa ^ pb ^ 1;
                size[ra] += size[rb];
            }
            else
            {
                parent[ra] = rb;
                parity[ra] = pa ^ pb ^ 1;
                size[rb] += size[ra];
            }
        }
    }

    public static unsafe class DsuParityFind
    {
        public static int Run(int* parent, int* parity, int x)
        {
            int root = x;
            int acc = 0;
            while (parent[root] != root)
            {
                acc ^= parity[root];
                root = parent[root];
            }
            int cur = x;
            int curParity = 0;
            while (parent[cur] != root)
            {
                int next = parent[cur];
                int oldPar = parity[cur];
                parity[cur] = acc ^ curParity;
                curParity ^= oldPar;
                parent[cur] = root;
                cur = next;
            }
            return root;
        }
    }

    public static unsafe class DsuParityUnion
    {
        public static bool Run(int* parent, int* parity, int a, int b, int w)
        {
            int ra = DsuParityFind.Run(parent, parity, a);
            int rb = DsuParityFind.Run(parent, parity, b);
            if (ra == rb)
            {
                return ((parity[a] ^ parity[b]) & 1) == (w & 1);
            }
            if (parent[ra] == ra)
            {
                parent[ra] = rb;
                parity[ra] = parity[a] ^ parity[b] ^ w;
            }
            else
            {
                parent[rb] = ra;
                parity[rb] = parity[a] ^ parity[b] ^ w;
            }
            return true;
        }
    }

    public static unsafe class SmallToLargeMerge
    {
        public static void Run(int* parent, int* heavy, int n)
        {
            for (int i = 0; i < n; i++)
                heavy[i] = -1;
        }
    }
}