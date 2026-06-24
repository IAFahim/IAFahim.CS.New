# IAFahim.Math.Transform

## Description
This package implements discrete transforms on algebraic structures, subset convolutions, fast Walsh-Hadamard transforms (bitwise OR, AND, XOR), poset zeta and Mobius transforms, partition-based convolutions, XOR vector space bases operations, and tropical min-plus/max-plus convolutions.

## Complexity
The fast Walsh-Hadamard and subset transforms operate in O(N log N) or O(N 2^N) steps. The poset transforms run in O(N^2) where N is the size of the poset. Min-plus general convolution runs in O(N * M) steps, while convex-convex cases run in O(N + M) steps.

## API Signature
```csharp
namespace IAFahim.Math.Transform
{
    public static unsafe class SubsetConvolutionRanked
    {
        public static void Run(long* a, long* b, long* c, int logN, long mod, long* f, long* g, long* h);
    }

    public static unsafe class FwhtConvolution
    {
        public enum FwhtType { Xor, Or, And }
        public static void Run(long* a, long* b, long* c, int n, FwhtType type);
    }

    public static unsafe class SubsetConvolution
    {
        public static void Run(long* a, long* b, long* c, int n);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Math.Transform;

public unsafe class Example
{
    public static void Run()
    {
        int n = 8;
        long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* b = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* c = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        try
        {
            a[0] = 1;
            a[1] = 2;
            b[0] = 3;
            b[1] = 4;
            SubsetConvolution.Run(a, b, c, n);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)b);
            Marshal.FreeHGlobal((nint)c);
        }
    }
}
```