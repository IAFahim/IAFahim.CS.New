# IAFahim.DP.Optimization

## Description
This package provides dynamic programming optimizations. It features Knuth optimization for reducing complexity on interval DP, and a Li Chao tree line-insertion optimization for linear function queries.

## Complexity
- Knuth Optimization: O(N^2) instead of O(N^3) for interval DP.
- Li Chao Tree insertion/query: O(log R) where R is the coordinate range.

## API Signature
```csharp
public static unsafe class KnuthOptimization
{
    public static long Run(int n, long* dp, long* a, long* opt);
}

public static unsafe class LiChaoAddLine
{
    public static void Run(long* seg, long m, long b, int node, int l, int r, long x1, long x2);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DP.Optimization;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        long* dp = (long*)Marshal.AllocHGlobal((n * n) * sizeof(long));
        long* a = (long*)Marshal.AllocHGlobal((n * n) * sizeof(long));
        long* opt = (long*)Marshal.AllocHGlobal((n * n) * sizeof(long));
        try
        {
            long cost = KnuthOptimization.Run(n, dp, a, opt);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)dp);
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)opt);
        }
    }
}
```