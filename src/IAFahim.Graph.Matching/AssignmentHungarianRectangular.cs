namespace IAFahim.Graph.Matching
{
    using System.Runtime.CompilerServices;

    public static unsafe class AssignmentHungarianRectangular
    {
        // Minimum-cost assignment on an n x m cost matrix via the Hungarian
        // (Kuhn-Munkres) algorithm. The matrix is padded to square N x N
        // (N = max(n, m)) with a large sentinel so unmatched rows/columns map
        // to padding and are reported as -1.
        //   cost[i * m + j]   = cost of row i, column j.
        //   matchLeft[i]      = column assigned to row i, or -1.
        //   matchRight[j]     = row assigned to column j, or -1.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* cost, int n, int m, int* matchLeft, int* matchRight)
        {
            for (int i = 0; i < n; i++) matchLeft[i] = -1;
            for (int j = 0; j < m; j++) matchRight[j] = -1;
            if (n <= 0 || m <= 0) return;

            int nm = n * m;
            long totalAbs = 0;
            for (int i = 0; i < nm; i++) totalAbs += cost[i] < 0 ? -cost[i] : cost[i];
            long big = totalAbs + 1;

            int bigDim = n > m ? n : m;

            long* c = stackalloc long[bigDim * bigDim];
            for (int i = 0; i < bigDim; i++)
                for (int j = 0; j < bigDim; j++)
                    c[i * bigDim + j] = (i < n && j < m) ? cost[i * m + j] : big;

            long* u = stackalloc long[bigDim + 1];
            long* v = stackalloc long[bigDim + 1];
            long* p = stackalloc long[bigDim + 1];
            long* way = stackalloc long[bigDim + 1];
            long* minv = stackalloc long[bigDim + 1];
            byte* used = stackalloc byte[bigDim + 1];
            for (int k = 0; k <= bigDim; k++) { u[k] = 0; v[k] = 0; p[k] = 0; way[k] = 0; }

            long inf = long.MaxValue >> 2;

            for (int i = 1; i <= bigDim; i++)
            {
                p[0] = i;
                int j0 = 0;
                for (int k = 0; k <= bigDim; k++) { minv[k] = inf; used[k] = 0; }
                int j1;
                do
                {
                    used[j0] = 1;
                    int i0 = (int)p[j0];
                    long delta = inf;
                    j1 = 0;
                    ScanColumns(c, u, v, used, minv, way, bigDim, i0, j0, ref delta, ref j1);
                    ApplyPotentials(p, u, v, used, minv, bigDim, delta);
                    j0 = j1;
                } while (p[j0] != 0);

                do
                {
                    j1 = (int)way[j0];
                    p[j0] = p[j1];
                    j0 = j1;
                } while (j0 != 0);
            }

            // p[j] (1-based) = row assigned to column j. Invert to col-per-row.
            int* colForRow = stackalloc int[bigDim + 1];
            for (int j = 1; j <= bigDim; j++) colForRow[(int)p[j]] = j; // p[j] in 1..bigDim

            for (int i = 0; i < n; i++)
            {
                int col1 = colForRow[i + 1];
                matchLeft[i] = col1 <= m ? col1 - 1 : -1;
            }
            for (int j = 0; j < m; j++)
            {
                long row1 = p[j + 1];
                matchRight[j] = (row1 >= 1 && row1 <= n) ? (int)row1 - 1 : -1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ScanColumns(long* c, long* u, long* v, byte* used, long* minv, long* way, int bigDim, int i0, int j0, ref long delta, ref int j1)
        {
            for (int j = 1; j <= bigDim; j++)
            {
                if (used[j] == 0)
                {
                    long cur = c[(i0 - 1) * bigDim + (j - 1)] - u[i0] - v[j];
                    if (cur < minv[j]) { minv[j] = cur; way[j] = j0; }
                    if (minv[j] < delta) { delta = minv[j]; j1 = j; }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ApplyPotentials(long* p, long* u, long* v, byte* used, long* minv, int bigDim, long delta)
        {
            for (int j = 0; j <= bigDim; j++)
            {
                if (used[j] != 0) { u[(int)p[j]] += delta; v[j] -= delta; }
                else minv[j] -= delta;
            }
        }
    }
}
