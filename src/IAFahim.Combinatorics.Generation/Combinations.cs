namespace IAFahim.Combinatorics.Generation;

using System;
using System.Runtime.CompilerServices;

public static unsafe class Combinations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryNextMultiset(int* m, int n, int k, int* comb, ref bool first)
    {
        if (first)
        {
            int sum = 0;
            for (int i = n - 1; i >= 0; i--)
            {
                comb[i] = Math.Min(m[i], k - sum);
                sum += comb[i];
            }
            first = false;
            return sum == k;
        }

        int idx = n - 1;
        while (idx >= 0 && comb[idx] == 0) idx--;
        if (idx < 0) return false;

        int j = idx - 1;
        while (j >= 0 && comb[j] == m[j]) j--;
        if (j < 0) return false;

        comb[j]++;
        comb[idx] = 0;

        int curSum = 0;
        for (int x = 0; x <= j; x++) curSum += comb[x];

        for (int x = j + 1; x < n; x++)
        {
            int take = Math.Min(m[x], k - curSum);
            comb[x] = take;
            curSum += take;
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGenerateCoolLex(int n, int t, int* c, int* res, int* scratch)
    {
        if (c == null)
        {
            c = scratch;
            c[0] = 1;
            for (int i = 0; i < t; i++)
            {
                c[i + 1] = n - t + i;
                res[i] = c[i + 1];
            }
            return true;
        }

        int L = n;
        for (int i = 2; i < n; i++)
        {
            bool bit_im2 = false;
            bool bit_im1 = false;
            for (int k = 1; k <= t; k++)
            {
                if (c[k] == i - 2) bit_im2 = true;
                if (c[k] == i - 1) bit_im1 = true;
            }
            if (!bit_im2 && bit_im1)
            {
                L = i + 1;
                break;
            }
        }

        int* nextP = stackalloc int[t];
        for (int k = 0; k < t; k++)
        {
            int val = c[k + 1];
            if (val < L)
            {
                nextP[k] = (val + 1) % L;
            }
            else
            {
                nextP[k] = val;
            }
        }

        for (int i = 1; i < t; i++)
        {
            int key = nextP[i];
            int j = i - 1;
            while (j >= 0 && nextP[j] > key)
            {
                nextP[j + 1] = nextP[j];
                j--;
            }
            nextP[j + 1] = key;
        }

        bool isStart = true;
        for (int i = 0; i < t; i++)
        {
            if (nextP[i] != n - t + i)
            {
                isStart = false;
                break;
            }
        }

        if (isStart)
        {
            return false;
        }

        for (int i = 0; i < t; i++)
        {
            c[i + 1] = nextP[i];
            res[i] = nextP[i];
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGenerateRevolvingDoor(int n, int k, int* c, int* res, int* scratch)
    {
        if (c == null)
        {
            c = scratch;
            c[0] = 0;
            for (int i = 1; i <= k; i++)
            {
                c[i] = i;
                res[i - 1] = i - 1;
            }
            return true;
        }

        int rank = c[0];
        int j = 0;
        while (true)
        {
            if (0 < j || (k % 2) == 0)
            {
                j++;
                if (k < j)
                {
                    return false;
                }
                if (c[j] != j)
                {
                    c[j]--;
                    if (j != 1)
                    {
                        c[j - 1] = j - 1;
                    }
                    c[0] = rank + 1;
                    for (int i = 0; i < k; i++)
                    {
                        res[i] = c[i + 1] - 1;
                    }
                    return true;
                }
            }

            j++;
            if (j < k)
            {
                if (c[j] != c[j + 1] - 1)
                {
                    break;
                }
            }
            else
            {
                if (c[j] != n)
                {
                    break;
                }
            }
        }

        c[j] += 1;
        if (j != 1)
        {
            c[j - 1] = c[j] - 1;
        }
        c[0] = rank + 1;
        for (int i = 0; i < k; i++)
        {
            res[i] = c[i + 1] - 1;
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGenerateChase(int n, int k, int* c, int* res, int* scratch)
    {
        return TryGenerateRevolvingDoor(n, k, c, res, scratch);
    }

    public static long RankMultisetCombination(int* comb, int* m, int n, int k)
    {
        return 0;
    }

    public static bool UnrankMultisetCombination(long rank, int* m, int n, int k, int* comb)
    {
        return false;
    }
}