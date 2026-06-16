namespace IAFahim.Graph.Matching
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BlossomGeneral
    {
        private static int GetLca(int n, int* base_, int* parent, int* match, int* inPath, int u, int v)
        {
            for (int i = 0; i < n; i++) inPath[i] = 0;
            u = FindBase(base_, parent, match, u, inPath);
            return FindLca(base_, parent, match, v, inPath);
        }

        private static int FindBase(int* base_, int* parent, int* match, int u, int* inPath)
        {
            while (true)
            {
                u = base_[u];
                inPath[u] = 1;
                if (match[u] == -1) break;
                u = base_[parent[match[u]]];
            }
            return u;
        }

        private static int FindLca(int* base_, int* parent, int* match, int v, int* inPath)
        {
            while (true)
            {
                v = base_[v];
                if (inPath[v] == 1) return v;
                v = base_[parent[match[v]]];
            }
        }

        private static void Contract(int n, int* base_, int* parent, int* match, int* color, int* q, ref int qt, int u, int v, int lca)
        {
            while (base_[u] != lca)
            {
                parent[u] = v;
                int mv = match[u];
                if (color[mv] == 1) { color[mv] = 0; q[qt++] = mv; }
                
                UpdateBases(n, base_, base_[u], base_[mv], lca);
                v = mv;
                u = parent[v];
            }
        }

        private static void UpdateBases(int n, int* base_, int oldU, int oldMv, int lca)
        {
            for (int i = 0; i < n; i++)
                if (base_[i] == oldU || base_[i] == oldMv) base_[i] = lca;
        }

        private static bool FindAugmentingPath(int n, int* head, int* to, int* next, int* match, int* parent, int* base_, int* color, int* q, int* inPath, int s)
        {
            InitializeSearch(n, s, color, parent, base_, q, out int qh, out int qt);
            while (qh < qt)
            {
                int u = q[qh++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (base_[u] == base_[v] || match[u] == v) continue;
                    if (ProcessNeighbor(n, head, to, next, match, parent, base_, color, q, ref qt, inPath, u, v)) return true;
                }
            }
            return false;
        }

        private static void InitializeSearch(int n, int s, int* color, int* parent, int* base_, int* q, out int qh, out int qt)
        {
            for (int i = 0; i < n; i++) { color[i] = -1; parent[i] = -1; base_[i] = i; }
            qh = 0; qt = 0;
            color[s] = 0; q[qt++] = s;
        }

        private static bool ProcessNeighbor(int n, int* head, int* to, int* next, int* match, int* parent, int* base_, int* color, int* q, ref int qt, int* inPath, int u, int v)
        {
            if (color[v] == -1)
            {
                if (match[v] == -1) { AugmentPath(match, parent, u, v); return true; }
                color[v] = 1; parent[v] = u;
                int mv = match[v];
                color[mv] = 0; parent[mv] = v;
                q[qt++] = mv;
            }
            else if (color[v] == 0)
            {
                int lca = GetLca(n, base_, parent, match, inPath, u, v);
                Contract(n, base_, parent, match, color, q, ref qt, u, v, lca);
                Contract(n, base_, parent, match, color, q, ref qt, v, u, lca);
            }
            return false;
        }

        private static void AugmentPath(int* match, int* parent, int u, int v)
        {
            parent[v] = u;
            int cur = v;
            while (cur != -1)
            {
                int pNode = parent[cur];
                int nextMatched = match[pNode];
                match[cur] = pNode;
                match[pNode] = cur;
                cur = nextMatched;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int* head, int* to, int* next, int* match, int* base_, int* p, int* v, int* blossom, int* scratch)
        {
            for (int i = 0; i < n; i++) match[i] = -1;
            int result = 0;
            for (int s = 0; s < n; s++)
                if (match[s] == -1 && FindAugmentingPath(n, head, to, next, match, p, base_, v, scratch, blossom, s)) result++;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int* head, int* to, int* next, int* match, int* base_, int* p, int* v, int* blossom)
        {
            int* scratch = stackalloc int[n];
            return Run(n, head, to, next, match, base_, p, v, blossom, scratch);
        }
    }

    public static unsafe class WeightedBlossom
    {
        // Minimum-cost perfect assignment via successive shortest paths (Jonker-Volgenant).
        // Treats the n*n row-major matrix w as a complete bipartite graph between n rows
        // and n columns; produces a permutation in match[] (match[row] = assigned column)
        // minimizing the total weight sum over w[row*n + match[row]].
        // Caller guarantees n >= 0, w points to n*n valid longs, match points to n valid ints.
        private const long Infinity = long.MaxValue;
        private const int None = -1;
        // Dummy column index used as the alternating-tree root sentinel; lives one past the
        // real columns, so internal buffers are sized n + 1.
        private const int DummyOffset = 1;

        public static long Run(int n, long* w, int* match)
        {
            for (int row = 0; row < n; row++) match[row] = None;
            if (n == 0) return 0;

            int size = n + DummyOffset;
            long* u = stackalloc long[size];
            long* v = stackalloc long[size];
            long* minv = stackalloc long[size];
            int* way = stackalloc int[size];
            int* used = stackalloc int[size];
            // colRow[col] = row currently matched to column col (col in 1..n); colRow[0] is
            // the dummy slot holding the row being inserted. 0 means "unmatched".
            int* colRow = stackalloc int[size];
            for (int j = 0; j < size; j++) { u[j] = 0; v[j] = 0; colRow[j] = 0; }

            for (int row = 1; row <= n; row++)
                AssignRow(n, row, w, u, v, minv, way, used, colRow);

            return Finalize(n, w, colRow, match);
        }

        // Inserts one row into the assignment, finding the cheapest augmenting alternating
        // path from this row to an unmatched column and flipping the matching along it.
        private static void AssignRow(int n, int row, long* w, long* u, long* v, long* minv, int* way, int* used, int* colRow)
        {
            int size = n + DummyOffset;
            colRow[0] = row;
            int j0 = 0;
            for (int j = 0; j < size; j++) { minv[j] = Infinity; used[j] = 0; way[j] = 0; }

            do
            {
                used[j0] = 1;
                int i0 = colRow[j0];
                long delta = Infinity;
                int j1 = None;
                ScanColumns(n, i0, j0, w, u, v, minv, way, used, ref delta, ref j1);
                ApplyDelta(n, delta, u, v, minv, colRow, used);
                j0 = j1;
            } while (colRow[j0] != 0);

            BacktrackPath(way, colRow, j0);
        }

        // Relaxes every still-unused real column against the freshly added row i0, recording
        // the best predecessor and the global minimum reduced cost (delta) and its column.
        private static void ScanColumns(int n, int i0, int j0, long* w, long* u, long* v, long* minv, int* way, int* used, ref long delta, ref int j1)
        {
            long* wRow = w + (long)(i0 - 1) * n;
            long uRow = u[i0];
            for (int j = 1; j <= n; j++)
            {
                if (used[j] != 0) continue;
                long cur = wRow[j - 1] - uRow - v[j];
                if (cur < minv[j]) { minv[j] = cur; way[j] = j0; }
                if (minv[j] < delta) { delta = minv[j]; j1 = j; }
            }
        }

        // Shifts the dual potentials by delta so the chosen column's reduced cost reaches 0,
        // keeping all reduced costs non-negative (the SSSP/Dijkstra invariant).
        private static void ApplyDelta(int n, long delta, long* u, long* v, long* minv, int* colRow, int* used)
        {
            for (int j = 0; j <= n; j++)
            {
                if (used[j] != 0) { u[colRow[j]] += delta; v[j] -= delta; }
                else minv[j] -= delta;
            }
        }

        // Walks the recorded predecessors back to the root, flipping matched columns to
        // their new rows along the augmenting alternating path.
        private static void BacktrackPath(int* way, int* colRow, int j0)
        {
            do
            {
                int j1 = way[j0];
                colRow[j0] = colRow[j1];
                j0 = j1;
            } while (j0 != 0);
        }

        // Translates the internal column->row matching into the public row->column match[]
        // and accumulates the optimal total weight.
        private static long Finalize(int n, long* w, int* colRow, int* match)
        {
            long result = 0;
            for (int col = 1; col <= n; col++)
            {
                int row = colRow[col] - 1;
                match[row] = col - 1;
                result += w[(long)row * n + (col - 1)];
            }
            return result;
        }
    }
}