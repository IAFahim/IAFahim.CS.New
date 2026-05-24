namespace IAFahim.DS.PersistentDsu
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PersistentDsu
    {
        public static int Build(int l, int r, int* parent, int* size, int* allocCnt, int* lc, int* rc)
        {
            int node = ++(*allocCnt);
            if (l == r) { parent[node] = l; size[node] = 1; return node; }
            int mid = (l + r) >> 1;
            lc[node] = Build(l, mid, parent, size, allocCnt, lc, rc);
            rc[node] = Build(mid + 1, r, parent, size, allocCnt, lc, rc);
            return node;
        }

        public static int Update(int root, int l, int r, int idx, int val, int s, int* parent, int* size, int* allocCnt, int* lc, int* rc)
        {
            int node = ++(*allocCnt);
            lc[node] = lc[root]; rc[node] = rc[root];
            if (l == r) { parent[node] = val; size[node] = s; return node; }
            int mid = (l + r) >> 1;
            if (idx <= mid) lc[node] = Update(lc[root], l, mid, idx, val, s, parent, size, allocCnt, lc, rc);
            else rc[node] = Update(rc[root], mid + 1, r, idx, val, s, parent, size, allocCnt, lc, rc);
            return node;
        }

        public static int Query(int root, int l, int r, int idx, int* parent, int* lc, int* rc, out int s, int* size)
        {
            if (l == r) { s = size[root]; return parent[root]; }
            int mid = (l + r) >> 1;
            if (idx <= mid) return Query(lc[root], l, mid, idx, parent, lc, rc, out s, size);
            return Query(rc[root], mid + 1, r, idx, parent, lc, rc, out s, size);
        }

        public static int Find(int root, int n, int x, int* parent, int* lc, int* rc, int* size, out int s)
        {
            while (true)
            {
                int p = Query(root, 0, n - 1, x, parent, lc, rc, out s, size);
                if (p == x) return x;
                x = p;
            }
        }

        public static int Union(int root, int n, int a, int b, int* parent, int* size, int* allocCnt, int* lc, int* rc)
        {
            int ra = Find(root, n, a, parent, lc, rc, size, out int sa);
            int rb = Find(root, n, b, parent, lc, rc, size, out int sb);
            if (ra == rb) return root;
            if (sa < sb) { Swap(ref ra, ref rb); Swap(ref sa, ref sb); }
            int nextRoot = Update(root, 0, n - 1, rb, ra, sb, parent, size, allocCnt, lc, rc);
            if (sa == sb) nextRoot = Update(nextRoot, 0, n - 1, ra, ra, sa + 1, parent, size, allocCnt, lc, rc);
            return nextRoot;
        }

        private static void Swap(ref int a, ref int b) { int t = a; a = b; b = t; }
    }
}
