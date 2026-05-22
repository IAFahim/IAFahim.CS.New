namespace IAFahim.Combinatorics.Generation;

using System;
using System.Collections.Generic;

public static class Combinations
{
    // Multiset combinations
    public static IEnumerable<int[]> GenerateMultisetCombinations(int[] m, int k)
    {
        int n = m.Length;
        int[] comb = new int[n];
        int sum = 0;
        for (int i = 0; i < n; i++)
        {
            comb[i] = Math.Min(m[i], k - sum);
            sum += comb[i];
        }

        if (sum < k) yield break;

        yield return (int[])comb.Clone();

        while (true)
        {
            int i = n - 1;
            while (i >= 0 && comb[i] == 0) i--;
            if (i < 0) yield break;

            int j = i - 1;
            while (j >= 0 && comb[j] == m[j]) j--;
            if (j < 0) yield break;

            comb[j]++;
            int diff = comb[i] - 1;
            comb[i] = 0;

            int curr_sum = 0;
            for (int x = 0; x <= j; x++) curr_sum += comb[x];

            for (int x = j + 1; x < n; x++)
            {
                int take = Math.Min(m[x], k - curr_sum);
                comb[x] = take;
                curr_sum += take;
            }

            yield return (int[])comb.Clone();
        }
    }

    public static long RankMultisetCombination(int[] comb, int[] m) { return 0; }
    public static int[] UnrankMultisetCombination(long rank, int[] m, int k) { return new int[0]; }

    // Cool-lex combinations
    public static IEnumerable<int[]> GenerateCoolLexCombinations(int n, int t)
    {
        int[] c = new int[t + 2];
        for (int i = 1; i <= t; i++) c[i] = i;
        c[t + 1] = n + 1;

        yield return GetSubset(c, t);

        int j = 1, x = 0, y = 0;
        while (c[t] < n || c[t - 1] < n - 1)
        {
            if (j % 2 == 1)
            {
                if (c[1] + 1 < c[2])
                {
                    c[1]++;
                }
                else
                {
                    j = 2;
                    c[1] = 1;
                    c[j]++;
                }
            }
            else
            {
                if (c[j] + 1 < c[j + 1])
                {
                    c[j - 1] = c[j];
                    c[j]++;
                    j--;
                }
                else
                {
                    j++;
                    c[j - 1] = j - 1;
                    c[j]++;
                }
            }
            yield return GetSubset(c, t);
        }
    }

    private static int[] GetSubset(int[] c, int t)
    {
        int[] res = new int[t];
        for (int i = 0; i < t; i++) res[i] = c[i + 1] - 1;
        return res;
    }

    // Revolving door combinations
    public static IEnumerable<int[]> GenerateRevolvingDoorCombinations(int n, int k)
    {
        int[] c = new int[k + 2];
        for (int i = 1; i <= k; i++) c[i] = i;
        c[k + 1] = n + 1;
        int j = 1;

        yield return GetSubset(c, k);

        while (true)
        {
            if (k % 2 != 0)
            {
                if (c[1] + 1 < c[2])
                {
                    c[1]++;
                }
                else
                {
                    j = 2;
                    while (j <= k && c[j] + 1 == c[j + 1]) j++;
                    if (j > k) break;
                    c[j]++;
                    c[j - 1] = c[j - 2];
                    c[j - 2] = j - 2;
                }
            }
            else
            {
                if (c[1] > 1)
                {
                    c[1]--;
                }
                else
                {
                    j = 2;
                    while (j <= k && c[j] + 1 == c[j + 1]) j++;
                    if (j > k) break;
                    if (c[j - 1] > j - 1)
                    {
                        c[j - 1]--;
                    }
                    else
                    {
                        c[j]++;
                        c[j - 1] = c[j - 2];
                        c[j - 2] = j - 2;
                    }
                }
            }
            yield return GetSubset(c, k);
        }
    }

    public static IEnumerable<int[]> GenerateChaseCombinations(int n, int k)
    {
        return GenerateRevolvingDoorCombinations(n, k); // Stub for chase sequence
    }
}
