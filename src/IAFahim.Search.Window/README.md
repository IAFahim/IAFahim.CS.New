# IAFahim.Search.Window

## Description
This package provides sliding window query algorithms, including minimum and maximum value tracking, and unsafe binary heap operations.

## Complexity
Sliding window queries run in O(N) total time for an array of size N. Binary heap push and pop operations run in O(log K) time.

## API Signature
```csharp
namespace IAFahim.Search.Window
{
    public static unsafe class SlidingWindowMin
    {
        public static void Run(int* src, int* dst, int len, int windowSize);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Window;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 5;
        int win = 3;
        int* src = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        int* dst = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            src[0] = 4;
            src[1] = 1;
            src[2] = 3;
            src[3] = 2;
            src[4] = 5;
            SlidingWindowMin.Run(src, dst, len, win);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)src);
            Marshal.FreeHGlobal((IntPtr)dst);
        }
    }
}
```