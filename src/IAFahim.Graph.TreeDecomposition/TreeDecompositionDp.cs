namespace IAFahim.Graph.TreeDecomposition
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class TreeDecompositionDp
    {
        public const int TYPE_LEAF = 0;
        public const int TYPE_INTRODUCE = 1;
        public const int TYPE_FORGET = 2;
        public const int TYPE_JOIN = 3;

        public static long MaxIndependentSet(
            int niceCount, int* type, int* lc, int* rc, int* vIv, int* bSz, int* bEl, int mBS,
            long* weights, int* gHead, int* gTo, int* gNext, int n, long* dp)
        {
            int maskSize = 1 << mBS;
            for (int u = niceCount - 1; u >= 0; u--)
            {
                long* curDp = dp + (long)u * maskSize;
                if (type[u] == TYPE_LEAF) curDp[0] = 0;
                else if (type[u] == TYPE_INTRODUCE) ProcessIntroduce(u, lc[u], vIv[u], bSz, bEl, mBS, weights, gHead, gTo, gNext, dp);
                else if (type[u] == TYPE_FORGET) ProcessForget(u, lc[u], vIv[u], bSz, bEl, mBS, dp);
                else if (type[u] == TYPE_JOIN) ProcessJoin(u, lc[u], rc[u], bSz, bEl, mBS, weights, dp);
            }
            long ans = 0; for (int m = 0; m < (1 << bSz[0]); m++) ans = Math.Max(ans, dp[m]);
            return ans;
        }

        private static void ProcessIntroduce(int u, int l, int v, int* bSz, int* bEl, int mBS, long* weights, int* gH, int* gT, int* gN, long* dp)
        {
            int mS = 1 << mBS; long* cur = dp + (long)u * mS, left = dp + (long)l * mS;
            int oldSz = bSz[l], newSz = bSz[u];
            for (int m = 0; m < (1 << oldSz); m++)
            {
                cur[m] = left[m];
                if (CanAdd(v, m, oldSz, l, mBS, bEl, gH, gT, gN)) cur[m | (1 << (newSz - 1))] = left[m] + weights[v];
            }
        }

        private static bool CanAdd(int v, int m, int oldSz, int l, int mBS, int* bEl, int* gH, int* gT, int* gN)
        {
            for (int i = 0; i < oldSz; i++)
                if (((m >> i) & 1) != 0 && AreAdjacent(v, bEl[l * mBS + i], gH, gT, gN)) return false;
            return true;
        }

        private static void ProcessForget(int u, int l, int v, int* bSz, int* bEl, int mBS, long* dp)
        {
            int mS = 1 << mBS; long* cur = dp + (long)u * mS, left = dp + (long)l * mS;
            int oldSz = bSz[l], fIdx = -1;
            for (int i = 0; i < oldSz; i++) if (bEl[l * mBS + i] == v) { fIdx = i; break; }
            for (int m = 0; m < (1 << oldSz); m++)
            {
                int nextM = 0, pos = 0;
                for (int i = 0; i < oldSz; i++) if (i != fIdx) { if (((m >> i) & 1) != 0) nextM |= (1 << pos); pos++; }
                cur[nextM] = Math.Max(cur[nextM], left[m]);
            }
        }

        private static void ProcessJoin(int u, int l, int r, int* bSz, int* bEl, int mBS, long* weights, long* dp)
        {
            int mS = 1 << mBS; long* cur = dp + (long)u * mS, left = dp + (long)l * mS, right = dp + (long)r * mS;
            int sz = bSz[u];
            for (int m = 0; m < (1 << sz); m++)
            {
                long wt = 0;
                for (int i = 0; i < sz; i++) if (((m >> i) & 1) != 0) wt += weights[bEl[u * mBS + i]];
                cur[m] = left[m] + right[m] - wt;
            }
        }

        private static bool AreAdjacent(int u, int v, int* gH, int* gT, int* gN)
        {
            for (int e = gH[u]; e != 0; e = gN[e]) if (gT[e] == v) return true;
            return false;
        }
    }
}
