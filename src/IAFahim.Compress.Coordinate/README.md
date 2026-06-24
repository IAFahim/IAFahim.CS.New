# IAFahim.Compress.Coordinate

## Description
This package provides coordinate discretization and rank compression for coordinates. It transforms an array of numbers into their relative sorted rank offsets, reducing the range of values to [0, U-1] where U is the count of unique values. This is useful for data structures that require small coordinate ranges.

## Complexity
- RankCompress: O(N log N) where N is the array length.
- CoordinateCompress: O(N log N) where N is the array length.
- Discretize: O(N log N) where N is the array length.

## API Signature
```csharp
public static unsafe class RankCompress
{
    public static int Run(int* src, int* dst, int* tmpSorted, int len);
}

public static unsafe class CoordinateCompress
{
    public static int Run(int* src, int* tmp, int* dstMap, int len);
}

public static unsafe class Discretize
{
    public static int Run(int* src, int len);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Compress.Coordinate;

public static unsafe class Example
{
    public static void Run()
    {
        int len = 4;
        int* src = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        int* dst = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        int* tmp = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            src[0] = 100; src[1] = 500; src[2] = 200; src[3] = 500;
            int uniqueCount = RankCompress.Run(src, dst, tmp, len);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)src);
            Marshal.FreeHGlobal((nint)dst);
            Marshal.FreeHGlobal((nint)tmp);
        }
    }
}
```