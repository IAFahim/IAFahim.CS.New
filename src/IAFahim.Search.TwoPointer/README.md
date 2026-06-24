# IAFahim.Search.TwoPointer

## Description
This package provides two-pointer traversal algorithms, including pair-sum detection and merging of sorted sequences.

## Complexity
Merging and pair-sum checks run in O(N + M) time where N and M are the sizes of the input sequences.

## API Signature
```csharp
namespace IAFahim.Search.TwoPointer
{
    public static unsafe class TwoPointers
    {
        public static int CountPairsWithSum(int* a, int aLen, int* b, int bLen, int target);
        public static bool HasPairWithSum(int* a, int aLen, int* b, int bLen, int target);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search.TwoPointer;

public static unsafe class Program
{
    public static void Main()
    {
        int aLen = 3;
        int bLen = 3;
        int target = 5;
        int* a = (int*)Marshal.AllocHGlobal(aLen * sizeof(int));
        int* b = (int*)Marshal.AllocHGlobal(bLen * sizeof(int));
        try
        {
            a[0] = 1; a[1] = 2; a[2] = 3;
            b[0] = 1; b[1] = 2; b[2] = 3;
            bool success = TwoPointers.HasPairWithSum(a, aLen, b, bLen, target);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)a);
            Marshal.FreeHGlobal((IntPtr)b);
        }
    }
}
```