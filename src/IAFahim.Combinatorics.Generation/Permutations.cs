namespace IAFahim.Combinatorics.Generation;

using System;
using System.Runtime.CompilerServices;

public static unsafe class Permutations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void HeapPermutation(int n, int* a, int* c, bool* hasNext, ref bool firstCall)
    {
        if (firstCall)
        {
            for (int i = 0; i < n; i++) a[i] = i;
            for (int i = 0; i < n; i++) c[i] = 0;
            firstCall = false;
            *hasNext = true;
            return;
        }

        int iVar = 1;
        while (iVar < n)
        {
            if (c[iVar] < iVar)
            {
                if ((iVar & 1) == 0)
                {
                    int tmp = a[0]; a[0] = a[iVar]; a[iVar] = tmp;
                }
                else
                {
                    int tmp = a[c[iVar]]; a[c[iVar]] = a[iVar]; a[iVar] = tmp;
                }
                *hasNext = true;
                c[iVar]++;
                return;
            }
            c[iVar] = 0;
            iVar++;
        }
        *hasNext = false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void JohnsonTrotter(int n, int* a, bool* dir, bool* hasNext, ref bool firstCall)
    {
        if (firstCall)
        {
            for (int i = 0; i < n; i++)
            {
                a[i] = i;
                dir[i] = false;
            }
            firstCall = false;
            *hasNext = true;
            return;
        }

        int mobileIdx = -1;
        int mobileVal = -1;
        for (int i = 0; i < n; i++)
        {
            if (dir[a[i]] == false && i > 0 && a[i] > a[i - 1])
            {
                if (a[i] > mobileVal) { mobileVal = a[i]; mobileIdx = i; }
            }
            if (dir[a[i]] == true && i < n - 1 && a[i] > a[i + 1])
            {
                if (a[i] > mobileVal) { mobileVal = a[i]; mobileIdx = i; }
            }
        }

        if (mobileIdx == -1) { *hasNext = false; return; }

        int swapIdx = dir[a[mobileIdx]] ? mobileIdx + 1 : mobileIdx - 1;
        int tmp = a[mobileIdx]; a[mobileIdx] = a[swapIdx]; a[swapIdx] = tmp;

        for (int i = 0; i < n; i++)
        {
            if (a[i] > mobileVal) dir[a[i]] = !dir[a[i]];
        }
        *hasNext = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PermutationsWithDuplicates(int n, int* elements, bool* hasNext, ref bool firstCall)
    {
        if (firstCall)
        {
            firstCall = false;
            *hasNext = true;
            return;
        }

        int i = n - 2;
        while (i >= 0 && elements[i] >= elements[i + 1]) i--;
        if (i < 0) { *hasNext = false; return; }

        int j = n - 1;
        while (elements[j] <= elements[i]) j--;

        int tmp = elements[i]; elements[i] = elements[j]; elements[j] = tmp;

        int l = i + 1, r = n - 1;
        while (l < r)
        {
            tmp = elements[l]; elements[l] = elements[r]; elements[r] = tmp;
            l++; r--;
        }
        *hasNext = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Derangement(int n, int* a, bool* hasNext, ref bool firstCall)
    {
        if (firstCall)
        {
            for (int i = 0; i < n; i++) a[i] = i;
            firstCall = false;
        }

        while (true)
        {
            int i = n - 2;
            while (i >= 0 && a[i] >= a[i + 1]) i--;
            if (i < 0) { *hasNext = false; return; }

            int j = n - 1;
            while (a[j] <= a[i]) j--;

            int tmp = a[i]; a[i] = a[j]; a[j] = tmp;

            int l = i + 1, r = n - 1;
            while (l < r)
            {
                tmp = a[l]; a[l] = a[r]; a[r] = tmp;
                l++; r--;
            }

            bool ok = true;
            for (int k = 0; k < n; k++) if (a[k] == k) { ok = false; break; }
            if (ok) { *hasNext = true; return; }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RandomPermutation(int n, int* a, ref uint seed)
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
    public static void RandomDerangement(int n, int* a, ref uint seed)
    {
        RandomPermutation(n, a, ref seed);
        for (int i = 0; i < n; i++)
        {
            if (a[i] == i)
            {
                int j = (i + 1) % n;
                int tmp = a[i]; a[i] = a[j]; a[j] = tmp;
            }
        }
    }

    public static long InvolutionCount(int n)
    {
        if (n <= 1) return 1;
        long* dp = stackalloc long[n + 1];
        dp[0] = 1; dp[1] = 1;
        for (int i = 2; i <= n; i++) dp[i] = dp[i - 1] + (i - 1) * dp[i - 2];
        return dp[n];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Involution(int n, int* a, bool* hasNext, ref bool firstCall)
    {
        if (firstCall)
        {
            for (int i = 0; i < n; i++) a[i] = i;
            firstCall = false;
            *hasNext = true;
            return;
        }

        while (true)
        {
            int i = n - 2;
            while (i >= 0 && a[i] >= a[i + 1]) i--;
            if (i < 0) { *hasNext = false; return; }

            int j = n - 1;
            while (a[j] <= a[i]) j--;

            int tmp = a[i]; a[i] = a[j]; a[j] = tmp;

            int l = i + 1, r = n - 1;
            while (l < r)
            {
                tmp = a[l]; a[l] = a[r]; a[r] = tmp;
                l++; r--;
            }

            bool ok = true;
            for (int k = 0; k < n; k++) if (a[a[k]] != k) { ok = false; break; }
            if (ok) { *hasNext = true; return; }
        }
    }
}