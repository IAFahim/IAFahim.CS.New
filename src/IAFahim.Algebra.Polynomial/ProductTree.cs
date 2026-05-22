namespace IAFahim.Algebra.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ProductTree
    {
        public static int Build(long* values, int n, int MOD, long* tree, int* offsets, int* sizes)
        {
            int nodeCount = 0;
            for (int i = 0; i < n; i++)
            {
                offsets[nodeCount] = i * 2;
                sizes[nodeCount] = 2;
                tree[i * 2] = (MOD - values[i] % MOD) % MOD;
                tree[i * 2 + 1] = 1;
                nodeCount++;
            }
            int levelStart = 0;
            int levelSize = n;
            int writeOff = n * 2;
            while (levelSize > 1)
            {
                int nextStart = nodeCount;
                int nextSize = 0;
                for (int i = 0; i < levelSize; i += 2)
                {
                    if (i + 1 < levelSize)
                    {
                        offsets[nodeCount] = writeOff;
                        int sa = sizes[levelStart + i], sb = sizes[levelStart + i + 1];
                        int sr = sa + sb - 1;
                        sizes[nodeCount] = sr;
                        ToomCook.Multiply(
                            tree + offsets[levelStart + i],
                            tree + offsets[levelStart + i + 1],
                            tree + writeOff,
                            Math.Max(sa, sb), MOD);
                        writeOff += sr;
                        nodeCount++;
                        nextSize++;
                    }
                    else
                    {
                        offsets[nodeCount] = offsets[levelStart + i];
                        sizes[nodeCount] = sizes[levelStart + i];
                        nodeCount++;
                        nextSize++;
                    }
                }
                levelStart = nextStart;
                levelSize = nextSize;
            }
            return nodeCount;
        }
    }
}
