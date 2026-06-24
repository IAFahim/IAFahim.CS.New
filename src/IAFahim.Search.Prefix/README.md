# IAFahim.Search.Prefix

## Description
This package provides prefix sum, prefix min, prefix max, and prefix XOR algorithms, along with string pattern searching.

## Complexity
Prefix operations run in O(N) time and use O(1) auxiliary space. String pattern matching runs in O(N + M) time.

## API Signature
```csharp
namespace IAFahim.Search.Prefix
{
    public static unsafe class PrefixSums
    {
        public static long Run(long* ptr, int len);
        public static int Run(int* ptr, int len);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Prefix;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int* ptr = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            ptr[0] = 1;
            ptr[1] = 2;
            ptr[2] = 3;
            ptr[3] = 4;
            ptr[4] = 5;
            PrefixSums.Run(ptr, len);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
```