# IAFahim.Search.Suffix

## Description
This package provides suffix-based query algorithms, including suffix sums, suffix minimums, and suffix maximums on linear sequences.

## Complexity
Suffix array operations run in O(N) time and use O(1) auxiliary space where N is sequence length.

## API Signature
```csharp
namespace IAFahim.Search.Suffix
{
    public static unsafe class SuffixSums
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
using IAFahim.Search.Suffix;

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
            SuffixSums.Run(ptr, len);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
```