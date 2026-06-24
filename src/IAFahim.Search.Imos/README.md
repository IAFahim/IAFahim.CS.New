# IAFahim.Search.Imos

## Description
This package implements multi-dimensional prefix sums and range update algorithms on grids and linear buffers, and solves grid bounding rectangle problems.

## Complexity
Range updates run in O(1) time. Grid building runs in O(Width * Height) time.

## API Signature
```csharp
namespace IAFahim.Search.Imos
{
    public static unsafe class Imos1D
    {
        public static void Add(int* diff, int len, int start, int end, int val);
        public static void Build(int* dst, int* diff, int len);
    }

    public static unsafe class Imos2D
    {
        public static void Add(int* diff, int width, int height, int r1, int c1, int r2, int c2, int val);
        public static void Build(int* dst, int* diff, int width, int height);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Imos;

public static unsafe class Program
{
    public static void Main()
    {
        int len = 10;
        int* diff = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        int* dst = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            int i = 0;
            while (i < len)
            {
                diff[i] = 0;
                i = i + 1;
            }
            Imos1D.Add(diff, len, 2, 7, 5);
            Imos1D.Build(dst, diff, len);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)diff);
            Marshal.FreeHGlobal((IntPtr)dst);
        }
    }
}
```