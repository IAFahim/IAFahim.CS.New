# IAFahim.Optimization.Treewidth

## Description
This package provides algorithms for treewidth-based dynamic programming optimization. It includes Cut and Count for graph problems on tree decompositions, Convex Hull checks for Monge properties, treewidth rank dynamic programming, fast subset dynamic programming, and rank transformations.

## Complexity
Fast subset dynamic programming runs in O(3^N) steps. Rank dynamic programming runs in O(N * 2^W) steps where W is the treewidth. Cut and Count runs in O(c^W * N) steps. Rank transformation runs in O(N log N) steps.

## API Signature
```csharp
namespace IAFahim.Optimization.Treewidth
{
    public static unsafe class FastSubsetDp
    {
        public static void Run(long* f, long* g, int n, int k);
    }

    public static unsafe class RankTransform
    {
        public static void Run(int* x, int n, int* rank);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Treewidth;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        int* x = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        int* rank = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        try
        {
            x[0] = 40;
            x[1] = 10;
            x[2] = 30;
            x[3] = 20;
            RankTransform.Run(x, n, rank);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)x);
            Marshal.FreeHGlobal((nint)rank);
        }
    }
}
```