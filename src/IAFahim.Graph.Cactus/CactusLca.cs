namespace IAFahim.Graph.Cactus
{
    using System.Runtime.CompilerServices;

    public static unsafe class CactusLca
    {
        // Binary-lifting LCA on a prebuilt tree (block-cut tree, cactus tree, etc.).
        // parent[root] = -1; depth[root] = 0. up[v * maxLog + k] = 2^k-th ancestor.
        // Call BuildJump then Query.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxLog(int n)
        {
            int lg = 1;
            while ((1 << lg) <= n) lg++;
            return lg;
        }

        public static void BuildJump(int n, int* parent, int* depth, int* up, int maxLog)
        {
            for (int v = 0; v < n; v++)
            {
                up[v * maxLog + 0] = parent[v];
                for (int k = 1; k < maxLog; k++)
                {
                    int mid = up[v * maxLog + (k - 1)];
                    up[v * maxLog + k] = mid < 0 ? -1 : up[mid * maxLog + (k - 1)];
                }
            }
            // depths: if caller left zeros, recompute from parents when possible
            for (int v = 0; v < n; v++)
            {
                if (parent[v] < 0) depth[v] = 0;
            }
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int v = 0; v < n; v++)
                {
                    int p = parent[v];
                    if (p >= 0 && depth[v] != depth[p] + 1)
                    {
                        // only set if parent depth known (root chain)
                        if (parent[p] < 0 || depth[p] > 0 || p == 0)
                        {
                            int nd = depth[p] + 1;
                            if (depth[v] != nd) { depth[v] = nd; changed = true; }
                        }
                    }
                }
            }
        }

        // Build depths via BFS from roots (parent[r]<0).
        public static void BuildDepth(int n, int* parent, int* depth, int* queue)
        {
            for (int i = 0; i < n; i++) depth[i] = -1;
            int qh = 0, qt = 0;
            for (int i = 0; i < n; i++)
                if (parent[i] < 0) { depth[i] = 0; queue[qt++] = i; }
            while (qh < qt)
            {
                int u = queue[qh++];
                for (int v = 0; v < n; v++)
                {
                    if (parent[v] == u && depth[v] < 0)
                    {
                        depth[v] = depth[u] + 1;
                        queue[qt++] = v;
                    }
                }
            }
        }

        public static int Query(int u, int v, int* depth, int* up, int maxLog)
        {
            if (depth[u] < depth[v]) { int t = u; u = v; v = t; }
            int diff = depth[u] - depth[v];
            for (int k = 0; k < maxLog; k++)
                if (((diff >> k) & 1) != 0) u = up[u * maxLog + k];
            if (u == v) return u;
            for (int k = maxLog - 1; k >= 0; k--)
            {
                int uu = up[u * maxLog + k];
                int vv = up[v * maxLog + k];
                if (uu != vv && uu >= 0)
                {
                    u = uu;
                    v = vv;
                }
            }
            return up[u * maxLog];
        }

        // Backward-compatible name: requires prebuilt jump tables.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int u, int v, int* depth, int* up, int maxLog)
            => Query(u, v, depth, up, maxLog);
    }

    public static unsafe class BlockCutTreeLca
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxLog(int n) => CactusLca.MaxLog(n);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BuildJump(int n, int* parent, int* depth, int* up, int maxLog)
            => CactusLca.BuildJump(n, parent, depth, up, maxLog);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BuildDepth(int n, int* parent, int* depth, int* queue)
            => CactusLca.BuildDepth(n, parent, depth, queue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Query(int u, int v, int* depth, int* up, int maxLog)
            => CactusLca.Query(u, v, depth, up, maxLog);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int u, int v, int* depth, int* up, int maxLog)
            => CactusLca.Run(u, v, depth, up, maxLog);
    }
}
