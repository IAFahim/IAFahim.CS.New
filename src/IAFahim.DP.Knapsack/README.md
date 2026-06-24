# IAFahim.DP.Knapsack

## Description
This package provides dynamic programming algorithms specifically for knapsack optimization. It features implementations for the 0-1 knapsack, unbounded knapsack, and bounded knapsack models, along with subset sum and bitset-accelerated subset sum solvers.

## Complexity
- 0-1 Knapsack: O(N * W) where N is the number of items and W is the capacity.
- Unbounded Knapsack: O(N * W) where N is the number of items and W is the capacity.
- Bounded Knapsack: O(W * Sum(log(Count_i))) using binary split.

## API Signature
```csharp
public static unsafe class Knapsack01
{
    public static long Run(int n, long cap, long* w, long* v, long* dp);
}

public static unsafe class KnapsackUnbounded
{
    public static long Run(int n, long cap, long* w, long* v, long* dp);
}

public static unsafe class KnapsackBounded
{
    public static long Run(int n, long cap, long* w, long* v, long* cnt, long* dp);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DP.Knapsack;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 3;
        long cap = 50;
        long* w = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* v = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* dp = (long*)Marshal.AllocHGlobal((cap + 1) * sizeof(long));
        try
        {
            w[0] = 10; v[0] = 60;
            w[1] = 20; v[1] = 100;
            w[2] = 30; v[2] = 120;
            long maxVal = Knapsack01.Run(n, cap, w, v, dp);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)w);
            Marshal.FreeHGlobal((nint)v);
            Marshal.FreeHGlobal((nint)dp);
        }
    }
}
```