# IAFahim.Search.Specialized

## Description
This package implements specialized search algorithms, including binary search bounds, ternary search, scheduling generators, and stress testing utilities.

## Complexity
Lower bound and upper bound binary searches run in O(log N) time. Ternary search runs in O(log N) time.

## API Signature
```csharp
namespace IAFahim.Search.Specialized
{
    public static unsafe class BinarySearch
    {
        public static bool TryFind(int* ptr, int len, int key, out int index);
    }

    public static unsafe class UpperBound
    {
        public static int Run(int* ptr, int len, int key);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Specialized;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int key = 3;
        int idx = 0;
        int* ptr = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            ptr[0] = 1;
            ptr[1] = 2;
            ptr[2] = 3;
            ptr[3] = 4;
            ptr[4] = 5;
            bool found = BinarySearch.TryFind(ptr, len, key, out idx);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
```