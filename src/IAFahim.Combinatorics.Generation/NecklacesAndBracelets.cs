namespace IAFahim.Combinatorics.Generation;

using System;
using System.Runtime.CompilerServices;

public static unsafe class NecklacesAndBracelets
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGenerateLyndon(int n, int k, int* w, int* res, ref int jState, int* scratch)
    {
        if (w == null)
        {
            w = scratch;
            for (int i = 0; i <= n + 1; i++) w[i] = 0;
            jState = 1;
        }

        while (jState > 0)
        {
            int currentJ = jState;
            bool yieldIt = (n % currentJ == 0);
            if (yieldIt)
            {
                for (int i = 0; i < currentJ; i++) res[i] = w[i + 1];
                w[0] = currentJ;
            }

            int nextJ = n;
            while (nextJ > 0 && w[nextJ] == k - 1) nextJ--;
            if (nextJ > 0)
            {
                w[nextJ]++;
                for (int m = nextJ + 1; m <= n; m++) w[m] = w[m - nextJ];
            }
            else
            {
                nextJ = 0;
            }

            jState = nextJ;

            if (yieldIt)
            {
                return true;
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int DeBruijnFromLyndon(int k, int n, int* outSeq)
    {
        int* w = stackalloc int[n + 1];
        for (int i = 0; i <= n; i++) w[i] = 0;
        int j = 1;
        int pos = 0;
        while (j > 0)
        {
            if (n % j == 0)
                for (int i = 1; i <= j; i++) outSeq[pos++] = w[i];
            j = n;
            while (j > 0 && w[j] == k - 1) j--;
            if (j > 0)
            {
                w[j]++;
                for (int m = j + 1; m <= n; m++) w[m] = w[m - j];
            }
        }
        return pos;
    }

    public static long NecklaceRank(int* necklace, int n, int k)
    {
        return 0;
    }

    public static bool NecklaceUnrank(long rank, int n, int k, int* outObj)
    {
        return false;
    }

    public static long BraceletRank(int* bracelet, int n, int k)
    {
        return 0;
    }

    public static bool BraceletUnrank(long rank, int n, int k, int* outObj)
    {
        return false;
    }

    public static long LyndonWordRank(int* word, int n, int k)
    {
        return 0;
    }

    public static bool LyndonWordUnrank(long rank, int n, int k, int* outObj)
    {
        return false;
    }
}