# IAFahim.DP

## Description
This package provides a collection of dynamic programming algorithms and optimizations. It includes multiple knapsack variants, subset sum solvers, divide and conquer optimization, Knuth optimization, Convex Hull Trick, Li Chao Tree, SMAWK algorithm, Alien DP, and Sum over Subsets (SOS) DP.

## Complexity
- Knapsack 01: O(N * W) where N is the item count and W is the capacity.
- SMAWK: O(N + M) for an N x M totally monotone grid.
- Sum over Subsets: O(N * 2^N) where N is the number of bits.
- Convex Hull Trick query: O(log N) or O(1).

## API Signature
```csharp
public static unsafe class Knapsack01
{
    public static long Run(int n, long capacity, long* weight, long* value, long* dp);
    public static long RunSpaceOptimized(int n, long capacity, long* weight, long* value, long* dp);
}

public static unsafe class SubsetSum
{
    public static bool Run(int n, long target, long* arr, bool* dp);
}

public static unsafe class SosDp
{
    public static void Run(int n, long* f);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DP;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 3;
        long target = 10;
        long* arr = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        bool* dp = (bool*)Marshal.AllocHGlobal((target + 1) * sizeof(bool));
        try
        {
            arr[0] = 3; arr[1] = 5; arr[2] = 8;
            bool possible = SubsetSum.Run(n, target, arr, dp);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)arr);
            Marshal.FreeHGlobal((nint)dp);
        }
    }
}
```