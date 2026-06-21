namespace IAFahim.String.Parse
{
using System.Runtime.InteropServices;
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Cyk
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Parse(int* terminals, int* productions, int prodCount, int startVar, byte* input, int len)
        {
            int n = len;
            if (n <= 0) return false;
            bool*** dp = (bool***)Marshal.AllocHGlobal(sizeof(bool**) * n);
            for (int i = 0; i < n; i++)
            {
                dp[i] = (bool**)Marshal.AllocHGlobal(sizeof(bool*) * n);
                for (int j = 0; j < n; j++)
                    dp[i][j] = (bool*)Marshal.AllocHGlobal(sizeof(bool) * 256);
            }
            for (int i = 0; i < n; i++)
            {
                for (int s = 0; s < 256; s++)
                    dp[i][i][s] = false;
                for (int p = 0; p < prodCount; p++)
                {
                    if (terminals[p] == input[i])
                        dp[i][i][productions[p]] = true;
                }
            }
            for (int l = 2; l <= n; l++)
            {
                for (int i = 0; i <= n - l; i++)
                {
                    int j = i + l - 1;
                    for (int k = i; k < j; k++)
                    {
                        for (int p = 0; p < prodCount; p++)
                        {
                            int left = productions[p * 3];
                            int mid = productions[p * 3 + 1];
                            int right = productions[p * 3 + 2];
                            if (dp[i][k][left] && dp[k + 1][j][right])
                                dp[i][j][mid] = true;
                        }
                    }
                }
            }
            bool result = dp[0][n - 1][startVar];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    Marshal.FreeHGlobal((nint)dp[i][j]);
                Marshal.FreeHGlobal((nint)dp[i]);
            }
            Marshal.FreeHGlobal((nint)dp);
            return result;
        }
    }
}
