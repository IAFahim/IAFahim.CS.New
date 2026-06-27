namespace IAFahim.DS.SegmentTree
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LiChaoTree
    {
        public struct Line
        {
            public long M, C;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public long Eval(long x) => M * x + C;
        }
    }

    public static unsafe class OnlineChtAdd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* slopes, long* intercepts, int* head, int* tail, long m, long c)
        {
            int t = *tail;
            while (t - *head >= 2)
            {
                long m1 = slopes[t - 2], c1 = intercepts[t - 2];
                long m2 = slopes[t - 1], c2 = intercepts[t - 1];
                long lhs = (c - c1) * (m1 - m2);
                long rhs = (c2 - c1) * (m1 - m);
                if (lhs >= rhs) t--;
                else break;
            }
            slopes[t] = m;
            intercepts[t] = c;
            *tail = t + 1;
        }
    }

    public static unsafe class OnlineChtQuery
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* slopes, long* intercepts, int* head, int tail, long x)
        {
            int h = *head;
            while (h + 1 < tail)
            {
                long v0 = slopes[h] * x + intercepts[h];
                long v1 = slopes[h + 1] * x + intercepts[h + 1];
                if (v1 <= v0) h++;
                else break;
            }
            *head = h;
            return slopes[h] * x + intercepts[h];
        }
    }

    public static unsafe class LiChaoInit
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(long* segM, long* segC, int size)
        {
            for (int i = 0; i < size; i++) { segM[i] = 0L; segC[i] = long.MaxValue; }
        }
    }

    public static unsafe class LiChaoRollback
    {
        public static void Add(long* segM, long* segC, int* histNode, long* histM, long* histC, int* top,
                               int node, int l, int r, long m, long c)
        {
            long curM = segM[node];
            long curC = segC[node];
            int mid = (l + r) >> 1;
            long newMid = m * mid + c;
            long curMid = curM * mid + curC;
            if (newMid < curMid)
            {
                int t = *top;
                histNode[t] = node; histM[t] = curM; histC[t] = curC;
                *top = t + 1;
                segM[node] = m; segC[node] = c;
                long tm = m, tc = c;
                m = curM; c = curC;
                curM = tm; curC = tc;
            }
            if (l == r) return;
            long displL = m * l + c;
            long storedL = segM[node] * l + segC[node];
            if (displL < storedL)
                Add(segM, segC, histNode, histM, histC, top, node << 1, l, mid, m, c);
            else
            {
                long displR = m * r + c;
                long storedR = segM[node] * r + segC[node];
                if (displR < storedR)
                    Add(segM, segC, histNode, histM, histC, top, (node << 1) | 1, mid + 1, r, m, c);
            }
        }

        public static long Query(long* segM, long* segC, int node, int l, int r, long x)
        {
            long best = segM[node] * x + segC[node];
            if (l == r) return best;
            int mid = (l + r) >> 1;
            long sub;
            if (x <= mid) sub = Query(segM, segC, node << 1, l, mid, x);
            else sub = Query(segM, segC, (node << 1) | 1, mid + 1, r, x);
            return sub < best ? sub : best;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rollback(long* segM, long* segC, int* histNode, long* histM, long* histC, int* top, int checkpoint)
        {
            while (*top > checkpoint)
            {
                int t = --(*top);
                int node = histNode[t];
                segM[node] = histM[t];
                segC[node] = histC[t];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCheckpoint(int* top) => *top;
    }

    public static unsafe class DynamicLiChaoRollback
    {
        public static void Init(long* pM, long* pC, int* pLc, int* pRc, int* pCount, long xLo, long xHi, int* counter)
        {
            pM[1] = 0L; pC[1] = long.MaxValue;
            pLc[1] = 0; pRc[1] = 0;
            *pCount = 1;
            counter[0] = 0;
        }

        public static void Add(long* pM, long* pC, int* pLc, int* pRc, int* pCount,
                               int* histNode, long* histM, long* histC, int* histSide, int* top,
                               int node, long l, long r, long m, long c)
        {
            long curM = pM[node];
            long curC = pC[node];
            long mid = (l + r) >> 1;
            long newMid = m * mid + c;
            long curMid = curM * mid + curC;
            if (newMid < curMid)
            {
                int t = *top;
                histNode[t] = node; histM[t] = curM; histC[t] = curC;
                histSide[t] = 0;
                *top = t + 1;
                pM[node] = m; pC[node] = c;
                long tm = m, tc = c;
                m = curM; c = curC;
                curM = tm; curC = tc;
            }
            if (l == r) return;
            long displL = m * l + c;
            long storedL = pM[node] * l + pC[node];
            if (displL < storedL)
            {
                int child = pLc[node];
                if (child == 0)
                {
                    int t = *top;
                    histNode[t] = node; histSide[t] = 1; histM[t] = 0; histC[t] = 0;
                    *top = t + 1;
                    child = ++(*pCount);
                    pM[child] = 0L; pC[child] = long.MaxValue; pLc[child] = 0; pRc[child] = 0;
                    pLc[node] = child;
                }
                Add(pM, pC, pLc, pRc, pCount, histNode, histM, histC, histSide, top, child, l, mid, m, c);
            }
            else
            {
                long displR = m * r + c;
                long storedR = pM[node] * r + pC[node];
                if (displR < storedR)
                {
                    int child = pRc[node];
                    if (child == 0)
                    {
                        int t = *top;
                        histNode[t] = node; histSide[t] = 2; histM[t] = 0; histC[t] = 0;
                        *top = t + 1;
                        child = ++(*pCount);
                        pM[child] = 0L; pC[child] = long.MaxValue; pLc[child] = 0; pRc[child] = 0;
                        pRc[node] = child;
                    }
                    Add(pM, pC, pLc, pRc, pCount, histNode, histM, histC, histSide, top, child, mid + 1, r, m, c);
                }
            }
        }

        public static long Query(long* pM, long* pC, int* pLc, int* pRc, int node, long l, long r, long x)
        {
            long best = pM[node] * x + pC[node];
            if (l == r) return best;
            long mid = (l + r) >> 1;
            long sub = best;
            if (x <= mid)
            {
                int child = pLc[node];
                if (child != 0) sub = Query(pM, pC, pLc, pRc, child, l, mid, x);
            }
            else
            {
                int child = pRc[node];
                if (child != 0) sub = Query(pM, pC, pLc, pRc, child, mid + 1, r, x);
            }
            return sub < best ? sub : best;
        }

        public static void Rollback(long* pM, long* pC, int* pLc, int* pRc, int* top,
                                    int* histNode, long* histM, long* histC, int* histSide, int checkpoint)
        {
            while (*top > checkpoint)
            {
                int t = --(*top);
                int node = histNode[t];
                int side = histSide[t];
                if (side == 0) { pM[node] = histM[t]; pC[node] = histC[t]; }
                else if (side == 1) { pLc[node] = 0; }
                else if (side == 2) { pRc[node] = 0; }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCheckpoint(int* top) => *top;
    }

    public static unsafe class PersistentLiChaoAdd
    {
        public static int Run(long* pM, long* pC, int* pLc, int* pRc, int* pCount,
                              int root, long l, long r, long m, long c)
        {
            int nr = ++(*pCount);
            long curM = pM[root];
            long curC = pC[root];
            pM[nr] = curM; pC[nr] = curC;
            pLc[nr] = pLc[root];
            pRc[nr] = pRc[root];
            long mid = (l + r) >> 1;
            long newMid = m * mid + c;
            long curMid = curM * mid + curC;
            long keepM, keepC, dispM, dispC;
            if (newMid < curMid) { keepM = m; keepC = c; dispM = curM; dispC = curC; }
            else { keepM = curM; keepC = curC; dispM = m; dispC = c; }
            pM[nr] = keepM; pC[nr] = keepC;
            if (l == r) return nr;
            long dispL = dispM * l + dispC;
            long keepL = keepM * l + keepC;
            if (dispL < keepL)
            {
                int child = pLc[nr];
                int newChild = child == 0
                    ? NewLeaf(pM, pC, pLc, pRc, pCount)
                    : Run(pM, pC, pLc, pRc, pCount, child, l, mid, dispM, dispC);
                pLc[nr] = newChild;
            }
            else
            {
                long dispR = dispM * r + dispC;
                long keepR = keepM * r + keepC;
                if (dispR < keepR)
                {
                    int child = pRc[nr];
                    int newChild = child == 0
                        ? NewLeaf(pM, pC, pLc, pRc, pCount)
                        : Run(pM, pC, pLc, pRc, pCount, child, mid + 1, r, dispM, dispC);
                    pRc[nr] = newChild;
                }
            }
            return nr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int NewLeaf(long* pM, long* pC, int* pLc, int* pRc, int* pCount)
        {
            int n = ++(*pCount);
            pM[n] = 0L; pC[n] = long.MaxValue; pLc[n] = 0; pRc[n] = 0;
            return n;
        }
    }

    public static unsafe class PersistentLiChaoQuery
    {
        public static long Run(long* pM, long* pC, int* pLc, int* pRc, int root, long l, long r, long x)
        {
            long best = pM[root] * x + pC[root];
            if (l == r) return best;
            long mid = (l + r) >> 1;
            long sub = best;
            if (x <= mid)
            {
                int child = pLc[root];
                if (child != 0) sub = Run(pM, pC, pLc, pRc, child, l, mid, x);
            }
            else
            {
                int child = pRc[root];
                if (child != 0) sub = Run(pM, pC, pLc, pRc, child, mid + 1, r, x);
            }
            return sub < best ? sub : best;
        }
    }

    public static unsafe class DivideConquerHullOptimization
    {
        public static void Run(long* dpPrev, long* dpCur, long* cost, int n, int* opt)
        {
            Solve(dpPrev, dpCur, cost, n, opt, 1, n - 1, 0, n - 2);
        }

        private static void Solve(long* dpPrev, long* dpCur, long* cost, int n, int* opt, int lo, int hi, int optLo, int optHi)
        {
            if (lo > hi) return;
            int mid = (lo + hi) >> 1;
            int upper = optHi < mid - 1 ? optHi : mid - 1;
            long best = long.MaxValue;
            int bestJ = optLo;
            if (upper >= optLo)
            {
                for (int j = optLo; j <= upper; j++)
                {
                    long v = dpPrev[j] + cost[j * n + mid];
                    if (v < best) { best = v; bestJ = j; }
                }
                dpCur[mid] = best;
            }
            opt[mid] = bestJ;
            Solve(dpPrev, dpCur, cost, n, opt, lo, mid - 1, optLo, bestJ);
            Solve(dpPrev, dpCur, cost, n, opt, mid + 1, hi, bestJ, optHi);
        }
    }
}
