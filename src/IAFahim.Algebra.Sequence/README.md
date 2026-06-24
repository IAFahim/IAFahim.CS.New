# IAFahim.Algebra.Sequence

## Description
This package provides methods to generate, rank, and transform combinatorial sequences and values. It supports Prufer sequence transformations, binomial transforms, Stirling numbers of the first and second kind, Bell numbers, Eulerian numbers, Narayana numbers, and Lah numbers. It also supports generating function operations such as exponential and ordinary generating function products.

## Complexity
- Stirling numbers row computation: O(N log N) where N is the row index.
- Binomial transform: O(N log N) where N is the sequence length.
- Egf / Ogf product: O(N log N) where N is the sequence length.
- Prufer sequence rank: O(N log N) where N is the sequence length.

## API Signature
```csharp
public static unsafe class Prufer
{
    public static long Rank(int* seq, int n, int MOD);
    public static void Unrank(long rank, int n, int MOD, int* seq);
}

public static unsafe class Transform
{
    public static void Binomial(long* a, int n, int MOD, long* b);
    public static void InverseBinomial(long* a, int n, int MOD, long* b);
}

public static unsafe class Combinatorial
{
    public static long Eulerian(int n, int k, int MOD);
    public static long Lah(int n, int k, int MOD);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Algebra.Sequence;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 5;
        long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* b = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        try
        {
            a[0] = 1; a[1] = 1; a[2] = 1; a[3] = 1; a[4] = 1;
            Transform.Binomial(a, n, 998244353, b);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)b);
        }
    }
}
```