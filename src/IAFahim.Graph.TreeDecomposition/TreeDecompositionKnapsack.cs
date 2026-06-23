namespace IAFahim.Graph.TreeDecomposition
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class TreeDecompositionKnapsackAlgorithm
    {
        public const int TYPE_LEAF = 0;
        public const int TYPE_INTRODUCE = 1;
        public const int TYPE_FORGET = 2;
        public const int TYPE_JOIN = 3;

        private const long NEG_INF = long.MinValue / 4;

        // dp[u][mask][c] = max total value over the subtree rooted at u, given that
        // 'mask' is the subset of bag(u) currently selected and 'c' is the total weight
        // of every selected vertex in that subtree. Infeasible states hold NEG_INF.
        // Bag layout matches the rest of this module: bagElements[u * maxBagSize + i]
        // is the vertex sitting at bit i of node u's mask. Node 0 is the root.
        // Caller guarantees valid (non-null, consistent) input by design.
        public static long TreeDecompositionKnapsack(
            int niceNodesCount,
            int* nodeType,
            int* leftChild, int* rightChild,
            int* introForgetVertex,
            int* bagSizes,
            int* bagElements,
            int maxBagSize,
            int capacity,
            long* weights,
            long* values)
        {
            long maskSize = 1L << maxBagSize;
            long capStride = (long)capacity + 1;
            long nodeStride = maskSize * capStride;
            long dpSize = (long)niceNodesCount * nodeStride * sizeof(long);

            long* dp = (long*)Marshal.AllocHGlobal((nint)dpSize);
            try
            {
                long cells = (long)niceNodesCount * nodeStride;
                for (long i = 0; i < cells; i++) dp[i] = NEG_INF;

                for (int u = niceNodesCount - 1; u >= 0; u--)
                {
                    int type = nodeType[u];
                    switch (type)
                    {
                        case TYPE_LEAF:
                            ProcessLeaf(u, bagSizes, bagElements, maxBagSize, capacity, capStride, nodeStride, weights, values, dp);
                            break;
                        case TYPE_INTRODUCE:
                            ProcessIntroduce(u, leftChild[u], introForgetVertex[u], bagSizes, capacity, capStride, nodeStride, weights, values, dp);
                            break;
                        case TYPE_FORGET:
                            ProcessForget(u, leftChild[u], introForgetVertex[u], bagSizes, bagElements, maxBagSize, capStride, nodeStride, dp);
                            break;
                        case TYPE_JOIN:
                            ProcessJoin(u, leftChild[u], rightChild[u], bagSizes, bagElements, maxBagSize, capacity, capStride, nodeStride, weights, values, dp);
                            break;
                    }
                }

                long* root = dp;
                long rootCells = nodeStride;
                long best = 0;
                for (long i = 0; i < rootCells; i++)
                {
                    long v = root[i];
                    if (v > best) best = v;
                }
                return best;
            }
            finally
            {
                Marshal.FreeHGlobal((nint)dp);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SumMaskSubset(int u, int m, int sz, int* bEl, int mBS, long* weights, long* values, out long w, out long val)
        {
            long lw = 0, lval = 0;
            for (int i = 0; i < sz; i++)
            {
                if (((m >> i) & 1) != 0)
                {
                    int vtx = bEl[u * mBS + i];
                    lw += weights[vtx];
                    lval += values[vtx];
                }
            }
            w = lw;
            val = lval;
        }

        private static void ProcessLeaf(
            int u, int* bSz, int* bEl, int mBS, int capacity, long capStride, long nodeStride,
            long* weights, long* values, long* dp)
        {
            long* cur = dp + (long)u * nodeStride;
            int sz = bSz[u];
            int subsetCount = 1 << sz;
            for (int m = 0; m < subsetCount; m++)
            {
                long w, val;
                SumMaskSubset(u, m, sz, bEl, mBS, weights, values, out w, out val);
                if (w <= capacity)
                {
                    long* slot = cur + (long)m * capStride + w;
                    if (val > *slot) *slot = val;
                }
            }
        }

        private static void ProcessIntroduce(
            int u, int l, int v, int* bSz, int capacity, long capStride, long nodeStride,
            long* weights, long* values, long* dp)
        {
            long* cur = dp + (long)u * nodeStride;
            long* left = dp + (long)l * nodeStride;
            int oldSz = bSz[l];
            int newSz = bSz[u];
            int introBit = 1 << (newSz - 1);
            long wV = weights[v];
            long valV = values[v];
            int oldMaskCount = 1 << oldSz;
            for (int m = 0; m < oldMaskCount; m++)
            {
                long* srcRow = left + (long)m * capStride;
                long* dstRowKeep = cur + (long)m * capStride;
                long* dstRowTake = cur + (long)(m | introBit) * capStride;
                for (int c = 0; c <= capacity; c++)
                {
                    long src = srcRow[c];
                    if (src == NEG_INF) continue;
                    if (src > dstRowKeep[c]) dstRowKeep[c] = src;
                    long nc = c + wV;
                    if (nc <= capacity)
                    {
                        long cand = src + valV;
                        long* slot = dstRowTake + nc;
                        if (cand > *slot) *slot = cand;
                    }
                }
            }
        }

        private static void ProcessForget(
            int u, int l, int v, int* bSz, int* bEl, int mBS, long capStride, long nodeStride, long* dp)
        {
            long* cur = dp + (long)u * nodeStride;
            long* left = dp + (long)l * nodeStride;
            int oldSz = bSz[l];
            int fIdx = -1;
            for (int i = 0; i < oldSz; i++)
            {
                if (bEl[l * mBS + i] == v) { fIdx = i; break; }
            }
            int oldMaskCount = 1 << oldSz;
            for (int m = 0; m < oldMaskCount; m++)
            {
                int nextM = 0, pos = 0;
                for (int i = 0; i < oldSz; i++)
                {
                    if (i != fIdx)
                    {
                        if (((m >> i) & 1) != 0) nextM |= (1 << pos);
                        pos++;
                    }
                }
                long* srcRow = left + (long)m * capStride;
                long* dstRow = cur + (long)nextM * capStride;
                long cc = capStride;
                for (long c = 0; c < cc; c++)
                {
                    long src = srcRow[c];
                    if (src > dstRow[c]) dstRow[c] = src;
                }
            }
        }

        private static void ProcessJoin(
            int u, int l, int r, int* bSz, int* bEl, int mBS, int capacity, long capStride, long nodeStride,
            long* weights, long* values, long* dp)
        {
            long* cur = dp + (long)u * nodeStride;
            long* left = dp + (long)l * nodeStride;
            long* right = dp + (long)r * nodeStride;
            int sz = bSz[u];
            int maskCount = 1 << sz;
            for (int m = 0; m < maskCount; m++)
            {
                long sharedW, sharedVal;
                SumMaskSubset(u, m, sz, bEl, mBS, weights, values, out sharedW, out sharedVal);
                if (sharedW > capacity) continue;
                int sharedWi = (int)sharedW;
                long* leftRow = left + (long)m * capStride;
                long* rightRow = right + (long)m * capStride;
                long* dstRow = cur + (long)m * capStride;
                for (int c1 = sharedWi; c1 <= capacity; c1++)
                {
                    long lv = leftRow[c1];
                    if (lv == NEG_INF) continue;
                    // cNew = c1 + c2 - sharedW must be in [0, capacity].
                    int c2Max = capacity - c1 + sharedWi;
                    for (int c2 = sharedWi; c2 <= c2Max; c2++)
                    {
                        long rv = rightRow[c2];
                        if (rv == NEG_INF) continue;
                        long cand = lv + rv - sharedVal;
                        long cNew = (long)c1 + c2 - sharedW;
                        long* slot = dstRow + cNew;
                        if (cand > *slot) *slot = cand;
                    }
                }
            }
        }
    }
}
