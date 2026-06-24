# IAFahim.Math.Transform.Ntt

## Description
This package implements the Number Theoretic Transform (NTT) for integer convolution modulo a prime. It supports fast number-theoretic forward and inverse transforms.

## Complexity
Forward and inverse transforms run in O(N log N) steps. Convolution runs in O((N+M) log(N+M)) steps.

## API Signature
```csharp
namespace IAFahim.Math.Transform.Ntt
{
    public static unsafe class NttInit
    {
        public static void Run(int logN, long mod, long g, long* roots, long* invRoots);
    }

    public static unsafe class NttTransform
    {
        public static void Forward(long* a, int n, long mod, long* roots);
        public static void Inverse(long* a, int n, long mod, long* invRoots);
    }

    public static unsafe class NttConvolution
    {
        public static int Run(long* a, int n, long* b, int m, long* res, long mod, long g);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Math.Transform.Ntt;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* roots = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        try
        {
            a[0] = 1;
            NttTransform.Forward(a, n, 998244353, roots);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)roots);
        }
    }
}
```