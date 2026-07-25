namespace IAFahim.Graph.Dominator
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class SimpleDominators
    {
        // Iterative data-flow immediate dominators for a flowgraph.
        // head/to/next adjacency (edge index 0 unused). idom[root]=root; idom[v]=idom for others.
        public static bool Run(int n, int root, int* head, int* to, int* next, int* idom)
        {
            if (n <= 0) return false;
            if ((uint)root >= (uint)n) return false;

            byte* reachable = (byte*)Marshal.AllocHGlobal(n);
            int* order = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* stack = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* predHead = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* predTo = (int*)Marshal.AllocHGlobal((n * n + 2) * sizeof(int));
            int* predNext = (int*)Marshal.AllocHGlobal((n * n + 2) * sizeof(int));
            int orderLen = 0;
            for (int i = 0; i < n; i++) { reachable[i] = 0; idom[i] = -1; predHead[i] = 0; }

            // Reverse edges for predecessor scan.
            int pe = 1;
            for (int u = 0; u < n; u++)
            {
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    predTo[pe] = u;
                    predNext[pe] = predHead[v];
                    predHead[v] = pe++;
                }
            }

            int ss = 0;
            stack[ss++] = root;
            reachable[root] = 1;
            while (ss > 0)
            {
                int u = stack[--ss];
                order[orderLen++] = u;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (reachable[v] == 0)
                    {
                        reachable[v] = 1;
                        stack[ss++] = v;
                    }
                }
            }

            int* pos = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            for (int i = 0; i < n; i++) pos[i] = -1;
            for (int i = 0; i < orderLen; i++) pos[order[i]] = i;

            idom[root] = root;
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int oi = 1; oi < orderLen; oi++)
                {
                    int b = order[oi];
                    int newIDom = -1;
                    for (int e = predHead[b]; e != 0; e = predNext[e])
                    {
                        int p = predTo[e];
                        if (reachable[p] == 0 || idom[p] < 0) continue;
                        if (newIDom < 0) newIDom = p;
                        else newIDom = Intersect(idom, pos, p, newIDom);
                    }
                    if (newIDom >= 0 && idom[b] != newIDom)
                    {
                        idom[b] = newIDom;
                        changed = true;
                    }
                }
            }

            bool ok = true;
            for (int i = 0; i < n; i++)
                if (reachable[i] != 0 && idom[i] < 0) ok = false;

            Marshal.FreeHGlobal((nint)pos);
            Marshal.FreeHGlobal((nint)predNext);
            Marshal.FreeHGlobal((nint)predTo);
            Marshal.FreeHGlobal((nint)predHead);
            Marshal.FreeHGlobal((nint)stack);
            Marshal.FreeHGlobal((nint)order);
            Marshal.FreeHGlobal((nint)reachable);
            return ok;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Dominates(int* idom, int a, int b)
        {
            int x = b;
            int guard = 0;
            while (x != a && x >= 0 && idom[x] != x && guard++ < 1000000)
                x = idom[x];
            return x == a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Intersect(int* idom, int* pos, int b1, int b2)
        {
            int f1 = b1, f2 = b2;
            while (f1 != f2)
            {
                while (pos[f1] > pos[f2]) f1 = idom[f1];
                while (pos[f2] > pos[f1]) f2 = idom[f2];
            }
            return f1;
        }
    }
}
