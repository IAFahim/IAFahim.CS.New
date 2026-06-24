# IAFahim.DS.Fenwick

## Description
This package provides a Fenwick Tree (Binary Indexed Tree) implementation. It supports point updates, range sum queries in both one and two dimensions, range updates with point queries, and persistent variants.

## Complexity
- Point update / Prefix sum query: O(log N) where N is the array size.
- 2D point update / 2D prefix query: O(log N * log M) where N x M is the grid size.
- Persistent update / query: O(log N) time and space.

## API Signature
```csharp
public static unsafe class Fenwick
{
    public static void AddInt64(long* bit, int n, int idx, long val);
    public static long SumInt64(long* bit, int idx);
    public static long RangeSumInt64(long* bit, int l, int r);
}

public static unsafe class Fenwick2DAdd
{
    public static void Run(long* bit, int n, int m, int x, int y, long val);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.Fenwick;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 10;
        long* bit = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
        try
        {
            for (int i = 0; i <= n; i++)
                bit[i] = 0;
            Fenwick.AddInt64(bit, n, 3, 5);
            long sum = Fenwick.SumInt64(bit, 3);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)bit);
        }
    }
}
```