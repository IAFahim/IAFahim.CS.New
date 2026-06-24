# IAFahim.Optimization.Submodular

## Description
This package provides algorithms for submodular optimization. It includes Max-Cut solvers (using local search and the Goemans-Williamson semidefinite programming approximation), submodular greedy solvers, greedy set cover solvers, and rounding methods (random rounding, dependent rounding, pipage rounding).

## Complexity
Submodular greedy solvers run in O(k * N) steps. Max-Cut local search runs in O(N^2) expected steps. Goemans-Williamson solver runs in polynomial time. Pipage rounding runs in O(N^2) steps.

## API Signature
```csharp
namespace IAFahim.Optimization.Submodular
{
    public static unsafe class SubmodularGreedy
    {
        public static long Run(int n, long* gain, int k, int* selected);
        public static long GreedySetCover(int n, int* elemCounts, int** sets, int m, int* cover);
    }

    public static unsafe class Rounding
    {
        public static void Dependent(int n, double* frac, int* result);
        public static void Pipage(int n, double* frac, int* result);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Submodular;

public unsafe class Example
{
    public static void Run()
    {
        int n = 5;
        long* gain = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        int* selected = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        try
        {
            gain[0] = 10;
            gain[1] = 8;
            gain[2] = 6;
            gain[3] = 4;
            gain[4] = 2;
            long total = SubmodularGreedy.Run(n, gain, 3, selected);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)gain);
            Marshal.FreeHGlobal((nint)selected);
        }
    }
}
```