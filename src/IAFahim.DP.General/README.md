# IAFahim.DP.General

## Description
This package provides general dynamic programming routines. It implements profile DP, broken profile DP, tree knapsack, interval DP, min-plus convolution, and quadrangle inequality DP optimizations. It operates using raw pointers for speed.

## Complexity
- Profile DP: O(N * 2^M) where M is the profile width and N is the grid length.
- Tree Knapsack: O(N * W) where N is the tree node count and W is the capacity constraint.
- Interval DP: O(N^3) or O(N^2) with optimization.

## API Signature
```csharp
public static unsafe class ProfileDp
{
    public static long Run(int m, int n, int* a, long* dp, long* tmp);
}

public static unsafe class TreeKnapsack
{
    public static void Run(int u, int p, int* head, int* to, int* next, int* w, long* v, long* dp, long* tmp, int cap);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DP.General;

public static unsafe class Example
{
    public static void Run()
    {
        int m = 3;
        int n = 3;
        int* a = (int*)Marshal.AllocHGlobal((m * n) * sizeof(int));
        long* dp = (long*)Marshal.AllocHGlobal((1 << m) * sizeof(long));
        long* tmp = (long*)Marshal.AllocHGlobal((1 << m) * sizeof(long));
        try
        {
            a[0] = 1; a[1] = 0; a[2] = 1;
            a[3] = 0; a[4] = 1; a[5] = 0;
            a[6] = 1; a[7] = 0; a[8] = 1;
            long result = ProfileDp.Run(m, n, a, dp, tmp);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)dp);
            Marshal.FreeHGlobal((nint)tmp);
        }
    }
}
```