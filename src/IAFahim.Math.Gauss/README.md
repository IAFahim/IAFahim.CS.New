# IAFahim.Math.Gauss

## Description
Provides Gaussian elimination solver for linear equation systems over real numbers (double) and modular arithmetic (mod P). Also computes the determinant of a square matrix mod P.

## Complexity
- GaussEliminationDouble / GaussModP: O(N^2 * M) time, O(1) space.
- Determinant: O(N^3) time, O(1) space.

## API Signature
- public static int GaussEliminationDouble.Run(double* a, double* b, double* x, int n, int m)
- public static bool GaussModP.Run(long* a, long* b, long* x, int n, int m, long mod)
- public static long GaussModP.Determinant(long* a, int n, long mod)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Math.Gauss;

public unsafe class Example
{
    public static void Main()
    {
        int n = 2;
        int m = 3;
        double* a = (double*)Marshal.AllocHGlobal(n * m * sizeof(double));
        double* b = (double*)Marshal.AllocHGlobal(n * sizeof(double));
        double* x = (double*)Marshal.AllocHGlobal(n * sizeof(double));
        try
        {
            a[0] = 2.0; a[1] = 1.0; a[2] = 0.0;
            a[3] = 1.0; a[4] = -1.0; a[5] = 0.0;
            b[0] = 5.0; b[1] = 1.0;
            int rank = GaussEliminationDouble.Run(a, b, x, n, m);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)a);
            Marshal.FreeHGlobal((IntPtr)b);
            Marshal.FreeHGlobal((IntPtr)x);
        }
    }
}
```