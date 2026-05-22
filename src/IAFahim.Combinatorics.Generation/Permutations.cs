namespace IAFahim.Combinatorics.Generation;

using System;
using System.Collections.Generic;

public static class Permutations
{
    public static IEnumerable<int[]> GenerateHeapPermutations(int n)
    {
        int[] a = new int[n];
        for (int i = 0; i < n; i++) a[i] = i;
        int[] c = new int[n];

        yield return (int[])a.Clone();

        int i_var = 1;
        while (i_var < n)
        {
            if (c[i_var] < i_var)
            {
                if (i_var % 2 == 0)
                {
                    (a[0], a[i_var]) = (a[i_var], a[0]);
                }
                else
                {
                    (a[c[i_var]], a[i_var]) = (a[i_var], a[c[i_var]]);
                }
                yield return (int[])a.Clone();
                c[i_var]++;
                i_var = 1;
            }
            else
            {
                c[i_var] = 0;
                i_var++;
            }
        }
    }

    public static IEnumerable<int[]> GenerateJohnsonTrotter(int n)
    {
        int[] a = new int[n];
        bool[] dir = new bool[n]; // false: left, true: right
        for (int i = 0; i < n; i++)
        {
            a[i] = i;
            dir[i] = false;
        }

        yield return (int[])a.Clone();

        while (true)
        {
            int mobile_idx = -1;
            int mobile_val = -1;
            for (int i = 0; i < n; i++)
            {
                if (dir[a[i]] == false && i > 0 && a[i] > a[i - 1])
                {
                    if (a[i] > mobile_val) { mobile_val = a[i]; mobile_idx = i; }
                }
                if (dir[a[i]] == true && i < n - 1 && a[i] > a[i + 1])
                {
                    if (a[i] > mobile_val) { mobile_val = a[i]; mobile_idx = i; }
                }
            }

            if (mobile_idx == -1) break;

            int swap_idx = dir[a[mobile_idx]] ? mobile_idx + 1 : mobile_idx - 1;
            (a[mobile_idx], a[swap_idx]) = (a[swap_idx], a[mobile_idx]);

            for (int i = 0; i < n; i++)
            {
                if (a[i] > mobile_val)
                {
                    dir[a[i]] = !dir[a[i]];
                }
            }
            yield return (int[])a.Clone();
        }
    }

    public static IEnumerable<int[]> GeneratePlainChanges(int n) => GenerateJohnsonTrotter(n);

    public static IEnumerable<int[]> GeneratePermutationsWithDuplicates(int[] elements)
    {
        Array.Sort(elements);
        do
        {
            yield return (int[])elements.Clone();
        } while (NextPermutation(elements));
    }

    private static bool NextPermutation(int[] elements)
    {
        int i = elements.Length - 2;
        while (i >= 0 && elements[i] >= elements[i + 1]) i--;
        if (i < 0) return false;

        int j = elements.Length - 1;
        while (elements[j] <= elements[i]) j--;

        (elements[i], elements[j]) = (elements[j], elements[i]);
        Array.Reverse(elements, i + 1, elements.Length - i - 1);
        return true;
    }

    public static IEnumerable<int[]> GenerateDerangements(int n)
    {
        int[] a = new int[n];
        for (int i = 0; i < n; i++) a[i] = i;
        do
        {
            bool ok = true;
            for (int i = 0; i < n; i++) if (a[i] == i) { ok = false; break; }
            if (ok) yield return (int[])a.Clone();
        } while (NextPermutation(a));
    }

    public static long InvolutionCount(int n)
    {
        if (n <= 1) return 1;
        long[] dp = new long[n + 1];
        dp[0] = 1; dp[1] = 1;
        for (int i = 2; i <= n; i++)
        {
            dp[i] = dp[i - 1] + (i - 1) * dp[i - 2];
        }
        return dp[n];
    }

    public static IEnumerable<int[]> GenerateInvolutions(int n)
    {
        int[] a = new int[n];
        for (int i = 0; i < n; i++) a[i] = i;
        do
        {
            bool ok = true;
            for (int i = 0; i < n; i++) if (a[a[i]] != i) { ok = false; break; }
            if (ok) yield return (int[])a.Clone();
        } while (NextPermutation(a));
    }

    public static int[] RandomPermutation(int n)
    {
        Random rnd = new Random();
        int[] a = new int[n];
        for (int i = 0; i < n; i++) a[i] = i;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            (a[i], a[j]) = (a[j], a[i]);
        }
        return a;
    }
}
