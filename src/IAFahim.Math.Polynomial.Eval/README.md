# IAFahim.Math.Polynomial.Eval

## Description
Provides advanced polynomial evaluation techniques. Includes multi-point evaluation of a polynomial at multiple points, and the Chirp Z-Transform (CZT) for evaluating a polynomial at points in a geometric progression.

## Complexity
- MultiPointEval: O((N + M) * log^2(N)) time, O(N + M) space.
- ChirpZTransform: O((N + M) * log(N + M)) time, O(N + M) space.

## API Signature
- public static void MultiPointEval.Run(int n, long* poly, int m, long* x, long* res, long mod)
- public static int ChirpZTransform.Run(int n, long* a, long c, long d, long* res, long mod)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Math.Polynomial.Eval;

public unsafe class Example
{
    public static void Main()
    {
        int n = 3;
        int m = 2;
        long mod = 998244353;
        long* poly = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* x = (long*)Marshal.AllocHGlobal(m * sizeof(long));
        long* res = (long*)Marshal.AllocHGlobal(m * sizeof(long));
        try
        {
            poly[0] = 1; poly[1] = 2; poly[2] = 1;
            x[0] = 2; x[1] = 3;
            MultiPointEval.Run(n, poly, m, x, res, mod);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)poly);
            Marshal.FreeHGlobal((IntPtr)x);
            Marshal.FreeHGlobal((IntPtr)res);
        }
    }
}
```