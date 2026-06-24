# IAFahim.Search.DifferenceArray

## Description
This package provides a difference buffer structure to support range additions and value updates on linear memory buffers.

## Complexity
Applying a range increment runs in O(1) time. Building the original representation runs in O(N) time where N is the buffer length.

## API Signature
```csharp
namespace IAFahim.Search.DifferenceArray
{
    public static unsafe class Diff
    {
        public static void Apply(int* diff, int len, int start, int end, int val);
        public static void Build(int* output, int* diff, int len);
        public static int RangeSum(int* prefix, int idx);
        public static void PrefixFromDiff(int* prefix, int* diff, int len);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.DifferenceArray;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 10;
        int* diff = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        int* output = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            int i = 0;
            while (i < len)
            {
                diff[i] = 0;
                i = i + 1;
            }
            Diff.Apply(diff, len, 2, 5, 10);
            Diff.Build(output, diff, len);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)diff);
            Marshal.FreeHGlobal((IntPtr)output);
        }
    }
}
```