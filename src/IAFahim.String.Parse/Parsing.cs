namespace IAFahim.String.Parse
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class Cyk
    {
        private const int SymbolCount = 256;

        private const int ProductionStride = 3;

        private const int ProductionLeft = 0;

        private const int ProductionMid = 1;

        private const int ProductionRight = 2;

        private const int MinCompoundSpan = 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool*** AllocCube(int n)
        {
            bool*** dp = (bool***)Marshal.AllocHGlobal((nint)((long)n * sizeof(bool**)));
            for (int i = 0; i < n; i++)
            {
                dp[i] = (bool**)Marshal.AllocHGlobal((nint)((long)n * sizeof(bool*)));
                for (int j = 0; j < n; j++)
                    dp[i][j] = (bool*)Marshal.AllocHGlobal((nint)((long)SymbolCount * sizeof(bool)));
            }
            return dp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitBaseCase(bool*** dp, int* terminals, int* productions, int prodCount, byte* input, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int s = 0; s < SymbolCount; s++)
                    dp[i][i][s] = false;
                for (int p = 0; p < prodCount; p++)
                {
                    if (terminals[p] == input[i])
                        dp[i][i][productions[p]] = true;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FillUpperTriangle(bool*** dp, int* productions, int prodCount, int n)
        {
            for (int l = MinCompoundSpan; l <= n; l++)
            {
                for (int i = 0; i <= n - l; i++)
                {
                    int j = i + l - 1;
                    for (int k = i; k < j; k++)
                    {
                        for (int p = 0; p < prodCount; p++)
                        {
                            int left = productions[p * ProductionStride + ProductionLeft];
                            int mid = productions[p * ProductionStride + ProductionMid];
                            int right = productions[p * ProductionStride + ProductionRight];
                            if (dp[i][k][left] && dp[k + 1][j][right])
                                dp[i][j][mid] = true;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FreeCube(bool*** dp, int n)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    Marshal.FreeHGlobal((nint)dp[i][j]);
                Marshal.FreeHGlobal((nint)dp[i]);
            }
            Marshal.FreeHGlobal((nint)dp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Parse(int* terminals, int* productions, int prodCount, int startVar, byte* input, int len)
        {
            int n = len;
            if (n <= 0) return false;
            bool*** dp = AllocCube(n);
            InitBaseCase(dp, terminals, productions, prodCount, input, n);
            FillUpperTriangle(dp, productions, prodCount, n);
            bool result = dp[0][n - 1][startVar];
            FreeCube(dp, n);
            return result;
        }
    }
}
