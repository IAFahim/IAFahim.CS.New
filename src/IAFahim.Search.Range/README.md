# IAFahim.Search.Range

## Description
This package provides range sum, range minimum, range maximum, and range minimum excluded value query structures like sparse tables.

## Complexity
Sparse table construction runs in O(N log N) time and space. Range queries run in O(1) time.

## API Signature
```csharp
namespace IAFahim.Search.Range
{
    public static unsafe class RangeMin
    {
        public static void BuildSparse(int* dst, int* src, int len);
        public static int Query(int* sparse, int* src, int len, int start, int end);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Range;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 4;
        int* src = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        int* dst = (int*)Marshal.AllocHGlobal(len * 4 * sizeof(int));
        try
        {
            src[0] = 4;
            src[1] = 1;
            src[2] = 3;
            src[3] = 2;
            RangeMin.BuildSparse(dst, src, len);
            int minVal = RangeMin.Query(dst, src, len, 1, 3);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)src);
            Marshal.FreeHGlobal((IntPtr)dst);
        }
    }
}
```