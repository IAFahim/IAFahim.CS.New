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
            int ra = a;
            while (parent[ra] != ra)
            {
                ra = parent[ra];
            }
            int rb = b;
            while (parent[rb] != rb)
            {
                rb = parent[rb];
            }
            if (ra == rb)
            {
                return false;
            }
            if (size[ra] < size[rb])
            {
                int tmp = ra;
                ra = rb;
                rb = tmp;
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
            if (parent[a] == b)
            {
                parent[a] = a;
                size[b] -= size[a];
            }
            else if (parent[b] == a)
            {
                parent[b] = b;
                size[a] -= size[b];
            }
        }
    }

    public static unsafe class DsuBipartiteAdd
    {
        public static bool Run(int* parent, int* parity, int* size, int* hist, int* histSize, int a, int b)
        {
            int ra = a;
            int pa = 0;
            while (parent[ra] != ra)
            {
                pa ^= parity[ra];
                ra = parent[ra];
            }
            int rb = b;
            int pb = 0;
            while (parent[rb] != rb)
            {
                pb ^= parity[rb];
                rb = parent[rb];
            }
            if (ra == rb)
            {
                return (pa ^ pb) != 0;
            }
            hist[(*histSize)++] = ra;
            hist[(*histSize)++] = rb;
            hist[(*histSize)++] = size[ra];
            hist[(*histSize)++] = size[rb];
            hist[(*histSize)++] = parent[ra];
            hist[(*histSize)++] = parent[rb];
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
            return true;
        }
    }

    public static unsafe class DsuParityFind
    {
        public static int Run(int* parent, int* parity, int x)
        {
            if (parent[x] == x)
            {
                return x;
            }
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