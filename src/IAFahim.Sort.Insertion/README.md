# IAFahim.Sort.Insertion

## Description
This package provides insertion sorting algorithms for arrays of values using raw memory pointer blocks.

## Complexity
The algorithm sorts values in O(N^2) time in the worst case and O(N) in the best case, and uses O(1) auxiliary memory space.

## API Signature
```csharp
namespace IAFahim.Sort.Insertion
{
    public static unsafe class Insertion
    {
        public static void Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>;
        public static void RunDescending<T>(T* ptr, int len) where T : unmanaged, IComparable<T>;
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Sort.Insertion;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int* ptr = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            ptr[0] = 5;
            ptr[1] = 2;
            ptr[2] = 4;
            ptr[3] = 1;
            ptr[4] = 3;
            Insertion.Run(ptr, len);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
```