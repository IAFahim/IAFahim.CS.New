# IAFahim.Search.Selection

## Description
This package provides selection algorithms, including quick-select for finding the K-th smallest element and maintaining rolling medians.

## Complexity
Finding the K-th element runs in O(N) average time and O(N^2) worst-case time. Rolling median operations run in O(N log N) time.

## API Signature
```csharp
namespace IAFahim.Search.Selection
{
    public static unsafe class Selection
    {
        public static void SelectTopK(int* ptr, int len, int k);
        public static bool TryGetKth(int* ptr, int len, int k, out int result);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Selection;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int k = 2;
        int result = 0;
        int* ptr = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            ptr[0] = 9;
            ptr[1] = 1;
            ptr[2] = 8;
            ptr[3] = 2;
            ptr[4] = 7;
            bool success = Selection.TryGetKth(ptr, len, k, out result);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
```