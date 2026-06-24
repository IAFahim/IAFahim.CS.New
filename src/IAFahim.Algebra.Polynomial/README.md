# IAFahim.Algebra.Polynomial

## Description
This package provides algorithms for univariate polynomial operations over finite fields. It includes division, greatest common divisor, multipoint evaluation, interpolation, roots searching, and factorization. It also implements polynomial product computation using Number Theoretic Transform, Schonhage-Strassen, and Toom-Cook algorithms. All methods run on raw pointers for maximum performance.

## Complexity
- Polynomial product computation (NTT): O(N log N) where N is the polynomial degree.
- Division and GCD: O(N log^2 N) where N is the polynomial degree.
- Multipoint evaluation and interpolation: O(N log^2 N) where N is the number of points.
- Cantor-Zassenhaus factorization: O(D^3 * log Q) where D is the degree and Q is the field size.
- Berlekamp-Massey algorithm: O(N^2) where N is the sequence length.

## API Signature
```csharp
public static unsafe class BerlekampMassey
{
    public static int Run(long* s, int n, int MOD, long* c);
}

public static unsafe class CantorZassenhaus
{
    public static int Run(long* poly, int n, int MOD, long* outF, int* outL);
}

public static unsafe class PowMod
{
    public static void Run(long* poly, int lenPoly, long exponent, long* modPoly, int lenModPoly, long* result, out int lenResult, int MOD);
}

public static unsafe class Gcd
{
    public static void Run(long* a, int lenA, long* b, int lenB, long* gcd, out int lenGcd, int MOD);
}

public static unsafe class BostanMori
{
    public static long Run(long* p, int pLen, long* q, int qLen, long k, int MOD);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Algebra.Polynomial;

public static unsafe class Example
{
    public static void Run()
    {
        int lenA = 3;
        int lenB = 2;
        long* a = (long*)Marshal.AllocHGlobal(lenA * sizeof(long));
        long* b = (long*)Marshal.AllocHGlobal(lenB * sizeof(long));
        long* gcd = (long*)Marshal.AllocHGlobal(lenA * sizeof(long));
        try
        {
            a[0] = 1; a[1] = 2; a[2] = 1;
            b[0] = 1; b[1] = 1;
            int lenGcd;
            Gcd.Run(a, lenA, b, lenB, gcd, out lenGcd, 998244353);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)b);
            Marshal.FreeHGlobal((nint)gcd);
        }
    }
}
```