# IAFahim.Optimization.Knapsack

## Description
This package implements various knapsack optimization algorithms. It includes divide-and-conquer knapsack solvers, multiple choice knapsack solvers, bounded knapsack solvers using binary split or monotone queue optimization, meet-in-the-middle knapsack solvers, K-Sum solvers, and Subset Sum solvers.

## Complexity
Divide-and-conquer and bounded knapsack solvers run in O(N * W) steps where W is the weight limit. Meet-in-the-middle runs in O(2^(N/2)) steps. Subset sum checking runs in O(N * Target / 64) steps using bit-level parallelism.

## API Signature
```csharp
namespace IAFahim.Optimization.Knapsack
{
    public static unsafe class SubsetSum
    {
        public static bool Can(long* w, int n, long target);
    }

    public static unsafe class MeetInMiddle
    {
        public static long Run(long* w, long* v, int n, long cap, long* left);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Knapsack;

public unsafe class Example
{
    public static void Run()
    {
        int n = 3;
        long* w = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        try
        {
            w[0] = 5;
            w[1] = 10;
            w[2] = 12;
            bool possible = SubsetSum.Can(w, n, 15);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)w);
        }
    }
}
```