# IAFahim.Algebra.GraphPoly

## Description
This package provides functions to evaluate graph polynomials. It supports Tutte polynomials, independence polynomials, matching polynomials, reliability polynomials, rook polynomials, and chromatic polynomials. All calculations use unsafe raw pointers to achieve maximum efficiency without managed overhead.

## Complexity
- Tutte polynomial subset evaluation: O(2^E) where E is the number of edges.
- Independence polynomial evaluation: O(2^V) where V is the number of vertices.
- Chromatic polynomial subset evaluation: O(2^V * V) where V is the number of vertices.
- Matching polynomial evaluation: O(2^V) where V is the number of vertices.
- Reliability polynomial: O(2^E) where E is the number of edges.
- Rook polynomial evaluation: O(2^(N*M)) where N and M are the grid dimensions.

## API Signature
```csharp
public static unsafe class Tutte
{
    public static long Subset(int n, int edges, int* from, int* to, long x, long y, int MOD);
}

public static unsafe class Independence
{
    public static long Polynomial(int n, bool* adj, long x, int MOD);
}

public static unsafe class Chromatic
{
    public static void Subset(int n, bool* adj, int MOD, long* coeffs);
    public static int NumberDp(int n, bool* adj, int MOD);
    public static void DeletionContraction(int n, bool* adj, int edges, int* from, int* to, int MOD, long* coeffs);
}

public static unsafe class Matching
{
    public static long Polynomial(int n, bool* adj, long x, int MOD);
}

public static unsafe class Reliability
{
    public static long Run(int n, int edges, int* from, int* to, long p, int MOD);
}

public static unsafe class Rook
{
    public static long Run(int n, int m, bool* blocked, long x, int MOD);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Algebra.GraphPoly;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 3;
        int edges = 3;
        int* from = (int*)Marshal.AllocHGlobal(edges * sizeof(int));
        int* to = (int*)Marshal.AllocHGlobal(edges * sizeof(int));
        try
        {
            from[0] = 0; to[0] = 1;
            from[1] = 1; to[1] = 2;
            from[2] = 2; to[2] = 0;
            long result = Tutte.Subset(n, edges, from, to, 2, 2, 998244353);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)from);
            Marshal.FreeHGlobal((nint)to);
        }
    }
}
```