# IAFahim.Search.Numerical

## Description
This package provides numerical search, optimization, and integration methods, including simulated annealing, ternary real search, and adaptive integration.

## Complexity
Annealing runs for a fixed iteration count. Ternary search converges in O(log((hi - lo)/tol)) operations. Adaptive integration runs dynamically.

## API Signature
```csharp
namespace IAFahim.Search.Numerical
{
    public static unsafe class TernaryReal
    {
        public static double Run(double* func, int maxIter, double lo, double hi);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Numerical;

public static unsafe class Program
{
    public static void Main()
    {
        int maxIter = 100;
        double lo = 0.0;
        double hi = 10.0;
        double* func = (double*)Marshal.AllocHGlobal(sizeof(double));
        try
        {
            double res = TernaryReal.Run(func, maxIter, lo, hi);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)func);
        }
    }
}
```