namespace IAFahim.String.Pattern
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class AhoPersistentBuild
    {
        public static void Insert(byte* str, int len, 
                                  byte** patterns, int* lengths, ref int numPatterns,
                                  int* roots, int* nexts, int* fails, int* counts, 
                                  ref int nodeCount, ref int activeMask, int sigma = 26, byte baseChar = (byte)'a')
        {
            patterns[numPatterns] = str;
            lengths[numPatterns] = len;
            numPatterns++;

            int bit = 0;
            while ((activeMask & (1 << bit)) != 0) bit++;

            int startIdx = numPatterns - (1 << bit);
            int root = ++nodeCount;
            roots[bit] = root;

            for (int i = 0; i < sigma; i++) nexts[root * sigma + i] = root;
            fails[root] = root;
            counts[root] = 0;

            for (int i = startIdx; i < numPatterns; i++)
            {
                int u = root;
                for (int j = 0; j < lengths[i]; j++)
                {
                    int c = patterns[i][j] - baseChar;
                    if (nexts[u * sigma + c] == root || nexts[u * sigma + c] == 0)
                    {
                        int v = ++nodeCount;
                        for (int k = 0; k < sigma; k++) nexts[v * sigma + k] = root;
                        fails[v] = root;
                        counts[v] = 0;
                        nexts[u * sigma + c] = v;
                    }
                    u = nexts[u * sigma + c];
                }
                counts[u]++;
            }

            int* q = stackalloc int[nodeCount - root + 1];
            int head = 0, tail = 0;

            for (int i = 0; i < sigma; i++)
            {
                if (nexts[root * sigma + i] != root)
                {
                    fails[nexts[root * sigma + i]] = root;
                    q[tail++] = nexts[root * sigma + i];
                }
            }

            while (head < tail)
            {
                int u = q[head++];
                counts[u] += counts[fails[u]];
                for (int i = 0; i < sigma; i++)
                {
                    if (nexts[u * sigma + i] != root)
                    {
                        fails[nexts[u * sigma + i]] = nexts[fails[u] * sigma + i];
                        q[tail++] = nexts[u * sigma + i];
                    }
                    else
                    {
                        nexts[u * sigma + i] = nexts[fails[u] * sigma + i];
                    }
                }
            }

            activeMask = (activeMask & ~((1 << bit) - 1)) | (1 << bit);
        }
    }

    public static unsafe class AhoPersistentQuery
    {
        public static long Run(byte* text, int len, int* roots, int activeMask, int* nexts, int* counts, int sigma = 26, byte baseChar = (byte)'a')
        {
            long totalMatches = 0;
            for (int bit = 0; bit < 32; bit++)
            {
                if ((activeMask & (1 << bit)) != 0)
                {
                    int u = roots[bit];
                    for (int i = 0; i < len; i++)
                    {
                        int c = text[i] - baseChar;
                        u = nexts[u * sigma + c];
                        totalMatches += counts[u];
                    }
                }
            }
            return totalMatches;
        }
    }
}
