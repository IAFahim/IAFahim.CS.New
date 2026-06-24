# IAFahim.Search.LIS

## Description
This package computes the length and elements of the longest increasing subsequence in an array of values.

## Complexity
The algorithm runs in O(N log N) time and uses O(N) space where N is the length of the input.

## API Signature
```csharp
namespace IAFahim.Search.LIS
{
    public static unsafe class Lis
    {
        public static int Run(int* ptr, int len, int* result);
        public static int RunLong(long* ptr, int len, int* result);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.LIS;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int* ptr = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        int* res = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            ptr[0] = 3;
            ptr[1] = 1;
            ptr[2] = 4;
            ptr[3] = 2;
            ptr[4] = 5;
            int size = Lis.Run(ptr, len, res);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
            Marshal.FreeHGlobal((IntPtr)res);
        }
    }
}
```