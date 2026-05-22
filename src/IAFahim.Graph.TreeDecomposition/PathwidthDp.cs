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
            try
            {
                UnsafeUtilityMemClear(dp, dpSize);
                long* curDp = dp;
                long* nextDp = dp + (1L << maxBagSize);

                curDp[0] = 0;
                
                for (int i = 0; i < pathLength - 1; i++)
                {
                    int oldSize = bagSizes[i];
                    int newSize = bagSizes[i + 1];
                    
                    UnsafeUtilityMemClear(nextDp, (1L << maxBagSize) * sizeof(long));
                    
                    for (int mask = 0; mask < (1 << oldSize); mask++)
                    {
                        if (curDp[mask] == 0 && mask != 0) continue;
                        
                        // We map the old mask to the new mask
                        // Just an illustrative mapping
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
                        
                        if (curDp[mask] > nextDp[newMaskBase])
                        {
                            nextDp[newMaskBase] = curDp[mask];
                        }
                    }
                    
                    // Now try to introduce new vertices in bag i+1
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
                                        long cand = nextDp[mask] + weights[v];
                                        if (cand > nextDp[mask | (1 << j)])
                                        {
                                            nextDp[mask | (1 << j)] = cand;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                    long* temp = curDp;
                    curDp = nextDp;
                    nextDp = temp;
                }
                
                long ans = 0;
                for (int mask = 0; mask < (1 << bagSizes[pathLength - 1]); mask++)
                {
                    if (curDp[mask] > ans)
                    {
                        ans = curDp[mask];
                    }
                }
                return ans;
            }
            finally
            {
                Marshal.FreeHGlobal((nint)dp);
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
