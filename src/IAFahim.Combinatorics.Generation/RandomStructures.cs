namespace IAFahim.Combinatorics.Generation;

using System;
using System.Runtime.CompilerServices;

public static unsafe class RandomStructures
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void RandomPermutation(int n, int* a, ref uint seed)
    {
        for (int i = 0; i < n; i++) a[i] = i;
        for (int i = n - 1; i > 0; i--)
        {
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            int j = (int)(seed % (uint)(i + 1));
            int t = a[i]; a[i] = a[j]; a[j] = t;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RandomTreePrufer(int n, int* prufer, ref uint seed)
    {
        if (n <= 2) return;
        for (int i = 0; i < n - 2; i++)
        {
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            prufer[i] = (int)(seed % (uint)n);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RandomConnectedGraph(int n, int m, int* outFrom, int* outTo, ref uint seed)
    {
        if (m < n - 1) m = n - 1;
        long maxEdges = (long)n * (n - 1) / 2;
        if (m > maxEdges) m = (int)maxEdges;

        int* perm = stackalloc int[n];
        RandomPermutation(n, perm, ref seed);

        int edgeCount = 0;
        for (int i = 1; i < n; i++)
        {
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            int v = (int)(seed % (uint)i);
            outFrom[edgeCount] = perm[i];
            outTo[edgeCount] = perm[v];
            edgeCount++;
        }

        while (edgeCount < m)
        {
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            int u = (int)(seed % (uint)n);
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            int v = (int)(seed % (uint)n);
            if (u == v) continue;

            bool exists = false;
            for (int i = 0; i < edgeCount; i++)
            {
                if ((outFrom[i] == u && outTo[i] == v) || (outFrom[i] == v && outTo[i] == u))
                {
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                outFrom[edgeCount] = u;
                outTo[edgeCount++] = v;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RandomDAG(int n, int m, int* outFrom, int* outTo, ref uint seed)
    {
        long maxEdges = (long)n * (n - 1) / 2;
        if (m > maxEdges) m = (int)maxEdges;

        int edgeCount = 0;
        while (edgeCount < m)
        {
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            int u = (int)(seed % (uint)n);
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            int v = (int)(seed % (uint)n);
            if (u >= v) continue;

            bool exists = false;
            for (int i = 0; i < edgeCount; i++)
            {
                if (outFrom[i] == u && outTo[i] == v)
                {
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                outFrom[edgeCount] = u;
                outTo[edgeCount++] = v;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RandomErdosRenyi(int n, double p, int* outFrom, int* outTo, ref uint seed, int* edgeCount)
    {
        *edgeCount = 0;
        int maxEdges = n * (n - 1) / 2;

        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                seed ^= seed << 13;
                seed ^= seed >> 17;
                seed ^= seed << 5;
                if ((seed % 1000) < p * 1000)
                {
                    if (*edgeCount < maxEdges)
                    {
                        outFrom[*edgeCount] = i;
                        outTo[*edgeCount] = j;
                        (*edgeCount)++;
                    }
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RandomBipartiteGraph(int n1, int n2, int m, int* outFrom, int* outTo, ref uint seed)
    {
        int edgeCount = 0;
        while (edgeCount < m)
        {
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            int u = (int)(seed % (uint)n1);
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            int v = (int)(seed % (uint)n2);

            bool exists = false;
            for (int i = 0; i < edgeCount; i++)
            {
                if (outFrom[i] == u && outTo[i] == v)
                {
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                outFrom[edgeCount] = u;
                outTo[edgeCount++] = v;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RandomRegular(int n, int d, int* outFrom, int* outTo, ref uint seed, int* edgeCount)
    {
        *edgeCount = 0;
        if (n * d % 2 != 0) return;

        int* points = stackalloc int[n * d];
        for (int i = 0; i < n * d; i++) points[i] = i / d;

        for (int i = n * d - 1; i > 0; i--)
        {
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            int j = (int)(seed % (uint)(i + 1));
            int tmp = points[i]; points[i] = points[j]; points[j] = tmp;
        }

        for (int i = 0; i < n * d; i += 2)
        {
            int u = points[i];
            int v = points[i + 1];
            if (u == v) return;

            bool exists = false;
            for (int k = 0; k < *edgeCount; k++)
            {
                if ((outFrom[k] == u && outTo[k] == v) || (outFrom[k] == v && outTo[k] == u))
                {
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                outFrom[*edgeCount] = u;
                outTo[*edgeCount] = v;
                (*edgeCount)++;
            }
        }
    }
}