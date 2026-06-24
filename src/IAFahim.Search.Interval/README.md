# IAFahim.Search.Interval

## Description
This package contains methods to merge, intersect, and normalize sets of intervals, and search for interval overlaps.

## Complexity
Merging and normalization run in O(N log N) time due to sorting, where N is interval count. Space complexity is O(1) auxiliary.

## API Signature
```csharp
namespace IAFahim.Search.Interval
{
    public struct Interval
    {
        public int Start;
        public int End;
    }

    public static unsafe class MergeIntervals
    {
        public static int Run(Interval* ptr, int len);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Interval;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 2;
        Interval* ptr = (Interval*)Marshal.AllocHGlobal(len * sizeof(Interval));
        try
        {
            ptr[0].Start = 1;
            ptr[0].End = 3;
            ptr[1].Start = 2;
            ptr[1].End = 4;
            int count = MergeIntervals.Run(ptr, len);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
```