namespace IAFahim.Combinatorics.Generation;

using System;
using System.Runtime.CompilerServices;

public static unsafe class CatalanStructures
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGenerateDyckWord(int n, byte* a, ref bool first)
    {
        if (n == 0) return false;
        if (first)
        {
            for (int i = 0; i < n; i++)
            {
                a[i] = 1;
                a[n + i] = 0;
            }
            first = false;
            return true;
        }

        int ones = n;
        int zeros = n;

        for (int i = 2 * n - 1; i >= 0; i--)
        {
            if (a[i] == 1)
            {
                ones--;
            }
            else
            {
                zeros--;
            }

            if (a[i] == 1)
            {
                if (ones >= zeros + 1)
                {
                    a[i] = 0;
                    int remainingOnes = n - ones;
                    for (int j = i + 1; j < 2 * n; j++)
                    {
                        if (remainingOnes > 0)
                        {
                            a[j] = 1;
                            remainingOnes--;
                        }
                        else
                        {
                            a[j] = 0;
                        }
                    }
                    return true;
                }
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UnrankCatalanObject(long rank, int n, byte* outObj)
    {
        if (n == 0) return;
        long* dp = stackalloc long[(n + 2) * (n + 2)];
        for (int i = 0; i < (n + 2) * (n + 2); i++) dp[i] = 0;
        dp[n * (n + 2) + n] = 1;
        for (int i = n; i >= 0; i--)
        {
            for (int j = n; j >= 0; j--)
            {
                if (i < j) continue;
                if (i == n && j == n) continue;
                long val = 0;
                if (i + 1 <= n) val += dp[(i + 1) * (n + 2) + j];
                if (j + 1 <= i) val += dp[i * (n + 2) + (j + 1)];
                dp[i * (n + 2) + j] = val;
            }
        }

        int x = 0, y = 0;
        for (int i = 0; i < 2 * n; i++)
        {
            long ways = 0;
            if (x + 1 <= n) ways = dp[(x + 1) * (n + 2) + y];
            if (rank < ways)
            {
                outObj[i] = 1;
                x++;
            }
            else
            {
                outObj[i] = 0;
                rank -= ways;
                y++;
            }
        }
    }

    public static long RankCatalanObject(byte* obj, int n)
    {
        if (n == 0) return 0;
        long* dp = stackalloc long[(n + 2) * (n + 2)];
        for (int i = 0; i < (n + 2) * (n + 2); i++) dp[i] = 0;
        dp[n * (n + 2) + n] = 1;
        for (int i = n; i >= 0; i--)
        {
            for (int j = n; j >= 0; j--)
            {
                if (i < j) continue;
                if (i == n && j == n) continue;
                long val = 0;
                if (i + 1 <= n) val += dp[(i + 1) * (n + 2) + j];
                if (j + 1 <= i) val += dp[i * (n + 2) + (j + 1)];
                dp[i * (n + 2) + j] = val;
            }
        }

        int x = 0, y = 0;
        long rank = 0;
        for (int i = 0; i < 2 * n; i++)
        {
            if (obj[i] == 0)
            {
                if (x + 1 <= n) rank += dp[(x + 1) * (n + 2) + y];
                y++;
            }
            else
            {
                x++;
            }
        }
        return rank;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UnrankDyckWord(long rank, int n, byte* outObj)
    {
        UnrankCatalanObject(rank, n, outObj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long RankDyckWord(byte* word, int n)
    {
        return RankCatalanObject(word, n);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long RankBalancedParentheses(byte* s, int n) => RankDyckWord(s, n);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UnrankBalancedParentheses(long rank, int n, byte* outObj) => UnrankDyckWord(rank, n, outObj);
}