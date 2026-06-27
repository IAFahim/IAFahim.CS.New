namespace IAFahim.Graph.TreeIsomorphism
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class UnorderedTreeEditDistance
    {
        // Unordered tree edit distance (unit costs: insert = delete = 1, relabel = 0, structure-only)
        // is NP-hard (Zhang-Statman-Shasha 1992) under the unrestricted operation set, so the
        // unconstrained Run stays a documented throw. RunConstrained provides a polynomial exact
        // value for the CONSTRAINED (aligned) variant: a matched node's children are matched pairwise
        // via min-cost assignment (no delete-skips across levels); cost of an unmatched child = its
        // subtree size. This is the well-defined tree-alignment distance and equals the true distance
        // whenever the optimal edit never relocates a subtree across a level boundary. Trees are parent
        // arrays; root = p[i] < 0.
        public static int RunConstrained(int* p1, int n1, int* p2, int n2)
        {
            if (n1 == 0 && n2 == 0) return 0;
            if (n1 == 0) return n2;
            if (n2 == 0) return n1;

            BuildAdj(p1, n1, out int* ch1, out int* nx1, out int* hd1);
            BuildAdj(p2, n2, out int* ch2, out int* nx2, out int* hd2);
            int root1 = FindRoot(p1, n1);
            int root2 = FindRoot(p2, n2);
            int* size1 = BuildSizes(n1, root1, ch1, nx1, hd1);
            int* size2 = BuildSizes(n2, root2, ch2, nx2, hd2);
            int* post1 = BuildPost(n1, root1, ch1, nx1, hd1);
            int* post2 = BuildPost(n2, root2, ch2, nx2, hd2);

            int dim = n1 > n2 ? n1 : n2;
            int* ed = (int*)Marshal.AllocHGlobal(sizeof(int) * n1 * n2);
            int* matchBuf = (int*)Marshal.AllocHGlobal(sizeof(int) * (dim + 1) * 2);
            int* hungCost = (int*)Marshal.AllocHGlobal(sizeof(int) * (dim + 1) * (dim + 1));
            try
            {
                for (int i = 0; i < n1 * n2; i++) ed[i] = 0;
                for (int ii = 0; ii < n1; ii++)
                {
                    int a = post1[ii];
                    for (int jj = 0; jj < n2; jj++)
                    {
                        int b = post2[jj];
                        ed[a * n2 + b] = SolveNode(a, b, ch1, nx1, hd1, ch2, nx2, hd2, size1, size2, ed, n2, matchBuf, hungCost);
                    }
                }
                return root1 >= 0 && root2 >= 0 ? ed[root1 * n2 + root2] : Math.Max(n1, n2);
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr)ch1); Marshal.FreeHGlobal((IntPtr)nx1); Marshal.FreeHGlobal((IntPtr)hd1);
                Marshal.FreeHGlobal((IntPtr)ch2); Marshal.FreeHGlobal((IntPtr)nx2); Marshal.FreeHGlobal((IntPtr)hd2);
                Marshal.FreeHGlobal((IntPtr)size1); Marshal.FreeHGlobal((IntPtr)size2);
                Marshal.FreeHGlobal((IntPtr)post1); Marshal.FreeHGlobal((IntPtr)post2);
                Marshal.FreeHGlobal((IntPtr)ed);
                Marshal.FreeHGlobal((IntPtr)matchBuf);
                Marshal.FreeHGlobal((IntPtr)hungCost);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* p1, int n1, int* p2, int n2)
        {
            throw new NotImplementedException(
                "Unordered tree edit distance is NP-hard (Zhang-Statman-Shasha 1992); the unconstrained " +
                "parent-array contract admits no correct polynomial/Burst-friendly algorithm. Use " +
                "RunConstrained for the polynomial aligned (direct-child-matching) distance, or " +
                "OrderedTreeEditDistance / TreeIsomorphismAhU.");
        }

        private static int SolveNode(int a, int b,
            int* ch1, int* nx1, int* hd1, int* ch2, int* nx2, int* hd2,
            int* size1, int* size2, int* ed, int n2, int* matchBuf, int* hungCost)
        {
            int ca = CountCh(hd1, nx1, a);
            int cb = CountCh(hd2, nx2, b);
            if (ca == 0 && cb == 0) return 0;
            if (ca == 0) { int s = 0; for (int e = hd2[b]; e != -1; e = nx2[e]) s += size2[ch2[e]]; return s; }
            if (cb == 0) { int s = 0; for (int e = hd1[a]; e != -1; e = nx1[e]) s += size1[ch1[e]]; return s; }

            int dim = ca > cb ? ca : cb;
            int* chA = matchBuf;
            int* chB = matchBuf + dim;
            int idx = 0;
            for (int e = hd1[a]; e != -1; e = nx1[e]) chA[idx++] = ch1[e];
            idx = 0;
            for (int e = hd2[b]; e != -1; e = nx2[e]) chB[idx++] = ch2[e];

            for (int i = 0; i < dim; i++)
                for (int j = 0; j < dim; j++)
                {
                    int c;
                    if (i < ca && j < cb) c = ed[chA[i] * n2 + chB[j]];
                    else if (i < ca) c = size1[chA[i]];
                    else if (j < cb) c = size2[chB[j]];
                    else c = 0;
                    hungCost[i * dim + j] = c;
                }
            return Assign(hungCost, dim);
        }

        private static int Assign(int* cost, int n)
        {
            if (n == 0) return 0;
            if (n > 20) return Hungarian(cost, n);
            int size = 1 << n;
            int* dp = stackalloc int[size];
            dp[0] = 0;
            int inf = int.MaxValue >> 2;
            for (int mask = 1; mask < size; mask++)
            {
                int i = Popcount(mask) - 1;
                int best = inf;
                int m = mask;
                while (m != 0)
                {
                    int lob = m & (-m);
                    int j = Log2(lob);
                    int prev = dp[mask ^ lob];
                    if (prev < inf)
                    {
                        int c = prev + cost[i * n + j];
                        if (c < best) best = c;
                    }
                    m ^= lob;
                }
                dp[mask] = best;
            }
            return dp[size - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Popcount(int x)
        {
            x = x - ((x >> 1) & 0x55555555);
            x = (x & 0x33333333) + ((x >> 2) & 0x33333333);
            x = (x + (x >> 4)) & 0x0F0F0F0F;
            return (x * 0x01010101) >> 24;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Log2(int v) => BitShift(v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BitShift(int v)
        {
            int r = 0; while ((1 << r) != v) r++; return r;
        }

        private static int Hungarian(int* cost, int n)
        {
            if (n == 0) return 0;
            int* u = stackalloc int[n + 1];
            int* v = stackalloc int[n + 1];
            int* p = stackalloc int[n + 1];
            int* way = stackalloc int[n + 1];
            int* minv = stackalloc int[n + 1];
            byte* used = stackalloc byte[n + 1];
            for (int i = 0; i <= n; i++) { u[i] = 0; v[i] = 0; p[i] = 0; way[i] = 0; }
            for (int i = 1; i <= n; i++)
            {
                p[0] = i;
                int j0 = 0;
                for (int k = 0; k <= n; k++) { minv[k] = int.MaxValue; used[k] = 0; }
                do
                {
                    used[j0] = 1;
                    int i0 = p[j0], delta = int.MaxValue, j1 = 0;
                    for (int j = 1; j <= n; j++)
                    {
                        if (used[j] != 0) continue;
                        int cur = cost[(i0 - 1) * n + (j - 1)] - u[i0] - v[j];
                        if (cur < minv[j]) { minv[j] = cur; way[j] = j0; }
                        if (minv[j] < delta) { delta = minv[j]; j1 = j; }
                    }
                    for (int j = 0; j <= n; j++)
                    {
                        if (used[j] != 0) u[p[j]] += delta;
                        else { minv[j] -= delta; v[j] += delta; }
                    }
                    j0 = j1;
                } while (p[j0] != 0);
                do { int j1 = way[j0]; p[j0] = p[j1]; j0 = j1; } while (j0 != 0);
            }
            int total = 0;
            for (int j = 1; j <= n; j++) total += cost[(p[j] - 1) * n + (j - 1)];
            return total;
        }

        private static void BuildAdj(int* p, int n, out int* child, out int* nextE, out int* head)
        {
            child = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            nextE = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            head = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            for (int i = 0; i < n; i++) head[i] = -1;
            int edge = 0;
            for (int c = 0; c < n; c++)
            {
                int par = p[c];
                if (par < 0 || par >= n) continue;
                child[edge] = c;
                nextE[edge] = head[par];
                head[par] = edge;
                edge++;
            }
        }

        private static int CountCh(int* head, int* nextE, int node)
        {
            int c = 0;
            for (int e = head[node]; e != -1; e = nextE[e]) c++;
            return c;
        }

        private static int* BuildSizes(int n, int root, int* child, int* nextE, int* head)
        {
            int* size = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            int* post = BuildPost(n, root, child, nextE, head);
            for (int i = 0; i < n; i++) size[i] = 1;
            for (int i = 0; i < n; i++)
            {
                int node = post[i];
                for (int e = head[node]; e != -1; e = nextE[e]) size[node] += size[child[e]];
            }
            Marshal.FreeHGlobal((IntPtr)post);
            return size;
        }

        private static int* BuildPost(int n, int root, int* child, int* nextE, int* head)
        {
            int* order = (int*)Marshal.AllocHGlobal(sizeof(int) * n);
            int* stack = (int*)Marshal.AllocHGlobal(sizeof(int) * (n + 1));
            int sp = 0, oc = 0;
            if (root >= 0) stack[sp++] = root;
            while (sp > 0)
            {
                int node = stack[--sp];
                if (oc >= n) break;
                order[oc++] = node;
                for (int e = head[node]; e != -1; e = nextE[e]) stack[sp++] = child[e];
            }
            for (int i = 0; i < oc / 2; i++) { int t = order[i]; order[i] = order[oc - 1 - i]; order[oc - 1 - i] = t; }
            Marshal.FreeHGlobal((IntPtr)stack);
            return order;
        }

        private static int FindRoot(int* p, int n)
        {
            for (int i = 0; i < n; i++) if (p[i] < 0) return i;
            return -1;
        }
    }
}
