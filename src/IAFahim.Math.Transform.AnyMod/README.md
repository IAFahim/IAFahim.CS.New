# IAFahim.Math.Transform.AnyMod

## Description
This package performs convolution modulo any integer (not necessarily prime or power of two) using double-precision arithmetic.

## Complexity
Runs in O(N log N) steps where N is the size of the arrays.

## API Signature
```csharp
namespace IAFahim.Math.Transform.AnyMod
{
    public static unsafe class ArbitraryModConvolution
    {
        public static int Run(long* a, int n, long* b, int m, long* res, long mod);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Math.Transform.AnyMod;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        int m = 4;
        long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* b = (long*)Marshal.AllocHGlobal(m * sizeof(long));
        long* res = (long*)Marshal.AllocHGlobal((n + m) * sizeof(long));
        try
        {
            a[0] = 1;
            b[0] = 2;
            ArbitraryModConvolution.Run(a, n, b, m, res, 998244353);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)b);
            Marshal.FreeHGlobal((nint)res);
        }
    }
}
```