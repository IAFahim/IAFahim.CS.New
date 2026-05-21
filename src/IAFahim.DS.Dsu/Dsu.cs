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
            while (parent[root] != root) root = parent[root];
            while (parent[x] != root)
            {
                int next = parent[x];
                parent[x] = root;
                x = next;
            }
            return root;
        }

        public static int RunPathCompression(int* parent, int x)
        {
            if (parent[x] == x) return x;
            return parent[x] = RunPathCompression(parent, parent[x]);
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
                *currentHistSize -= 3;
                int sz = history[*currentHistSize + 2];
                int child = history[*currentHistSize + 1];
                int par = history[*currentHistSize];
                parent[child] = child;
                size[par] = sz;
            }
        }
    }

    public static unsafe class DsuRollbackUnion
    {
        public static bool Run(int* parent, int* size, int* history, int* histSize, int a, int b)
        {
            int ra = DsuFind.Run(parent, a);
            int rb = DsuFind.Run(parent, b);
            if (ra == rb) return false;
            if (size[ra] < size[rb])
            {
                int tmp = ra; ra = rb; rb = tmp;
            }
            history[(*histSize)++] = ra;
            history[(*histSize)++] = rb;
            history[(*histSize)++] = size[ra];
            parent[rb] = ra;
            size[ra] += size[rb];
            return true;
        }
    }

    public static unsafe class DsuUndo
    {
        public static void Run(int* parent, int* size, int a, int b)
        {
            int ra = DsuFind.Run(parent, a);
            int rb = DsuFind.Run(parent, b);
            if (ra == rb) return;
            if (size[ra] > size[rb])
            {
                parent[rb] = ra;
                size[ra] -= size[rb];
            }
            else
            {
                parent[ra] = rb;
                size[rb] -= size[ra];
            }
        }
    }

    public static unsafe class DsuBipartiteAdd
    {
        public static bool Run(int* parent, int* parity, int* size, int* hist, int* histSize, int a, int b)
        {
            int ra = DsuFind.Run(parent, a);
            int rb = DsuFind.Run(parent, b);
            if (ra == rb)
            {
                return ((parity[a] ^ parity[b]) & 1) == 0;
            }
            hist[(*histSize)++] = ra;
            hist[(*histSize)++] = rb;
            hist[(*histSize)++] = size[ra];
            hist[(*histSize)++] = size[rb];
            hist[(*histSize)++] = parent[ra];
            hist[(*histSize)++] = parent[rb];
            int da = parity[a];
            int db = parity[b];
            if (size[ra] > size[rb])
            {
                parent[rb] = ra;
                parity[rb] = da ^ db ^ 1;
                size[ra] += size[rb];
            }
            else
            {
                parent[ra] = rb;
                parity[ra] = da ^ db ^ 1;
                size[rb] += size[ra];
            }
            return true;
        }
    }

    public static unsafe class DsuParityFind
    {
        public static int Run(int* parent, int* parity, int x)
        {
            if (parent[x] == x) return parity[x];
            int p = parent[x];
            int root = DsuParityFind.Run(parent, parity, p);
            parity[x] ^= parity[p];
            parent[x] = root;
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