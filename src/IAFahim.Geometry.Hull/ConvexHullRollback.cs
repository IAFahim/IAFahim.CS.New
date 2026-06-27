namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ConvexHullRollbackAdd
    {
        public static void Run(int* px, int* py, int* count, int* hullIdx, int* hullLen,
                               int* histCount, int* top, int* scratchIdx, int x, int y)
        {
            histCount[*top] = *count;
            *top = *top + 1;
            int c = *count;
            px[c] = x;
            py[c] = y;
            *count = c + 1;
            ConvexHullRollbackUtil.BuildHull(px, py, *count, hullIdx, hullLen, scratchIdx);
        }
    }

    public static unsafe class ConvexHullRollbackQuery
    {
        public static int Run(int* px, int* py, int* hullIdx, int hullLen, long dx, long dy)
        {
            if (hullLen <= 0) return -1;
            int best = hullIdx[0];
            long bestDot = dx * px[best] + dy * py[best];
            for (int i = 1; i < hullLen; i++)
            {
                int idx = hullIdx[i];
                long d = dx * px[idx] + dy * py[idx];
                if (d > bestDot) { bestDot = d; best = idx; }
            }
            return best;
        }
    }

    public static unsafe class ConvexHullRollback
    {
        public static void Run(int* px, int* py, int* count, int* hullIdx, int* hullLen,
                               int* histCount, int* top, int* scratchIdx, int checkpoint)
        {
            while (*top > checkpoint)
            {
                *top = *top - 1;
                *count = histCount[*top];
            }
            ConvexHullRollbackUtil.BuildHull(px, py, *count, hullIdx, hullLen, scratchIdx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCheckpoint(int* top) => *top;
    }

    internal static unsafe class ConvexHullRollbackUtil
    {
        public static void BuildHull(int* px, int* py, int n, int* hullIdx, int* hullLen, int* order)
        {
            if (n == 0) { *hullLen = 0; return; }
            if (n == 1) { hullIdx[0] = 0; *hullLen = 1; return; }
            for (int i = 0; i < n; i++) order[i] = i;
            HeapSortOrder(px, py, order, n);

            int k = 0;
            for (int i = 0; i < n; i++)
            {
                while (k >= 2 && Cross(px, py, hullIdx[k - 2], hullIdx[k - 1], order[i]) <= 0L) k--;
                hullIdx[k] = order[i];
                k++;
            }
            int lower = k + 1;
            for (int i = n - 2; i >= 0; i--)
            {
                while (k >= lower && Cross(px, py, hullIdx[k - 2], hullIdx[k - 1], order[i]) <= 0L) k--;
                hullIdx[k] = order[i];
                k++;
            }
            *hullLen = k - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Cross(int* px, int* py, int a, int b, int c)
        {
            return (long)(px[b] - px[a]) * (py[c] - py[a]) - (long)(py[b] - py[a]) * (px[c] - px[a]);
        }

        private static void HeapSortOrder(int* px, int* py, int* a, int len)
        {
            for (int i = (len >> 1) - 1; i >= 0; i--) Sift(px, py, a, i, len);
            for (int i = len - 1; i > 0; i--)
            {
                int t = a[0]; a[0] = a[i]; a[i] = t;
                Sift(px, py, a, 0, i);
            }
        }

        private static void Sift(int* px, int* py, int* a, int i, int len)
        {
            int half = len >> 1;
            while (i < half)
            {
                int child = (i << 1) + 1;
                int right = child + 1;
                if (right < len && Cmp(px, py, a[right], a[child]) > 0) child = right;
                if (Cmp(px, py, a[child], a[i]) <= 0) break;
                int t = a[i]; a[i] = a[child]; a[child] = t;
                i = child;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Cmp(int* px, int* py, int a, int b)
        {
            if (px[a] != px[b]) return px[a] < px[b] ? 1 : -1;
            if (py[a] != py[b]) return py[a] < py[b] ? 1 : -1;
            return 0;
        }
    }
}
