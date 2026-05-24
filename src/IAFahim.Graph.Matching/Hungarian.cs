namespace IAFahim.Graph.Matching
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class HungarianMin
    {
        public static long Run(int n, long* a, long* matchL, long* matchR)
        {
            long* u = stackalloc long[n + 1], v = stackalloc long[n + 1];
            int* p = stackalloc int[n + 1], way = stackalloc int[n + 1];
            InitializeArrays(n, u, v, p, way);

            for (int i = 1; i <= n; i++)
            {
                p[0] = i;
                int j0 = 0;
                long* minv = stackalloc long[n + 1];
                for (int j = 0; j <= n; j++) minv[j] = long.MaxValue;
                int* used = stackalloc int[n + 1];
                for (int j = 0; j <= n; j++) used[j] = 0;

                do
                {
                    used[j0] = 1;
                    int i0 = p[j0], j1 = 0;
                    long delta = long.MaxValue;
                    UpdateMinvAndWay(n, i0, j0, a, u, v, minv, way, used, ref delta, ref j1);
                    ApplyDelta(n, delta, u, v, minv, p, used);
                    j0 = j1;
                } while (p[j0] != 0);

                BacktrackPath(way, p, j0);
            }
            return FinalizeResult(n, a, p, matchL, matchR);
        }

        private static void InitializeArrays(int n, long* u, long* v, int* p, int* way)
        {
            for (int i = 0; i <= n; i++) { u[i] = 0; v[i] = 0; p[i] = 0; way[i] = 0; }
        }

        private static void UpdateMinvAndWay(int n, int i0, int j0, long* a, long* u, long* v, long* minv, int* way, int* used, ref long delta, ref int j1)
        {
            for (int j = 1; j <= n; j++)
            {
                if (used[j] != 0) continue;
                long cur = a[(i0 - 1) * n + (j - 1)] - u[i0] - v[j];
                if (cur < minv[j]) { minv[j] = cur; way[j] = j0; }
                if (minv[j] < delta) { delta = minv[j]; j1 = j; }
            }
        }

        private static void ApplyDelta(int n, long delta, long* u, long* v, long* minv, int* p, int* used)
        {
            for (int j = 0; j <= n; j++)
            {
                if (used[j] != 0) { u[p[j]] += delta; v[j] -= delta; }
                else minv[j] -= delta;
            }
        }

        private static void BacktrackPath(int* way, int* p, int j0)
        {
            do { int j1 = way[j0]; p[j0] = p[j1]; j0 = j1; } while (j0 != 0);
        }

        private static long FinalizeResult(int n, long* a, int* p, long* matchL, long* matchR)
        {
            for (int j = 1; j <= n; j++) matchR[j - 1] = p[j] - 1;
            long result = 0;
            for (int j = 0; j < n; j++)
            {
                long row = matchR[j];
                result += a[row * n + j];
                matchL[row] = j;
            }
            return result;
        }
    }

    public static unsafe class HungarianMax
    {
        public static long Run(int n, long* a, int* matchL, int* matchR)
        {
            long* maxA = stackalloc long[n * n];
            for (int i = 0; i < n * n; i++) maxA[i] = -a[i];
            long* mL = stackalloc long[n], mR = stackalloc long[n];
            long res = HungarianMin.Run(n, maxA, mL, mR);
            for (int i = 0; i < n; i++) { matchL[i] = (int)mL[i]; matchR[i] = (int)mR[i]; }
            return -res;
        }
    }

    public static unsafe class AssignmentSolve
    {
        public static bool Run(int n, long* cost, int* assign, long* totalCost)
        {
            long* matchL = stackalloc long[n];
            long* matchR = stackalloc long[n];
            long result = HungarianMin.Run(n, cost, matchL, matchR);
            for (int i = 0; i < n; i++)
            {
                assign[i] = (int)matchL[i];
            }
            *totalCost = result;
            return true;
        }
    }
}