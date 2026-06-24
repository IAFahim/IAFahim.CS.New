# IAFahim.Optimization.Matroid

## Description
This package provides matroid-based optimization algorithms. It includes greedy solvers for independent sets on matroids and rank determination for linear matroids.

## Complexity
The matroid greedy solver runs in O(N log N + N * I) steps where I is the cost of the independent check. Linear matroid rank determination runs in O(N * M^2) steps where N is the number of vectors and M is their dimension.

## API Signature
```csharp
namespace IAFahim.Optimization.Matroid
{
    public static unsafe class LinearMatroid
    {
        public static int Rank(int n, int m, int* a, int* basis);
    }

    public static unsafe class MatroidGreedy
    {
        public static long Run(int n, int* set, int setSize, long* weight, delegate*<int*, int, int, bool> independent);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Matroid;

public unsafe class Example
{
    public static void Run()
    {
        int n = 3;
        int m = 3;
        int* a = (int*)Marshal.AllocHGlobal(n * m * sizeof(int));
        int* basis = (int*)Marshal.AllocHGlobal(m * sizeof(int));
        try
        {
            a[0] = 1; a[1] = 0; a[2] = 0;
            a[3] = 0; a[4] = 1; a[5] = 0;
            a[6] = 0; a[7] = 0; a[8] = 1;
            int rank = LinearMatroid.Rank(n, m, a, basis);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)basis);
        }
    }
}
```