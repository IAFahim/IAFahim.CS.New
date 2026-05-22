namespace IAFahim.Combinatorics.Generation;

using System;
using System.Collections.Generic;

public static class NecklacesAndBracelets
{
    public static long NecklaceRank(int[] necklace, int k) { return 0; }
    public static int[] NecklaceUnrank(long rank, int n, int k) { return new int[0]; }
    public static long BraceletRank(int[] bracelet, int k) { return 0; }
    public static int[] BraceletUnrank(long rank, int n, int k) { return new int[0]; }

    public static IEnumerable<int[]> GenerateLyndonWords(int n, int k)
    {
        int[] w = new int[n + 1];
        for (int i = 0; i <= n; i++) w[i] = 0;

        int j = 1;
        while (j > 0)
        {
            if (n % j == 0)
            {
                int[] res = new int[j];
                Array.Copy(w, 1, res, 0, j);
                yield return res;
            }
            j = n;
            while (j > 0 && w[j] == k - 1)
            {
                j--;
            }
            if (j > 0)
            {
                w[j]++;
                for (int m = j + 1; m <= n; m++)
                {
                    w[m] = w[m - j];
                }
            }
        }
    }

    public static long LyndonWordRank(int[] word, int k) { return 0; }
    public static int[] LyndonWordUnrank(long rank, int n, int k) { return new int[0]; }

    public static int[] DeBruijnFromLyndon(int k, int n)
    {
        var seq = new List<int>();
        int[] w = new int[n + 1];
        for (int i = 0; i <= n; i++) w[i] = 0;

        int j = 1;
        while (j > 0)
        {
            if (n % j == 0)
            {
                for (int i = 1; i <= j; i++) seq.Add(w[i]);
            }
            j = n;
            while (j > 0 && w[j] == k - 1)
            {
                j--;
            }
            if (j > 0)
            {
                w[j]++;
                for (int m = j + 1; m <= n; m++)
                {
                    w[m] = w[m - j];
                }
            }
        }
        return seq.ToArray();
    }
}
