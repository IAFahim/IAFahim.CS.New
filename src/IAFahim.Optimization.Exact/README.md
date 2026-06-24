# IAFahim.Optimization.Exact

## Description
This package provides exact solvers for NP-hard problems, including Maximum Independent Set, Minimum Set Cover, Maximum Clique, Hamiltonian Path, Hamiltonian Cycle, Traveling Salesperson Problem (using Held-Karp, bitonic, and meet-in-the-middle methods), Minimum Dominating Set, Graph Coloring, and the Steiner Tree problem using the Dreyfus-Wagner algorithm.

## Complexity
These exact solvers solve NP-hard problems with exponential time complexity. Held-Karp runs in O(N^2 * 2^N) steps. Dreyfus-Wagner runs in O(3^T * N + 2^T * N^2) steps where T is the terminal set size.

## API Signature
```csharp
namespace IAFahim.Optimization.Exact
{
    public static unsafe class MaxIndependentSet
    {
        public static int Run(int n, bool* adj, int* used, int* best, int* tmp);
    }

    public static unsafe class TspHeldKarp
    {
        public static long Run(int n, long* w, long inf, long* dp);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Exact;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        long* w = (long*)Marshal.AllocHGlobal(n * n * sizeof(long));
        long* dp = (long*)Marshal.AllocHGlobal((1 << n) * n * sizeof(long));
        try
        {
            w[0] = 0;
            w[1] = 10;
            w[2] = 15;
            w[3] = 20;
            long result = TspHeldKarp.Run(n, w, 999999, dp);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)w);
            Marshal.FreeHGlobal((nint)dp);
        }
    }
}
```