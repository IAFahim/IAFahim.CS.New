# IAFahim.DS.HilbertOrder

## Description
This package provides algorithms to encode multi-dimensional coordinates into one-dimensional order values. It features the Hilbert space-filling curve, Gilbert curve for arbitrary grid sizes, and block-based query ordering for offline query sorting algorithms.

## Complexity
- Hilbert encode: O(log N) where N is the grid dimension.
- Gilbert encode: O(log(W * H)) where W and H are the grid dimensions.
- Block sort order encode / decode: O(1).

## API Signature
```csharp
public static unsafe class HilbertOrder
{
    public static long Run(long x, long y, int pow, int rot);
    public static long Encode(long x, long y, int logN);
}

public static unsafe class GilbertOrder
{
    public static long Encode(long x, long y, int w, int h);
}

public static unsafe class BlockOrder
{
    public static long Encode(int l, int r, int blockSize);
    public static void Decode(long code, int n, int blockSize, int* l, int* r);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.HilbertOrder;

public static unsafe class Example
{
    public static void Run()
    {
        long x = 5;
        long y = 12;
        int logN = 4;
        long hilbertCode = HilbertOrder.Encode(x, y, logN);
    }
}
```