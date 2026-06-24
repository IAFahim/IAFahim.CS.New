# IAFahim.Math.Polynomial

## Description
Implements comprehensive operations on polynomials. Includes addition, subtraction, finding products, quotient and remainder division, derivative, integral, inverse, logarithm, exponent, power, square root, multipoint evaluation, Lagrange interpolation, Taylor shift, composition, and shift operations.

## Complexity
- Add/Sub/Shift: O(N) time.
- KaratsubaMultiply: O(N^1.585) time.
- Div/Mod/Derivative/Integral: O(N * M) or O(N) time.
- Inverse/Log/Exp/Pow/Sqrt: O(N * log(N)) time.
- MultipointEval/Interpolate: O(N * log^2(N)) time.

## API Signature
- public static int PolynomialAdd.Run(int n, long* a, int m, long* b, long* res)
- public static int PolynomialSub.Run(int n, long* a, int m, long* b, long* res)
- public static int PolynomialMul.Run(int n, long* a, int m, long* b, long* res)
- public static int PolynomialDiv.Run(int n, long* a, int m, long* b, long* q, long* r)
- public static int KaratsubaMultiply.Run(int n, long* a, int m, long* b, long* res, long* scratch)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Math.Polynomial;

public unsafe class Example
{
    public static void Main()
    {
        int n = 2;
        int m = 2;
        long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* b = (long*)Marshal.AllocHGlobal(m * sizeof(long));
        long* res = (long*)Marshal.AllocHGlobal((n + m) * sizeof(long));
        try
        {
            a[0] = 1; a[1] = 2;
            b[0] = 3; b[1] = 4;
            int degree = PolynomialAdd.Run(n, a, m, b, res);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)a);
            Marshal.FreeHGlobal((IntPtr)b);
            Marshal.FreeHGlobal((IntPtr)res);
        }
    }
}
```