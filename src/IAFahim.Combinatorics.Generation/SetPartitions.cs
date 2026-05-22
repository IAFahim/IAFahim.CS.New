namespace IAFahim.Combinatorics.Generation;

using System;
using System.Collections.Generic;

public static class SetPartitions
{
    // Integer Partitions
    public static IEnumerable<int[]> GenerateIntegerPartitions(int n)
    {
        int[] p = new int[n];
        int k = 0;
        p[k] = n;
        while (true)
        {
            int[] res = new int[k + 1];
            Array.Copy(p, res, k + 1);
            yield return res;
            int rem_val = 0;
            while (k >= 0 && p[k] == 1)
            {
                rem_val += p[k];
                k--;
            }
            if (k < 0) yield break;
            p[k]--;
            rem_val++;
            while (rem_val > p[k])
            {
                p[k + 1] = p[k];
                rem_val -= p[k];
                k++;
            }
            p[k + 1] = rem_val;
            k++;
        }
    }

    public static long RankIntegerPartition(int[] partition, int n)
    {
        // Placeholder for rank
        return 0;
    }

    public static int[] UnrankIntegerPartition(long rank, int n)
    {
        // Placeholder for unrank
        return new int[0];
    }

    // Set Partitions
    public static IEnumerable<int[]> GenerateSetPartitions(int n)
    {
        int[] kappa = new int[n];
        int[] m = new int[n];
        yield return (int[])kappa.Clone();

        while (true)
        {
            int i = n - 1;
            while (i > 0 && kappa[i] == m[i - 1] + 1) i--;
            if (i == 0) yield break;
            kappa[i]++;
            m[i] = Math.Max(m[i], kappa[i]);
            for (int j = i + 1; j < n; j++)
            {
                kappa[j] = 0;
                m[j] = m[i];
            }
            yield return (int[])kappa.Clone();
        }
    }

    public static long RankSetPartition(int[] partition)
    {
        return 0;
    }

    public static int[] UnrankSetPartition(long rank, int n)
    {
        return new int[n];
    }

    // Compositions
    public static IEnumerable<int[]> GenerateCompositions(int n, int k)
    {
        int[] comp = new int[k];
        comp[0] = n;
        yield return (int[])comp.Clone();
        while (comp[k - 1] != n)
        {
            int i = k - 2;
            while (comp[i] == 0) i--;
            comp[i]--;
            int val = comp[i + 1];
            comp[i + 1] = n;
            comp[k - 1] = val + 1;
            int sum = 0;
            for (int j = 0; j < k; j++)
            {
                if (j != i + 1 && j != k - 1) sum += comp[j];
            }
            comp[i + 1] = n - sum - comp[k - 1];
            yield return (int[])comp.Clone();
        }
    }

    public static long RankComposition(int[] composition)
    {
        return 0;
    }

    public static int[] UnrankComposition(long rank, int n, int k)
    {
        return new int[k];
    }
}
