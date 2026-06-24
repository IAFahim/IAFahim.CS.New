# IAFahim.Search.Bit

## Description
This package provides bitwise operations on arrays of bits, including logical operations, shifting, and search algorithms like longest increasing subsequence lengths.

## Complexity
Bitwise operations run in O(N) time where N is the word count. Binary search runs in O(log N) time. Longest increasing subsequence runs in O(N log N) time.

## API Signature
```csharp
namespace IAFahim.Search.Bit
{
    public static unsafe class BitsetOr
    {
        public static void Run(int n, long* a, long* b, long* res, int wordsPerRow);
    }

    public static unsafe class BitSearch
    {
        public static int Run(int n, int* arr);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Bit;

public static unsafe class Program
{
    public static void Main()
    {
        int n = 64;
        int words = 1;
        long* a = (long*)Marshal.AllocHGlobal(words * sizeof(long));
        long* b = (long*)Marshal.AllocHGlobal(words * sizeof(long));
        long* res = (long*)Marshal.AllocHGlobal(words * sizeof(long));
        try
        {
            a[0] = 1;
            b[0] = 2;
            BitsetOr.Run(n, a, b, res, words);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)a);
            Marshal.FreeHGlobal((IntPtr)b);
            Marshal.FreeHGlobal((IntPtr)res);
        }
    }
}
```