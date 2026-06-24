# IAFahim.Search.RangeQueries

## Description
This package contains advanced range query algorithms, segment trees with lazy propagation, offline queries, and majority query mechanisms.

## Complexity
Segment tree queries and updates run in O(log N) time. Range majority queries run in O(log N) time.

## API Signature
```csharp
namespace IAFahim.Search.RangeQueries
{
    public static unsafe class RangeMajorityQuery
    {
        public static int Run(int* arr, int n, int l, int r);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.RangeQueries;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int* arr = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            arr[0] = 2;
            arr[1] = 2;
            arr[2] = 3;
            arr[3] = 2;
            arr[4] = 4;
            int maj = RangeMajorityQuery.Run(arr, len, 0, 4);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)arr);
        }
    }
}
```