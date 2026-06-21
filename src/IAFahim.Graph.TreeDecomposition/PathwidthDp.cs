namespace IAFahim.Graph.TreeDecomposition
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class PathwidthDpAlgorithm
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PathwidthDpIndependentSet(
            int pathLength,
            int* bagSizes,
            int* bagElements, // flattened: bagElements[i * maxBagSize + j]
            int maxBagSize,
            long* weights,
            int* graphHead, int* graphTo, int* graphNext)
        {
            long dpSize = 2L * (1L << maxBagSize) * sizeof(long);
            long* dp = (long*)Marshal.AllocHGlobal((nint)dpSize);
            long reachedSize = 2L * (1L << maxBagSize) * sizeof(byte);
            byte* reached = (byte*)Marshal.AllocHGlobal((nint)reachedSize);
            try
            {
                UnsafeUtilityMemClear(dp, dpSize);
                UnsafeUtilityMemClear(reached, reachedSize);
                long* curDp = dp;
                long* nextDp = dp + (1L << maxBagSize);
                byte* reachedCur = reached;
                byte* reachedNext = reached + (1L << maxBagSize);

                curDp[0] = 0;
                reachedCur[0] = 1;
                
                for (int i = 0; i < pathLength - 1; i++)
                {
                    int oldSize = bagSizes[i];
                    int newSize = bagSizes[i + 1];
                    
                    UnsafeUtilityMemClear(nextDp, (1L << maxBagSize) * sizeof(long));
                    UnsafeUtilityMemClear(reachedNext, (1L << maxBagSize) * sizeof(byte));
                    
                    MapOldToNewMasks(oldSize, newSize, i, maxBagSize, bagElements, curDp, nextDp, reachedCur, reachedNext);
                    TryIntroduceNewVertices(oldSize, newSize, i, maxBagSize, bagElements, weights, graphHead, graphTo, graphNext, nextDp, reachedNext);
                    
                    long* temp = curDp;
                    curDp = nextDp;
                    nextDp = temp;
                    byte* tempR = reachedCur;
                    reachedCur = reachedNext;
                    reachedNext = tempR;
                }
                
                long ans = 0;
                for (int mask = 0; mask < (1 << bagSizes[pathLength - 1]); mask++)
                {
                    if (reachedCur[mask] != 0 && curDp[mask] > ans)
                    {
                        ans = curDp[mask];
                    }
                }
                return ans;
            }
            finally
            {
                Marshal.FreeHGlobal((nint)dp);
                Marshal.FreeHGlobal((nint)reached);
            }
        }

        private static bool AreAdjacent(int u, int v, int* graphHead, int* graphTo, int* graphNext)
        {
            for (int e = graphHead[u]; e != 0; e = graphNext[e])
            {
                if (graphTo[e] == v) return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MapOldToNewMasks(
            int oldSize, int newSize, int i, int maxBagSize,
            int* bagElements, long* curDp, long* nextDp, byte* reachedCur, byte* reachedNext)
        {
            for (int mask = 0; mask < (1 << oldSize); mask++)
            {
                if (reachedCur[mask] == 0) continue;
                int newMaskBase = 0;
                for (int j = 0; j < newSize; j++)
                {
                    int v = bagElements[(i + 1) * maxBagSize + j];
                    for (int k = 0; k < oldSize; k++)
                    {
                        if (bagElements[i * maxBagSize + k] == v && ((mask >> k) & 1) != 0)
                        {
                            newMaskBase |= (1 << j);
                        }
                    }
                }
                if (reachedNext[newMaskBase] == 0 || curDp[mask] > nextDp[newMaskBase])
                {
                    nextDp[newMaskBase] = curDp[mask];
                    reachedNext[newMaskBase] = 1;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TryIntroduceNewVertices(
            int oldSize, int newSize, int i, int maxBagSize,
            int* bagElements, long* weights, int* graphHead, int* graphTo, int* graphNext,
            long* nextDp, byte* reachedNext)
        {
            for (int j = 0; j < newSize; j++)
            {
                int v = bagElements[(i + 1) * maxBagSize + j];
                bool isNew = true;
                for (int k = 0; k < oldSize; k++)
                {
                    if (bagElements[i * maxBagSize + k] == v) isNew = false;
                }
                if (isNew)
                {
                    for (int mask = (1 << newSize) - 1; mask >= 0; mask--)
                    {
                        if (((mask >> j) & 1) == 0)
                        {
                            bool canAdd = true;
                            for (int k = 0; k < newSize; k++)
                            {
                                if (((mask >> k) & 1) != 0)
                                {
                                    int u = bagElements[(i + 1) * maxBagSize + k];
                                    if (AreAdjacent(u, v, graphHead, graphTo, graphNext))
                                    {
                                        canAdd = false;
                                        break;
                                    }
                                }
                            }
                            if (canAdd)
                            {
                                int target = mask | (1 << j);
                                if (reachedNext[mask] != 0)
                                {
                                    long cand = nextDp[mask] + weights[v];
                                    if (reachedNext[target] == 0 || cand > nextDp[target])
                                    {
                                        nextDp[target] = cand;
                                        reachedNext[target] = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void UnsafeUtilityMemClear(void* ptr, long size)
        {
            byte* bPtr = (byte*)ptr;
            for (long i = 0; i < size; i++)
            {
                bPtr[i] = 0;
            }
        }
    }
}
