# IAFahim.Search.Subset

## Description
This package provides algorithms to enumerate sub-masks, super-masks, and same pop-count integer masks using bitwise search techniques.

## Complexity
Enumerate operations run in O(2^K) time where K is the number of active bits. Space complexity is O(1) auxiliary.

## API Signature
```csharp
namespace IAFahim.Search.Subset
{
    public static unsafe class EnumerateSubsets
    {
        public static int Count(int superMask);
        public static void Run(int superMask, int* dst);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.Subset;

public static unsafe class Program
{
    public static void Main()
    {
        int superMask = 5;
        int size = EnumerateSubsets.Count(superMask);
        int* dst = (int*)Marshal.AllocHGlobal(size * sizeof(int));
        try
        {
            EnumerateSubsets.Run(superMask, dst);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)dst);
        }
    }
}
```