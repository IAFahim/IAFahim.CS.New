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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MaxIndependentSet(
            int niceNodesCount,
            int* nodeType,
            int* leftChild, int* rightChild,
            int* introForgetVertex,
            int* bagSizes,
            int* bagElements,
            int maxBagSize,
            long* weights,
            int* graphHead, int* graphTo, int* graphNext,
            int n)
        {
            long dpSize = (long)niceNodesCount * (1L << maxBagSize) * sizeof(long);
            long* dp = (long*)Marshal.AllocHGlobal((nint)dpSize);
            try
            {
                UnsafeUtilityMemClear(dp, dpSize);
                long* isIndependent = (long*)Marshal.AllocHGlobal((nint)((1L << maxBagSize) * sizeof(long)));
                try
                {
                    for (int u = niceNodesCount - 1; u >= 0; u--)
                    {
                        int type = nodeType[u];
                        long* curDp = dp + (long)u * (1L << maxBagSize);
                        
                        if (type == TYPE_LEAF)
                        {
                            curDp[0] = 0;
                        }
                        else if (type == TYPE_INTRODUCE)
                        {
                            int lc = leftChild[u];
                            long* leftDp = dp + (long)lc * (1L << maxBagSize);
                            int v = introForgetVertex[u];
                            
                            int oldSize = bagSizes[lc];
                            int newSize = bagSizes[u];
                            
                            for (int mask = 0; mask < (1 << oldSize); mask++)
                            {
                                curDp[mask] = leftDp[mask];
                                bool canAdd = true;
                                for (int i = 0; i < oldSize; i++)
                                {
                                    if (((mask >> i) & 1) != 0)
                                    {
                                        int oldV = bagElements[lc * maxBagSize + i];
                                        if (AreAdjacent(v, oldV, graphHead, graphTo, graphNext))
                                        {
                                            canAdd = false;
                                            break;
                                        }
                                    }
                                }
                                if (canAdd)
                                {
                                    curDp[mask | (1 << (newSize - 1))] = leftDp[mask] + weights[v];
                                }
                            }
                        }
                        else if (type == TYPE_FORGET)
                        {
                            int lc = leftChild[u];
                            long* leftDp = dp + (long)lc * (1L << maxBagSize);
                            int v = introForgetVertex[u];
                            
                            int oldSize = bagSizes[lc];
                            int newSize = bagSizes[u];
                            
                            int forgetIdx = -1;
                            for (int i = 0; i < oldSize; i++)
                            {
                                if (bagElements[lc * maxBagSize + i] == v)
                                {
                                    forgetIdx = i;
                                    break;
                                }
                            }
                            
                            for (int mask = 0; mask < (1 << oldSize); mask++)
                            {
                                int newMask = 0;
                                int bitPos = 0;
                                for (int i = 0; i < oldSize; i++)
                                {
                                    if (i != forgetIdx)
                                    {
                                        if (((mask >> i) & 1) != 0)
                                        {
                                            newMask |= (1 << bitPos);
                                        }
                                        bitPos++;
                                    }
                                }
                                if (leftDp[mask] > curDp[newMask])
                                {
                                    curDp[newMask] = leftDp[mask];
                                }
                            }
                        }
                        else if (type == TYPE_JOIN)
                        {
                            int lc = leftChild[u];
                            int rc = rightChild[u];
                            long* leftDp = dp + (long)lc * (1L << maxBagSize);
                            long* rightDp = dp + (long)rc * (1L << maxBagSize);
                            
                            int size = bagSizes[u];
                            for (int mask = 0; mask < (1 << size); mask++)
                            {
                                long wt = 0;
                                for (int i = 0; i < size; i++)
                                {
                                    if (((mask >> i) & 1) != 0)
                                    {
                                        wt += weights[bagElements[u * maxBagSize + i]];
                                    }
                                }
                                curDp[mask] = leftDp[mask] + rightDp[mask] - wt;
                            }
                        }
                    }
                    
                    long ans = 0;
                    long* rootDp = dp + 0 * (1L << maxBagSize);
                    for (int mask = 0; mask < (1 << bagSizes[0]); mask++)
                    {
                        if (rootDp[mask] > ans)
                        {
                            ans = rootDp[mask];
                        }
                    }
                    return ans;
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)isIndependent);
                }
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
